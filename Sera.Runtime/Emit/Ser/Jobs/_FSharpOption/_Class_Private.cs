using System;
using System.Reflection;
using Sera.Runtime.Emit.Deps;

namespace Sera.Runtime.Emit.Ser.Jobs._FSharpOption;

internal class _Class_Private(Type FSharpOption, Type UnderlyingType) : _Private(UnderlyingType)
{
    public Type WrapperType { get; set; } = null!;
    public Type BaseType { get; set; } = null!;
    public Type ImplType { get; set; } = null!;

    public override void Init(EmitStub stub, EmitMeta target)
    {
        (WrapperType, BaseType, ImplType) = FSharpOptionPrivateImpl.Get(FSharpOption);
        Type = BaseType.MakeGenericType(UnderlyingType);
    }

    public override EmitTransform[] CollectTransforms(EmitStub stub, EmitMeta target)
        => new EmitTransform[] { new Transforms._FSharpOptionSerializeImplWrapper(WrapperType, BaseType) };

    public override object CreateInst(EmitStub stub, EmitMeta target, RuntimeDeps deps)
    {
        var dep = deps.Get(0);
        var wrapper = dep.MakeSerializeWrapper(UnderlyingType);
        var inst_type = ImplType.MakeGenericType(UnderlyingType, wrapper);
        var ctor = inst_type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, new[] { wrapper })!;
        var inst = ctor.Invoke(new object?[] { null });
        return inst;
    }
}

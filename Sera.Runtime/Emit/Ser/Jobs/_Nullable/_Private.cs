using System;
using System.Reflection;
using Sera.Core.Impls;
using Sera.Runtime.Emit.Deps;

namespace Sera.Runtime.Emit.Ser.Jobs._Nullable;

internal class _Private(Type UnderlyingType) : _Nullable(UnderlyingType)
{
    private static readonly EmitTransform[] Transforms =
    {
        new Transforms._NullableSerializeImplWrapper()
    };

    public Type ImplType { get; set; } = null!;

    public override void Init(EmitStub stub, EmitMeta target)
    {
        ImplType = typeof(NullableSerializeImplBase<>).MakeGenericType(UnderlyingType);
    }

    public override EmitTransform[] CollectTransforms(EmitStub stub, EmitMeta target)
        => Transforms;
    
    public override Type GetEmitType(EmitStub stub, EmitMeta target, DepItem[] deps)
        => ImplType;

    public override Type GetRuntimeType(EmitStub stub, EmitMeta target, DepItem[] deps)
        => ImplType;

    public override Type GetEmitPlaceholderType(EmitStub stub, EmitMeta target)
        => ImplType;

    public override Type GetRuntimePlaceholderType(EmitStub stub, EmitMeta target)
        => ImplType;

    public override void Emit(EmitStub stub, EmitMeta target, EmitDeps deps) { }

    public override object CreateInst(EmitStub stub, EmitMeta target, RuntimeDeps deps)
    {
        var dep = deps.Get(0);
        var wrapper = dep.MakeSerializeWrapper(UnderlyingType);
        var inst_type = typeof(NullableSerializeImpl<,>).MakeGenericType(UnderlyingType, wrapper);
        var ctor = inst_type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, new[] { wrapper })!;
        var inst = ctor.Invoke(new object?[] { null });
        return inst;
    }
}

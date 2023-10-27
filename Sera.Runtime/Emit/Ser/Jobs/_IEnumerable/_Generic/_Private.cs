using System;
using System.Reflection;
using Sera.Core.Impls;
using Sera.Runtime.Emit.Deps;

namespace Sera.Runtime.Emit.Ser.Jobs._IEnumerable._Generic;

internal class _Private(Type ItemType) : _Generic(ItemType)
{
    public Type BaseType { get; set; } = null!;

    public override void Init(EmitStub stub, EmitMeta target)
    {
        BaseType = typeof(IEnumerableSerializeImplBase<,>).MakeGenericType(target.Type, ItemType);
    }

    public override EmitTransform[] CollectTransforms(EmitStub stub, EmitMeta target) => target.IsValueType
        ? new EmitTransform[]
        {
            new Transforms._IEnumerableSerializeImplWrapper(ItemType),
        }
        : new EmitTransform[]
        {
            new Transforms._IEnumerableSerializeImplWrapper(ItemType),
            new Transforms._ReferenceTypeWrapperSerializeImpl(),
        };
    
    public override Type GetEmitPlaceholderType(EmitStub stub, EmitMeta target)
        => BaseType;

    public override Type GetEmitType(EmitStub stub, EmitMeta target, EmitDeps deps)
        => BaseType;

    public override Type GetRuntimePlaceholderType(EmitStub stub, EmitMeta target)
        => BaseType;

    public override Type GetRuntimeType(EmitStub stub, EmitMeta target, RuntimeDeps deps)
        => BaseType;

    public override void Emit(EmitStub stub, EmitMeta target, EmitDeps deps) { }

    public override object CreateInst(EmitStub stub, EmitMeta target, RuntimeDeps deps)
    {
        var dep = deps.Get(0);
        var wrapper = dep.MakeSerializeWrapper(ItemType);
        var inst_type = typeof(IEnumerableSerializeImpl<,,>).MakeGenericType(target.Type, ItemType, wrapper);
        var ctor = inst_type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, new[] { wrapper })!;
        var inst = ctor.Invoke(new object?[] { null });
        return inst;
    }
}

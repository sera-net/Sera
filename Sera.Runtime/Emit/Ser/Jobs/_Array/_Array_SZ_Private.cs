using System;
using Sera.Core.Impls;
using Sera.Runtime.Emit.Deps;
using Sera.Runtime.Emit.Transform;
using BindingFlags = System.Reflection.BindingFlags;

namespace Sera.Runtime.Emit.Ser.Jobs;

internal class _Array_SZ_Private(Type ItemType) : _Array_Private(ItemType)
{
    public static readonly EmitTransform[] Transforms =
    {
        new EmitTransformArraySerializeImplWrapper(),
        new EmitTransformReferenceTypeWrapperSerializeImpl(),
    };

    public override void Init(EmitStub stub, EmitMeta target)
    {
        BaseType = typeof(ArraySerializeImplBase<>).MakeGenericType(ItemType);
    }

    public override EmitTransform[] CollectTransforms(EmitStub stub, EmitMeta target)
        => Transforms;
    
    public override object CreateInst(EmitStub stub, EmitMeta target, RuntimeDeps deps)
    {
        var dep = deps.Get(0);
        var wrapper = dep.MakeSerializeWrapper(ItemType);
        var inst_type = typeof(ArraySerializeImpl<,>).MakeGenericType(ItemType, wrapper);
        var ctor = inst_type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, new[] { wrapper })!;
        var inst = ctor.Invoke(new object?[] { null });
        return inst;
    }
}

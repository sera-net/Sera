using System;
using System.Reflection;
using Sera.Core.Impls;
using Sera.Runtime.Emit.Deps;
using Sera.Runtime.Emit.Transform;

namespace Sera.Runtime.Emit.Ser.Jobs;

internal class _Array_ReadOnlyMemory_Private(Type ItemType) : _Array_Private(ItemType)
{
    public static readonly EmitTransform[] Transforms =
    {
         new EmitTransformReadOnlyMemorySerializeImplWrapper(),
    };

    public override void Init(EmitStub stub, EmitMeta target)
    {
        BaseType = typeof(ReadOnlyMemorySerializeImplBase<>).MakeGenericType(ItemType);
    }
    
    public override EmitTransform[] CollectTransforms(EmitStub stub, EmitMeta target)
        => Transforms;

    public override object CreateInst(EmitStub stub, EmitMeta target, RuntimeDeps deps)
    {
        var dep = deps.Get(0);
        var wrapper = dep.MakeSerializeWrapper(ItemType);
        var inst_type = typeof(ReadOnlyMemorySerializeImpl<,>).MakeGenericType(ItemType, wrapper);
        var ctor = inst_type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, new[] { wrapper })!;
        var inst = ctor.Invoke(new object?[] { null });
        return inst;
    }
}

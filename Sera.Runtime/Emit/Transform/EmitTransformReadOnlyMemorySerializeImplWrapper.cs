using System;
using Sera.Core.Impls;

namespace Sera.Runtime.Emit.Transform;

internal class EmitTransformReadOnlyMemorySerializeImplWrapper : EmitTransform
{
    public override Type TransformType(EmitMeta target, Type prevType)
    {
        var item_type = target.Type.GetGenericArguments()[0];
        return typeof(ReadOnlyMemorySerializeImplWrapper<>).MakeGenericType(item_type);
    }

    public override object TransformInst(EmitMeta target, Type type, Type prevType, object prevInst)
    {
        var item_type = target.Type.GetGenericArguments()[0];
        var ctor = type.GetConstructor(new[]
            { typeof(ReadOnlyMemorySerializeImplBase<>).MakeGenericType(item_type) })!;
        return ctor.Invoke(new[] { prevInst });
    }
}

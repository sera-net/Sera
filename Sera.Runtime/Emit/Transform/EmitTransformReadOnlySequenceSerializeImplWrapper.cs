using System;
using Sera.Core.Impls;

namespace Sera.Runtime.Emit.Transform;

internal class EmitTransformReadOnlySequenceSerializeImplWrapper : EmitTransform
{
    public override Type TransformType(EmitMeta target, Type prevType)
    {
        if (!target.IsSZArray) throw new ArgumentException("Not a SZArray");
        var item_type = target.Type.GetElementType()!;
        return typeof(ReadOnlySequenceSerializeImplWrapper<>).MakeGenericType(item_type);
    }

    public override object TransformInst(EmitMeta target, Type type, Type prevType, object prevInst)
    {
        if (!target.IsSZArray) throw new ArgumentException("Not a SZArray");
        var item_type = target.Type.GetElementType()!;
        var ctor = type.GetConstructor(new[]
            { typeof(ReadOnlySequenceSerializeImplBase<>).MakeGenericType(item_type) })!;
        return ctor.Invoke(new[] { prevInst });
    }
}

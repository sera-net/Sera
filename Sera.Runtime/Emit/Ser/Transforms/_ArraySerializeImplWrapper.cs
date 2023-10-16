using System;
using Sera.Core.Impls;

namespace Sera.Runtime.Emit.Ser.Transforms;

internal class _ArraySerializeImplWrapper  : EmitTransform
{
    public override Type TransformType(EmitMeta target, Type prevType)
    {
        if (!target.IsSZArray) throw new ArgumentException("Not a SZArray");
        var item_type = target.Type.GetElementType()!;
        return typeof(ArraySerializeImplWrapper<>).MakeGenericType(item_type);
    }

    public override object TransformInst(EmitMeta target, Type type, Type prevType, object prevInst)
    {
        if (!target.IsSZArray) throw new ArgumentException("Not a SZArray");
        var item_type = target.Type.GetElementType()!;
        var ctor = type.GetConstructor(new[] { typeof(ArraySerializeImplBase<>).MakeGenericType(item_type) })!;
        return ctor.Invoke(new[] { prevInst });
    }
}

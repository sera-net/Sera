using System;
using Sera.Core.Impls;

namespace Sera.Runtime.Emit.Ser.Transforms;

internal class _ListSerializeImplWrapper(Type ItemType) : EmitTransform
{
    public override Type TransformType(EmitMeta target, Type prevType)
    {
        return typeof(ListSerializeImplWrapper<,>).MakeGenericType(target.Type, ItemType);
    }

    public override object TransformInst(EmitMeta target, Type type, Type prevType, object prevInst)
    {
        var ctor = type.GetConstructor(new[]
            { typeof(ListSerializeImplBase<,>).MakeGenericType(target.Type, ItemType) })!;
        return ctor.Invoke(new[] { prevInst });
    }
}

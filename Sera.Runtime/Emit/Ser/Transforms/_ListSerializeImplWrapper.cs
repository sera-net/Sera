using System;
using Sera.Core.Impls;

namespace Sera.Runtime.Emit.Ser.Transforms;

internal class _ListSerializeImplWrapper : EmitTransform
{
    public override Type TransformType(EmitMeta target, Type prevType)
    {
        var item_type = target.Type.GetGenericArguments()[0];
        return typeof(ListSerializeImplWrapper<,>).MakeGenericType(target.Type, item_type);
    }

    public override object TransformInst(EmitMeta target, Type type, Type prevType, object prevInst)
    {
        var item_type = target.Type.GetGenericArguments()[0];
        var ctor = type.GetConstructor(new[]
            { typeof(ListSerializeImplBase<,>).MakeGenericType(target.Type, item_type) })!;
        return ctor.Invoke(new[] { prevInst });
    }
}

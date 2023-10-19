using System;
using Sera.Core.Impls;

namespace Sera.Runtime.Emit.Ser.Transforms;

internal class _IEnumerableSerializeImplWrapper(Type ItemType) : EmitTransform
{
    public override Type TransformType(EmitMeta target, Type prevType)
    {
        return typeof(IEnumerableSerializeImplWrapper<,>).MakeGenericType(target.Type, ItemType);
    }

    public override object TransformInst(EmitMeta target, Type type, Type prevType, object prevInst)
    {
        var ctor = type.GetConstructor(new[]
            { typeof(IEnumerableSerializeImplBase<,>).MakeGenericType(target.Type, ItemType) })!;
        return ctor.Invoke(new[] { prevInst });
    }
}

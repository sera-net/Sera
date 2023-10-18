using System;
using Sera.Core.Impls;

namespace Sera.Runtime.Emit.Ser.Transforms;

internal class _NullableSerializeImplWrapper : EmitTransform
{
    public override Type TransformType(EmitMeta target, Type prevType)
    {
        var underlying_type = target.Type.GetGenericArguments()[0];
        return typeof(NullableSerializeImplWrapper<>).MakeGenericType(underlying_type);
    }

    public override object TransformInst(EmitMeta target, Type type, Type prevType, object prevInst)
    {
        var underlying_type = target.Type.GetGenericArguments()[0];
        var ctor = type.GetConstructor(new[]
            { typeof(NullableSerializeImplBase<>).MakeGenericType(underlying_type) })!;
        return ctor.Invoke(new[] { prevInst });
    }
}

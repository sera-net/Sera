using System;
using Sera.Core.Impls;

namespace Sera.Runtime.Emit.Ser.Transforms;

internal class _KeyValuePairSerializeImplWrapper : EmitTransform
{
    public override Type TransformType(EmitMeta target, Type prevType)
    {
        var generics = target.Type.GetGenericArguments();
        var key_type = generics[0];
        var val_type = generics[1];
        return typeof(KeyValuePairSerializeImplWrapper<,>).MakeGenericType(key_type, val_type);
    }

    public override object TransformInst(EmitMeta target, Type type, Type prevType, object prevInst)
    {
        var generics = target.Type.GetGenericArguments();
        var key_type = generics[0];
        var val_type = generics[1];
        var ctor = type.GetConstructor(new[]
            { typeof(KeyValuePairSerializeImplBase<,>).MakeGenericType(key_type, val_type) })!;
        return ctor.Invoke(new[] { prevInst });
    }
}

using System;

namespace Sera.Runtime.Emit.Ser.Transforms;

internal class _FSharpOptionSerializeImplWrapper(
    Type WrapperType,
    Type BaseType
) : EmitTransform
{
    public override Type TransformType(EmitMeta target, Type prevType)
    {
        var underlying_type = target.Type.GetGenericArguments()[0];
        return WrapperType.MakeGenericType(underlying_type);
    }

    public override object TransformInst(EmitMeta target, Type type, Type prevType, object prevInst)
    {
        var underlying_type = target.Type.GetGenericArguments()[0];
        var ctor = type.GetConstructor(new[]
            { BaseType.MakeGenericType(underlying_type) })!;
        return ctor.Invoke(new[] { prevInst });
    }
}

using System;
using Sera.Core;

namespace Sera.Runtime.Emit.Ser;

public abstract class SerEmitJob : EmitJob
{
    public static readonly EmitTransform[] NullableClassImplTransforms =
        { new Transforms._NullableClassImpl() };

    public static readonly EmitTransform[] ReferenceTypeTransforms =
        { new Transforms._ReferenceImpl() };

    public override Type GetEmitPlaceholderType(EmitStub stub, EmitMeta target)
    {
        return typeof(ISeraVision<>).MakeGenericType(target.Type);
    }

    public override Type GetRuntimePlaceholderType(EmitStub stub, EmitMeta target)
    {
        return typeof(ISeraVision<>).MakeGenericType(target.Type);
    }
}

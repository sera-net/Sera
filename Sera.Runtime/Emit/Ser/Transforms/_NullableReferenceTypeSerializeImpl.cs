﻿using System;
using Sera.Core.Impls;

namespace Sera.Runtime.Emit.Ser.Transforms;

internal class _NullableReferenceTypeSerializeImpl : EmitTransform
{
    public override Type TransformType(EmitMeta target, Type prevType)
    {
        return typeof(NullableReferenceTypeSerializeImpl<,>).MakeGenericType(target.Type, prevType);
    }

    public override object TransformInst(EmitMeta target, Type type, Type prevType, object prevInst)
    {
        var ctor = type.GetConstructor(new[] { prevType })!;
        return ctor.Invoke(new[] { prevInst });
    }
}
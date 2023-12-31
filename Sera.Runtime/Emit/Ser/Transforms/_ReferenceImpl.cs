﻿using System;
using Sera.Core.Impls;
using Sera.Core.Impls.Ser;

namespace Sera.Runtime.Emit.Ser.Transforms;

public class _ReferenceImpl : EmitTransform
{
    public override Type TransformType(EmitMeta target, Type prevType)
    {
        return typeof(ReferenceImpl<,>).MakeGenericType(target.Type, prevType);
    }

    public override object TransformInst(EmitMeta target, Type type, Type prevType, object prevInst)
    {
        var ctor = type.GetConstructor(new[] { prevType })!;
        return ctor.Invoke(new[] { prevInst });
    }
}

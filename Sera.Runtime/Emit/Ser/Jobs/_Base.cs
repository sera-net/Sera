﻿using System;
using Sera.Core;

namespace Sera.Runtime.Emit.Ser.Jobs;

internal abstract class _Base : EmitJob
{
    public override Type GetEmitPlaceholderType(EmitStub stub, EmitMeta target)
    {
        return typeof(ISeraVision<>).MakeGenericType(target.Type);
    }

    public override Type GetRuntimePlaceholderType(EmitStub stub, EmitMeta target)
    {
        return typeof(ISeraVision<>).MakeGenericType(target.Type);
    }
}

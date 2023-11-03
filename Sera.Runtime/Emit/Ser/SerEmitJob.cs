using System;
using Sera.Core;

namespace Sera.Runtime.Emit.Ser;

public abstract class SerEmitJob : EmitJob
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

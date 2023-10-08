using System;

namespace Sera.Runtime.Emit.Ser;

internal abstract record EmitSerJob : EmitJob
{
    public override Type GetEmitPlaceholderType(EmitStub stub, EmitMeta target)
    {
        return typeof(ISerialize<>).MakeGenericType(target.Type);
    }

    public override Type GetRuntimePlaceholderType(EmitStub stub, EmitMeta target)
    {
        return typeof(ISerialize<>).MakeGenericType(target.Type);
    }
}

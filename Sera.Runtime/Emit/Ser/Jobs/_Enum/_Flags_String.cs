using System;
using Sera.Core.Impls.Ser;
using Sera.Runtime.Emit.Deps;

namespace Sera.Runtime.Emit.Ser.Jobs._Enum;

internal class _Flags_String(Type UnderlyingType) : _Flags(UnderlyingType)
{
    public override void Init(EmitStub stub, EmitMeta target)
    {
        ImplType = typeof(ToStringImpl<>).MakeGenericType(target.Type);
    }

    public override object CreateInst(EmitStub stub, EmitMeta target, RuntimeDeps deps)
    {
        return Activator.CreateInstance(ImplType)!;
    }
}

using System;
using System.Reflection;
using Sera.Core.Impls;
using Sera.Runtime.Emit.Deps;

namespace Sera.Runtime.Emit.Ser.Jobs._Enum;

internal class _Flags_StringSplit(Type UnderlyingType) : _Flags(UnderlyingType)
{
    public override void Init(EmitStub stub, EmitMeta target)
    {
        ImplType = typeof(FlagsToStringSplitSerializeImpl<>).MakeGenericType(target.Type);
    }

    public override object CreateInst(EmitStub stub, EmitMeta target, RuntimeDeps deps)
    {
        var inst_field = ImplType.GetProperty(nameof(FlagsToStringSplitSerializeImpl<Enum>.Instance),
            BindingFlags.Public | BindingFlags.Static)!;
        return inst_field.GetValue(null)!;
    }
}

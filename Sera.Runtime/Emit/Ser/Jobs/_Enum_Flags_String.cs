using System;
using System.Reflection;
using Sera.Core.Impls;
using Sera.Runtime.Emit.Deps;

namespace Sera.Runtime.Emit.Ser.Jobs;

internal record _Enum_Flags_String(Type UnderlyingType) : _Enum_Flags(UnderlyingType)
{
    public override void Init(EmitStub stub, EmitMeta target)
    {
        ImplType = typeof(ToStringSerializeImpl<>).MakeGenericType(target.Type);
    }

    public override object CreateInst(EmitStub stub, EmitMeta target, RuntimeDeps deps)
    {
        var inst_field = ImplType.GetProperty(nameof(ToStringSerializeImpl<Enum>.Instance),
            BindingFlags.Public | BindingFlags.Static)!;
        return inst_field.GetValue(null)!;
    }
}

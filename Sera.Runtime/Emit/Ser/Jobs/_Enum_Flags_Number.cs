using System;
using System.Reflection;
using Sera.Core.Impls;
using Sera.Core.Ser;
using Sera.Runtime.Emit.Deps;
using Sera.Runtime.Emit.Ser.Internal;

namespace Sera.Runtime.Emit.Ser.Jobs;

internal record _Enum_Flags_Number(Type UnderlyingType)  : _Enum_Flags(UnderlyingType)
{
    private Type NumberImplType = null!;

    public override void Init(EmitStub stub, EmitMeta target)
    {
        NumberImplType = typeof(PrimitiveImpl<>).MakeGenericType(UnderlyingType);
        ImplType = typeof(FlagsAsUnderlyingSerializeImpl<,,>).MakeGenericType(target.Type, UnderlyingType, NumberImplType);
    }
    
    public override object CreateInst(EmitStub stub, EmitMeta target, RuntimeDeps deps)
    {
        var number_impl_inst_ctor = NumberImplType.GetConstructor(
            BindingFlags.Public | BindingFlags.Instance,
            new[] { typeof(SerializerPrimitiveHint?) }
        )!;
        var number_impl_inst = number_impl_inst_ctor.Invoke(new object?[] { null });

        var inst_ctor = ImplType.GetConstructor(
            BindingFlags.Public | BindingFlags.Instance,
            new[] { NumberImplType }
        )!;
        return inst_ctor.Invoke(new[] { number_impl_inst });
    }
}

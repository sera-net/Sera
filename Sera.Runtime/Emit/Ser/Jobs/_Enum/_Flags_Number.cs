using System;
using System.Reflection;
using Sera.Core.Impls.Ser;
using Sera.Runtime.Emit.Deps;

namespace Sera.Runtime.Emit.Ser.Jobs._Enum;

internal class _Flags_Number(Type UnderlyingType) : _Flags(UnderlyingType)
{
    public override void Init(EmitStub stub, EmitMeta target)
    {
        ImplType = typeof(EnumAsUnderlyingImpl<,,>).MakeGenericType(target.Type, UnderlyingType, typeof(PrimitiveImpl));
    }

    public override object CreateInst(EmitStub stub, EmitMeta target, RuntimeDeps deps)
    {
        var inst_ctor = ImplType.GetConstructor(
            BindingFlags.Public | BindingFlags.Instance,
            new[] { typeof(PrimitiveImpl) }
        )!;
        return inst_ctor.Invoke(new object?[] { new PrimitiveImpl() });
    }
}

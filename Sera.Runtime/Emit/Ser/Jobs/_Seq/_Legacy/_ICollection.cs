using System;
using Sera.Core.Impls.Ser;
using Sera.Runtime.Emit.Deps;

namespace Sera.Runtime.Emit.Ser.Jobs._Seq._Legacy;

internal class _ICollection : _Legacy
{
    private Type EmitType { get; set; } = null!;
    private Type RuntimeType { get; set; } = null!;

    public override void Init(EmitStub stub, EmitMeta target) { }

    public override Type GetEmitType(EmitStub stub, EmitMeta target, EmitDeps deps)
        => EmitType =
            typeof(SeqICollectionLegacyRuntimeImpl<>).MakeGenericType(target.Type);

    public override Type GetRuntimeType(EmitStub stub, EmitMeta target, RuntimeDeps deps)
        => RuntimeType =
            typeof(SeqICollectionLegacyRuntimeImpl<>).MakeGenericType(target.Type);

    public override void Emit(EmitStub stub, EmitMeta target, EmitDeps deps) { }

    public override object CreateInst(EmitStub stub, EmitMeta target, RuntimeDeps deps)
    {
        return Activator.CreateInstance(RuntimeType)!;
    }
}

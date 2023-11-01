using System;
using Sera.Runtime.Emit.Deps;

namespace Sera.Runtime.Emit.Ser.Jobs;

internal class _Static(Type Type, object Inst) : _Base
{
    public override void Init(EmitStub stub, EmitMeta target) { }

    public override EmitTransform[] CollectTransforms(EmitStub stub, EmitMeta target)
        => Array.Empty<EmitTransform>();

    public override DepMeta[] CollectDeps(EmitStub stub, EmitMeta target)
        => Array.Empty<DepMeta>();

    public override Type GetEmitType(EmitStub stub, EmitMeta target, EmitDeps deps)
        => Type;

    public override Type GetRuntimeType(EmitStub stub, EmitMeta target, RuntimeDeps deps)
        => Type;

    public override Type GetEmitPlaceholderType(EmitStub stub, EmitMeta target)
        => Type;

    public override Type GetRuntimePlaceholderType(EmitStub stub, EmitMeta target)
        => Type;

    public override void Emit(EmitStub stub, EmitMeta target, EmitDeps deps) { }

    public override object CreateInst(EmitStub stub, EmitMeta target, RuntimeDeps deps)
        => Inst;
}

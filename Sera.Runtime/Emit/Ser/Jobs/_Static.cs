using System;
using Sera.Runtime.Emit.Deps;
using Sera.Runtime.Emit.Transform;

namespace Sera.Runtime.Emit.Ser.Jobs;

internal record _Static(Type Type, object Inst) : _Base
{
    public override bool? EmitTypeIsTypeBuilder => false;
    public override void Init(EmitStub stub, EmitMeta target) { }

    public override EmitTransform[] CollectTransforms(EmitStub stub, EmitMeta target)
        => Array.Empty<EmitTransform>();

    public override DepMeta[] CollectDeps(EmitStub stub, EmitMeta target)
        => Array.Empty<DepMeta>();

    public override Type GetEmitType(EmitStub stub, EmitMeta target, DepItem[] deps)
        => Type;

    public override Type GetRuntimeType(EmitStub stub, EmitMeta target, DepItem[] deps)
        => Type;

    public override void Emit(EmitStub stub, EmitMeta target, EmitDeps deps) { }

    public override object CreateInst(EmitStub stub, EmitMeta target, RuntimeDeps deps)
        => Inst;
}

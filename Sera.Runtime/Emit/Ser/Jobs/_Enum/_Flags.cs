using System;
using Sera.Runtime.Emit.Deps;

namespace Sera.Runtime.Emit.Ser.Jobs._Enum;

internal abstract class _Flags(Type UnderlyingType) : _Base
{
    public override EmitTransform[] CollectTransforms(EmitStub stub, EmitMeta target)
        => EmitTransform.EmptyTransforms;

    public override DepMeta[] CollectDeps(EmitStub stub, EmitMeta target)
        => Array.Empty<DepMeta>();

    protected Type ImplType { get; set; } = null!;

    public override Type GetEmitType(EmitStub stub, EmitMeta target, EmitDeps deps)
        => ImplType;

    public override Type GetRuntimeType(EmitStub stub, EmitMeta target, RuntimeDeps deps)
        => ImplType;

    public override Type GetEmitPlaceholderType(EmitStub stub, EmitMeta target)
        => ImplType;

    public override Type GetRuntimePlaceholderType(EmitStub stub, EmitMeta target)
        => ImplType;
    
    public override void Emit(EmitStub stub, EmitMeta target, EmitDeps deps) { }
}

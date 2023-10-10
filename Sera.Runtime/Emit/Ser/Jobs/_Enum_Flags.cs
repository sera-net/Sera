using System;
using Sera.Runtime.Emit.Deps;
using Sera.Runtime.Emit.Transform;

namespace Sera.Runtime.Emit.Ser.Jobs;

internal abstract record _Enum_Flags(Type UnderlyingType) : _Base
{
    public override bool? EmitTypeIsTypeBuilder => false;

    public override EmitTransform[] CollectTransforms(EmitStub stub, EmitMeta target)
        => EmitTransform.EmptyTransforms;

    public override DepMeta[] CollectDeps(EmitStub stub, EmitMeta target)
        => Array.Empty<DepMeta>();

    protected Type ImplType { get; set; } = null!;

    public override Type GetEmitType(EmitStub stub, EmitMeta target, DepItem[] deps)
        => ImplType;

    public override Type GetRuntimeType(EmitStub stub, EmitMeta target, DepItem[] deps)
        => ImplType;

    public override Type GetEmitPlaceholderType(EmitStub stub, EmitMeta target)
        => ImplType;

    public override Type GetRuntimePlaceholderType(EmitStub stub, EmitMeta target)
        => ImplType;
    
    public override void Emit(EmitStub stub, EmitMeta target, EmitDeps deps) { }
}

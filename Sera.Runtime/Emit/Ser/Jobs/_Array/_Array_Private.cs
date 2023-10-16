using System;
using Sera.Runtime.Emit.Deps;

namespace Sera.Runtime.Emit.Ser.Jobs;

internal abstract class _Array_Private(Type ItemType) : _Array(ItemType)
{
    public Type BaseType { get; set; } = null!;

    public override Type GetEmitPlaceholderType(EmitStub stub, EmitMeta target)
        => BaseType;

    public override Type GetEmitType(EmitStub stub, EmitMeta target, DepItem[] deps)
        => BaseType;

    public override Type GetRuntimePlaceholderType(EmitStub stub, EmitMeta target)
        => BaseType;

    public override Type GetRuntimeType(EmitStub stub, EmitMeta target, DepItem[] deps)
        => BaseType;

    public override void Emit(EmitStub stub, EmitMeta target, EmitDeps deps) { }
}

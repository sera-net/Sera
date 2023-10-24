using System;
using Sera.Runtime.Emit.Deps;

namespace Sera.Runtime.Emit.Ser.Jobs._IDictionary._Generic;

internal abstract class _Private(Type KeyType, Type ValueType) : _Generic(KeyType, ValueType)
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

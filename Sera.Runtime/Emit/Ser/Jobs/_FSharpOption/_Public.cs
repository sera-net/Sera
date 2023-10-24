using System;
using System.Reflection.Emit;
using Sera.Runtime.Emit.Deps;
using Sera.Runtime.Utils;

namespace Sera.Runtime.Emit.Ser.Jobs._FSharpOption;

internal abstract class _Public(Type UnderlyingType) : _FSharpOption(UnderlyingType)
{
    public TypeBuilder TypeBuilder { get; set; } = null!;
    public Type RuntimeType { get; set; } = null!;

    public override void Init(EmitStub stub, EmitMeta target)
    {
        TypeBuilder = CreateTypeBuilderStruct($"Ser_{target.Type.Name}");
        TypeBuilder.MarkReadonly();
    }
    
    public override Type GetEmitPlaceholderType(EmitStub stub, EmitMeta target)
        => TypeBuilder;

    public override Type GetRuntimePlaceholderType(EmitStub stub, EmitMeta target)
        => RuntimeType;

    public override Type GetEmitType(EmitStub stub, EmitMeta target, DepItem[] deps)
        => TypeBuilder;

    public override Type GetRuntimeType(EmitStub stub, EmitMeta target, DepItem[] deps)
        => RuntimeType;
}

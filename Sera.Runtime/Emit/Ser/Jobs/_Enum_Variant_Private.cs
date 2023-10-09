using System;
using Sera.Core.Ser;
using Sera.Runtime.Emit.Deps;
using Sera.Runtime.Emit.Transform;
using Sera.Runtime.Utils;

namespace Sera.Runtime.Emit.Ser.Jobs;

internal record _Enum_Variant_Private
    (Type UnderlyingType, EnumInfo[] Items, EnumJumpTables? JumpTable, SeraEnumAttribute? EnumAttr)
    : _Base
{
    public SeraEnumAttribute? EnumAttr { get; private set; } = EnumAttr;
    public SerializerVariantHint? RootHint { get; } = EnumAttr?.SerHint;

    public override bool? EmitTypeIsTypeBuilder { get; }

    public override void Init(EmitStub stub, EmitMeta target)
    {
        throw new NotImplementedException();
    }

    public override EmitTransform[] CollectTransforms(EmitStub stub, EmitMeta target)
    {
        throw new NotImplementedException();
    }

    public override DepMeta[] CollectDeps(EmitStub stub, EmitMeta target)
    {
        throw new NotImplementedException();
    }

    public override Type GetEmitType(EmitStub stub, EmitMeta target, DepItem[] deps)
    {
        throw new NotImplementedException();
    }

    public override Type GetRuntimeType(EmitStub stub, EmitMeta target, DepItem[] deps)
    {
        throw new NotImplementedException();
    }

    public override void Emit(EmitStub stub, EmitMeta target, EmitDeps deps)
    {
        throw new NotImplementedException();
    }

    public override object CreateInst(EmitStub stub, EmitMeta target, RuntimeDeps deps)
    {
        throw new NotImplementedException();
    }
}

using System;
using System.Reflection;
using Sera.Runtime.Emit.Deps;
using Sera.Runtime.Emit.Transform;
using Sera.Runtime.Utils;

namespace Sera.Runtime.Emit.Ser.Jobs;

internal abstract class _Array(Type ItemType) : _Base
{
    private static readonly EmitTransform[] transforms = { new EmitTransformReferenceTypeWrapperSerializeImpl() };

    public override EmitTransform[] CollectTransforms(EmitStub stub, EmitMeta target)
        => transforms;

    public override DepMeta[] CollectDeps(EmitStub stub, EmitMeta target)
    {
        var item_nullable = target.TypeMeta.Nullability?.NullabilityInfo?.ElementType;
        var transforms = !ItemType.IsValueType && item_nullable is not
            { ReadState: NullabilityState.NotNull }
            ? SerializeEmitProvider.NullableReferenceTypeTransforms
            : EmitTransform.EmptyTransforms;
        var meta = new DepMeta(
            new(TypeMetas.GetTypeMeta(ItemType, new NullabilityMeta(item_nullable)), EmitData.Default),
            transforms);
        return new[] { meta };
    }
}

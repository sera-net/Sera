using System;
using System.Buffers;
using System.Reflection;
using Sera.Runtime.Emit.Deps;
using Sera.Runtime.Utils;

namespace Sera.Runtime.Emit.Ser.Jobs;

// todo ArraySegment, ImmutableArray

internal abstract class _Array_Like(Type ItemType) : _Base
{
    public override EmitTransform[] CollectTransforms(EmitStub stub, EmitMeta target)
        => target.IsValueType ? EmitTransform.EmptyTransforms : SerializeEmitProvider.ReferenceTypeTransforms;

    protected virtual NullabilityInfo? GetElementNullabilityInfo(EmitMeta target)
    {
        if (target.Type.IsSZArray) return target.TypeMeta.Nullability?.NullabilityInfo?.ElementType;
        var type = target.Type;
        if (type.IsGenericType)
        {
            var del = type.GetGenericTypeDefinition();
            if (del == typeof(ReadOnlySequence<>) || del == typeof(ReadOnlyMemory<>) || del == typeof(Memory<>))
            {
                return target.TypeMeta.Nullability?.NullabilityInfo?.GenericTypeArguments[0];
            }
        }
        return null;
    }

    public override DepMeta[] CollectDeps(EmitStub stub, EmitMeta target)
    {
        var item_nullable = GetElementNullabilityInfo(target);
        var transforms = !ItemType.IsValueType && item_nullable is not
            { ReadState: NullabilityState.NotNull }
            ? SerializeEmitProvider.NullableClassImplTransforms
            : EmitTransform.EmptyTransforms;
        var meta = new DepMeta(
            new(TypeMetas.GetTypeMeta(ItemType, new NullabilityMeta(item_nullable)), target.Styles.TakeFormats()),
            transforms);
        return new[] { meta };
    }
}

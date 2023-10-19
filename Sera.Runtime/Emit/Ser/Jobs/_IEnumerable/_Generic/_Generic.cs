using System;
using System.Reflection;
using Sera.Runtime.Emit.Deps;
using Sera.Runtime.Utils;

namespace Sera.Runtime.Emit.Ser.Jobs._IEnumerable._Generic;

internal abstract class _Generic(Type ItemType) : _IEnumerable
{
    public override EmitTransform[] CollectTransforms(EmitStub stub, EmitMeta target)
        => target.IsValueType ? EmitTransform.EmptyTransforms : SerializeEmitProvider.ReferenceTypeTransforms;
    
    public override DepMeta[] CollectDeps(EmitStub stub, EmitMeta target)
    {
        var item_nullable = target.TypeMeta.Nullability?.NullabilityInfo?.ElementType;
        var transforms = !ItemType.IsValueType && item_nullable is not
            { ReadState: NullabilityState.NotNull }
            ? SerializeEmitProvider.NullableReferenceTypeTransforms
            : EmitTransform.EmptyTransforms;
        var meta = new DepMeta(
            new(TypeMetas.GetTypeMeta(ItemType, new NullabilityMeta(item_nullable)), target.Data),
            transforms);
        return new[] { meta };
    }
}

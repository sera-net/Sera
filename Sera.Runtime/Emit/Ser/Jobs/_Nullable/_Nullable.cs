using System;
using Sera.Runtime.Emit.Deps;
using Sera.Runtime.Utils;

namespace Sera.Runtime.Emit.Ser.Jobs._Nullable;

internal abstract class _Nullable(Type UnderlyingType) : _Base
{
    public override DepMeta[] CollectDeps(EmitStub stub, EmitMeta target)
    {
        var item_nullable = target.TypeMeta.Nullability?.NullabilityInfo?.ElementType;
        var transforms = EmitTransform.EmptyTransforms;
        var meta = new DepMeta(
            new(TypeMetas.GetTypeMeta(UnderlyingType, new NullabilityMeta(item_nullable)), target.Data),
            transforms);
        return new[] { meta };
    }
}

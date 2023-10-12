using System;
using Sera.Runtime.Emit.Transform;

namespace Sera.Runtime.Emit.Ser.Jobs;

internal abstract class _Array(Type ItemType) : _ArrayLike(ItemType)
{
    public override EmitTransform[] CollectTransforms(EmitStub stub, EmitMeta target)
        => SerializeEmitProvider.ReferenceTypeTransforms;
}

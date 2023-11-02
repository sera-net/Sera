using System;
using System.Collections.Frozen;
using System.Runtime.CompilerServices;
using Sera.Core;

namespace Sera.Runtime.Emit.Ser.Internal;

public readonly struct PrivateEnumImpl<T> : ISeraVision<T>, IUnionSeraVision<T> where T : Enum
{
    private readonly Inner inner;

    internal PrivateEnumImpl(
        string unionName, Func<T, VariantTag> toTag,
        FrozenDictionary<T, VariantMeta> metas,
        UnionStyle? unionStyle, SeraUnionMode mode
    ) => inner = new(unionName, toTag, metas, unionStyle, mode);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, T value) where V : ASeraVisitor<R>
        => visitor.VUnion(this, value);

    public string Name => inner.Name;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R AcceptUnion<R, V>(V visitor, T value) where V : AUnionSeraVisitor<R>
        => inner.AcceptUnion<R, V>(visitor, value);

    private sealed class Inner(
        string unionName, Func<T, VariantTag> toTag,
        FrozenDictionary<T, VariantMeta> metas,
        UnionStyle? unionStyle, SeraUnionMode mode
    )
    {
        public string Name => unionName;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R AcceptUnion<R, V>(V visitor, T value) where V : AUnionSeraVisitor<R>
        {
            if (metas.TryGetValue(value, out var meta))
            {
                return visitor.VVariant(new Variant(meta.Name, toTag(value)), unionStyle, meta.Style);
            }
            else
            {
                if (mode is SeraUnionMode.Exhaustive)
                {
                    return visitor.VNone();
                }
                else
                {
                    return visitor.VVariant(new Variant(toTag(value)), unionStyle, null);
                }
            }
        }
    }
}

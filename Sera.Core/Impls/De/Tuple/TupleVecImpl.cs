using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BetterCollections;
using Sera.Utils;

namespace Sera.Core.Impls.De;

public readonly struct TupleVecImpl<I, D>(D d, int? size)
    : ISeraColion<Vec<I>>, ITupleSeraColion<Vec<I>>
    where D : ISeraColion<I>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<Vec<I>>? t = null) where C : ISeraColctor<Vec<I>, R>
        => colctor.CTuple(this, new IdentityMapper<Vec<I>>(), new Type<Vec<I>>());

    public int? Size
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => size;
    }

    public int? TotalSize
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => size;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vec<I> Builder(Type<Vec<I>> b = default) =>
        new SeqCapCtor<I>().Ctor(size, (InType<Vec<I>>?)null);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R CollectItem<R, C>(ref C colctor, int index, Type<Vec<I>> b = default)
        where C : ITupleSeraColctor<Vec<I>, R>
        => colctor.CItem(d, new SeqVecEffector<I>(), new Type<I>());
}

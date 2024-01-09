using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sera.Utils;

namespace Sera.Core.Impls.De;

public readonly struct TupleListImpl<I, D>(D d, int? size)
    : ISeraColion<List<I>>, ITupleSeraColion<List<I>>
    where D : ISeraColion<I>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<List<I>>? t = null) where C : ISeraColctor<List<I>, R>
        => colctor.CTuple(this, new IdentityMapper<List<I>>(), new Type<List<I>>());

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
    public List<I> Builder(Type<List<I>> b = default) =>
        new SeqCapCtor<I>().Ctor(size, (InType<List<I>>?)null);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R CollectItem<R, C>(ref C colctor, int index, Type<List<I>> b = default)
        where C : ITupleSeraColctor<List<I>, R>
        => colctor.CItem(d, new SeqListEffector<I>(), new Type<I>());
}

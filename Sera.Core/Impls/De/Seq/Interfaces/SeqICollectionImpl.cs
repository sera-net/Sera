using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sera.Core.Impls.De;
using Sera.Utils;

namespace Sera.Core.Impls.De;

public readonly struct SeqICollectionImpl<T, I, D, N>(D d, N ctor) :
    ISeraColion<T>, ISeqSeraColion<T, I>
    where T : ICollection<I>
    where D : ISeraColion<I>
    where N : ICapSeraCtor<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<T>? t = null) where C : ISeraColctor<T, R>
        => colctor.CSeq(this, new IdentityMapper<T>(), new Type<T>(), new Type<I>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Builder(int? cap, Type<T> b = default) => ctor.Ctor(cap);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R CollectItem<R, C>(ref C colctor, Type<T> b = default) where C : ISeqSeraColctor<T, I, R>
        => colctor.CItem(d, new SeqICollectionEffector<T, I>());
}

public readonly struct SeqICollectionEffector<T, I> : ISeraEffector<T, I>
    where T : ICollection<I>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Effect(ref T target, I value) => target.Add(value);
}

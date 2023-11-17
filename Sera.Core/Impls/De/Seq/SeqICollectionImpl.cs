using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sera.Core.Impls.De.Misc;
using Sera.Utils;

namespace Sera.Core.Impls.De;

public readonly struct SeqICollectionImpl<T, I, D, N>(D d, N ctor) :
    ISeraColion<T>, ISeqSeraColion<T>
    where T : ICollection<I>
    where D : ISeraColion<I>
    where N : ICapSeraCtor<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<T>? t = null) where C : ISeraColctor<T, R>
        => colctor.CSeq(this, new IdentityMapper<T>(), new Type<T>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Builder(int? cap, Type<T> b = default) => ctor.Ctor(cap);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R CollectItem<R, C>(ref C colctor, Type<T> b = default) where C : ISeqSeraColctor<T, R>
        => colctor.CItem(d, new ICollectionEffector(), new Type<I>());

    private readonly struct ICollectionEffector : ISeraEffector<T, I>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Effect(ref T target, I value) => target.Add(value);
    }
}

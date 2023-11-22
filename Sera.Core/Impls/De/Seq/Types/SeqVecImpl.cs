using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sera.Core.Impls.De;
using Sera.Utils;

namespace Sera.Core.Impls.De;

public readonly struct SeqVecImpl<I, D>(D d) : ISeraColion<Vec<I>>, ISeqSeraColion<Vec<I>, I>
    where D : ISeraColion<I>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<Vec<I>>? t = null) where C : ISeraColctor<Vec<I>, R>
        => colctor.CSeq(this, new IdentityMapper<Vec<I>>(), new Type<Vec<I>>(), new Type<I>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vec<I> Builder(int? cap, Type<Vec<I>> b = default) =>
        new SeqCapCtor<I>().Ctor(cap, (InType<Vec<I>>?)null);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R CollectItem<R, C>(ref C colctor, Type<Vec<I>> b = default) where C : ISeqSeraColctor<Vec<I>, I, R>
        => colctor.CItem(d, new SeqVecEffector<I>());
}

public readonly struct SeqVecEffector<I> : ISeraEffector<Vec<I>, I>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Effect(ref Vec<I> target, I value) => target.Add(value);
}

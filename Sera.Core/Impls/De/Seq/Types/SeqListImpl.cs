using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sera.Core.Impls.De.Misc;
using Sera.Utils;

namespace Sera.Core.Impls.De;

public readonly struct SeqListImpl<I, D>(D d) : ISeraColion<List<I>>, ISeqSeraColion<List<I>, I>
    where D : ISeraColion<I>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<List<I>>? t = null) where C : ISeraColctor<List<I>, R>
        => colctor.CSeq(this, new IdentityMapper<List<I>>(), new Type<List<I>>(), new Type<I>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public List<I> Builder(int? cap, Type<List<I>> b = default) =>
        new SeqCapCtor<I>().Ctor(cap, (InType<List<I>>?)null);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R CollectItem<R, C>(ref C colctor, Type<List<I>> b = default) where C : ISeqSeraColctor<List<I>, I, R>
        => colctor.CItem(d, new SeqListEffector<I>());
}

public readonly struct SeqListEffector<I> : ISeraEffector<List<I>, I>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Effect(ref List<I> target, I value) => target.Add(value);
}

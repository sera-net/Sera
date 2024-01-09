using System.Runtime.CompilerServices;
using Sera.Utils;

namespace Sera.Core.Impls.De;

public readonly struct SeqImpl<T, I, D, B, M, N, E>(D d, M mapper, N ctor, E effector) :
    ISeraColion<T>, ISeqSeraColion<B, I>
    where D : ISeraColion<I>
    where M : ISeraMapper<B, T>
    where N : ICapSeraCtor<B>
    where E : ISeraEffector<B, I>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<T>? t = null) where C : ISeraColctor<T, R>
        => colctor.CSeq(this, mapper, new Type<B>(), new Type<I>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public B Builder(int? cap, Type<B> b = default) => ctor.Ctor(cap);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R CollectItem<R, C>(ref C colctor, Type<B> b = default) where C : ISeqSeraColctor<B, I, R>
        => colctor.CItem(d, effector);
}
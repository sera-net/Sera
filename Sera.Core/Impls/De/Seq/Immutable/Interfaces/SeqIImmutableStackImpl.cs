using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using Sera.Core.Impls.De.Misc;
using Sera.Utils;

namespace Sera.Core.Impls.De;

public readonly struct SeqIImmutableStackImpl<T, I, D, N>(D d, N ctor) :
    ISeraColion<T>, ISeqSeraColion<T, I>
    where T : IImmutableStack<I>
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
        => colctor.CItem(d, new SeqIImmutableStackEffector<T, I>());
}

public readonly struct SeqIImmutableStackEffector<T, I> : ISeraEffector<T, I>
    where T : IImmutableStack<I>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Effect(ref T target, I value) => target = (T)target.Push(value);
}

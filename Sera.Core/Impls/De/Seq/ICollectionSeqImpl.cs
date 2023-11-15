using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sera.Utils;

namespace Sera.Core.Impls.De;

public readonly struct ICollectionSeqImpl<T, I, N, D, A>(D d) :
    ISeraColion<ICollectionSeqAsmer<T, I, N, A>>,
    ISeqSeraColion<ICollectionSeqAsmer<T, I, N, A>>
    where T : ICollection<I>
    where N : ICapSeraCtor<T>
    where D : ISeraColion<A>
    where A : ISeraAsmer<I>
{
    [AssocType("A")]
    public abstract class _A(ICollectionSeqAsmer<T, I, N, A> type);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C, B>(C colctor, B asmer)
        where C : ASeraColctor<R> where B : IRef<ICollectionSeqAsmer<T, I, N, A>>
        => colctor.CSeq(this, asmer, new Type<ICollectionSeqAsmer<T, I, N, A>>(), new Type<List<I>>(), new Type<I>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R CollectItem<R, C, B>(ref C colctor, B asmer)
        where C : ISeqSeraColctor<R> where B : IRef<ICollectionSeqAsmer<T, I, N, A>>
        => colctor.CItem(d, new ICollectionSeqAsmer<T, I, N, A>.ItemRef<B>(asmer), new Type<A>(), new Type<I>());
}

public readonly struct ICollectionSeqAsmable<T, I, N, D, A>(N ctor, D d) : ISeraAsmable<ICollectionSeqAsmer<T, I, N, A>>
    where T : ICollection<I>
    where N : ICapSeraCtor<T>
    where D : ISeraAsmable<A>
    where A : ISeraAsmer<I>
{
    [AssocType("A")]
    public abstract class _A(ICollectionSeqAsmer<T, I, N, A> type);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ICollectionSeqAsmer<T, I, N, A> Asmer() => new(ctor, d.Asmer());
}

public struct ICollectionSeqAsmer<T, I, N, A>(N ctor, A a) :
    ISeraAsmer<T>, ISeqSeraAsmer
    where T : ICollection<I>
    where N : ICapSeraCtor<T>
    where A : ISeraAsmer<I>
{
    private A a = a;
    private T? target = default!;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Asm() => target!;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Init(int? count) => target = ctor.Ctor(count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add() => target!.Add(a.Asm());

    public readonly struct ItemRef<B>(B asmer) : IRef<A> where B : IRef<ICollectionSeqAsmer<T, I, N, A>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref A GetRef() => ref asmer.GetRef().a;
    }
}

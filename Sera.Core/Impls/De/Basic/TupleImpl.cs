using System;
using System.Runtime.CompilerServices;
using Sera.Utils;

namespace Sera.Core.Impls.De;

#region Size1

public readonly struct TupleImpl<T1, D1, A1>(D1 d1) :
    ISeraColion<TupleAsmer<T1, A1>>,
    ITupleSeraColion<TupleAsmer<T1, A1>>
    where D1 : ISeraColion<A1>
    where A1 : ISeraAsmer<T1>
{
    [AssocType]
    public abstract class A(TupleAsmer<T1, A1> type);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C, B>(C colctor, B asmer) where C : ASeraColctor<R> where B : IRef<TupleAsmer<T1, A1>>
        => colctor.CTuple(this, asmer, new Type<TupleAsmer<T1, A1>>(), new Type<ValueTuple<T1>>());

    public int Size
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R CollectItem<R, C, B>(ref C colctor, B asmer, int index)
        where C : ITupleSeraColctor<R> where B : IRef<TupleAsmer<T1, A1>>
        => index switch
        {
            0 => colctor.CItem(d1, new TupleAsmer<T1, A1>.Item1Ref<B>(asmer), new Type<A1>(), new Type<T1>()),
            _ => colctor.CNone(),
        };
}

public readonly struct TupleAsmable<T1, D1, A1>(D1 d1) : ISeraAsmable<TupleAsmer<T1, A1>>
    where D1 : ISeraAsmable<A1> where A1 : ISeraAsmer<T1>
{
    [AssocType]
    public abstract class A(TupleAsmer<T1, A1> type);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TupleAsmer<T1, A1> Asmer() => new(d1.Asmer());
}

public struct TupleAsmer<T1, A1>(A1 a1) : ISeraAsmer<ValueTuple<T1>>, ISeraAsmer<Tuple<T1>>
    where A1 : ISeraAsmer<T1>
{
    private A1 a1 = a1;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    ValueTuple<T1> ISeraAsmer<ValueTuple<T1>>.Asm() => new(a1.Asm());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    Tuple<T1> ISeraAsmer<Tuple<T1>>.Asm() => new(a1.Asm());

    public readonly struct Item1Ref<B>(B asmer) : IRef<A1>
        where B : IRef<TupleAsmer<T1, A1>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref A1 GetRef() => ref asmer.GetRef().a1;
    }
}

#endregion

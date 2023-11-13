using System;
using System.Runtime.CompilerServices;
using Sera.Utils;

namespace Sera.Core.Impls.De;

#region Size1

public readonly struct TupleImpl<T1, D1, A1>(D1 d1) :
    ISeraColion<ValueTuple<T1>, TupleAsmer<T1, A1>>,
    ISeraTupleColion<ValueTuple<T1>, TupleAsmer<T1, A1>>,
    ISeraColion<Tuple<T1>, TupleAsmer<T1, A1>>,
    ISeraTupleColion<Tuple<T1>, TupleAsmer<T1, A1>>
    where D1 : ISeraColion<T1, A1>
    where A1 : ISeraAsmer<T1>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TupleAsmer<T1, A1> Asmer(Type<TupleAsmer<T1, A1>> a) => new(d1.Asmer(new()));


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C, B>(C colctor, B asmer, Type<ValueTuple<T1>> t)
        where C : ASeraColctor<R> where B : IRef<TupleAsmer<T1, A1>>
        => colctor.CTuple(this, asmer, new Type<TupleAsmer<T1, A1>>(), t);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C, B>(C colctor, B asmer, Type<Tuple<T1>> t)
        where C : ASeraColctor<R> where B : IRef<TupleAsmer<T1, A1>>
        => colctor.CTuple(this, asmer, new Type<TupleAsmer<T1, A1>>(), t);

    public int Size
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R CollectItem<R, C, B>(ref C colctor, B asmer, int index, Type<ValueTuple<T1>> t)
        where C : ITupleSeraColctor<R> where B : IRef<TupleAsmer<T1, A1>>
        => index switch
        {
            0 => colctor.CItem(d1, new TupleAsmer<T1, A1>.Item1Ref<B>(asmer), new Type<A1>(), new Type<T1>()),
            _ => colctor.CNone(),
        };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R CollectItem<R, C, B>(ref C colctor, B asmer, int index, Type<Tuple<T1>> t) where C : ITupleSeraColctor<R>
        where B : IRef<TupleAsmer<T1, A1>>
        => index switch
        {
            0 => colctor.CItem(d1, new TupleAsmer<T1, A1>.Item1Ref<B>(asmer), new Type<A1>(), new Type<T1>()),
            _ => colctor.CNone(),
        };
}

public struct TupleAsmer<T1, A1>(A1 a1) : ISeraAsmer<ValueTuple<T1>>, ISeraAsmer<Tuple<T1>>
    where A1 : ISeraAsmer<T1>
{
    internal A1 a1 = a1;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTuple<T1> Asm(Type<ValueTuple<T1>> t) => new(a1.Asm(new()));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Tuple<T1> Asm(Type<Tuple<T1>> t) => new(a1.Asm(new()));

    public readonly struct Item1Ref<B>(B asmer) : IRef<A1>
        where B : IRef<TupleAsmer<T1, A1>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref A1 GetRef() => ref asmer.GetRef().a1;
    }
}

#endregion

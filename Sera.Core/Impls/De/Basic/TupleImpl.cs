using System;
using System.Runtime.CompilerServices;
using Sera.Core.Impls.De;
using Sera.Utils;

namespace Sera.Core.Impls.De;

#region Size1

public readonly struct TupleImpl<T1, D1>(D1 d1) :
    ISeraColion<ValueTuple<T1>>,
    ISeraColion<Tuple<T1>>,
    ITupleSeraColion<ValueTuple<T1>>
    where D1 : ISeraColion<T1>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<ValueTuple<T1>>? t) where C : ISeraColctor<ValueTuple<T1>, R>
        => colctor.CTuple(this, new IdentityMapper<ValueTuple<T1>>(), new Type<ValueTuple<T1>>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<Tuple<T1>>? t) where C : ISeraColctor<Tuple<T1>, R>
        => colctor.CTuple(this, new ValueTupleToTupleMapper<T1>(), new Type<ValueTuple<T1>>());

    public int? Size
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTuple<T1> Builder(Type<ValueTuple<T1>> b) => default;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R CollectItem<R, C>(ref C colctor, int index, Type<ValueTuple<T1>> b)
        where C : ITupleSeraColctor<ValueTuple<T1>, R>
        => index switch
        {
            0 => colctor.CItem(d1, new ValueTupleEffector<T1>.Item1(), new Type<T1>()),
            _ => colctor.CNone(),
        };
}

public static class ValueTupleEffector<T1>
{
    public readonly struct Item1 : ISeraEffector<ValueTuple<T1>, T1>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Effect(ref ValueTuple<T1> target, T1 value)
            => target.Item1 = value;
    }
}

public readonly struct ValueTupleToTupleMapper<T1> : ISeraMapper<ValueTuple<T1>, Tuple<T1>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Tuple<T1> Map(ValueTuple<T1> value, InType<Tuple<T1>>? u) => new(value.Item1);
}

#endregion

#region Size2

public readonly struct TupleImpl<T1, T2, D1, D2>(D1 d1, D2 d2) :
    ISeraColion<ValueTuple<T1, T2>>,
    ISeraColion<Tuple<T1, T2>>,
    ITupleSeraColion<ValueTuple<T1, T2>>
    where D1 : ISeraColion<T1>
    where D2 : ISeraColion<T2>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<ValueTuple<T1, T2>>? t) where C : ISeraColctor<ValueTuple<T1, T2>, R>
        => colctor.CTuple(this, new IdentityMapper<ValueTuple<T1, T2>>(), new Type<ValueTuple<T1, T2>>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<Tuple<T1, T2>>? t) where C : ISeraColctor<Tuple<T1, T2>, R>
        => colctor.CTuple(this, new ValueTupleToTupleMapper<T1, T2>(), new Type<ValueTuple<T1, T2>>());

    public int? Size
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => 2;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTuple<T1, T2> Builder(Type<ValueTuple<T1, T2>> b) => default;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R CollectItem<R, C>(ref C colctor, int index, Type<ValueTuple<T1, T2>> b)
        where C : ITupleSeraColctor<ValueTuple<T1, T2>, R>
        => index switch
        {
            0 => colctor.CItem(d1, new ValueTupleEffector<T1, T2>.Item1(), new Type<T1>()),
            1 => colctor.CItem(d2, new ValueTupleEffector<T1, T2>.Item2(), new Type<T2>()),
            _ => colctor.CNone(),
        };
}

public static class ValueTupleEffector<T1, T2>
{
    public readonly struct Item1 : ISeraEffector<ValueTuple<T1, T2>, T1>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Effect(ref ValueTuple<T1, T2> target, T1 value)
            => target.Item1 = value;
    }

    public readonly struct Item2 : ISeraEffector<ValueTuple<T1, T2>, T2>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Effect(ref ValueTuple<T1, T2> target, T2 value)
            => target.Item2 = value;
    }
}

public readonly struct ValueTupleToTupleMapper<T1, T2> : ISeraMapper<ValueTuple<T1, T2>, Tuple<T1, T2>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Tuple<T1, T2> Map(ValueTuple<T1, T2> value, InType<Tuple<T1, T2>>? u)
        => new(value.Item1, value.Item2);
}

#endregion

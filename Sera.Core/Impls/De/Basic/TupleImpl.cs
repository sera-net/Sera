using System;
using System.Runtime.CompilerServices;
using Sera.Core.Impls.De.Misc;
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
        => colctor.CTuple(this, new ValueTuple2TupleMapper(), new Type<ValueTuple<T1>>());

    public int Size
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
            0 => colctor.CItem(d1, new Item1Effector(), new Type<T1>()),
            _ => colctor.CNone(),
        };

    private readonly struct Item1Effector : ISeraEffector<ValueTuple<T1>, T1>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Effect(ref ValueTuple<T1> target, T1 value)
            => target.Item1 = value;
    }

    public readonly struct ValueTuple2TupleMapper : ISeraMapper<ValueTuple<T1>, Tuple<T1>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Tuple<T1> Map(ValueTuple<T1> value, InType<Tuple<T1>>? u) => new(value.Item1);
    }
}

#endregion

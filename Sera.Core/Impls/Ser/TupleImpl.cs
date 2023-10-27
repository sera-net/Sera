﻿using System;
using System.Runtime.CompilerServices;

namespace Sera.Core.Impls.Ser;

#region Empty

public readonly struct TupleImpl : ISeraVision<ValueTuple>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, ValueTuple value) where V : ASeraVisitor<R>
        => visitor.VTuple(new Impl(), value);

    public readonly struct Impl : ITupleSeraVision<ValueTuple>
    {
        public int Size => 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R AcceptItem<R, V>(V visitor, ValueTuple _, int index) where V : ATupleSeraVisitor<R>
            => visitor.VNone();
    }
}

#endregion

#region Size 1

public readonly struct TupleImpl<T1, D1>(D1 d1) :
    ISeraVision<ValueTuple<T1>>, ISeraVision<Tuple<T1>>
    where D1 : ISeraVision<T1>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, ValueTuple<T1> value) where V : ASeraVisitor<R>
        => visitor.VTuple(new Value(d1), value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, Tuple<T1> value) where V : ASeraVisitor<R>
        => visitor.VTuple(new Class(d1), value);

    public readonly struct Value(D1 dep) : ITupleSeraVision<ValueTuple<T1>>
    {
        public int Size => 1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R AcceptItem<R, V>(V visitor, ValueTuple<T1> value, int index) where V : ATupleSeraVisitor<R>
            => index switch
            {
                0 => visitor.VItem(dep, value.Item1),
                _ => visitor.VNone(),
            };
    }

    public readonly struct Class(D1 dep) : ITupleSeraVision<Tuple<T1>>
    {
        public int Size => 1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R AcceptItem<R, V>(V visitor, Tuple<T1> value, int index) where V : ATupleSeraVisitor<R>
            => index switch
            {
                0 => visitor.VItem(dep, value.Item1),
                _ => visitor.VNone(),
            };
    }
}

#endregion

#region Size 2

public readonly struct TupleImpl<T1, T2, D1, D2>(D1 d1, D2 d2) :
    ISeraVision<ValueTuple<T1, T2>>, ISeraVision<Tuple<T1, T2>>
    where D1 : ISeraVision<T1>
    where D2 : ISeraVision<T2>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, ValueTuple<T1, T2> value) where V : ASeraVisitor<R>
        => visitor.VTuple(new Value((d1, d2)), value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, Tuple<T1, T2> value) where V : ASeraVisitor<R>
        => visitor.VTuple(new Class((d1, d2)), value);

    public readonly struct Value((D1, D2) dep) : ITupleSeraVision<ValueTuple<T1, T2>>
    {
        public int Size => 2;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R AcceptItem<R, V>(V visitor, ValueTuple<T1, T2> value, int index) where V : ATupleSeraVisitor<R>
            => index switch
            {
                0 => visitor.VItem(dep.Item1, value.Item1),
                1 => visitor.VItem(dep.Item2, value.Item2),
                _ => visitor.VNone(),
            };
    }

    public readonly struct Class((D1, D2) dep) : ITupleSeraVision<Tuple<T1, T2>>
    {
        public int Size => 2;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R AcceptItem<R, V>(V visitor, Tuple<T1, T2> value, int index) where V : ATupleSeraVisitor<R>
            => index switch
            {
                0 => visitor.VItem(dep.Item1, value.Item1),
                1 => visitor.VItem(dep.Item2, value.Item2),
                _ => visitor.VNone(),
            };
    }
}

#endregion

#region Size 3

public readonly struct TupleImpl<T1, T2, T3, D1, D2, D3>(D1 d1, D2 d2, D3 d3) :
    ISeraVision<ValueTuple<T1, T2, T3>>, ISeraVision<Tuple<T1, T2, T3>>
    where D1 : ISeraVision<T1>
    where D2 : ISeraVision<T2>
    where D3 : ISeraVision<T3>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, ValueTuple<T1, T2, T3> value) where V : ASeraVisitor<R>
        => visitor.VTuple(new Value((d1, d2, d3)), value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, Tuple<T1, T2, T3> value) where V : ASeraVisitor<R>
        => visitor.VTuple(new Class((d1, d2, d3)), value);

    public readonly struct Value((D1, D2, D3) dep) : ITupleSeraVision<ValueTuple<T1, T2, T3>>
    {
        public int Size => 3;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R AcceptItem<R, V>(V visitor, ValueTuple<T1, T2, T3> value, int index) where V : ATupleSeraVisitor<R>
            => index switch
            {
                0 => visitor.VItem(dep.Item1, value.Item1),
                1 => visitor.VItem(dep.Item2, value.Item2),
                2 => visitor.VItem(dep.Item3, value.Item3),
                _ => visitor.VNone(),
            };
    }

    public readonly struct Class((D1, D2, D3) dep) : ITupleSeraVision<Tuple<T1, T2, T3>>
    {
        public int Size => 3;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R AcceptItem<R, V>(V visitor, Tuple<T1, T2, T3> value, int index) where V : ATupleSeraVisitor<R>
            => index switch
            {
                0 => visitor.VItem(dep.Item1, value.Item1),
                1 => visitor.VItem(dep.Item2, value.Item2),
                2 => visitor.VItem(dep.Item3, value.Item3),
                _ => visitor.VNone(),
            };
    }
}

#endregion

#region Size 4

public readonly struct TupleImpl<T1, T2, T3, T4, D1, D2, D3, D4>(D1 d1, D2 d2, D3 d3, D4 d4) :
    ISeraVision<ValueTuple<T1, T2, T3, T4>>, ISeraVision<Tuple<T1, T2, T3, T4>>
    where D1 : ISeraVision<T1>
    where D2 : ISeraVision<T2>
    where D3 : ISeraVision<T3>
    where D4 : ISeraVision<T4>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, ValueTuple<T1, T2, T3, T4> value) where V : ASeraVisitor<R>
        => visitor.VTuple(new Value((d1, d2, d3, d4)), value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, Tuple<T1, T2, T3, T4> value) where V : ASeraVisitor<R>
        => visitor.VTuple(new Class((d1, d2, d3, d4)), value);

    public readonly struct Value((D1, D2, D3, D4) dep) : ITupleSeraVision<ValueTuple<T1, T2, T3, T4>>
    {
        public int Size => 4;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R AcceptItem<R, V>(V visitor, ValueTuple<T1, T2, T3, T4> value, int index) where V : ATupleSeraVisitor<R>
            => index switch
            {
                0 => visitor.VItem(dep.Item1, value.Item1),
                1 => visitor.VItem(dep.Item2, value.Item2),
                2 => visitor.VItem(dep.Item3, value.Item3),
                3 => visitor.VItem(dep.Item4, value.Item4),
                _ => visitor.VNone(),
            };
    }

    public readonly struct Class((D1, D2, D3, D4) dep) : ITupleSeraVision<Tuple<T1, T2, T3, T4>>
    {
        public int Size => 4;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R AcceptItem<R, V>(V visitor, Tuple<T1, T2, T3, T4> value, int index) where V : ATupleSeraVisitor<R>
            => index switch
            {
                0 => visitor.VItem(dep.Item1, value.Item1),
                1 => visitor.VItem(dep.Item2, value.Item2),
                2 => visitor.VItem(dep.Item3, value.Item3),
                3 => visitor.VItem(dep.Item4, value.Item4),
                _ => visitor.VNone(),
            };
    }
}

#endregion

#region Size 5

public readonly struct TupleImpl<T1, T2, T3, T4, T5, D1, D2, D3, D4, D5>(D1 d1, D2 d2, D3 d3, D4 d4, D5 d5) :
    ISeraVision<ValueTuple<T1, T2, T3, T4, T5>>, ISeraVision<Tuple<T1, T2, T3, T4, T5>>
    where D1 : ISeraVision<T1>
    where D2 : ISeraVision<T2>
    where D3 : ISeraVision<T3>
    where D4 : ISeraVision<T4>
    where D5 : ISeraVision<T5>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, ValueTuple<T1, T2, T3, T4, T5> value) where V : ASeraVisitor<R>
        => visitor.VTuple(new Value((d1, d2, d3, d4, d5)), value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, Tuple<T1, T2, T3, T4, T5> value) where V : ASeraVisitor<R>
        => visitor.VTuple(new Class((d1, d2, d3, d4, d5)), value);

    public readonly struct Value((D1, D2, D3, D4, D5) dep) : ITupleSeraVision<ValueTuple<T1, T2, T3, T4, T5>>
    {
        public int Size => 5;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R AcceptItem<R, V>(V visitor, ValueTuple<T1, T2, T3, T4, T5> value, int index)
            where V : ATupleSeraVisitor<R>
            => index switch
            {
                0 => visitor.VItem(dep.Item1, value.Item1),
                1 => visitor.VItem(dep.Item2, value.Item2),
                2 => visitor.VItem(dep.Item3, value.Item3),
                3 => visitor.VItem(dep.Item4, value.Item4),
                4 => visitor.VItem(dep.Item5, value.Item5),
                _ => visitor.VNone(),
            };
    }

    public readonly struct Class((D1, D2, D3, D4, D5) dep) : ITupleSeraVision<Tuple<T1, T2, T3, T4, T5>>
    {
        public int Size => 5;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R AcceptItem<R, V>(V visitor, Tuple<T1, T2, T3, T4, T5> value, int index) where V : ATupleSeraVisitor<R>
            => index switch
            {
                0 => visitor.VItem(dep.Item1, value.Item1),
                1 => visitor.VItem(dep.Item2, value.Item2),
                2 => visitor.VItem(dep.Item3, value.Item3),
                3 => visitor.VItem(dep.Item4, value.Item4),
                4 => visitor.VItem(dep.Item5, value.Item5),
                _ => visitor.VNone(),
            };
    }
}

#endregion

#region Size 6

public readonly struct TupleImpl<T1, T2, T3, T4, T5, T6, D1, D2, D3, D4, D5, D6>
    (D1 d1, D2 d2, D3 d3, D4 d4, D5 d5, D6 d6) :
        ISeraVision<ValueTuple<T1, T2, T3, T4, T5, T6>>,
        ISeraVision<Tuple<T1, T2, T3, T4, T5, T6>>
    where D1 : ISeraVision<T1>
    where D2 : ISeraVision<T2>
    where D3 : ISeraVision<T3>
    where D4 : ISeraVision<T4>
    where D5 : ISeraVision<T5>
    where D6 : ISeraVision<T6>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, ValueTuple<T1, T2, T3, T4, T5, T6> value) where V : ASeraVisitor<R>
        => visitor.VTuple(new Value((d1, d2, d3, d4, d5, d6)), value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, Tuple<T1, T2, T3, T4, T5, T6> value) where V : ASeraVisitor<R>
        => visitor.VTuple(new Class((d1, d2, d3, d4, d5, d6)), value);

    public readonly struct Value(
        (D1, D2, D3, D4, D5, D6) dep
    ) : ITupleSeraVision<ValueTuple<T1, T2, T3, T4, T5, T6>>
    {
        public int Size => 6;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R AcceptItem<R, V>(V visitor, ValueTuple<T1, T2, T3, T4, T5, T6> value, int index)
            where V : ATupleSeraVisitor<R>
            => index switch
            {
                0 => visitor.VItem(dep.Item1, value.Item1),
                1 => visitor.VItem(dep.Item2, value.Item2),
                2 => visitor.VItem(dep.Item3, value.Item3),
                3 => visitor.VItem(dep.Item4, value.Item4),
                4 => visitor.VItem(dep.Item5, value.Item5),
                5 => visitor.VItem(dep.Item6, value.Item6),
                _ => visitor.VNone(),
            };
    }

    public readonly struct Class(
        (D1, D2, D3, D4, D5, D6) dep
    ) : ITupleSeraVision<Tuple<T1, T2, T3, T4, T5, T6>>
    {
        public int Size => 6;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R AcceptItem<R, V>(V visitor, Tuple<T1, T2, T3, T4, T5, T6> value, int index)
            where V : ATupleSeraVisitor<R>
            => index switch
            {
                0 => visitor.VItem(dep.Item1, value.Item1),
                1 => visitor.VItem(dep.Item2, value.Item2),
                2 => visitor.VItem(dep.Item3, value.Item3),
                3 => visitor.VItem(dep.Item4, value.Item4),
                4 => visitor.VItem(dep.Item5, value.Item5),
                5 => visitor.VItem(dep.Item6, value.Item6),
                _ => visitor.VNone(),
            };
    }
}

#endregion

#region Size 7

public readonly struct TupleImpl<T1, T2, T3, T4, T5, T6, T7, D1, D2, D3, D4, D5, D6, D7>
    (D1 d1, D2 d2, D3 d3, D4 d4, D5 d5, D6 d6, D7 d7) :
        ISeraVision<ValueTuple<T1, T2, T3, T4, T5, T6, T7>>,
        ISeraVision<Tuple<T1, T2, T3, T4, T5, T6, T7>>
    where D1 : ISeraVision<T1>
    where D2 : ISeraVision<T2>
    where D3 : ISeraVision<T3>
    where D4 : ISeraVision<T4>
    where D5 : ISeraVision<T5>
    where D6 : ISeraVision<T6>
    where D7 : ISeraVision<T7>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, ValueTuple<T1, T2, T3, T4, T5, T6, T7> value) where V : ASeraVisitor<R>
        => visitor.VTuple(new Value((d1, d2, d3, d4, d5, d6, d7)), value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, Tuple<T1, T2, T3, T4, T5, T6, T7> value) where V : ASeraVisitor<R>
        => visitor.VTuple(new Class((d1, d2, d3, d4, d5, d6, d7)), value);

    public readonly struct Value(
        (D1, D2, D3, D4, D5, D6, D7) dep
    ) : ITupleSeraVision<ValueTuple<T1, T2, T3, T4, T5, T6, T7>>
    {
        public int Size => 7;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R AcceptItem<R, V>(V visitor, ValueTuple<T1, T2, T3, T4, T5, T6, T7> value, int index)
            where V : ATupleSeraVisitor<R>
            => index switch
            {
                0 => visitor.VItem(dep.Item1, value.Item1),
                1 => visitor.VItem(dep.Item2, value.Item2),
                2 => visitor.VItem(dep.Item3, value.Item3),
                3 => visitor.VItem(dep.Item4, value.Item4),
                4 => visitor.VItem(dep.Item5, value.Item5),
                5 => visitor.VItem(dep.Item6, value.Item6),
                6 => visitor.VItem(dep.Item7, value.Item7),
                _ => visitor.VNone(),
            };
    }

    public readonly struct Class(
        (D1, D2, D3, D4, D5, D6, D7) dep
    ) : ITupleSeraVision<Tuple<T1, T2, T3, T4, T5, T6, T7>>
    {
        public int Size => 7;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R AcceptItem<R, V>(V visitor, Tuple<T1, T2, T3, T4, T5, T6, T7> value, int index)
            where V : ATupleSeraVisitor<R>
            => index switch
            {
                0 => visitor.VItem(dep.Item1, value.Item1),
                1 => visitor.VItem(dep.Item2, value.Item2),
                2 => visitor.VItem(dep.Item3, value.Item3),
                3 => visitor.VItem(dep.Item4, value.Item4),
                4 => visitor.VItem(dep.Item5, value.Item5),
                5 => visitor.VItem(dep.Item6, value.Item6),
                6 => visitor.VItem(dep.Item7, value.Item7),
                _ => visitor.VNone(),
            };
    }
}

#endregion

#region Size >= 8

public readonly struct TupleRestValueImpl<
        T1, T2, T3, T4, T5, T6, T7, TR,
        D1, D2, D3, D4, D5, D6, D7, DR
    >
    (D1 d1, D2 d2, D3 d3, D4 d4, D5 d5, D6 d6, D7 d7, DR dr, int size) :
        ISeraVision<ValueTuple<T1, T2, T3, T4, T5, T6, T7, TR>>
    where D1 : ISeraVision<T1>
    where D2 : ISeraVision<T2>
    where D3 : ISeraVision<T3>
    where D4 : ISeraVision<T4>
    where D5 : ISeraVision<T5>
    where D6 : ISeraVision<T6>
    where D7 : ISeraVision<T7>
    where TR : struct
    where DR : ITupleSeraVision<TR>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, ValueTuple<T1, T2, T3, T4, T5, T6, T7, TR> value) where V : ASeraVisitor<R>
        => visitor.VTuple(new Impl((d1, d2, d3, d4, d5, d6, d7), dr, size), value);

    public readonly struct Impl(
        (D1, D2, D3, D4, D5, D6, D7) dep,
        DR dr, int size
    ) : ITupleSeraVision<ValueTuple<T1, T2, T3, T4, T5, T6, T7, TR>>
    {
        public int Size => size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R AcceptItem<R, V>(V visitor, ValueTuple<T1, T2, T3, T4, T5, T6, T7, TR> value, int index)
            where V : ATupleSeraVisitor<R>
            => index switch
            {
                0 => visitor.VItem(dep.Item1, value.Item1),
                1 => visitor.VItem(dep.Item2, value.Item2),
                2 => visitor.VItem(dep.Item3, value.Item3),
                3 => visitor.VItem(dep.Item4, value.Item4),
                4 => visitor.VItem(dep.Item5, value.Item5),
                5 => visitor.VItem(dep.Item6, value.Item6),
                6 => visitor.VItem(dep.Item7, value.Item7),
                >= 7 => dr.AcceptItem<R, V>(visitor, value.Rest, index - 7),
                _ => visitor.VNone(),
            };
    }
}

public readonly struct TupleRestClassImpl<
        T1, T2, T3, T4, T5, T6, T7, TR,
        D1, D2, D3, D4, D5, D6, D7, DR
    >
    (D1 d1, D2 d2, D3 d3, D4 d4, D5 d5, D6 d6, D7 d7, DR dr, int size) :
        ISeraVision<Tuple<T1, T2, T3, T4, T5, T6, T7, TR>>
    where D1 : ISeraVision<T1>
    where D2 : ISeraVision<T2>
    where D3 : ISeraVision<T3>
    where D4 : ISeraVision<T4>
    where D5 : ISeraVision<T5>
    where D6 : ISeraVision<T6>
    where D7 : ISeraVision<T7>
    where TR : notnull
    where DR : ITupleSeraVision<TR>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, Tuple<T1, T2, T3, T4, T5, T6, T7, TR> value) where V : ASeraVisitor<R>
        => visitor.VTuple(new Impl((d1, d2, d3, d4, d5, d6, d7), dr, size), value);

    public readonly struct Impl(
        (D1, D2, D3, D4, D5, D6, D7) dep,
        DR dr, int size
    ) : ITupleSeraVision<Tuple<T1, T2, T3, T4, T5, T6, T7, TR>>
    {
        public int Size => size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R AcceptItem<R, V>(V visitor, Tuple<T1, T2, T3, T4, T5, T6, T7, TR> value, int index)
            where V : ATupleSeraVisitor<R>
            => index switch
            {
                0 => visitor.VItem(dep.Item1, value.Item1),
                1 => visitor.VItem(dep.Item2, value.Item2),
                2 => visitor.VItem(dep.Item3, value.Item3),
                3 => visitor.VItem(dep.Item4, value.Item4),
                4 => visitor.VItem(dep.Item5, value.Item5),
                5 => visitor.VItem(dep.Item6, value.Item6),
                6 => visitor.VItem(dep.Item7, value.Item7),
                >= 7 => dr.AcceptItem<R, V>(visitor, value.Rest, index - 7),
                _ => visitor.VNone(),
            };
    }
}

#endregion

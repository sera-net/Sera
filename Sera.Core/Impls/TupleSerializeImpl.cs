﻿using System;
using System.Threading.Tasks;
using Sera.Core.Impls.Deps;
using Sera.Core.Ser;

namespace Sera.Core.Impls.Tuples;

#region SyncBase

public abstract record TupleSerializeImplBase<T1> :
    ISerialize<Tuple<T1>>, ISeqSerializerReceiver<Tuple<T1>>
{
    public abstract void Write<S>(S serializer, Tuple<T1> value, ISeraOptions options) where S : ISerializer;
    public abstract void Receive<S>(Tuple<T1> value, S serializer) where S : ISeqSerializer;
}

public abstract record TupleSerializeImplBase<T1, T2> :
    ISerialize<Tuple<T1, T2>>, ISeqSerializerReceiver<Tuple<T1, T2>>
{
    public abstract void Write<S>(S serializer, Tuple<T1, T2> value, ISeraOptions options) where S : ISerializer;
    public abstract void Receive<S>(Tuple<T1, T2> value, S serializer) where S : ISeqSerializer;
}

public abstract record TupleSerializeImplBase<T1, T2, T3> :
    ISerialize<Tuple<T1, T2, T3>>, ISeqSerializerReceiver<Tuple<T1, T2, T3>>
{
    public abstract void Write<S>(S serializer, Tuple<T1, T2, T3> value, ISeraOptions options)
        where S : ISerializer;

    public abstract void Receive<S>(Tuple<T1, T2, T3> value, S serializer) where S : ISeqSerializer;
}

public abstract record TupleSerializeImplBase<T1, T2, T3, T4> :
    ISerialize<Tuple<T1, T2, T3, T4>>, ISeqSerializerReceiver<Tuple<T1, T2, T3, T4>>
{
    public abstract void Write<S>(S serializer, Tuple<T1, T2, T3, T4> value, ISeraOptions options)
        where S : ISerializer;

    public abstract void Receive<S>(Tuple<T1, T2, T3, T4> value, S serializer) where S : ISeqSerializer;
}

public abstract record TupleSerializeImplBase<T1, T2, T3, T4, T5> :
    ISerialize<Tuple<T1, T2, T3, T4, T5>>, ISeqSerializerReceiver<Tuple<T1, T2, T3, T4, T5>>
{
    public abstract void Write<S>(S serializer, Tuple<T1, T2, T3, T4, T5> value, ISeraOptions options)
        where S : ISerializer;

    public abstract void Receive<S>(Tuple<T1, T2, T3, T4, T5> value, S serializer) where S : ISeqSerializer;
}

public abstract record TupleSerializeImplBase<T1, T2, T3, T4, T5, T6> :
    ISerialize<Tuple<T1, T2, T3, T4, T5, T6>>, ISeqSerializerReceiver<Tuple<T1, T2, T3, T4, T5, T6>>
{
    public abstract void Write<S>(S serializer, Tuple<T1, T2, T3, T4, T5, T6> value, ISeraOptions options)
        where S : ISerializer;

    public abstract void Receive<S>(Tuple<T1, T2, T3, T4, T5, T6> value, S serializer) where S : ISeqSerializer;
}

public abstract record TupleSerializeImplBase<T1, T2, T3, T4, T5, T6, T7> :
    ISerialize<Tuple<T1, T2, T3, T4, T5, T6, T7>>, ISeqSerializerReceiver<Tuple<T1, T2, T3, T4, T5, T6, T7>>
{
    public abstract void Write<S>(S serializer, Tuple<T1, T2, T3, T4, T5, T6, T7> value, ISeraOptions options)
        where S : ISerializer;

    public abstract void Receive<S>(Tuple<T1, T2, T3, T4, T5, T6, T7> value, S serializer)
        where S : ISeqSerializer;
}

public abstract record TupleRestSerializeImplBase<T1, T2, T3, T4, T5, T6, T7, TR> :
    ISerialize<Tuple<T1, T2, T3, T4, T5, T6, T7, TR>>,
    ISeqSerializerReceiver<Tuple<T1, T2, T3, T4, T5, T6, T7, TR>>
    where TR : notnull
{
    public abstract void Write<S>(S serializer, Tuple<T1, T2, T3, T4, T5, T6, T7, TR> value, ISeraOptions options)
        where S : ISerializer;

    public abstract void Receive<S>(Tuple<T1, T2, T3, T4, T5, T6, T7, TR> value, S serializer)
        where S : ISeqSerializer;
}

#endregion

#region Sync Abstract Impl

public abstract record TupleSerializeImplAbstract<T1, ST1> :
    TupleSerializeImplBase<T1>
    where ST1 : ISerialize<T1>
{
    public abstract ST1 Serialize1 { get; }

    public override void Write<S>(S serializer, Tuple<T1> value, ISeraOptions options)
        => serializer.StartSeq(1, value, this);

    public override void Receive<S>(Tuple<T1> value, S serializer)
    {
        serializer.WriteElement(value.Item1, Serialize1);
    }
}

public abstract record TupleSerializeImplAbstract<T1, T2, ST1, ST2> :
    TupleSerializeImplBase<T1, T2>
    where ST1 : ISerialize<T1> where ST2 : ISerialize<T2>
{
    public abstract ST1 Serialize1 { get; }
    public abstract ST2 Serialize2 { get; }

    public override void Write<S>(S serializer, Tuple<T1, T2> value, ISeraOptions options)
        => serializer.StartSeq(2, value, this);

    public override void Receive<S>(Tuple<T1, T2> value, S serializer)
    {
        serializer.WriteElement(value.Item1, Serialize1);
        serializer.WriteElement(value.Item2, Serialize2);
    }
}

public abstract record TupleSerializeImplAbstract<T1, T2, T3, ST1, ST2, ST3> :
    TupleSerializeImplBase<T1, T2, T3>
    where ST1 : ISerialize<T1> where ST2 : ISerialize<T2> where ST3 : ISerialize<T3>
{
    public abstract ST1 Serialize1 { get; }
    public abstract ST2 Serialize2 { get; }
    public abstract ST3 Serialize3 { get; }

    public override void Write<S>(S serializer, Tuple<T1, T2, T3> value, ISeraOptions options)
        => serializer.StartSeq(3, value, this);

    public override void Receive<S>(Tuple<T1, T2, T3> value, S serializer)
    {
        serializer.WriteElement(value.Item1, Serialize1);
        serializer.WriteElement(value.Item2, Serialize2);
        serializer.WriteElement(value.Item3, Serialize3);
    }
}

public abstract record TupleSerializeImplAbstract<
    T1, T2, T3, T4,
    ST1, ST2, ST3, ST4
> :
    TupleSerializeImplBase<T1, T2, T3, T4>
    where ST1 : ISerialize<T1> where ST2 : ISerialize<T2> where ST3 : ISerialize<T3> where ST4 : ISerialize<T4>
{
    public abstract ST1 Serialize1 { get; }
    public abstract ST2 Serialize2 { get; }
    public abstract ST3 Serialize3 { get; }
    public abstract ST4 Serialize4 { get; }

    public override void Write<S>(S serializer, Tuple<T1, T2, T3, T4> value, ISeraOptions options)
        => serializer.StartSeq(4, value, this);

    public override void Receive<S>(Tuple<T1, T2, T3, T4> value, S serializer)
    {
        serializer.WriteElement(value.Item1, Serialize1);
        serializer.WriteElement(value.Item2, Serialize2);
        serializer.WriteElement(value.Item3, Serialize3);
        serializer.WriteElement(value.Item4, Serialize4);
    }
}

public abstract record TupleSerializeImplAbstract<
    T1, T2, T3, T4, T5,
    ST1, ST2, ST3, ST4, ST5
> :
    TupleSerializeImplBase<T1, T2, T3, T4, T5>
    where ST1 : ISerialize<T1>
    where ST2 : ISerialize<T2>
    where ST3 : ISerialize<T3>
    where ST4 : ISerialize<T4>
    where ST5 : ISerialize<T5>
{
    public abstract ST1 Serialize1 { get; }
    public abstract ST2 Serialize2 { get; }
    public abstract ST3 Serialize3 { get; }
    public abstract ST4 Serialize4 { get; }
    public abstract ST5 Serialize5 { get; }

    public override void Write<S>(S serializer, Tuple<T1, T2, T3, T4, T5> value, ISeraOptions options)
        => serializer.StartSeq(5, value, this);

    public override void Receive<S>(Tuple<T1, T2, T3, T4, T5> value, S serializer)
    {
        serializer.WriteElement(value.Item1, Serialize1);
        serializer.WriteElement(value.Item2, Serialize2);
        serializer.WriteElement(value.Item3, Serialize3);
        serializer.WriteElement(value.Item4, Serialize4);
        serializer.WriteElement(value.Item5, Serialize5);
    }
}

public abstract record TupleSerializeImplAbstract<
    T1, T2, T3, T4, T5, T6,
    ST1, ST2, ST3, ST4, ST5, ST6
> :
    TupleSerializeImplBase<T1, T2, T3, T4, T5, T6>
    where ST1 : ISerialize<T1>
    where ST2 : ISerialize<T2>
    where ST3 : ISerialize<T3>
    where ST4 : ISerialize<T4>
    where ST5 : ISerialize<T5>
    where ST6 : ISerialize<T6>
{
    public abstract ST1 Serialize1 { get; }
    public abstract ST2 Serialize2 { get; }
    public abstract ST3 Serialize3 { get; }
    public abstract ST4 Serialize4 { get; }
    public abstract ST5 Serialize5 { get; }
    public abstract ST6 Serialize6 { get; }

    public override void Write<S>(S serializer, Tuple<T1, T2, T3, T4, T5, T6> value, ISeraOptions options)
        => serializer.StartSeq(6, value, this);

    public override void Receive<S>(Tuple<T1, T2, T3, T4, T5, T6> value, S serializer)
    {
        serializer.WriteElement(value.Item1, Serialize1);
        serializer.WriteElement(value.Item2, Serialize2);
        serializer.WriteElement(value.Item3, Serialize3);
        serializer.WriteElement(value.Item4, Serialize4);
        serializer.WriteElement(value.Item5, Serialize5);
        serializer.WriteElement(value.Item6, Serialize6);
    }
}

public abstract record TupleSerializeImplAbstract<
    T1, T2, T3, T4, T5, T6, T7,
    ST1, ST2, ST3, ST4, ST5, ST6, ST7
> :
    TupleSerializeImplBase<T1, T2, T3, T4, T5, T6, T7>
    where ST1 : ISerialize<T1>
    where ST2 : ISerialize<T2>
    where ST3 : ISerialize<T3>
    where ST4 : ISerialize<T4>
    where ST5 : ISerialize<T5>
    where ST6 : ISerialize<T6>
    where ST7 : ISerialize<T7>
{
    public abstract ST1 Serialize1 { get; }
    public abstract ST2 Serialize2 { get; }
    public abstract ST3 Serialize3 { get; }
    public abstract ST4 Serialize4 { get; }
    public abstract ST5 Serialize5 { get; }
    public abstract ST6 Serialize6 { get; }
    public abstract ST7 Serialize7 { get; }

    public override void Write<S>(S serializer, Tuple<T1, T2, T3, T4, T5, T6, T7> value, ISeraOptions options)
        => serializer.StartSeq(7, value, this);

    public override void Receive<S>(Tuple<T1, T2, T3, T4, T5, T6, T7> value, S serializer)
    {
        serializer.WriteElement(value.Item1, Serialize1);
        serializer.WriteElement(value.Item2, Serialize2);
        serializer.WriteElement(value.Item3, Serialize3);
        serializer.WriteElement(value.Item4, Serialize4);
        serializer.WriteElement(value.Item5, Serialize5);
        serializer.WriteElement(value.Item6, Serialize6);
        serializer.WriteElement(value.Item7, Serialize7);
    }
}

public abstract record TupleRestSerializeImplAbstract<
    T1, T2, T3, T4, T5, T6, T7, TR,
    ST1, ST2, ST3, ST4, ST5, ST6, ST7, RTR
>(nuint Size) :
    TupleRestSerializeImplBase<T1, T2, T3, T4, T5, T6, T7, TR>
    where ST1 : ISerialize<T1>
    where ST2 : ISerialize<T2>
    where ST3 : ISerialize<T3>
    where ST4 : ISerialize<T4>
    where ST5 : ISerialize<T5>
    where ST6 : ISerialize<T6>
    where ST7 : ISerialize<T7>
    where TR : notnull
    where RTR : ISeqSerializerReceiver<TR>
{
    public abstract ST1 Serialize1 { get; }
    public abstract ST2 Serialize2 { get; }
    public abstract ST3 Serialize3 { get; }
    public abstract ST4 Serialize4 { get; }
    public abstract ST5 Serialize5 { get; }
    public abstract ST6 Serialize6 { get; }
    public abstract ST7 Serialize7 { get; }
    public abstract RTR Rest { get; }

    public override void Write<S>(S serializer, Tuple<T1, T2, T3, T4, T5, T6, T7, TR> value, ISeraOptions options)
        => serializer.StartSeq(Size, value, this);

    public override void Receive<S>(Tuple<T1, T2, T3, T4, T5, T6, T7, TR> value, S serializer)
    {
        serializer.WriteElement(value.Item1, Serialize1);
        serializer.WriteElement(value.Item2, Serialize2);
        serializer.WriteElement(value.Item3, Serialize3);
        serializer.WriteElement(value.Item4, Serialize4);
        serializer.WriteElement(value.Item5, Serialize5);
        serializer.WriteElement(value.Item6, Serialize6);
        serializer.WriteElement(value.Item7, Serialize7);
        Rest.Receive(value.Rest, serializer);
    }
}

#endregion

#region Sync Record Impl

public record TupleSerializeImpl<T1, ST1>(ST1 Serialize1) :
    TupleSerializeImplAbstract<T1, ST1>
    where ST1 : ISerialize<T1>
{
    public override ST1 Serialize1 { get; } = Serialize1;
}

public record TupleSerializeImpl<T1, T2, ST1, ST2>(ST1 Serialize1, ST2 Serialize2) :
    TupleSerializeImplAbstract<T1, T2, ST1, ST2>
    where ST1 : ISerialize<T1> where ST2 : ISerialize<T2>
{
    public override ST1 Serialize1 { get; } = Serialize1;
    public override ST2 Serialize2 { get; } = Serialize2;
}

public record TupleSerializeImpl<T1, T2, T3, ST1, ST2, ST3>(ST1 Serialize1, ST2 Serialize2, ST3 Serialize3) :
    TupleSerializeImplAbstract<T1, T2, T3, ST1, ST2, ST3>
    where ST1 : ISerialize<T1> where ST2 : ISerialize<T2> where ST3 : ISerialize<T3>
{
    public override ST1 Serialize1 { get; } = Serialize1;
    public override ST2 Serialize2 { get; } = Serialize2;
    public override ST3 Serialize3 { get; } = Serialize3;
}

public record TupleSerializeImpl<
    T1, T2, T3, T4,
    ST1, ST2, ST3, ST4
>(ST1 Serialize1, ST2 Serialize2, ST3 Serialize3, ST4 Serialize4) :
    TupleSerializeImplAbstract<
        T1, T2, T3, T4,
        ST1, ST2, ST3, ST4
    >
    where ST1 : ISerialize<T1> where ST2 : ISerialize<T2> where ST3 : ISerialize<T3> where ST4 : ISerialize<T4>
{
    public override ST1 Serialize1 { get; } = Serialize1;
    public override ST2 Serialize2 { get; } = Serialize2;
    public override ST3 Serialize3 { get; } = Serialize3;
    public override ST4 Serialize4 { get; } = Serialize4;
}

public record TupleSerializeImpl<
    T1, T2, T3, T4, T5,
    ST1, ST2, ST3, ST4, ST5
>(ST1 Serialize1, ST2 Serialize2, ST3 Serialize3, ST4 Serialize4, ST5 Serialize5) :
    TupleSerializeImplAbstract<
        T1, T2, T3, T4, T5,
        ST1, ST2, ST3, ST4, ST5
    >
    where ST1 : ISerialize<T1>
    where ST2 : ISerialize<T2>
    where ST3 : ISerialize<T3>
    where ST4 : ISerialize<T4>
    where ST5 : ISerialize<T5>
{
    public override ST1 Serialize1 { get; } = Serialize1;
    public override ST2 Serialize2 { get; } = Serialize2;
    public override ST3 Serialize3 { get; } = Serialize3;
    public override ST4 Serialize4 { get; } = Serialize4;
    public override ST5 Serialize5 { get; } = Serialize5;
}

public record TupleSerializeImpl<
    T1, T2, T3, T4, T5, T6,
    ST1, ST2, ST3, ST4, ST5, ST6
>(ST1 Serialize1, ST2 Serialize2, ST3 Serialize3, ST4 Serialize4, ST5 Serialize5, ST6 Serialize6) :
    TupleSerializeImplAbstract<
        T1, T2, T3, T4, T5, T6,
        ST1, ST2, ST3, ST4, ST5, ST6
    >
    where ST1 : ISerialize<T1>
    where ST2 : ISerialize<T2>
    where ST3 : ISerialize<T3>
    where ST4 : ISerialize<T4>
    where ST5 : ISerialize<T5>
    where ST6 : ISerialize<T6>
{
    public override ST1 Serialize1 { get; } = Serialize1;
    public override ST2 Serialize2 { get; } = Serialize2;
    public override ST3 Serialize3 { get; } = Serialize3;
    public override ST4 Serialize4 { get; } = Serialize4;
    public override ST5 Serialize5 { get; } = Serialize5;
    public override ST6 Serialize6 { get; } = Serialize6;
}

public record TupleSerializeImpl<
    T1, T2, T3, T4, T5, T6, T7,
    ST1, ST2, ST3, ST4, ST5, ST6, ST7
>(ST1 Serialize1, ST2 Serialize2, ST3 Serialize3, ST4 Serialize4, ST5 Serialize5, ST6 Serialize6, ST7 Serialize7) :
    TupleSerializeImplAbstract<
        T1, T2, T3, T4, T5, T6, T7,
        ST1, ST2, ST3, ST4, ST5, ST6, ST7
    >
    where ST1 : ISerialize<T1>
    where ST2 : ISerialize<T2>
    where ST3 : ISerialize<T3>
    where ST4 : ISerialize<T4>
    where ST5 : ISerialize<T5>
    where ST6 : ISerialize<T6>
    where ST7 : ISerialize<T7>
{
    public override ST1 Serialize1 { get; } = Serialize1;
    public override ST2 Serialize2 { get; } = Serialize2;
    public override ST3 Serialize3 { get; } = Serialize3;
    public override ST4 Serialize4 { get; } = Serialize4;
    public override ST5 Serialize5 { get; } = Serialize5;
    public override ST6 Serialize6 { get; } = Serialize6;
    public override ST7 Serialize7 { get; } = Serialize7;
}

public record TupleRestSerializeImpl<
    T1, T2, T3, T4, T5, T6, T7, TR,
    ST1, ST2, ST3, ST4, ST5, ST6, ST7, RTR
>(
    ST1 Serialize1, ST2 Serialize2, ST3 Serialize3, ST4 Serialize4, ST5 Serialize5, ST6 Serialize6, ST7 Serialize7,
    RTR Rest, nuint Size
) :
    TupleRestSerializeImplAbstract<
        T1, T2, T3, T4, T5, T6, T7, TR,
        ST1, ST2, ST3, ST4, ST5, ST6, ST7, RTR
    >(Size)
    where ST1 : ISerialize<T1>
    where ST2 : ISerialize<T2>
    where ST3 : ISerialize<T3>
    where ST4 : ISerialize<T4>
    where ST5 : ISerialize<T5>
    where ST6 : ISerialize<T6>
    where ST7 : ISerialize<T7>
    where TR : notnull
    where RTR : ISeqSerializerReceiver<TR>
{
    public override ST1 Serialize1 { get; } = Serialize1;
    public override ST2 Serialize2 { get; } = Serialize2;
    public override ST3 Serialize3 { get; } = Serialize3;
    public override ST4 Serialize4 { get; } = Serialize4;
    public override ST5 Serialize5 { get; } = Serialize5;
    public override ST6 Serialize6 { get; } = Serialize6;
    public override ST7 Serialize7 { get; } = Serialize7;
    public override RTR Rest { get; } = Rest;
}

#endregion

#region Sync Deps Impl

public record TupleSerializeDepsImpl<T1, ST1, D> :
    TupleSerializeImplAbstract<T1, ST1>
    where ST1 : ISerialize<T1>
    where D : IDepsContainer<ST1>
{
    public override ST1 Serialize1 => D.Impl1!;
}

public record TupleSerializeDepsImpl<T1, T2, ST1, ST2, D> :
    TupleSerializeImplAbstract<T1, T2, ST1, ST2>
    where ST1 : ISerialize<T1>
    where ST2 : ISerialize<T2>
    where D : IDepsContainer<ST1, ST2>
{
    public override ST1 Serialize1 => D.Impl1!;
    public override ST2 Serialize2 => D.Impl2!;
}

public record TupleSerializeDepsImpl<T1, T2, T3, ST1, ST2, ST3, D> :
    TupleSerializeImplAbstract<T1, T2, T3, ST1, ST2, ST3>
    where ST1 : ISerialize<T1>
    where ST2 : ISerialize<T2>
    where ST3 : ISerialize<T3>
    where D : IDepsContainer<ST1, ST2, ST3>
{
    public override ST1 Serialize1 => D.Impl1!;
    public override ST2 Serialize2 => D.Impl2!;
    public override ST3 Serialize3 => D.Impl3!;
}

public record TupleSerializeDepsImpl<
    T1, T2, T3, T4,
    ST1, ST2, ST3, ST4,
    D
> :
    TupleSerializeImplAbstract<
        T1, T2, T3, T4,
        ST1, ST2, ST3, ST4
    >
    where ST1 : ISerialize<T1>
    where ST2 : ISerialize<T2>
    where ST3 : ISerialize<T3>
    where ST4 : ISerialize<T4>
    where D : IDepsContainer<ST1, ST2, ST3, ST4>
{
    public override ST1 Serialize1 => D.Impl1!;
    public override ST2 Serialize2 => D.Impl2!;
    public override ST3 Serialize3 => D.Impl3!;
    public override ST4 Serialize4 => D.Impl4!;
}

public record TupleSerializeDepsImpl<
    T1, T2, T3, T4, T5,
    ST1, ST2, ST3, ST4, ST5,
    D
> :
    TupleSerializeImplAbstract<
        T1, T2, T3, T4, T5,
        ST1, ST2, ST3, ST4, ST5
    >
    where ST1 : ISerialize<T1>
    where ST2 : ISerialize<T2>
    where ST3 : ISerialize<T3>
    where ST4 : ISerialize<T4>
    where ST5 : ISerialize<T5>
    where D : IDepsContainer<ST1, ST2, ST3, ST4, ST5>
{
    public override ST1 Serialize1 => D.Impl1!;
    public override ST2 Serialize2 => D.Impl2!;
    public override ST3 Serialize3 => D.Impl3!;
    public override ST4 Serialize4 => D.Impl4!;
    public override ST5 Serialize5 => D.Impl5!;
}

public record TupleSerializeDepsImpl<
    T1, T2, T3, T4, T5, T6,
    ST1, ST2, ST3, ST4, ST5, ST6,
    D
> :
    TupleSerializeImplAbstract<
        T1, T2, T3, T4, T5, T6,
        ST1, ST2, ST3, ST4, ST5, ST6
    >
    where ST1 : ISerialize<T1>
    where ST2 : ISerialize<T2>
    where ST3 : ISerialize<T3>
    where ST4 : ISerialize<T4>
    where ST5 : ISerialize<T5>
    where ST6 : ISerialize<T6>
    where D : IDepsContainer<ST1, ST2, ST3, ST4, ST5, ST6>
{
    public override ST1 Serialize1 => D.Impl1!;
    public override ST2 Serialize2 => D.Impl2!;
    public override ST3 Serialize3 => D.Impl3!;
    public override ST4 Serialize4 => D.Impl4!;
    public override ST5 Serialize5 => D.Impl5!;
    public override ST6 Serialize6 => D.Impl6!;
}

public record TupleSerializeDepsImpl<
    T1, T2, T3, T4, T5, T6, T7,
    ST1, ST2, ST3, ST4, ST5, ST6, ST7,
    D
> :
    TupleSerializeImplAbstract<
        T1, T2, T3, T4, T5, T6, T7,
        ST1, ST2, ST3, ST4, ST5, ST6, ST7
    >
    where ST1 : ISerialize<T1>
    where ST2 : ISerialize<T2>
    where ST3 : ISerialize<T3>
    where ST4 : ISerialize<T4>
    where ST5 : ISerialize<T5>
    where ST6 : ISerialize<T6>
    where ST7 : ISerialize<T7>
    where D : IDepsContainer<ST1, ST2, ST3, ST4, ST5, ST6, ST7>
{
    public override ST1 Serialize1 => D.Impl1!;
    public override ST2 Serialize2 => D.Impl2!;
    public override ST3 Serialize3 => D.Impl3!;
    public override ST4 Serialize4 => D.Impl4!;
    public override ST5 Serialize5 => D.Impl5!;
    public override ST6 Serialize6 => D.Impl6!;
    public override ST7 Serialize7 => D.Impl7!;
}

public record TupleRestSerializeDepsImpl<
    T1, T2, T3, T4, T5, T6, T7, TR,
    ST1, ST2, ST3, ST4, ST5, ST6, ST7, RTR,
    D
>(nuint Size) :
    TupleRestSerializeImplAbstract<
        T1, T2, T3, T4, T5, T6, T7, TR,
        ST1, ST2, ST3, ST4, ST5, ST6, ST7, RTR
    >(Size)
    where ST1 : ISerialize<T1>
    where ST2 : ISerialize<T2>
    where ST3 : ISerialize<T3>
    where ST4 : ISerialize<T4>
    where ST5 : ISerialize<T5>
    where ST6 : ISerialize<T6>
    where ST7 : ISerialize<T7>
    where TR : notnull
    where RTR : ISeqSerializerReceiver<TR>
    where D : IDepsContainer<ST1, ST2, ST3, ST4, ST5, ST6, ST7, RTR>
{
    public override ST1 Serialize1 => D.Impl1!;
    public override ST2 Serialize2 => D.Impl2!;
    public override ST3 Serialize3 => D.Impl3!;
    public override ST4 Serialize4 => D.Impl4!;
    public override ST5 Serialize5 => D.Impl5!;
    public override ST6 Serialize6 => D.Impl6!;
    public override ST7 Serialize7 => D.Impl7!;
    public override RTR Rest => D.Impl8!;
}

#endregion

#region Async Base

public abstract record AsyncTupleSerializeImplBase<T1> :
    IAsyncSerialize<Tuple<T1>>, IAsyncSeqSerializerReceiver<Tuple<T1>>
{
    public abstract ValueTask WriteAsync<S>(S serializer, Tuple<T1> value, ISeraOptions options)
        where S : IAsyncSerializer;

    public abstract ValueTask ReceiveAsync<S>(Tuple<T1> value, S serializer) where S : IAsyncSeqSerializer;
}

public abstract record AsyncTupleSerializeImplBase<T1, T2> :
    IAsyncSerialize<Tuple<T1, T2>>, IAsyncSeqSerializerReceiver<Tuple<T1, T2>>
{
    public abstract ValueTask WriteAsync<S>(S serializer, Tuple<T1, T2> value, ISeraOptions options)
        where S : IAsyncSerializer;

    public abstract ValueTask ReceiveAsync<S>(Tuple<T1, T2> value, S serializer) where S : IAsyncSeqSerializer;
}

public abstract record AsyncTupleSerializeImplBase<T1, T2, T3> :
    IAsyncSerialize<Tuple<T1, T2, T3>>, IAsyncSeqSerializerReceiver<Tuple<T1, T2, T3>>
{
    public abstract ValueTask WriteAsync<S>(S serializer, Tuple<T1, T2, T3> value, ISeraOptions options)
        where S : IAsyncSerializer;

    public abstract ValueTask ReceiveAsync<S>(Tuple<T1, T2, T3> value, S serializer) where S : IAsyncSeqSerializer;
}

public abstract record AsyncTupleSerializeImplBase<T1, T2, T3, T4> :
    IAsyncSerialize<Tuple<T1, T2, T3, T4>>, IAsyncSeqSerializerReceiver<Tuple<T1, T2, T3, T4>>
{
    public abstract ValueTask WriteAsync<S>(S serializer, Tuple<T1, T2, T3, T4> value, ISeraOptions options)
        where S : IAsyncSerializer;

    public abstract ValueTask ReceiveAsync<S>(Tuple<T1, T2, T3, T4> value, S serializer)
        where S : IAsyncSeqSerializer;
}

public abstract record AsyncTupleSerializeImplBase<T1, T2, T3, T4, T5> :
    IAsyncSerialize<Tuple<T1, T2, T3, T4, T5>>, IAsyncSeqSerializerReceiver<Tuple<T1, T2, T3, T4, T5>>
{
    public abstract ValueTask WriteAsync<S>(S serializer, Tuple<T1, T2, T3, T4, T5> value, ISeraOptions options)
        where S : IAsyncSerializer;

    public abstract ValueTask ReceiveAsync<S>(Tuple<T1, T2, T3, T4, T5> value, S serializer)
        where S : IAsyncSeqSerializer;
}

public abstract record AsyncTupleSerializeImplBase<T1, T2, T3, T4, T5, T6> :
    IAsyncSerialize<Tuple<T1, T2, T3, T4, T5, T6>>, IAsyncSeqSerializerReceiver<Tuple<T1, T2, T3, T4, T5, T6>>
{
    public abstract ValueTask WriteAsync<S>(S serializer, Tuple<T1, T2, T3, T4, T5, T6> value,
        ISeraOptions options)
        where S : IAsyncSerializer;

    public abstract ValueTask ReceiveAsync<S>(Tuple<T1, T2, T3, T4, T5, T6> value, S serializer)
        where S : IAsyncSeqSerializer;
}

public abstract record AsyncTupleSerializeImplBase<T1, T2, T3, T4, T5, T6, T7> :
    IAsyncSerialize<Tuple<T1, T2, T3, T4, T5, T6, T7>>,
    IAsyncSeqSerializerReceiver<Tuple<T1, T2, T3, T4, T5, T6, T7>>
{
    public abstract ValueTask WriteAsync<S>(S serializer, Tuple<T1, T2, T3, T4, T5, T6, T7> value,
        ISeraOptions options)
        where S : IAsyncSerializer;

    public abstract ValueTask ReceiveAsync<S>(Tuple<T1, T2, T3, T4, T5, T6, T7> value, S serializer)
        where S : IAsyncSeqSerializer;
}

public abstract record AsyncTupleSerializeImplBase<T1, T2, T3, T4, T5, T6, T7, T8> :
    IAsyncSerialize<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>>,
    IAsyncSeqSerializerReceiver<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>>
    where T8 : notnull
{
    public abstract ValueTask WriteAsync<S>(S serializer, Tuple<T1, T2, T3, T4, T5, T6, T7, T8> value,
        ISeraOptions options) where S : IAsyncSerializer;

    public abstract ValueTask ReceiveAsync<S>(Tuple<T1, T2, T3, T4, T5, T6, T7, T8> value, S serializer)
        where S : IAsyncSeqSerializer;
}

public abstract record AsyncTupleRestSerializeImplBase<T1, T2, T3, T4, T5, T6, T7, TR> :
    IAsyncSerialize<Tuple<T1, T2, T3, T4, T5, T6, T7, TR>>,
    IAsyncSeqSerializerReceiver<Tuple<T1, T2, T3, T4, T5, T6, T7, TR>>
    where TR : notnull
{
    public abstract ValueTask WriteAsync<S>(S serializer, Tuple<T1, T2, T3, T4, T5, T6, T7, TR> value,
        ISeraOptions options) where S : IAsyncSerializer;

    public abstract ValueTask ReceiveAsync<S>(Tuple<T1, T2, T3, T4, T5, T6, T7, TR> value, S serializer)
        where S : IAsyncSeqSerializer;
}

#endregion

#region Async

public record AsyncTupleSerializeImpl<T1, ST1>(ST1 Serialize1) :
    AsyncTupleSerializeImplBase<T1>
    where ST1 : IAsyncSerialize<T1>
{
    public override ValueTask WriteAsync<S>(S serializer, Tuple<T1> value, ISeraOptions options)
        => serializer.StartSeqAsync(1, value, this);

    public override ValueTask ReceiveAsync<S>(Tuple<T1> value, S serializer)
        => serializer.WriteElementAsync(value.Item1, Serialize1);
}

public record AsyncTupleSerializeImpl<T1, T2, ST1, ST2>(ST1 Serialize1, ST2 Serialize2) :
    AsyncTupleSerializeImplBase<T1, T2>
    where ST1 : IAsyncSerialize<T1> where ST2 : IAsyncSerialize<T2>
{
    public override ValueTask WriteAsync<S>(S serializer, Tuple<T1, T2> value, ISeraOptions options)
        => serializer.StartSeqAsync(2, value, this);

    public override async ValueTask ReceiveAsync<S>(Tuple<T1, T2> value, S serializer)
    {
        await serializer.WriteElementAsync(value.Item1, Serialize1);
        await serializer.WriteElementAsync(value.Item2, Serialize2);
    }
}

public record AsyncTupleSerializeImpl<T1, T2, T3, ST1, ST2, ST3>(ST1 Serialize1, ST2 Serialize2, ST3 Serialize3) :
    AsyncTupleSerializeImplBase<T1, T2, T3>
    where ST1 : IAsyncSerialize<T1> where ST2 : IAsyncSerialize<T2> where ST3 : IAsyncSerialize<T3>
{
    public override ValueTask WriteAsync<S>(S serializer, Tuple<T1, T2, T3> value, ISeraOptions options)
        => serializer.StartSeqAsync(3, value, this);

    public override async ValueTask ReceiveAsync<S>(Tuple<T1, T2, T3> value, S serializer)
    {
        await serializer.WriteElementAsync(value.Item1, Serialize1);
        await serializer.WriteElementAsync(value.Item2, Serialize2);
        await serializer.WriteElementAsync(value.Item3, Serialize3);
    }
}

public record AsyncTupleSerializeImpl<
    T1, T2, T3, T4,
    ST1, ST2, ST3, ST4
>(ST1 Serialize1, ST2 Serialize2, ST3 Serialize3, ST4 Serialize4) :
    AsyncTupleSerializeImplBase<T1, T2, T3, T4>
    where ST1 : IAsyncSerialize<T1>
    where ST2 : IAsyncSerialize<T2>
    where ST3 : IAsyncSerialize<T3>
    where ST4 : IAsyncSerialize<T4>
{
    public override ValueTask WriteAsync<S>(S serializer, Tuple<T1, T2, T3, T4> value, ISeraOptions options)
        => serializer.StartSeqAsync(4, value, this);

    public override async ValueTask ReceiveAsync<S>(Tuple<T1, T2, T3, T4> value, S serializer)
    {
        await serializer.WriteElementAsync(value.Item1, Serialize1);
        await serializer.WriteElementAsync(value.Item2, Serialize2);
        await serializer.WriteElementAsync(value.Item3, Serialize3);
        await serializer.WriteElementAsync(value.Item4, Serialize4);
    }
}

public record AsyncTupleSerializeImpl<
    T1, T2, T3, T4, T5,
    ST1, ST2, ST3, ST4, ST5
>(ST1 Serialize1, ST2 Serialize2, ST3 Serialize3, ST4 Serialize4, ST5 Serialize5) :
    AsyncTupleSerializeImplBase<T1, T2, T3, T4, T5>
    where ST1 : IAsyncSerialize<T1>
    where ST2 : IAsyncSerialize<T2>
    where ST3 : IAsyncSerialize<T3>
    where ST4 : IAsyncSerialize<T4>
    where ST5 : IAsyncSerialize<T5>
{
    public override ValueTask WriteAsync<S>(S serializer, Tuple<T1, T2, T3, T4, T5> value, ISeraOptions options)
        => serializer.StartSeqAsync(5, value, this);

    public override async ValueTask ReceiveAsync<S>(Tuple<T1, T2, T3, T4, T5> value, S serializer)
    {
        await serializer.WriteElementAsync(value.Item1, Serialize1);
        await serializer.WriteElementAsync(value.Item2, Serialize2);
        await serializer.WriteElementAsync(value.Item3, Serialize3);
        await serializer.WriteElementAsync(value.Item4, Serialize4);
        await serializer.WriteElementAsync(value.Item5, Serialize5);
    }
}

public record AsyncTupleSerializeImpl<
    T1, T2, T3, T4, T5, T6,
    ST1, ST2, ST3, ST4, ST5, ST6
>(ST1 Serialize1, ST2 Serialize2, ST3 Serialize3, ST4 Serialize4, ST5 Serialize5, ST6 Serialize6) :
    AsyncTupleSerializeImplBase<T1, T2, T3, T4, T5, T6>
    where ST1 : IAsyncSerialize<T1>
    where ST2 : IAsyncSerialize<T2>
    where ST3 : IAsyncSerialize<T3>
    where ST4 : IAsyncSerialize<T4>
    where ST5 : IAsyncSerialize<T5>
    where ST6 : IAsyncSerialize<T6>
{
    public override ValueTask WriteAsync<S>(S serializer, Tuple<T1, T2, T3, T4, T5, T6> value,
        ISeraOptions options)
        => serializer.StartSeqAsync(6, value, this);

    public override async ValueTask ReceiveAsync<S>(Tuple<T1, T2, T3, T4, T5, T6> value, S serializer)
    {
        await serializer.WriteElementAsync(value.Item1, Serialize1);
        await serializer.WriteElementAsync(value.Item2, Serialize2);
        await serializer.WriteElementAsync(value.Item3, Serialize3);
        await serializer.WriteElementAsync(value.Item4, Serialize4);
        await serializer.WriteElementAsync(value.Item5, Serialize5);
        await serializer.WriteElementAsync(value.Item6, Serialize6);
    }
}

public record AsyncTupleSerializeImpl<
    T1, T2, T3, T4, T5, T6, T7,
    ST1, ST2, ST3, ST4, ST5, ST6, ST7
>(ST1 Serialize1, ST2 Serialize2, ST3 Serialize3, ST4 Serialize4, ST5 Serialize5, ST6 Serialize6, ST7 Serialize7) :
    AsyncTupleSerializeImplBase<T1, T2, T3, T4, T5, T6, T7>
    where ST1 : IAsyncSerialize<T1>
    where ST2 : IAsyncSerialize<T2>
    where ST3 : IAsyncSerialize<T3>
    where ST4 : IAsyncSerialize<T4>
    where ST5 : IAsyncSerialize<T5>
    where ST6 : IAsyncSerialize<T6>
    where ST7 : IAsyncSerialize<T7>
{
    public override ValueTask WriteAsync<S>(S serializer, Tuple<T1, T2, T3, T4, T5, T6, T7> value,
        ISeraOptions options)
        => serializer.StartSeqAsync(7, value, this);

    public override async ValueTask ReceiveAsync<S>(Tuple<T1, T2, T3, T4, T5, T6, T7> value, S serializer)
    {
        await serializer.WriteElementAsync(value.Item1, Serialize1);
        await serializer.WriteElementAsync(value.Item2, Serialize2);
        await serializer.WriteElementAsync(value.Item3, Serialize3);
        await serializer.WriteElementAsync(value.Item4, Serialize4);
        await serializer.WriteElementAsync(value.Item5, Serialize5);
        await serializer.WriteElementAsync(value.Item6, Serialize6);
        await serializer.WriteElementAsync(value.Item7, Serialize7);
    }
}

public record AsyncTupleSerializeImpl<
    T1, T2, T3, T4, T5, T6, T7, T8,
    ST1, ST2, ST3, ST4, ST5, ST6, ST7, ST8
>(
    ST1 Serialize1, ST2 Serialize2, ST3 Serialize3, ST4 Serialize4, ST5 Serialize5, ST6 Serialize6, ST7 Serialize7,
    ST8 Serialize8
) :
    AsyncTupleSerializeImplBase<T1, T2, T3, T4, T5, T6, T7, T8>
    where ST1 : IAsyncSerialize<T1>
    where ST2 : IAsyncSerialize<T2>
    where ST3 : IAsyncSerialize<T3>
    where ST4 : IAsyncSerialize<T4>
    where ST5 : IAsyncSerialize<T5>
    where ST6 : IAsyncSerialize<T6>
    where ST7 : IAsyncSerialize<T7>
    where T8 : notnull
    where ST8 : IAsyncSerialize<T8>
{
    public override ValueTask WriteAsync<S>(S serializer, Tuple<T1, T2, T3, T4, T5, T6, T7, T8> value,
        ISeraOptions options)
        => serializer.StartSeqAsync(8, value, this);

    public override async ValueTask ReceiveAsync<S>(Tuple<T1, T2, T3, T4, T5, T6, T7, T8> value, S serializer)
    {
        await serializer.WriteElementAsync(value.Item1, Serialize1);
        await serializer.WriteElementAsync(value.Item2, Serialize2);
        await serializer.WriteElementAsync(value.Item3, Serialize3);
        await serializer.WriteElementAsync(value.Item4, Serialize4);
        await serializer.WriteElementAsync(value.Item5, Serialize5);
        await serializer.WriteElementAsync(value.Item6, Serialize6);
        await serializer.WriteElementAsync(value.Item7, Serialize7);
        await serializer.WriteElementAsync(value.Rest, Serialize8);
    }
}

public record AsyncTupleRestSerializeImpl<
    T1, T2, T3, T4, T5, T6, T7, TR,
    ST1, ST2, ST3, ST4, ST5, ST6, ST7, RTR
>(
    ST1 Serialize1, ST2 Serialize2, ST3 Serialize3, ST4 Serialize4, ST5 Serialize5, ST6 Serialize6, ST7 Serialize7,
    RTR Rest, nuint Size
) :
    AsyncTupleRestSerializeImplBase<T1, T2, T3, T4, T5, T6, T7, TR>
    where ST1 : IAsyncSerialize<T1>
    where ST2 : IAsyncSerialize<T2>
    where ST3 : IAsyncSerialize<T3>
    where ST4 : IAsyncSerialize<T4>
    where ST5 : IAsyncSerialize<T5>
    where ST6 : IAsyncSerialize<T6>
    where ST7 : IAsyncSerialize<T7>
    where TR : notnull
    where RTR : IAsyncSeqSerializerReceiver<TR>
{
    public override ValueTask WriteAsync<S>(S serializer, Tuple<T1, T2, T3, T4, T5, T6, T7, TR> value,
        ISeraOptions options)
        => serializer.StartSeqAsync(Size, value, this);

    public override async ValueTask ReceiveAsync<S>(Tuple<T1, T2, T3, T4, T5, T6, T7, TR> value, S serializer)
    {
        await serializer.WriteElementAsync(value.Item1, Serialize1);
        await serializer.WriteElementAsync(value.Item2, Serialize2);
        await serializer.WriteElementAsync(value.Item3, Serialize3);
        await serializer.WriteElementAsync(value.Item4, Serialize4);
        await serializer.WriteElementAsync(value.Item5, Serialize5);
        await serializer.WriteElementAsync(value.Item6, Serialize6);
        await serializer.WriteElementAsync(value.Item7, Serialize7);
        await Rest.ReceiveAsync(value.Rest, serializer);
    }
}

#endregion
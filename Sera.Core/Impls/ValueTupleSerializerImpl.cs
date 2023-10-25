using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Sera.Core.Ser;

namespace Sera.Core.Impls.Tuples;

#region SyncBaseWrapper

public readonly struct ValueTupleSerializeImplWrapper<T1>(ValueTupleSerializeImplBase<T1> Serialize) :
    ISerialize<ValueTuple<T1>>, ISeqSerializerReceiver<ValueTuple<T1>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<S>(S serializer, ValueTuple<T1> value, ISeraOptions options) where S : ISerializer
        => Serialize.Write(serializer, value, options);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Receive<S>(ValueTuple<T1> value, S serializer) where S : ISeqSerializer
        => Serialize.Receive(value, serializer);
}

public readonly struct ValueTupleSerializeImplWrapper<T1, T2>(ValueTupleSerializeImplBase<T1, T2> Serialize) :
    ISerialize<ValueTuple<T1, T2>>, ISeqSerializerReceiver<ValueTuple<T1, T2>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<S>(S serializer, ValueTuple<T1, T2> value, ISeraOptions options) where S : ISerializer
        => Serialize.Write(serializer, value, options);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Receive<S>(ValueTuple<T1, T2> value, S serializer) where S : ISeqSerializer
        => Serialize.Receive(value, serializer);
}

public readonly struct ValueTupleSerializeImplWrapper<T1, T2, T3>(ValueTupleSerializeImplBase<T1, T2, T3> Serialize) :
    ISerialize<ValueTuple<T1, T2, T3>>, ISeqSerializerReceiver<ValueTuple<T1, T2, T3>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<S>(S serializer, ValueTuple<T1, T2, T3> value, ISeraOptions options) where S : ISerializer
        => Serialize.Write(serializer, value, options);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Receive<S>(ValueTuple<T1, T2, T3> value, S serializer) where S : ISeqSerializer
        => Serialize.Receive(value, serializer);
}

public readonly struct ValueTupleSerializeImplWrapper<T1, T2, T3, T4>(
    ValueTupleSerializeImplBase<T1, T2, T3, T4> Serialize) :
    ISerialize<ValueTuple<T1, T2, T3, T4>>, ISeqSerializerReceiver<ValueTuple<T1, T2, T3, T4>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<S>(S serializer, ValueTuple<T1, T2, T3, T4> value, ISeraOptions options) where S : ISerializer
        => Serialize.Write(serializer, value, options);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Receive<S>(ValueTuple<T1, T2, T3, T4> value, S serializer) where S : ISeqSerializer
        => Serialize.Receive(value, serializer);
}

public readonly struct ValueTupleSerializeImplWrapper<T1, T2, T3, T4, T5>(
    ValueTupleSerializeImplBase<T1, T2, T3, T4, T5> Serialize) :
    ISerialize<ValueTuple<T1, T2, T3, T4, T5>>, ISeqSerializerReceiver<ValueTuple<T1, T2, T3, T4, T5>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<S>(S serializer, ValueTuple<T1, T2, T3, T4, T5> value, ISeraOptions options) where S : ISerializer
        => Serialize.Write(serializer, value, options);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Receive<S>(ValueTuple<T1, T2, T3, T4, T5> value, S serializer) where S : ISeqSerializer
        => Serialize.Receive(value, serializer);
}

public readonly struct ValueTupleSerializeImplWrapper<T1, T2, T3, T4, T5, T6>(
    ValueTupleSerializeImplBase<T1, T2, T3, T4, T5, T6> Serialize) :
    ISerialize<ValueTuple<T1, T2, T3, T4, T5, T6>>, ISeqSerializerReceiver<ValueTuple<T1, T2, T3, T4, T5, T6>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<S>(S serializer, ValueTuple<T1, T2, T3, T4, T5, T6> value, ISeraOptions options)
        where S : ISerializer
        => Serialize.Write(serializer, value, options);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Receive<S>(ValueTuple<T1, T2, T3, T4, T5, T6> value, S serializer) where S : ISeqSerializer
        => Serialize.Receive(value, serializer);
}

public readonly struct ValueTupleSerializeImplWrapper<T1, T2, T3, T4, T5, T6, T7>(
    ValueTupleSerializeImplBase<T1, T2, T3, T4, T5, T6, T7> Serialize) :
    ISerialize<ValueTuple<T1, T2, T3, T4, T5, T6, T7>>, ISeqSerializerReceiver<ValueTuple<T1, T2, T3, T4, T5, T6, T7>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<S>(S serializer, ValueTuple<T1, T2, T3, T4, T5, T6, T7> value, ISeraOptions options)
        where S : ISerializer
        => Serialize.Write(serializer, value, options);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Receive<S>(ValueTuple<T1, T2, T3, T4, T5, T6, T7> value, S serializer) where S : ISeqSerializer
        => Serialize.Receive(value, serializer);
}

public readonly struct ValueTupleSerializeImplWrapper<T1, T2, T3, T4, T5, T6, T7, TR>(
    ValueTupleRestSerializeImplBase<T1, T2, T3, T4, T5, T6, T7, TR> Serialize) :
    ISerialize<ValueTuple<T1, T2, T3, T4, T5, T6, T7, TR>>,
    ISeqSerializerReceiver<ValueTuple<T1, T2, T3, T4, T5, T6, T7, TR>>
    where TR : struct
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<S>(S serializer, ValueTuple<T1, T2, T3, T4, T5, T6, T7, TR> value, ISeraOptions options)
        where S : ISerializer
        => Serialize.Write(serializer, value, options);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Receive<S>(ValueTuple<T1, T2, T3, T4, T5, T6, T7, TR> value, S serializer) where S : ISeqSerializer
        => Serialize.Receive(value, serializer);
}

#endregion

#region SyncBase

public abstract record ValueTupleSerializeImplBase<T1> :
    ISerialize<ValueTuple<T1>>, ISeqSerializerReceiver<ValueTuple<T1>>
{
    public abstract void Write<S>(S serializer, ValueTuple<T1> value, ISeraOptions options) where S : ISerializer;
    public abstract void Receive<S>(ValueTuple<T1> value, S serializer) where S : ISeqSerializer;
}

public abstract record ValueTupleSerializeImplBase<T1, T2> :
    ISerialize<ValueTuple<T1, T2>>, ISeqSerializerReceiver<ValueTuple<T1, T2>>
{
    public abstract void Write<S>(S serializer, ValueTuple<T1, T2> value, ISeraOptions options) where S : ISerializer;
    public abstract void Receive<S>(ValueTuple<T1, T2> value, S serializer) where S : ISeqSerializer;
}

public abstract record ValueTupleSerializeImplBase<T1, T2, T3> :
    ISerialize<ValueTuple<T1, T2, T3>>, ISeqSerializerReceiver<ValueTuple<T1, T2, T3>>
{
    public abstract void Write<S>(S serializer, ValueTuple<T1, T2, T3> value, ISeraOptions options)
        where S : ISerializer;

    public abstract void Receive<S>(ValueTuple<T1, T2, T3> value, S serializer) where S : ISeqSerializer;
}

public abstract record ValueTupleSerializeImplBase<T1, T2, T3, T4> :
    ISerialize<ValueTuple<T1, T2, T3, T4>>, ISeqSerializerReceiver<ValueTuple<T1, T2, T3, T4>>
{
    public abstract void Write<S>(S serializer, ValueTuple<T1, T2, T3, T4> value, ISeraOptions options)
        where S : ISerializer;

    public abstract void Receive<S>(ValueTuple<T1, T2, T3, T4> value, S serializer) where S : ISeqSerializer;
}

public abstract record ValueTupleSerializeImplBase<T1, T2, T3, T4, T5> :
    ISerialize<ValueTuple<T1, T2, T3, T4, T5>>, ISeqSerializerReceiver<ValueTuple<T1, T2, T3, T4, T5>>
{
    public abstract void Write<S>(S serializer, ValueTuple<T1, T2, T3, T4, T5> value, ISeraOptions options)
        where S : ISerializer;

    public abstract void Receive<S>(ValueTuple<T1, T2, T3, T4, T5> value, S serializer) where S : ISeqSerializer;
}

public abstract record ValueTupleSerializeImplBase<T1, T2, T3, T4, T5, T6> :
    ISerialize<ValueTuple<T1, T2, T3, T4, T5, T6>>, ISeqSerializerReceiver<ValueTuple<T1, T2, T3, T4, T5, T6>>
{
    public abstract void Write<S>(S serializer, ValueTuple<T1, T2, T3, T4, T5, T6> value, ISeraOptions options)
        where S : ISerializer;

    public abstract void Receive<S>(ValueTuple<T1, T2, T3, T4, T5, T6> value, S serializer) where S : ISeqSerializer;
}

public abstract record ValueTupleSerializeImplBase<T1, T2, T3, T4, T5, T6, T7> :
    ISerialize<ValueTuple<T1, T2, T3, T4, T5, T6, T7>>, ISeqSerializerReceiver<ValueTuple<T1, T2, T3, T4, T5, T6, T7>>
{
    public abstract void Write<S>(S serializer, ValueTuple<T1, T2, T3, T4, T5, T6, T7> value, ISeraOptions options)
        where S : ISerializer;

    public abstract void Receive<S>(ValueTuple<T1, T2, T3, T4, T5, T6, T7> value, S serializer)
        where S : ISeqSerializer;
}

public abstract record ValueTupleRestSerializeImplBase<T1, T2, T3, T4, T5, T6, T7, TR> :
    ISerialize<ValueTuple<T1, T2, T3, T4, T5, T6, T7, TR>>,
    ISeqSerializerReceiver<ValueTuple<T1, T2, T3, T4, T5, T6, T7, TR>>
    where TR : struct
{
    public abstract void Write<S>(S serializer, ValueTuple<T1, T2, T3, T4, T5, T6, T7, TR> value, ISeraOptions options)
        where S : ISerializer;

    public abstract void Receive<S>(ValueTuple<T1, T2, T3, T4, T5, T6, T7, TR> value, S serializer)
        where S : ISeqSerializer;
}

#endregion

#region Sync

public record ValueTupleSerializeImpl<T1, ST1>(ST1 Serialize1) :
    ValueTupleSerializeImplBase<T1>
    where ST1 : ISerialize<T1>
{
    public override void Write<S>(S serializer, ValueTuple<T1> value, ISeraOptions options)
        => serializer.StartSeq(1, value, this);

    public override void Receive<S>(ValueTuple<T1> value, S serializer)
    {
        serializer.WriteElement(value.Item1, Serialize1);
    }
}

public record ValueTupleSerializeImpl<T1, T2, ST1, ST2>(ST1 Serialize1, ST2 Serialize2) :
    ValueTupleSerializeImplBase<T1, T2>
    where ST1 : ISerialize<T1> where ST2 : ISerialize<T2>
{
    public override void Write<S>(S serializer, ValueTuple<T1, T2> value, ISeraOptions options)
        => serializer.StartSeq(2, value, this);

    public override void Receive<S>(ValueTuple<T1, T2> value, S serializer)
    {
        serializer.WriteElement(value.Item1, Serialize1);
        serializer.WriteElement(value.Item2, Serialize2);
    }
}

public record ValueTupleSerializeImpl<T1, T2, T3, ST1, ST2, ST3>(ST1 Serialize1, ST2 Serialize2, ST3 Serialize3) :
    ValueTupleSerializeImplBase<T1, T2, T3>
    where ST1 : ISerialize<T1> where ST2 : ISerialize<T2> where ST3 : ISerialize<T3>
{
    public override void Write<S>(S serializer, ValueTuple<T1, T2, T3> value, ISeraOptions options)
        => serializer.StartSeq(3, value, this);

    public override void Receive<S>(ValueTuple<T1, T2, T3> value, S serializer)
    {
        serializer.WriteElement(value.Item1, Serialize1);
        serializer.WriteElement(value.Item2, Serialize2);
        serializer.WriteElement(value.Item3, Serialize3);
    }
}

public record ValueTupleSerializeImpl<
    T1, T2, T3, T4,
    ST1, ST2, ST3, ST4
>(ST1 Serialize1, ST2 Serialize2, ST3 Serialize3, ST4 Serialize4) :
    ValueTupleSerializeImplBase<T1, T2, T3, T4>
    where ST1 : ISerialize<T1> where ST2 : ISerialize<T2> where ST3 : ISerialize<T3> where ST4 : ISerialize<T4>
{
    public override void Write<S>(S serializer, ValueTuple<T1, T2, T3, T4> value, ISeraOptions options)
        => serializer.StartSeq(4, value, this);

    public override void Receive<S>(ValueTuple<T1, T2, T3, T4> value, S serializer)
    {
        serializer.WriteElement(value.Item1, Serialize1);
        serializer.WriteElement(value.Item2, Serialize2);
        serializer.WriteElement(value.Item3, Serialize3);
        serializer.WriteElement(value.Item4, Serialize4);
    }
}

public record ValueTupleSerializeImpl<
    T1, T2, T3, T4, T5,
    ST1, ST2, ST3, ST4, ST5
>(ST1 Serialize1, ST2 Serialize2, ST3 Serialize3, ST4 Serialize4, ST5 Serialize5) :
    ValueTupleSerializeImplBase<T1, T2, T3, T4, T5>
    where ST1 : ISerialize<T1>
    where ST2 : ISerialize<T2>
    where ST3 : ISerialize<T3>
    where ST4 : ISerialize<T4>
    where ST5 : ISerialize<T5>
{
    public override void Write<S>(S serializer, ValueTuple<T1, T2, T3, T4, T5> value, ISeraOptions options)
        => serializer.StartSeq(5, value, this);

    public override void Receive<S>(ValueTuple<T1, T2, T3, T4, T5> value, S serializer)
    {
        serializer.WriteElement(value.Item1, Serialize1);
        serializer.WriteElement(value.Item2, Serialize2);
        serializer.WriteElement(value.Item3, Serialize3);
        serializer.WriteElement(value.Item4, Serialize4);
        serializer.WriteElement(value.Item5, Serialize5);
    }
}

public record ValueTupleSerializeImpl<
    T1, T2, T3, T4, T5, T6,
    ST1, ST2, ST3, ST4, ST5, ST6
>(ST1 Serialize1, ST2 Serialize2, ST3 Serialize3, ST4 Serialize4, ST5 Serialize5, ST6 Serialize6) :
    ValueTupleSerializeImplBase<T1, T2, T3, T4, T5, T6>
    where ST1 : ISerialize<T1>
    where ST2 : ISerialize<T2>
    where ST3 : ISerialize<T3>
    where ST4 : ISerialize<T4>
    where ST5 : ISerialize<T5>
    where ST6 : ISerialize<T6>
{
    public override void Write<S>(S serializer, ValueTuple<T1, T2, T3, T4, T5, T6> value, ISeraOptions options)
        => serializer.StartSeq(6, value, this);

    public override void Receive<S>(ValueTuple<T1, T2, T3, T4, T5, T6> value, S serializer)
    {
        serializer.WriteElement(value.Item1, Serialize1);
        serializer.WriteElement(value.Item2, Serialize2);
        serializer.WriteElement(value.Item3, Serialize3);
        serializer.WriteElement(value.Item4, Serialize4);
        serializer.WriteElement(value.Item5, Serialize5);
        serializer.WriteElement(value.Item6, Serialize6);
    }
}

public record ValueTupleSerializeImpl<
    T1, T2, T3, T4, T5, T6, T7,
    ST1, ST2, ST3, ST4, ST5, ST6, ST7
>(ST1 Serialize1, ST2 Serialize2, ST3 Serialize3, ST4 Serialize4, ST5 Serialize5, ST6 Serialize6, ST7 Serialize7) :
    ValueTupleSerializeImplBase<T1, T2, T3, T4, T5, T6, T7>
    where ST1 : ISerialize<T1>
    where ST2 : ISerialize<T2>
    where ST3 : ISerialize<T3>
    where ST4 : ISerialize<T4>
    where ST5 : ISerialize<T5>
    where ST6 : ISerialize<T6>
    where ST7 : ISerialize<T7>
{
    public override void Write<S>(S serializer, ValueTuple<T1, T2, T3, T4, T5, T6, T7> value, ISeraOptions options)
        => serializer.StartSeq(7, value, this);

    public override void Receive<S>(ValueTuple<T1, T2, T3, T4, T5, T6, T7> value, S serializer)
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

public record ValueTupleRestSerializeImpl<
    T1, T2, T3, T4, T5, T6, T7, TR,
    ST1, ST2, ST3, ST4, ST5, ST6, ST7, RTR
>(
    ST1 Serialize1, ST2 Serialize2, ST3 Serialize3, ST4 Serialize4, ST5 Serialize5, ST6 Serialize6, ST7 Serialize7,
    RTR Rest, nuint Size
) :
    ValueTupleRestSerializeImplBase<T1, T2, T3, T4, T5, T6, T7, TR>
    where ST1 : ISerialize<T1>
    where ST2 : ISerialize<T2>
    where ST3 : ISerialize<T3>
    where ST4 : ISerialize<T4>
    where ST5 : ISerialize<T5>
    where ST6 : ISerialize<T6>
    where ST7 : ISerialize<T7>
    where TR : struct
    where RTR : ISeqSerializerReceiver<TR>
{
    public override void Write<S>(S serializer, ValueTuple<T1, T2, T3, T4, T5, T6, T7, TR> value, ISeraOptions options)
        => serializer.StartSeq(Size, value, this);

    public override void Receive<S>(ValueTuple<T1, T2, T3, T4, T5, T6, T7, TR> value, S serializer)
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

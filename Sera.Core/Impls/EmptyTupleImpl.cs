using System;
using System.Threading.Tasks;
using Sera.Core.De;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

public struct EmptyTupleImpl<T> :
    ISerialize<T>, ISeqSerializerReceiver<T>
    where T : struct
{
    public static EmptyTupleImpl<T> Instance { get; } = new();

    public void Write<S>(S serializer, T value, ISeraOptions options) where S : ISerializer
        => serializer.StartSeq(0, value, this);

    public void Receive<S>(T value, S serializer) where S : ISeqSerializer { }
}

public struct EmptyTupleNewImpl<T> :
    ISerialize<T>, ISeqSerializerReceiver<T>
    where T : new()
{
    public static EmptyTupleNewImpl<T> Instance { get; } = new();

    public void Write<S>(S serializer, T value, ISeraOptions options) where S : ISerializer
        => serializer.StartSeq(0, value, this);

    public void Receive<S>(T value, S serializer) where S : ISeqSerializer { }
}

public record struct EmptyTupleByImpl<T>(Func<T> Create) :
    ISerialize<T>, ISeqSerializerReceiver<T>
{
    public void Write<S>(S serializer, T value, ISeraOptions options) where S : ISerializer
        => serializer.StartSeq(0, value, this);

    public void Receive<S>(T value, S serializer) where S : ISeqSerializer { }
}

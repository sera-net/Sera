using System;
using System.Threading.Tasks;
using Sera.Core.De;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

public struct EmptyTupleImpl<T> :
    ISerialize<T>, ISeqSerializerReceiver<T>,
    IAsyncSerialize<T>, IAsyncSeqSerializerReceiver<T>,
    IDeserialize<T>, ISeqDeserializerVisitor<T>,
    IAsyncDeserialize<T>, IAsyncSeqDeserializerVisitor<T>
    where T : struct
{
    public static EmptyTupleImpl<T> Instance { get; } = new();

    public void Write<S>(S serializer, T value, ISeraOptions options) where S : ISerializer
        => serializer.StartSeq(0, value, this);

    public void Receive<S>(T value, S serializer) where S : ISeqSerializer { }

    public ValueTask WriteAsync<S>(S serializer, T value, ISeraOptions options) where S : IAsyncSerializer
        => serializer.StartSeqAsync(0, value, this);

    public ValueTask ReceiveAsync<S>(T value, S serializer) where S : IAsyncSeqSerializer
        => ValueTask.CompletedTask;

    public T Read<D>(D deserializer, ISeraOptions options) where D : IDeserializer
        => deserializer.ReadSeq<T, EmptyTupleImpl<T>>(0, this);

    public T VisitSeq<A>(A access) where A : ISeqAccess
        => default;

    public ValueTask<T> ReadAsync<D>(D deserializer, ISeraOptions options) where D : IAsyncDeserializer
        => deserializer.ReadSeqAsync<T, EmptyTupleImpl<T>>(0, this);

    public ValueTask<T> VisitSeqAsync<A>(A access) where A : IAsyncSeqAccess
        => ValueTask.FromResult(default(T));
}

public struct EmptyTupleNewImpl<T> :
    ISerialize<T>, ISeqSerializerReceiver<T>,
    IAsyncSerialize<T>, IAsyncSeqSerializerReceiver<T>,
    IDeserialize<T>, ISeqDeserializerVisitor<T>,
    IAsyncDeserialize<T>, IAsyncSeqDeserializerVisitor<T>
    where T : new()
{
    public static EmptyTupleNewImpl<T> Instance { get; } = new();

    public void Write<S>(S serializer, T value, ISeraOptions options) where S : ISerializer
        => serializer.StartSeq(0, value, this);

    public void Receive<S>(T value, S serializer) where S : ISeqSerializer { }

    public ValueTask WriteAsync<S>(S serializer, T value, ISeraOptions options) where S : IAsyncSerializer
        => serializer.StartSeqAsync(0, value, this);

    public ValueTask ReceiveAsync<S>(T value, S serializer) where S : IAsyncSeqSerializer
        => ValueTask.CompletedTask;

    public T Read<D>(D deserializer, ISeraOptions options) where D : IDeserializer
        => deserializer.ReadSeq<T, EmptyTupleNewImpl<T>>(0, this);

    public T VisitSeq<A>(A access) where A : ISeqAccess
        => new();

    public ValueTask<T> ReadAsync<D>(D deserializer, ISeraOptions options) where D : IAsyncDeserializer
        => deserializer.ReadSeqAsync<T, EmptyTupleNewImpl<T>>(0, this);

    public ValueTask<T> VisitSeqAsync<A>(A access) where A : IAsyncSeqAccess
        => ValueTask.FromResult(new T());
}

public record struct EmptyTupleByImpl<T>(Func<T> Create) :
    ISerialize<T>, ISeqSerializerReceiver<T>,
    IAsyncSerialize<T>, IAsyncSeqSerializerReceiver<T>,
    IDeserialize<T>, ISeqDeserializerVisitor<T>,
    IAsyncDeserialize<T>, IAsyncSeqDeserializerVisitor<T>
{
    public void Write<S>(S serializer, T value, ISeraOptions options) where S : ISerializer
        => serializer.StartSeq(0, value, this);

    public void Receive<S>(T value, S serializer) where S : ISeqSerializer { }

    public ValueTask WriteAsync<S>(S serializer, T value, ISeraOptions options) where S : IAsyncSerializer
        => serializer.StartSeqAsync(0, value, this);

    public ValueTask ReceiveAsync<S>(T value, S serializer) where S : IAsyncSeqSerializer
        => ValueTask.CompletedTask;

    public T Read<D>(D deserializer, ISeraOptions options) where D : IDeserializer
        => deserializer.ReadSeq<T, EmptyTupleByImpl<T>>(0, this);

    public T VisitSeq<A>(A access) where A : ISeqAccess
        => Create();

    public ValueTask<T> ReadAsync<D>(D deserializer, ISeraOptions options) where D : IAsyncDeserializer
        => deserializer.ReadSeqAsync<T, EmptyTupleByImpl<T>>(0, this);

    public ValueTask<T> VisitSeqAsync<A>(A access) where A : IAsyncSeqAccess
        => ValueTask.FromResult(Create());
}

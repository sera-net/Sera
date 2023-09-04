using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

#region Serialize

public record IEnumerableSerializeImpl<E, T, ST>(ST Serialize) : ISerialize<E>, ISeqSerializerReceiver<E>
    where E : IEnumerable<T> where ST : ISerialize<T>
{
    public void Write<S>(S serializer, E value, ISeraOptions options) where S : ISerializer
        => serializer.StartSeq<T, E, IEnumerableSerializeImpl<E, T, ST>>(null, value, this);

    public void Receive<S>(E value, S serializer) where S : ISeqSerializer
    {
        foreach (var item in value)
        {
            serializer.WriteElement(item, Serialize);
        }
    }
}

public record AsyncIEnumerableSerializeImpl<E, T, ST>(ST Serialize) : IAsyncSerialize<E>, IAsyncSeqSerializerReceiver<E>
    where E : IEnumerable<T> where ST : IAsyncSerialize<T>
{
    public ValueTask WriteAsync<S>(S serializer, E value, ISeraOptions options) where S : IAsyncSerializer
        => serializer.StartSeqAsync<T, E, AsyncIEnumerableSerializeImpl<E, T, ST>>(null, value, this);

    public async ValueTask ReceiveAsync<S>(E value, S serializer) where S : IAsyncSeqSerializer
    {
        foreach (var item in value)
        {
            await serializer.WriteElementAsync(item, Serialize);
        }
    }
}

public record AsyncIAsyncEnumerableSerializeImpl<E, T, ST>(ST Serialize) : IAsyncSerialize<E>,
    IAsyncSeqSerializerReceiver<E>
    where E : IAsyncEnumerable<T> where ST : IAsyncSerialize<T>
{
    public ValueTask WriteAsync<S>(S serializer, E value, ISeraOptions options) where S : IAsyncSerializer
        => serializer.StartSeqAsync<T, E, AsyncIAsyncEnumerableSerializeImpl<E, T, ST>>(null, value, this);

    public async ValueTask ReceiveAsync<S>(E value, S serializer) where S : IAsyncSeqSerializer
    {
        await foreach (var item in value)
        {
            await serializer.WriteElementAsync(item, Serialize);
        }
    }
}

public record IEnumerableSerializeImpl<E, ST>(ST Serialize) : ISerialize<E>, ISeqSerializerReceiver<E>
    where E : IEnumerable where ST : ISerialize<object>
{
    public void Write<S>(S serializer, E value, ISeraOptions options) where S : ISerializer
        => serializer.StartSeq(null, value, this);

    public void Receive<S>(E value, S serializer) where S : ISeqSerializer
    {
        foreach (var item in value)
        {
            serializer.WriteElement(item, Serialize);
        }
    }
}

public record AsyncIEnumerableSerializeImpl<E, ST>(ST Serialize) : IAsyncSerialize<E>, IAsyncSeqSerializerReceiver<E>
    where E : IEnumerable where ST : IAsyncSerialize<object>
{
    public ValueTask WriteAsync<S>(S serializer, E value, ISeraOptions options) where S : IAsyncSerializer
        => serializer.StartSeqAsync(null, value, this);

    public async ValueTask ReceiveAsync<S>(E value, S serializer) where S : IAsyncSeqSerializer
    {
        foreach (var item in value)
        {
            await serializer.WriteElementAsync(item, Serialize);
        }
    }
}

#endregion

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

#region Serialize

public record ICollectionSerializeImpl<C, T, ST>(ST Serialize) : ISerialize<C>, ISeqSerializerReceiver<C>
    where C : ICollection<T> where ST : ISerialize<T>
{
    public void Write<S>(S serializer, C value, SeraOptions options) where S : ISerializer
        => serializer.StartSeq<T, C, ICollectionSerializeImpl<C, T, ST>>((nuint)value.Count, value, this);

    public void Receive<S>(C value, S serializer) where S : ISeqSerializer
    {
        foreach (var item in value)
        {
            serializer.WriteElement(item, Serialize);
        }
    }
}

public record AsyncICollectionSerializeImpl<C, T, ST>(ST Serialize) : IAsyncSerialize<C>, IAsyncSeqSerializerReceiver<C>
    where C : ICollection<T> where ST : IAsyncSerialize<T>
{
    public ValueTask WriteAsync<S>(S serializer, C value, SeraOptions options) where S : IAsyncSerializer
        => serializer.StartSeqAsync<T, C, AsyncICollectionSerializeImpl<C, T, ST>>((nuint)value.Count, value, this);

    public async ValueTask ReceiveAsync<S>(C value, S serializer) where S : IAsyncSeqSerializer
    {
        foreach (var item in value)
        {
            await serializer.WriteElementAsync(item, Serialize);
        }
    }
}

public record ICollectionSerializeImpl<C, ST>(ST Serialize) : ISerialize<C>, ISeqSerializerReceiver<C>
    where C : ICollection where ST : ISerialize<object?>
{
    public void Write<S>(S serializer, C value, SeraOptions options) where S : ISerializer
        => serializer.StartSeq((nuint)value.Count, value, this);

    public void Receive<S>(C value, S serializer) where S : ISeqSerializer
    {
        foreach (var item in value)
        {
            serializer.WriteElement(item, Serialize);
        }
    }
}

public record AsyncICollectionSerializeImpl<C, ST>(ST Serialize) : IAsyncSerialize<C>, IAsyncSeqSerializerReceiver<C>
    where C : ICollection where ST : IAsyncSerialize<object?>
{
    public ValueTask WriteAsync<S>(S serializer, C value, SeraOptions options) where S : IAsyncSerializer
        => serializer.StartSeqAsync((nuint)value.Count, value, this);

    public async ValueTask ReceiveAsync<S>(C value, S serializer) where S : IAsyncSeqSerializer
    {
        foreach (var item in value)
        {
            await serializer.WriteElementAsync(item, Serialize);
        }
    }
}

#endregion

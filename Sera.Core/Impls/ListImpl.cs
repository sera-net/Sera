using System.Collections.Generic;
using System.Threading.Tasks;
using Sera.Core.De;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

#region Serialize

public record ListSerializeImpl<L, T, ST>(ST Serialize) : ISerialize<L>, ISeqSerializerReceiver<L>
    where L : List<T> where ST : ISerialize<T>
{
    public void Write<S>(S serializer, L value, SeraOptions options) where S : ISerializer
    {
        serializer.StartSeq<T, L, ListSerializeImpl<L, T, ST>>((nuint)value.Count, value, this);
    }

    public void Receive<S>(L value, S serializer) where S : ISeqSerializer
    {
        foreach (var item in value)
        {
            serializer.WriteElement(item, Serialize);
        }
    }
}

public record AsyncListSerializeImpl<L, T, ST>(ST Serialize) : IAsyncSerialize<L>, IAsyncSeqSerializerReceiver<L>
    where L : List<T> where ST : IAsyncSerialize<T>
{
    public ValueTask WriteAsync<S>(S serializer, L value, SeraOptions options) where S : IAsyncSerializer
        => serializer.StartSeqAsync<T, L, AsyncListSerializeImpl<L, T, ST>>((nuint)value.Count, value, this);

    public async ValueTask ReceiveAsync<S>(L value, S serializer) where S : IAsyncSeqSerializer
    {
        foreach (var item in value)
        {
            await serializer.WriteElementAsync(item, Serialize);
        }
    }
}

#endregion

#region Deserialize

public record ListDeserializeImpl<T, DT>(DT Deserialize) : IDeserialize<List<T>>, ISeqDeserializerVisitor<List<T>>
    where DT : IDeserialize<T>
{
    public List<T> Read<D>(D deserializer, SeraOptions options) where D : IDeserializer
        => deserializer.ReadSeq<List<T>, ListDeserializeImpl<T, DT>>(null, this);

    public List<T> VisitSeq<A>(A access) where A : ISeqAccess
    {
        List<T> list;
        var cap = access.GetLength();
        if (cap.HasValue)
        {
            list = new((int)cap.Value);
            for (nuint i = 0; i < cap.Value; i++)
            {
                access.ReadElement(out T item, Deserialize);
                list.Add(item);
            }
        }
        else
        {
            list = new();
            while (access.HasNext())
            {
                access.ReadElement(out T item, Deserialize);
                list.Add(item);
            }
        }
        return list;
    }
}

public record AsyncListDeserializeImpl<T, DT>(DT Deserialize) : IAsyncDeserialize<List<T>>,
    IAsyncSeqDeserializerVisitor<List<T>>
    where DT : IAsyncDeserialize<T>
{
    public ValueTask<List<T>> ReadAsync<D>(D deserializer, SeraOptions options) where D : IAsyncDeserializer
        => deserializer.ReadSeqAsync<List<T>, AsyncListDeserializeImpl<T, DT>>(null, this);

    public async ValueTask<List<T>> VisitSeqAsync<A>(A access) where A : IAsyncSeqAccess
    {
        List<T> list;
        var cap = await access.GetLengthAsync();
        if (cap.HasValue)
        {
            list = new((int)cap.Value);
            for (nuint i = 0; i < cap.Value; i++)
            {
                var item = await access.ReadElementAsync<T, DT>(Deserialize);
                list.Add(item);
            }
        }
        else
        {
            list = new();
            while (await access.HasNextAsync())
            {
                var item = await access.ReadElementAsync<T, DT>(Deserialize);
                list.Add(item);
            }
        }
        return list;
    }
}

public record ListDeserializeImpl<L, T, DT>(DT Deserialize) : IDeserialize<L>, ISeqDeserializerVisitor<L>
    where L : List<T>, new() where DT : IDeserialize<T>
{
    public L Read<D>(D deserializer, SeraOptions options) where D : IDeserializer
        => deserializer.ReadSeq<L, ListDeserializeImpl<L, T, DT>>(null, this);

    public L VisitSeq<A>(A access) where A : ISeqAccess
    {
        var cap = access.GetLength();
        L list = new();
        if (cap.HasValue)
        {
            for (nuint i = 0; i < cap.Value; i++)
            {
                access.ReadElement(out T item, Deserialize);
                list.Add(item);
            }
        }
        else
        {
            while (access.HasNext())
            {
                access.ReadElement(out T item, Deserialize);
                list.Add(item);
            }
        }
        return list;
    }
}

public record AsyncListDeserializeImpl<L, T, DT>(DT Deserialize) : IAsyncDeserialize<L>, IAsyncSeqDeserializerVisitor<L>
    where L : List<T>, new() where DT : IAsyncDeserialize<T>
{
    public ValueTask<L> ReadAsync<D>(D deserializer, SeraOptions options) where D : IAsyncDeserializer
        => deserializer.ReadSeqAsync<L, AsyncListDeserializeImpl<L, T, DT>>(null, this);

    public async ValueTask<L> VisitSeqAsync<A>(A access) where A : IAsyncSeqAccess
    {
        var cap = await access.GetLengthAsync();
        L list = new();
        if (cap.HasValue)
        {
            for (nuint i = 0; i < cap.Value; i++)
            {
                var item = await access.ReadElementAsync<T, DT>(Deserialize);
                list.Add(item);
            }
        }
        else
        {
            while (await access.HasNextAsync())
            {
                var item = await access.ReadElementAsync<T, DT>(Deserialize);
                list.Add(item);
            }
        }
        return list;
    }
}

#endregion

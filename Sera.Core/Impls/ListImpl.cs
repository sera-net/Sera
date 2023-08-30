using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Sera.Core.De;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

#region Serialize

public record ListSerializeImpl<L, T, ST>(ST Serialize) : ISerialize<L>
    where L : List<T> where ST : ISerialize<T>
{
    public void Write<S>(S serializer, in L value, SeraOptions options) where S : ISerializer
    {
        serializer.WriteSeqStart<T>((nuint)value.Count);
        foreach (ref readonly var item in CollectionsMarshal.AsSpan(value))
        {
            serializer.WriteSeqElement(in item, Serialize);
        }
        serializer.WriteSeqEnd();
    }

    public async ValueTask WriteAsync<S>(S serializer, L value, SeraOptions options) where S : IAsyncSerializer
    {
        await serializer.WriteSeqStartAsync<T>((nuint)value.Count);
        foreach (var item in value)
        {
            await serializer.WriteSeqElementAsync(item, Serialize);
        }
        await serializer.WriteSeqEndAsync();
    }
}

#endregion

#region Deserialize

public record ListDeserializeImpl<T, DT>(DT Deserialize) : IDeserialize<List<T>>
    where DT : IDeserialize<T>
{
    public void Read<D>(D deserializer, out List<T> value, SeraOptions options) where D : IDeserializer
    {
        var cap = deserializer.ReadSeqStart<T>(null);
        if (cap.HasValue)
        {
            var len = (int)cap.Value;
            value = new(len);
            for (var i = 0; i < len; i++)
            {
                deserializer.ReadSeqElement(out T item, Deserialize);
                value.Add(item);
            }
        }
        else
        {
            value = new();
            while (deserializer.PeekSeqHasNext())
            {
                deserializer.ReadSeqElement(out T item, Deserialize);
                value.Add(item);
            }
        }
        deserializer.ReadSeqEnd();
    }

    public async ValueTask<List<T>> ReadAsync<D>(D deserializer, SeraOptions options) where D : IAsyncDeserializer
    {
        var cap = await deserializer.ReadSeqStartAsync<T>(null);
        List<T> value;
        if (cap.HasValue)
        {
            var len = (int)cap.Value;
            value = new(len);
            for (var i = 0; i < len; i++)
            {
                var item = await deserializer.ReadSeqElementAsync<T, DT>(Deserialize);
                value.Add(item);
            }
        }
        else
        {
            value = new();
            while (await deserializer.PeekSeqHasNextAsync())
            {
                var item = await deserializer.ReadSeqElementAsync<T, DT>(Deserialize);
                value.Add(item);
            }
        }
        await deserializer.ReadSeqEndAsync();
        return value;
    }
}

public record ListDeserializeImpl<L, T, DT>(DT Deserialize) : IDeserialize<L>
    where L : List<T>, new() where DT : IDeserialize<T>
{
    public void Read<D>(D deserializer, out L value, SeraOptions options) where D : IDeserializer
    {
        var cap = deserializer.ReadSeqStart<T>(null);
        value = new();
        if (cap.HasValue)
        {
            var len = (int)cap.Value;
            for (var i = 0; i < len; i++)
            {
                deserializer.ReadSeqElement(out T item, Deserialize);
                value.Add(item);
            }
        }
        else
        {
            while (deserializer.PeekSeqHasNext())
            {
                deserializer.ReadSeqElement(out T item, Deserialize);
                value.Add(item);
            }
        }
        deserializer.ReadSeqEnd();
    }

    public async ValueTask<L> ReadAsync<D>(D deserializer, SeraOptions options) where D : IAsyncDeserializer
    {
        var cap = await deserializer.ReadSeqStartAsync<T>(null);
        L value = new();
        if (cap.HasValue)
        {
            var len = (int)cap.Value;
            for (var i = 0; i < len; i++)
            {
                var item = await deserializer.ReadSeqElementAsync<T, DT>(Deserialize);
                value.Add(item);
            }
        }
        else
        {
            while (deserializer.PeekSeqHasNext())
            {
                var item = await deserializer.ReadSeqElementAsync<T, DT>(Deserialize);
                value.Add(item);
            }
        }
        await deserializer.ReadSeqEndAsync();
        return value;
    }
}

#endregion

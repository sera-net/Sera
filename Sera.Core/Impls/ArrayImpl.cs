using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sera.Core.De;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

#region Serialize

public record ArraySerializeImpl<T, ST>(ST Serialize) : ISerialize<T[]> where ST : ISerialize<T>
{
    public void Write<S>(S serializer, in T[] value, SeraOptions options) where S : ISerializer
    {
        serializer.WriteSeqStart<T>((nuint)value.Length);
        foreach (ref readonly var item in value.AsSpan())
        {
            serializer.WriteSeqElement(in item, Serialize);
        }
        serializer.WriteSeqEnd();
    }

    public async ValueTask WriteAsync<S>(S serializer, T[] value, SeraOptions options) where S : IAsyncSerializer
    {
        await serializer.WriteSeqStartAsync<T>((nuint)value.Length);
        foreach (var item in value)
        {
            await serializer.WriteSeqElementAsync(item, Serialize);
        }
        await serializer.WriteSeqEndAsync();
    }
}

#endregion

#region Deserialize

public record ArrayDeserializeImpl<T, DT>(DT Deserialize) : IDeserialize<T[]> where DT : IDeserialize<T>
{
    public void Read<D>(D deserializer, out T[] value, SeraOptions options) where D : IDeserializer
    {
        var cap = deserializer.ReadSeqStart<T>(null);
        if (cap.HasValue)
        {
            value = new T[cap.Value];
            for (nuint i = 0; i < cap.Value; i++)
            {
                deserializer.ReadSeqElement<T, DT>(out value[i], Deserialize);
            }
        }
        else
        {
            var list = new List<T>();
            while (deserializer.PeekSeqHasNext())
            {
                deserializer.ReadSeqElement(out T item, Deserialize);
                list.Add(item);
            }
            value = list.ToArray();
        }
        deserializer.ReadSeqEnd();
    }

    public async ValueTask<T[]> ReadAsync<D>(D deserializer, SeraOptions options) where D : IAsyncDeserializer
    {
        var cap = await deserializer.ReadSeqStartAsync<T>(null);
        T[] value;
        if (cap.HasValue)
        {
            value = new T[cap.Value];
            for (nuint i = 0; i < cap.Value; i++)
            {
                value[i] = await deserializer.ReadSeqElementAsync<T, DT>(Deserialize);
            }
        }
        else
        {
            var list = new List<T>();
            while (deserializer.PeekSeqHasNext())
            {
                var item = await deserializer.ReadSeqElementAsync<T, DT>(Deserialize);
                list.Add(item);
            }
            value = list.ToArray();
        }
        await deserializer.ReadSeqEndAsync();
        return value;
    }
}

#endregion

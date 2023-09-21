using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sera.Core.De;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

#region Serialize

public record ArraySerializeImpl<T, ST>(ST Serialize) : ISerialize<T[]>, ISeqSerializerReceiver<T[]>
    where ST : ISerialize<T>
{
    public void Write<S>(S serializer, T[] value, ISeraOptions options) where S : ISerializer
    {
        serializer.StartSeq<T, T[], ArraySerializeImpl<T, ST>>((nuint)value.Length, value, this);
    }

    public void Receive<S>(T[] value, S serialize) where S : ISeqSerializer
    {
        var len = value.LongLength;
        for (var i = 0L; i < len; i++)
        {
            serialize.WriteElement(value[i], Serialize);
        }
    }
}

public record AsyncArraySerializeImpl<T, ST>(ST Serialize) : IAsyncSerialize<T[]>, IAsyncSeqSerializerReceiver<T[]>
    where ST : IAsyncSerialize<T>
{
    public ValueTask WriteAsync<S>(S serializer, T[] value, ISeraOptions options) where S : IAsyncSerializer
        => serializer.StartSeqAsync<T, T[], AsyncArraySerializeImpl<T, ST>>((nuint)value.Length, value, this);

    public async ValueTask ReceiveAsync<S>(T[] value, S serialize) where S : IAsyncSeqSerializer
    {
        var len = value.LongLength;
        for (var i = 0L; i < len; i++)
        {
            await serialize.WriteElementAsync(value[i], Serialize);
        }
    }
}

#endregion

#region Deserialize

public record ArrayDeserializeImpl<T, DT>(DT Deserialize) : IDeserialize<T[]>, ISeqDeserializerVisitor<T[]>
    where DT : IDeserialize<T>
{
    public T[] Read<D>(D deserializer, ISeraOptions options) where D : IDeserializer
        => deserializer.ReadSeq<T[], ArrayDeserializeImpl<T, DT>>(null, this);

    public T[] VisitSeq<A>(A access) where A : ISeqAccess
    {
        var cap = access.GetLength();
        if (cap.HasValue)
        {
            var arr = new T[cap.Value];
            for (nuint i = 0; i < cap.Value; i++)
            {
                access.ReadElement(out arr[i], Deserialize);
            }
            return arr;
        }
        else
        {
            var list = new List<T>();
            while (access.HasNext())
            {
                access.ReadElement(out T item, Deserialize);
                list.Add(item);
            }
            return list.ToArray();
        }
    }
}

public record AsyncArrayDeserializeImpl<T, DT>(DT Deserialize) : IAsyncDeserialize<T[]>,
    IAsyncSeqDeserializerVisitor<T[]>
    where DT : IAsyncDeserialize<T>
{
    public ValueTask<T[]> ReadAsync<D>(D deserializer, ISeraOptions options) where D : IAsyncDeserializer
        => deserializer.ReadSeqAsync<T[], AsyncArrayDeserializeImpl<T, DT>>(null, this);

    public async ValueTask<T[]> VisitSeqAsync<A>(A access) where A : IAsyncSeqAccess
    {
        var cap = await access.GetLengthAsync();
        if (cap.HasValue)
        {
            var arr = new T[cap.Value];
            for (nuint i = 0; i < cap.Value; i++)
            {
                arr[i] = await access.ReadElementAsync<T, DT>(Deserialize);
            }
            return arr;
        }
        else
        {
            var list = new List<T>();
            while (await access.HasNextAsync())
            {
                var item = await access.ReadElementAsync<T, DT>(Deserialize);
                list.Add(item);
            }
            return list.ToArray();
        }
    }
}

#endregion

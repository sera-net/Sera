using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Sera.Core.De;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

#region Serialize

#region Sync

public abstract record ArraySerializeImplBase<T> : ISerialize<T[]>
{
    public abstract void Write<S>(S serializer, T[] value, ISeraOptions options) where S : ISerializer;
}

public sealed record ArraySerializeImpl<T, ST>(ST Serialize) : ArraySerializeImplBase<T>
    where ST : ISerialize<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void Write<S>(S serializer, T[] value, ISeraOptions options)
        => serializer.WriteArray(value, Serialize);
}

#endregion

#region Async

public abstract record AsyncArraySerializeImplBase<T> : IAsyncSerialize<T[]>
{
    public abstract ValueTask WriteAsync<S>(S serializer, T[] value, ISeraOptions options) where S : IAsyncSerializer;
}

public record AsyncArraySerializeImpl<T, ST>(ST Serialize) : AsyncArraySerializeImplBase<T>
    where ST : IAsyncSerialize<T>
{
    public override ValueTask WriteAsync<S>(S serializer, T[] value, ISeraOptions options)
        => serializer.WriteArrayAsync(value, Serialize);
}

#endregion

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

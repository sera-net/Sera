using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Sera.Core.De;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

#region Serialize

public readonly struct ListSerializeImplWrapper<L, T>(ListSerializeImplBase<L, T> Serialize) : ISerialize<L>
    where L : List<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<S>(S serializer, L value, ISeraOptions options) where S : ISerializer
        => Serialize.Write(serializer, value, options);
}

public abstract class ListSerializeImplBase<L, T> : ISerialize<L> where L : List<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public abstract void Write<S>(S serializer, L value, ISeraOptions options) where S : ISerializer;
}

public sealed class ListSerializeImpl<L, T, ST>(ST Serialize) : ListSerializeImplBase<L, T>
    where L : List<T> where ST : ISerialize<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void Write<S>(S serializer, L value, ISeraOptions options)
        => serializer.WriteArray(value, Serialize);
}

public readonly struct AsyncListSerializeImplWrapper<L, T>(AsyncListSerializeImplBase<L, T> Serialize)
    : IAsyncSerialize<L>
    where L : List<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask WriteAsync<S>(S serializer, L value, ISeraOptions options) where S : IAsyncSerializer
        => Serialize.WriteAsync(serializer, value, options);
}

public abstract class AsyncListSerializeImplBase<L, T> : IAsyncSerialize<L> where L : List<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public abstract ValueTask WriteAsync<S>(S serializer, L value, ISeraOptions options) where S : IAsyncSerializer;
}

public sealed class AsyncListSerializeImpl<L, T, ST>(ST Serialize) : AsyncListSerializeImplBase<L, T>
    where L : List<T> where ST : IAsyncSerialize<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ValueTask WriteAsync<S>(S serializer, L value, ISeraOptions options)
        => serializer.WriteArrayAsync(value, Serialize);
}

#endregion

#region Deserialize

public record ListDeserializeImpl<T, DT>(DT Deserialize) : IDeserialize<List<T>>, ISeqDeserializerVisitor<List<T>>
    where DT : IDeserialize<T>
{
    public List<T> Read<D>(D deserializer, ISeraOptions options) where D : IDeserializer
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
    public ValueTask<List<T>> ReadAsync<D>(D deserializer, ISeraOptions options) where D : IAsyncDeserializer
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
    public L Read<D>(D deserializer, ISeraOptions options) where D : IDeserializer
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
    public ValueTask<L> ReadAsync<D>(D deserializer, ISeraOptions options) where D : IAsyncDeserializer
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

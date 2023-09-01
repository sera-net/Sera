using System.Collections.Generic;
using System.Threading.Tasks;
using Sera.Core.De;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

#region Serialize

public record DictionarySerializeImpl<M, K, V, SK, SV>(SK KeySerialize, SV ValueSerialize) : ISerialize<M>,
    IMapSerializerReceiver<M>
    where M : Dictionary<K, V> where SK : ISerialize<K> where SV : ISerialize<V> where K : notnull
{
    public void Write<S>(S serializer, M value, SeraOptions options) where S : ISerializer
        => serializer.StartMap<K, V, M, DictionarySerializeImpl<M, K, V, SK, SV>>((nuint)value.Count, value, this);

    public void Receive<S>(M value, S serializer) where S : IMapSerializer
    {
        foreach (var (k, v) in value)
        {
            serializer.WriteEntry(k, v, KeySerialize, ValueSerialize);
        }
    }
}

public record AsyncDictionarySerializeImpl<M, K, V, SK, SV>(SK KeySerialize, SV ValueSerialize) : IAsyncSerialize<M>,
    IAsyncMapSerializerReceiver<M>
    where M : Dictionary<K, V> where SK : IAsyncSerialize<K> where SV : IAsyncSerialize<V> where K : notnull
{
    public ValueTask WriteAsync<S>(S serializer, M value, SeraOptions options) where S : IAsyncSerializer
        => serializer.StartMapAsync<K, V, M, AsyncDictionarySerializeImpl<M, K, V, SK, SV>>((nuint)value.Count, value,
            this);

    public async ValueTask ReceiveAsync<S>(M value, S serializer) where S : IAsyncMapSerializer
    {
        foreach (var (k, v) in value)
        {
            await serializer.WriteEntryAsync(k, v, KeySerialize, ValueSerialize);
        }
    }
}

#endregion

#region Deserialize

public record DictionaryDeserializeImpl<K, V, DK, DV>
    (DK KeyDeserialize, DV ValueDeserialize) : IDeserialize<Dictionary<K, V>>,
        IMapDeserializerVisitor<Dictionary<K, V>>
    where DK : IDeserialize<K> where DV : IDeserialize<V> where K : notnull
{
    public Dictionary<K, V> Read<D>(D deserializer, SeraOptions options) where D : IDeserializer
        => deserializer.ReadMap<Dictionary<K, V>, DictionaryDeserializeImpl<K, V, DK, DV>>(null, this);

    public Dictionary<K, V> VisitMap<A>(A access) where A : IMapAccess
    {
        var cap = access.GetLength();
        Dictionary<K, V> map;
        if (cap.HasValue)
        {
            var len = (int)cap.Value;
            map = new(len);
            for (var i = 0; i < len; i++)
            {
                access.ReadEntry(out K k, out V v, KeyDeserialize, ValueDeserialize);
                map[k] = v;
            }
        }
        else
        {
            map = new();
            while (access.HasNext())
            {
                access.ReadEntry(out K k, out V v, KeyDeserialize, ValueDeserialize);
                map[k] = v;
            }
        }
        return map;
    }
}

public record AsyncDictionaryDeserializeImpl<K, V, DK, DV>
    (DK KeyDeserialize, DV ValueDeserialize) : IAsyncDeserialize<Dictionary<K, V>>,
        IAsyncMapDeserializerVisitor<Dictionary<K, V>>
    where DK : IAsyncDeserialize<K> where DV : IAsyncDeserialize<V> where K : notnull
{
    public ValueTask<Dictionary<K, V>> ReadAsync<D>(D deserializer, SeraOptions options) where D : IAsyncDeserializer
        => deserializer.ReadMapAsync<Dictionary<K, V>, AsyncDictionaryDeserializeImpl<K, V, DK, DV>>(null, this);

    public async ValueTask<Dictionary<K, V>> VisitMapAsync<A>(A access) where A : IAsyncMapAccess
    {
        var cap = await access.GetLengthAsync();
        Dictionary<K, V> map;
        if (cap.HasValue)
        {
            var len = (int)cap.Value;
            map = new(len);
            for (var i = 0; i < len; i++)
            {
                var (k, v) = await access.ReadEntryAsync<K, V, DK, DV>(KeyDeserialize, ValueDeserialize);
                map[k] = v;
            }
        }
        else
        {
            map = new();
            while (await access.HasNextAsync())
            {
                var (k, v) = await access.ReadEntryAsync<K, V, DK, DV>(KeyDeserialize, ValueDeserialize);
                map[k] = v;
            }
        }
        return map;
    }
}

public record DictionaryDeserializeImpl<M, K, V, DK, DV>(DK KeyDeserialize, DV ValueDeserialize) :
    IDeserialize<M>, IMapDeserializerVisitor<M>
    where M : Dictionary<K, V>, new() where DK : IDeserialize<K> where DV : IDeserialize<V> where K : notnull
{
    public M Read<D>(D deserializer, SeraOptions options) where D : IDeserializer
        => deserializer.ReadMap<M, DictionaryDeserializeImpl<M, K, V, DK, DV>>(null, this);

    public M VisitMap<A>(A access) where A : IMapAccess
    {
        var cap = access.GetLength();
        var map = new M();
        if (cap.HasValue)
        {
            for (nuint i = 0; i < cap.Value; i++)
            {
                access.ReadEntry(out K k, out V v, KeyDeserialize, ValueDeserialize);
                map[k] = v;
            }
        }
        else
        {
            while (access.HasNext())
            {
                access.ReadEntry(out K k, out V v, KeyDeserialize, ValueDeserialize);
                map[k] = v;
            }
        }
        return map;
    }
}

public record AsyncDictionaryDeserializeImpl<M, K, V, DK, DV>(DK KeyDeserialize, DV ValueDeserialize) :
    IAsyncDeserialize<M>, IAsyncMapDeserializerVisitor<M>
    where M : Dictionary<K, V>, new() where DK : IAsyncDeserialize<K> where DV : IAsyncDeserialize<V> where K : notnull
{
    public ValueTask<M> ReadAsync<D>(D deserializer, SeraOptions options) where D : IAsyncDeserializer
        => deserializer.ReadMapAsync<M, AsyncDictionaryDeserializeImpl<M, K, V, DK, DV>>(null, this);

    public async ValueTask<M> VisitMapAsync<A>(A access) where A : IAsyncMapAccess
    {
        var cap = await access.GetLengthAsync();
        var map = new M();
        if (cap.HasValue)
        {
            for (nuint i = 0; i < cap.Value; i++)
            {
                var (k, v) = await access.ReadEntryAsync<K, V, DK, DV>(KeyDeserialize, ValueDeserialize);
                map[k] = v;
            }
        }
        else
        {
            while (await access.HasNextAsync())
            {
                var (k, v) = await access.ReadEntryAsync<K, V, DK, DV>(KeyDeserialize, ValueDeserialize);
                map[k] = v;
            }
        }
        return map;
    }
}

#endregion

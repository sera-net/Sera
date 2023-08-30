using System.Collections.Generic;
using System.Threading.Tasks;
using Sera.Core.De;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

#region Serialize

public record DictionarySerializeImpl<M, K, V, SK, SV>(SK KeySerialize, SV ValueSerialize) : ISerialize<M>
    where M : Dictionary<K, V> where SK : ISerialize<K> where SV : ISerialize<V> where K : notnull
{
    public void Write<S>(S serializer, in M value, SeraOptions options) where S : ISerializer
    {
        serializer.WriteMapStart<K, V>((nuint)value.Count);
        foreach (var (k, v) in value)
        {
            serializer.WriteMapEntry(in k, in v, KeySerialize, ValueSerialize);
        }
        serializer.WriteMapEnd();
    }

    public async ValueTask WriteAsync<S>(S serializer, M value, SeraOptions options) where S : IAsyncSerializer
    {
        await serializer.WriteMapStartAsync<K, V>((nuint)value.Count);
        foreach (var (k, v) in value)
        {
            await serializer.WriteMapEntryAsync(k, v, KeySerialize, ValueSerialize);
        }
        await serializer.WriteMapEndAsync();
    }
}

#endregion

#region Deserialize

public record DictionaryDeserializeImpl<K, V, DK, DV>
    (DK KeyDeserialize, DV ValueDeserialize) : IDeserialize<Dictionary<K, V>>
    where DK : IDeserialize<K> where DV : IDeserialize<V> where K : notnull
{
    public void Read<D>(D deserializer, out Dictionary<K, V> value, SeraOptions options) where D : IDeserializer
    {
        var cap = deserializer.ReadMapStart<K, V>(null);
        if (cap.HasValue)
        {
            var len = (int)cap.Value;
            value = new(len);
            for (var i = 0; i < len; i++)
            {
                deserializer.ReadMapEntry(out K k, out V v, KeyDeserialize, ValueDeserialize);
                value[k] = v;
            }
        }
        else
        {
            value = new();
            while (deserializer.PeekMapHasNext())
            {
                deserializer.ReadMapEntry(out K k, out V v, KeyDeserialize, ValueDeserialize);
                value[k] = v;
            }
        }
        deserializer.ReadMapEnd();
    }

    public async ValueTask<Dictionary<K, V>> ReadAsync<D>(D deserializer, SeraOptions options)
        where D : IAsyncDeserializer
    {
        var cap = await deserializer.ReadMapStartAsync<K, V>(null);
        Dictionary<K, V> value;
        if (cap.HasValue)
        {
            var len = (int)cap.Value;
            value = new(len);
            for (var i = 0; i < len; i++)
            {
                var (k, v) = await deserializer.ReadMapEntryAsync<K, V, DK, DV>(KeyDeserialize, ValueDeserialize);
                value[k] = v;
            }
        }
        else
        {
            value = new();
            while (await deserializer.PeekMapHasNextAsync())
            {
                var (k, v) = await deserializer.ReadMapEntryAsync<K, V, DK, DV>(KeyDeserialize, ValueDeserialize);
                value[k] = v;
            }
        }
        await deserializer.ReadMapEndAsync();
        return value;
    }
}

public record DictionaryDeserializeImpl<M, K, V, DK, DV>(DK KeyDeserialize, DV ValueDeserialize) : IDeserialize<M>
    where M : Dictionary<K, V>, new() where DK : IDeserialize<K> where DV : IDeserialize<V> where K : notnull
{
    public void Read<D>(D deserializer, out M value, SeraOptions options) where D : IDeserializer
    {
        var cap = deserializer.ReadMapStart<K, V>(null);
        value = new();
        if (cap.HasValue)
        {
            for (nuint i = 0; i < cap.Value; i++)
            {
                deserializer.ReadMapEntry(out K k, out V v, KeyDeserialize, ValueDeserialize);
                value[k] = v;
            }
        }
        else
        {
            while (deserializer.PeekMapHasNext())
            {
                deserializer.ReadMapEntry(out K k, out V v, KeyDeserialize, ValueDeserialize);
                value[k] = v;
            }
        }
        deserializer.ReadMapEnd();
    }

    public async ValueTask<M> ReadAsync<D>(D deserializer, SeraOptions options) where D : IAsyncDeserializer
    {
        var cap = await deserializer.ReadMapStartAsync<K, V>(null);
        var value = new M();
        if (cap.HasValue)
        {
            for (nuint i = 0; i < cap.Value; i++)
            {
                var (k, v) = await deserializer.ReadMapEntryAsync<K, V, DK, DV>(KeyDeserialize, ValueDeserialize);
                value[k] = v;
            }
        }
        else
        {
            while (await deserializer.PeekMapHasNextAsync())
            {
                var (k, v) = await deserializer.ReadMapEntryAsync<K, V, DK, DV>(KeyDeserialize, ValueDeserialize);
                value[k] = v;
            }
        }
        await deserializer.ReadMapEndAsync();
        return value;
    }
}

#endregion

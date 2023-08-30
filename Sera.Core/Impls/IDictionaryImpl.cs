using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

#region Serialize

public record IDictionaryImpl<M, K, V, SK, SV>(SK KeySerialize, SV ValueSerialize) : ISerialize<M>
    where M : IDictionary<K, V> where SK : ISerialize<K> where SV : ISerialize<V>
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

public record IDictionaryImpl<M, SK, SV>(SK KeySerialize, SV ValueSerialize) : ISerialize<M>
    where M : IDictionary where SK : ISerialize<object?> where SV : ISerialize<object?>
{
    public void Write<S>(S serializer, in M value, SeraOptions options) where S : ISerializer
    {
        serializer.WriteMapStart((nuint)value.Count);
        var enumerator = value.GetEnumerator();
        while (enumerator.MoveNext())
        {
            serializer.WriteMapEntry<object?, object?, SK, SV>(
                enumerator.Key, enumerator.Value, KeySerialize, ValueSerialize
            );
        }
        serializer.WriteMapEnd();
    }

    public async ValueTask WriteAsync<S>(S serializer, M value, SeraOptions options) where S : IAsyncSerializer
    {
        await serializer.WriteMapStartAsync((nuint)value.Count);
        var enumerator = value.GetEnumerator();
        while (enumerator.MoveNext())
        {
            await serializer.WriteMapEntryAsync<object?, object?, SK, SV>(
                enumerator.Key, enumerator.Value, KeySerialize, ValueSerialize
            );
        }
        await serializer.WriteMapEndAsync();
    }
}

public record IDictionaryImpl<M> : ISerialize<M>
    where M : IDictionary
{
    public void Write<S>(S serializer, in M value, SeraOptions options) where S : ISerializer
    {
        var dyn_serializer = options.RuntimeProvider.GetRuntimeSerialize();
        serializer.WriteMapStart((nuint)value.Count);
        var enumerator = value.GetEnumerator();
        while (enumerator.MoveNext())
        {
            serializer.WriteMapEntry<object?, object?, ISerialize<object?>, ISerialize<object?>>(
                enumerator.Key, enumerator.Value, dyn_serializer, dyn_serializer
            );
        }
        serializer.WriteMapEnd();
    }

    public async ValueTask WriteAsync<S>(S serializer, M value, SeraOptions options) where S : IAsyncSerializer
    {
        var dyn_serializer = options.RuntimeProvider.GetRuntimeSerialize();
        await serializer.WriteMapStartAsync((nuint)value.Count);
        var enumerator = value.GetEnumerator();
        while (enumerator.MoveNext())
        {
            await serializer.WriteMapEntryAsync<object?, object?, ISerialize<object?>, ISerialize<object?>>(
                enumerator.Key, enumerator.Value, dyn_serializer, dyn_serializer
            );
        }
        await serializer.WriteMapEndAsync();
    }
}

#endregion

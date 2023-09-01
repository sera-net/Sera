using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

#region Serialize

public record IDictionaryImpl<M, K, V, SK, SV>(SK KeySerialize, SV ValueSerialize) : ISerialize<M>,
    IMapSerializerReceiver<M>
    where M : IDictionary<K, V> where SK : ISerialize<K> where SV : ISerialize<V>
{
    public void Write<S>(S serializer, M value, SeraOptions options) where S : ISerializer
        => serializer.StartMap<K, V, M, IDictionaryImpl<M, K, V, SK, SV>>((nuint)value.Count, value, this);

    public void Receive<S>(M value, S serializer) where S : IMapSerializer
    {
        foreach (var (k, v) in value)
        {
            serializer.WriteEntry(k, v, KeySerialize, ValueSerialize);
        }
    }
}

public record AsyncIDictionaryImpl<M, K, V, SK, SV>(SK KeySerialize, SV ValueSerialize) : IAsyncSerialize<M>,
    IAsyncMapSerializerReceiver<M>
    where M : IDictionary<K, V> where SK : IAsyncSerialize<K> where SV : IAsyncSerialize<V>
{
    public ValueTask WriteAsync<S>(S serializer, M value, SeraOptions options) where S : IAsyncSerializer
        => serializer.StartMapAsync<K, V, M, AsyncIDictionaryImpl<M, K, V, SK, SV>>((nuint)value.Count, value, this);

    public async ValueTask ReceiveAsync<S>(M value, S serializer) where S : IAsyncMapSerializer
    {
        foreach (var (k, v) in value)
        {
            await serializer.WriteEntryAsync(k, v, KeySerialize, ValueSerialize);
        }
    }
}

public record AsyncIDictionaryImpl<M, SK, SV>(SK KeySerialize, SV ValueSerialize) : IAsyncSerialize<M>,
    IAsyncMapSerializerReceiver<M>
    where M : IDictionary where SK : IAsyncSerialize<object?> where SV : IAsyncSerialize<object?>
{
    public ValueTask WriteAsync<S>(S serializer, M value, SeraOptions options) where S : IAsyncSerializer
        => serializer.StartMapAsync((nuint)value.Count, value, this);

    public async ValueTask ReceiveAsync<S>(M value, S serializer) where S : IAsyncMapSerializer
    {
        var enumerator = value.GetEnumerator();
        while (enumerator.MoveNext())
        {
            await serializer.WriteEntryAsync<object?, object?, SK, SV>(
                enumerator.Key, enumerator.Value, KeySerialize, ValueSerialize
            );
        }
    }
}

#endregion

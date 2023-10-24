using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

#region Serialize

#region Sync

#region Mutable

public readonly struct IDictionarySerializeImplWrapper<M, K, V>(IDictionarySerializeImplBase<M, K, V> Serialize) :
    ISerialize<M>,
    IMapSerializerReceiver<M>
    where M : IDictionary<K, V>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<S>(S serializer, M value, ISeraOptions options) where S : ISerializer
        => Serialize.Write(serializer, value, options);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Receive<S>(M value, S serializer) where S : IMapSerializer
        => Serialize.Receive(value, serializer);
}

public abstract class IDictionarySerializeImplBase<M, K, V> : ISerialize<M>, IMapSerializerReceiver<M>
    where M : IDictionary<K, V>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public abstract void Write<S>(S serializer, M value, ISeraOptions options) where S : ISerializer;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public abstract void Receive<S>(M value, S serializer) where S : IMapSerializer;
}

public sealed class IDictionarySerializeImpl<M, K, V, SK, SV>
    (SK KeySerialize, SV ValueSerialize) : IDictionarySerializeImplBase<M, K, V>
    where M : IDictionary<K, V> where SK : ISerialize<K> where SV : ISerialize<V>
{
    public IDictionarySerializeReceiveImpl<M, K, V, SK, SV> ReceiveImpl { get; } = new(KeySerialize, ValueSerialize);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void Write<S>(S serializer, M value, ISeraOptions options)
        => serializer.StartMap<K, V, M, IDictionarySerializeReceiveImpl<M, K, V, SK, SV>>(
            (nuint)value.Count, value, ReceiveImpl
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void Receive<S>(M value, S serializer)
        => ReceiveImpl.Receive(value, serializer);
}

public readonly struct IDictionarySerializeReceiveImpl<M, K, V, SK, SV>
    (SK KeySerialize, SV ValueSerialize) : IMapSerializerReceiver<M>
    where M : IDictionary<K, V> where SK : ISerialize<K> where SV : ISerialize<V>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Receive<S>(M value, S serializer) where S : IMapSerializer
    {
        foreach (var (k, v) in value)
        {
            serializer.WriteEntry(k, v, KeySerialize, ValueSerialize);
        }
    }
}

#endregion

#region ReadOnly

public readonly struct IReadOnlyDictionarySerializeImplWrapper<M, K, V>(
    IReadOnlyDictionarySerializeImplBase<M, K, V> Serialize) :
    ISerialize<M>,
    IMapSerializerReceiver<M>
    where M : IReadOnlyDictionary<K, V>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<S>(S serializer, M value, ISeraOptions options) where S : ISerializer
        => Serialize.Write(serializer, value, options);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Receive<S>(M value, S serializer) where S : IMapSerializer
        => Serialize.Receive(value, serializer);
}

public abstract class IReadOnlyDictionarySerializeImplBase<M, K, V> : ISerialize<M>, IMapSerializerReceiver<M>
    where M : IReadOnlyDictionary<K, V>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public abstract void Write<S>(S serializer, M value, ISeraOptions options) where S : ISerializer;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public abstract void Receive<S>(M value, S serializer) where S : IMapSerializer;
}

public sealed class IReadOnlyDictionarySerializeImpl<M, K, V, SK, SV>
    (SK KeySerialize, SV ValueSerialize) : IReadOnlyDictionarySerializeImplBase<M, K, V>
    where M : IReadOnlyDictionary<K, V> where SK : ISerialize<K> where SV : ISerialize<V>
{
    public IReadOnlyDictionarySerializeReceiveImpl<M, K, V, SK, SV> ReceiveImpl { get; } =
        new(KeySerialize, ValueSerialize);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void Write<S>(S serializer, M value, ISeraOptions options)
        => serializer.StartMap<K, V, M, IReadOnlyDictionarySerializeReceiveImpl<M, K, V, SK, SV>>(
            (nuint)value.Count, value, ReceiveImpl
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void Receive<S>(M value, S serializer)
        => ReceiveImpl.Receive(value, serializer);
}

public readonly struct IReadOnlyDictionarySerializeReceiveImpl<M, K, V, SK, SV>
    (SK KeySerialize, SV ValueSerialize) : IMapSerializerReceiver<M>
    where M : IReadOnlyDictionary<K, V> where SK : ISerialize<K> where SV : ISerialize<V>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Receive<S>(M value, S serializer) where S : IMapSerializer
    {
        foreach (var (k, v) in value)
        {
            serializer.WriteEntry(k, v, KeySerialize, ValueSerialize);
        }
    }
}

#endregion

#endregion

#region Async

public record AsyncIDictionaryImpl<M, K, V, SK, SV>(SK KeySerialize, SV ValueSerialize) : IAsyncSerialize<M>,
    IAsyncMapSerializerReceiver<M>
    where M : IDictionary<K, V> where SK : IAsyncSerialize<K> where SV : IAsyncSerialize<V>
{
    public ValueTask WriteAsync<S>(S serializer, M value, ISeraOptions options) where S : IAsyncSerializer
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
    public ValueTask WriteAsync<S>(S serializer, M value, ISeraOptions options) where S : IAsyncSerializer
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

#endregion

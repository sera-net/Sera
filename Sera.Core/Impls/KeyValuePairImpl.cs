using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

#region Serialize

#region Sync

public readonly struct KeyValuePairSerializeImplWrapper<K, V>(KeyValuePairSerializeImplBase<K, V> Serialize)
    : ISerialize<KeyValuePair<K, V>>, ISeqSerializerReceiver<KeyValuePair<K, V>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<S>(S serializer, KeyValuePair<K, V> value, ISeraOptions options) where S : ISerializer
        => Serialize.Write(serializer, value, options);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Receive<S>(KeyValuePair<K, V> value, S serializer) where S : ISeqSerializer
        => Serialize.Receive(value, serializer);
}

public abstract class KeyValuePairSerializeImplBase<K, V>
    : ISerialize<KeyValuePair<K, V>>, ISeqSerializerReceiver<KeyValuePair<K, V>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public abstract void Write<S>(S serializer, KeyValuePair<K, V> value, ISeraOptions options) where S : ISerializer;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public abstract void Receive<S>(KeyValuePair<K, V> value, S serializer) where S : ISeqSerializer;
}

public sealed class KeyValuePairSerializeImpl<K, V, SK, SV>(SK KeySerialize, SV ValueSerialize)
    : KeyValuePairSerializeImplBase<K, V>
    where SK : ISerialize<K>
    where SV : ISerialize<V>
{
    public KeyValuePairSerializeReceiveImpl<K, V, SK, SV> ReceiveImpl { get; } = new(KeySerialize, ValueSerialize);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void Write<S>(S serializer, KeyValuePair<K, V> value, ISeraOptions options)
        => serializer.StartSeq(2, value, ReceiveImpl);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void Receive<S>(KeyValuePair<K, V> value, S serializer)
        => ReceiveImpl.Receive(value, serializer);
}

public readonly struct KeyValuePairSerializeReceiveImpl<K, V, SK, SV>(SK KeySerialize, SV ValueSerialize)
    : ISeqSerializerReceiver<KeyValuePair<K, V>>
    where SK : ISerialize<K>
    where SV : ISerialize<V>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Receive<S>(KeyValuePair<K, V> value, S serializer) where S : ISeqSerializer
    {
        serializer.WriteElement(value.Key, KeySerialize);
        serializer.WriteElement(value.Value, ValueSerialize);
    }
}

#endregion

#endregion

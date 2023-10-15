using System.Buffers;
using System.Runtime.CompilerServices;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

#region Sync

public readonly struct ReadOnlySequenceSerializeImplWrapper<T>(ReadOnlySequenceSerializeImplBase<T> Serialize)
    : ISerialize<ReadOnlySequence<T>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<S>(S serializer, ReadOnlySequence<T> value, ISeraOptions options) where S : ISerializer
        => Serialize.Write(serializer, value, options);
}

public abstract class ReadOnlySequenceSerializeImplBase<T> : ISerialize<ReadOnlySequence<T>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public abstract void Write<S>(S serializer, ReadOnlySequence<T> value, ISeraOptions options) where S : ISerializer;
}

public sealed class ReadOnlySequenceSerializeImpl<T, ST>(ST Serialize) : ReadOnlySequenceSerializeImplBase<T>
    where ST : ISerialize<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void Write<S>(S serializer, ReadOnlySequence<T> value, ISeraOptions options)
        => serializer.WriteArray(value, Serialize);
}

#endregion

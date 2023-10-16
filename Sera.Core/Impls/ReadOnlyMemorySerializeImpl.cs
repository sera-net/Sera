using System;
using System.Runtime.CompilerServices;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

#region Sync

public readonly struct ReadOnlyMemorySerializeImplWrapper<T>(ReadOnlyMemorySerializeImplBase<T> Serialize)
    : ISerialize<ReadOnlyMemory<T>>, ISerialize<Memory<T>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<S>(S serializer, ReadOnlyMemory<T> value, ISeraOptions options) where S : ISerializer
        => Serialize.Write(serializer, value, options);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<S>(S serializer, Memory<T> value, ISeraOptions options) where S : ISerializer
        => Serialize.Write(serializer, value, options);
}

public abstract class ReadOnlyMemorySerializeImplBase<T> : ISerialize<ReadOnlyMemory<T>>, ISerialize<Memory<T>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public abstract void Write<S>(S serializer, ReadOnlyMemory<T> value, ISeraOptions options) where S : ISerializer;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public abstract void Write<S>(S serializer, Memory<T> value, ISeraOptions options) where S : ISerializer;
}

public class ReadOnlyMemorySerializeImpl<T, ST>(ST Serialize) : ReadOnlyMemorySerializeImplBase<T>
    where ST : ISerialize<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void Write<S>(S serializer, ReadOnlyMemory<T> value, ISeraOptions options)
        => serializer.WriteArray(value, Serialize);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void Write<S>(S serializer, Memory<T> value, ISeraOptions options)
        => serializer.WriteArray((ReadOnlyMemory<T>)value, Serialize);
}

#endregion

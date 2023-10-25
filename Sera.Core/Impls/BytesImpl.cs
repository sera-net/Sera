using System;
using System.Buffers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sera.Core.De;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

public readonly struct BytesImpl :
    ISerialize<byte[]>
{
    public static BytesImpl Instance { get; } = new();

    public void Write<S>(S serializer, byte[] value, ISeraOptions options) where S : ISerializer
        => serializer.WriteBytes(value);
}

public readonly struct MemoryBytesImpl :
    ISerialize<Memory<byte>>
{
    public static MemoryBytesImpl Instance { get; } = new();

    public void Write<S>(S serializer, Memory<byte> value, ISeraOptions options) where S : ISerializer
        => serializer.WriteBytes(value);
}

public readonly struct ReadOnlyMemoryBytesImpl :
    ISerialize<ReadOnlyMemory<byte>>
{
    public static ReadOnlyMemoryBytesImpl Instance { get; } = new();

    public void Write<S>(S serializer, ReadOnlyMemory<byte> value, ISeraOptions options) where S : ISerializer
        => serializer.WriteBytes(value);
}

public readonly struct ListBytesImpl :
    ISerialize<List<byte>>
{
    public static ListBytesImpl Instance { get; } = new();

    public void Write<S>(S serializer, List<byte> value, ISeraOptions options) where S : ISerializer
        => serializer.WriteBytes(value);
}

public readonly struct ListBaseBytesSerializeImpl<L> :
    ISerialize<L>
    where L : List<byte>
{
    public void Write<S>(S serializer, L value, ISeraOptions options) where S : ISerializer
        => serializer.WriteBytes(value);
}

public readonly struct ReadOnlySequenceBytesImpl :
    ISerialize<ReadOnlySequence<byte>>
{
    public static ReadOnlySequenceBytesImpl Instance { get; } = new();

    public void Write<S>(S serializer, ReadOnlySequence<byte> value, ISeraOptions options) where S : ISerializer
        => serializer.WriteBytes(value);
}

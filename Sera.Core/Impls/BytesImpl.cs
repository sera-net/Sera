using System;
using System.Buffers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sera.Core.De;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

public readonly struct BytesImpl :
    ISerialize<byte[]>, IDeserialize<byte[]>,
    IAsyncSerialize<byte[]>, IAsyncDeserialize<byte[]>
{
    public static BytesImpl Instance { get; } = new();

    public void Write<S>(S serializer, byte[] value, ISeraOptions options) where S : ISerializer
        => serializer.WriteBytes(value);

    public byte[] Read<D>(D deserializer, ISeraOptions options) where D : IDeserializer
        => deserializer.ReadBytes();

    public ValueTask WriteAsync<S>(S serializer, byte[] value, ISeraOptions options) where S : IAsyncSerializer
        => serializer.WriteBytesAsync(value);

    public ValueTask<byte[]> ReadAsync<D>(D deserializer, ISeraOptions options) where D : IAsyncDeserializer
        => deserializer.ReadBytesAsync();
}

public readonly struct MemoryBytesImpl :
    ISerialize<Memory<byte>>, IDeserialize<Memory<byte>>,
    IAsyncSerialize<Memory<byte>>, IAsyncDeserialize<Memory<byte>>
{
    public static MemoryBytesImpl Instance { get; } = new();

    public void Write<S>(S serializer, Memory<byte> value, ISeraOptions options) where S : ISerializer
        => serializer.WriteBytes(value);

    public Memory<byte> Read<D>(D deserializer, ISeraOptions options) where D : IDeserializer
        => deserializer.ReadBytes<Memory<byte>, IdentityBytesMemoryVisitor>(new());

    public ValueTask WriteAsync<S>(S serializer, Memory<byte> value, ISeraOptions options) where S : IAsyncSerializer
        => serializer.WriteBytesAsync(value);

    public ValueTask<Memory<byte>> ReadAsync<D>(D deserializer, ISeraOptions options) where D : IAsyncDeserializer
        => deserializer.ReadBytesAsync<Memory<byte>, IdentityBytesMemoryVisitor>(new());
}

public readonly struct ReadOnlyMemoryBytesImpl :
    ISerialize<ReadOnlyMemory<byte>>, IDeserialize<ReadOnlyMemory<byte>>,
    IAsyncSerialize<ReadOnlyMemory<byte>>, IAsyncDeserialize<ReadOnlyMemory<byte>>
{
    public static ReadOnlyMemoryBytesImpl Instance { get; } = new();

    public void Write<S>(S serializer, ReadOnlyMemory<byte> value, ISeraOptions options) where S : ISerializer
        => serializer.WriteBytes(value);

    public ReadOnlyMemory<byte> Read<D>(D deserializer, ISeraOptions options) where D : IDeserializer
        => deserializer.ReadBytes<ReadOnlyMemory<byte>, IdentityBytesReadOnlyMemoryVisitor>(new());

    public ValueTask WriteAsync<S>(S serializer, ReadOnlyMemory<byte> value, ISeraOptions options)
        where S : IAsyncSerializer
        => serializer.WriteBytesAsync(value);

    public ValueTask<ReadOnlyMemory<byte>> ReadAsync<D>(D deserializer, ISeraOptions options)
        where D : IAsyncDeserializer
        => deserializer.ReadBytesAsync<ReadOnlyMemory<byte>, IdentityBytesReadOnlyMemoryVisitor>(new());
}

public readonly struct ListBytesImpl :
    ISerialize<List<byte>>, IDeserialize<List<byte>>,
    IAsyncSerialize<List<byte>>, IAsyncDeserialize<List<byte>>
{
    public static ListBytesImpl Instance { get; } = new();

    public void Write<S>(S serializer, List<byte> value, ISeraOptions options) where S : ISerializer
        => serializer.WriteBytes(value);

    public List<byte> Read<D>(D deserializer, ISeraOptions options) where D : IDeserializer
        => deserializer.ReadBytes<List<byte>, IdentityBytesListVisitor>(new());

    public ValueTask WriteAsync<S>(S serializer, List<byte> value, ISeraOptions options) where S : IAsyncSerializer
        => serializer.WriteBytesAsync(value);

    public async ValueTask<List<byte>> ReadAsync<D>(D deserializer, ISeraOptions options) where D : IAsyncDeserializer
        => await deserializer.ReadBytesAsync<List<byte>, IdentityBytesListVisitor>(new());
}

public readonly struct ListBaseBytesSerializeImpl<L> :
    ISerialize<L>, IAsyncSerialize<L>
    where L : List<byte>
{
    public void Write<S>(S serializer, L value, ISeraOptions options) where S : ISerializer
        => serializer.WriteBytes(value);
    
    public ValueTask WriteAsync<S>(S serializer, L value, ISeraOptions options) where S : IAsyncSerializer
        => serializer.WriteBytesAsync(value);
}

public readonly struct ReadOnlySequenceBytesImpl :
    ISerialize<ReadOnlySequence<byte>>, IDeserialize<ReadOnlySequence<byte>>,
    IAsyncSerialize<ReadOnlySequence<byte>>, IAsyncDeserialize<ReadOnlySequence<byte>>
{
    public static ReadOnlySequenceBytesImpl Instance { get; } = new();

    public void Write<S>(S serializer, ReadOnlySequence<byte> value, ISeraOptions options) where S : ISerializer
        => serializer.WriteBytes(value);

    public ReadOnlySequence<byte> Read<D>(D deserializer, ISeraOptions options) where D : IDeserializer
        => deserializer.ReadBytes<ReadOnlySequence<byte>, IdentityBytesReadOnlySequenceVisitor>(new());

    public ValueTask WriteAsync<S>(S serializer, ReadOnlySequence<byte> value, ISeraOptions options)
        where S : IAsyncSerializer
        => serializer.WriteBytesAsync(value);

    public async ValueTask<ReadOnlySequence<byte>> ReadAsync<D>(D deserializer, ISeraOptions options)
        where D : IAsyncDeserializer
        => await deserializer.ReadBytesAsync<ReadOnlySequence<byte>, IdentityBytesReadOnlySequenceVisitor>(new());
}

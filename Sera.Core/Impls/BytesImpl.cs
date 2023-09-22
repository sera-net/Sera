using System;
using System.Buffers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sera.Core.De;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

public struct BytesImpl :
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

public struct BytesMemoryImpl :
    ISerialize<Memory<byte>>, IDeserialize<Memory<byte>>,
    IAsyncSerialize<Memory<byte>>, IAsyncDeserialize<Memory<byte>>
{
    public static BytesMemoryImpl Instance { get; } = new();

    public void Write<S>(S serializer, Memory<byte> value, ISeraOptions options) where S : ISerializer
        => serializer.WriteBytes(value);

    public Memory<byte> Read<D>(D deserializer, ISeraOptions options) where D : IDeserializer
        => deserializer.ReadBytes<Memory<byte>, IdentityBytesMemoryVisitor>(new());

    public ValueTask WriteAsync<S>(S serializer, Memory<byte> value, ISeraOptions options) where S : IAsyncSerializer
        => serializer.WriteBytesAsync(value);

    public ValueTask<Memory<byte>> ReadAsync<D>(D deserializer, ISeraOptions options) where D : IAsyncDeserializer
        => deserializer.ReadBytesAsync<Memory<byte>, IdentityBytesMemoryVisitor>(new());
}

public struct BytesReadOnlyMemoryImpl :
    ISerialize<ReadOnlyMemory<byte>>, IDeserialize<ReadOnlyMemory<byte>>,
    IAsyncSerialize<ReadOnlyMemory<byte>>, IAsyncDeserialize<ReadOnlyMemory<byte>>
{
    public static BytesReadOnlyMemoryImpl Instance { get; } = new();

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

public struct BytesListImpl :
    ISerialize<List<byte>>, IDeserialize<List<byte>>,
    IAsyncSerialize<List<byte>>, IAsyncDeserialize<List<byte>>
{
    public static BytesListImpl Instance { get; } = new();

    public void Write<S>(S serializer, List<byte> value, ISeraOptions options) where S : ISerializer
        => serializer.WriteBytes(value);

    public List<byte> Read<D>(D deserializer, ISeraOptions options) where D : IDeserializer
        => deserializer.ReadBytes<List<byte>, IdentityBytesListVisitor>(new());

    public ValueTask WriteAsync<S>(S serializer, List<byte> value, ISeraOptions options) where S : IAsyncSerializer
        => serializer.WriteBytesAsync(value);

    public async ValueTask<List<byte>> ReadAsync<D>(D deserializer, ISeraOptions options) where D : IAsyncDeserializer
        => await deserializer.ReadBytesAsync<List<byte>, IdentityBytesListVisitor>(new());
}

public struct BytesReadOnlySequenceImpl :
    ISerialize<ReadOnlySequence<byte>>, IDeserialize<ReadOnlySequence<byte>>,
    IAsyncSerialize<ReadOnlySequence<byte>>, IAsyncDeserialize<ReadOnlySequence<byte>>
{
    public static BytesReadOnlySequenceImpl Instance { get; } = new();

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

using System;
using System.Threading.Tasks;
using Sera.Core.De;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

public class BytesImpl : ISerialize<byte[]>, IDeserialize<byte[]>, IAsyncSerialize<byte[]>, IAsyncDeserialize<byte[]>
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

public class BytesMemoryImpl : ISerialize<Memory<byte>>, IDeserialize<Memory<byte>>, IAsyncSerialize<Memory<byte>>,
    IAsyncDeserialize<Memory<byte>>
{
    public static BytesMemoryImpl Instance { get; } = new();

    public void Write<S>(S serializer, Memory<byte> value, ISeraOptions options) where S : ISerializer
        => serializer.WriteBytes(value);

    public Memory<byte> Read<D>(D deserializer, ISeraOptions options) where D : IDeserializer
        => deserializer.ReadBytes();

    public ValueTask WriteAsync<S>(S serializer, Memory<byte> value, ISeraOptions options) where S : IAsyncSerializer
        => serializer.WriteBytesAsync(value);

    public async ValueTask<Memory<byte>> ReadAsync<D>(D deserializer, ISeraOptions options) where D : IAsyncDeserializer
        => await deserializer.ReadBytesAsync();
}

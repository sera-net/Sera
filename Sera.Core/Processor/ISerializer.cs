using System;
using System.Text;
using System.Threading.Tasks;

namespace Sera.Core.Ser;

#region Basic

public partial interface ISerializer { }

public partial interface IAsyncSerializer { }

#endregion

#region Primitive

public partial interface ISerializer
{
    public void WritePrimitive<T>(T value);
}

public partial interface IAsyncSerializer
{
    public ValueTask WritePrimitiveAsync<T>(T value);
}

#endregion

#region String

public partial interface ISerializer
{
    public void WriteString(string value) => WriteString(value.AsSpan());
    public void WriteString(ReadOnlyMemory<char> value) => WriteString(value.Span);
    public void WriteString(ReadOnlySpan<char> value);

    public void WriteStringEncoded(ReadOnlyMemory<byte> value, Encoding encoding) =>
        WriteStringEncoded(value.Span, encoding);

    public void WriteStringEncoded(ReadOnlySpan<byte> value, Encoding encoding);
}

public partial interface IAsyncSerializer
{
    public ValueTask WriteStringAsync(string value) => WriteStringAsync(value.AsMemory());

    public ValueTask WriteStringAsync(ReadOnlyMemory<char> value);

    public ValueTask WriteStringEncodedAsync(ReadOnlyMemory<byte> value, Encoding encoding);
}

#endregion

#region Bytes

public partial interface ISerializer
{
    public void WriteBytes(ReadOnlyMemory<byte> value) => WriteBytes(value.Span);
    public void WriteBytes(ReadOnlySpan<byte> value);
}

public partial interface IAsyncSerializer
{
    public ValueTask WriteBytesAsync(ReadOnlyMemory<byte> value);
}

#endregion

#region Unit

public partial interface ISerializer
{
    public void WriteUnit();
}

public partial interface IAsyncSerializer
{
    public ValueTask WriteUnitAsync();
}

#endregion

#region Option

public partial interface ISerializer
{
    public void WriteNone();

    public void WriteNone<T>() => WriteNone();

    public void WriteSome<T, S>(T value, S serialize) where S : ISerialize<T>;
}

public partial interface IAsyncSerializer
{
    public ValueTask WriteNoneAsync();

    public ValueTask WriteNoneAsync<T>() => WriteNoneAsync();

    public ValueTask WriteSomeAsync<T, S>(T value, S serialize) where S : IAsyncSerialize<T>;
}

#endregion

#region Seq

public partial interface ISerializer
{
    public void StartSeq<T, R>(nuint? len, T value, R receiver) where R : ISeqSerializerReceiver<T>;

    public void StartSeq<I, T, R>(nuint? len, T value, R receiver) where R : ISeqSerializerReceiver<T>
        => StartSeq(len, value, receiver);
}

public interface ISeqSerializerReceiver<T>
{
    public void Receive<S>(T value, S serializer) where S : ISeqSerializer;
}

public interface ISeqSerializer
{
    public void WriteElement<T, S>(T value, S serializer) where S : ISerialize<T>;
}

public partial interface IAsyncSerializer
{
    public ValueTask StartSeqAsync<T, R>(nuint? len, T value, R receiver) where R : IAsyncSeqSerializerReceiver<T>;

    public ValueTask StartSeqAsync<I, T, R>(nuint? len, T value, R receiver) where R : IAsyncSeqSerializerReceiver<T>
        => StartSeqAsync(len, value, receiver);
}

public interface IAsyncSeqSerializerReceiver<T>
{
    public ValueTask ReceiveAsync<S>(T value, S serializer) where S : IAsyncSeqSerializer;
}

public interface IAsyncSeqSerializer
{
    public ValueTask WriteElementAsync<T, S>(T value, S serializer) where S : IAsyncSerialize<T>;
}

#endregion

#region Map

public partial interface ISerializer
{
    public void StartMap<T, R>(nuint? len, T value, R receiver) where R : IMapSerializerReceiver<T>;

    public void StartMap<K, V, T, R>(nuint? len, T value, R receiver) where R : IMapSerializerReceiver<T>
        => StartMap(len, value, receiver);
}

public interface IMapSerializerReceiver<in T>
{
    public void Receive<S>(T value, S serializer) where S : IMapSerializer;
}

public interface IMapSerializer
{
    public void WriteKey<K, SK>(K key, SK key_serializer) where SK : ISerialize<K>;
    public void WriteValue<V, SV>(V value, SV value_serializer) where SV : ISerialize<V>;


    public void WriteEntry<K, V, SK, SV>(K key, V value, SK key_serializer, SV value_serializer)
        where SK : ISerialize<K> where SV : ISerialize<V>
    {
        WriteKey(key, key_serializer);
        WriteValue(value, value_serializer);
    }
}

public partial interface IAsyncSerializer
{
    public ValueTask StartMapAsync<T, R>(nuint? len, T value, R receiver) where R : IAsyncMapSerializerReceiver<T>;

    public ValueTask StartMapAsync<K, V, T, R>(nuint? len, T value, R receiver) where R : IAsyncMapSerializerReceiver<T>
        => StartMapAsync(len, value, receiver);
}

public interface IAsyncMapSerializerReceiver<in T>
{
    public ValueTask ReceiveAsync<S>(T value, S serializer) where S : IAsyncMapSerializer;
}

public interface IAsyncMapSerializer
{
    public ValueTask WriteKeyAsync<K, SK>(K key, SK key_serializer) where SK : IAsyncSerialize<K>;
    public ValueTask WriteValueAsync<V, SV>(V value, SV value_serializer) where SV : IAsyncSerialize<V>;


    public async ValueTask WriteEntryAsync<K, V, SK, SV>(K key, V value, SK key_serializer, SV value_serializer)
        where SK : IAsyncSerialize<K> where SV : IAsyncSerialize<V>
    {
        await WriteKeyAsync(key, key_serializer);
        await WriteValueAsync(value, value_serializer);
    }
}

#endregion

#region Struct

public partial interface ISerializer
{
    public void StartStruct<T, R>(string? name, nuint len, T value, R receiver)
        where R : IStructSerializerReceiver<T>;

    public void StartStruct<S, T, R>(string? name, nuint len, T value, R receiver)
        where R : IStructSerializerReceiver<T>
        => StartStruct(name, len, value, receiver);
}

public interface IStructSerializerReceiver<T>
{
    public void Receive<S>(T value, S serialize) where S : IStructSerializer;
}

public interface IStructSerializer
{
    public void WriteField<T, S>(string key, T value, S serializer) where S : ISerialize<T>;
}

public partial interface IAsyncSerializer
{
    public ValueTask StartStructAsync<T, R>(string? name, nuint len, T value, R receiver)
        where R : IAsyncStructSerializerReceiver<T>;

    public ValueTask StartStructAsync<S, T, R>(string? name, nuint len, T value, R receiver)
        where R : IAsyncStructSerializerReceiver<T>
        => StartStructAsync(name, len, value, receiver);
}

public interface IAsyncStructSerializerReceiver<in T>
{
    public ValueTask ReceiveAsync<S>(T value, S serialize) where S : IAsyncStructSerializer;
}

public interface IAsyncStructSerializer
{
    public ValueTask WriteFieldAsync<T, S>(string key, T value, S serializer) where S : ISerialize<T>;
}

#endregion

#region Variant

public partial interface ISerializer
{
    public void WriteVariantUnit(string? union_name, Variant variant);

    public void WriteVariant<T, S>(string? union_name, Variant variant, T value, S serializer)
        where S : ISerialize<T>;

    public void WriteVariantUnit<U>(string? union_name, Variant variant)
        => WriteVariantUnit(union_name, variant);

    public void WriteVariant<U, T, S>(string? union_name, Variant variant, T value, S serializer)
        where S : ISerialize<T>
        => WriteVariant(union_name, variant, value, serializer);
}

public partial interface IAsyncSerializer
{
    public ValueTask WriteVariantUnitAsync(string? union_name, Variant variant);

    public ValueTask WriteVariantAsync<T, S>(string? union_name, Variant variant, T value, S serializer)
        where S : ISerialize<T>;

    public ValueTask WriteVariantUnitAsync<U>(string? union_name, Variant variant)
        => WriteVariantUnitAsync(union_name, variant);

    public ValueTask WriteVariantAsync<U, T, S>(string? union_name, Variant variant, T value, S serializer)
        where S : ISerialize<T>
        => WriteVariantAsync(union_name, variant, value, serializer);
}

#endregion

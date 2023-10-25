using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Sera.Core.SerDe;

namespace Sera.Core.Ser;

#region Basic

public partial interface ISerializer : ISeraAbility
{
    /// <summary>
    /// <para>If the serializer supports circular reference handling, this method can skip serialization of duplicate references.</para>
    /// <para>Must be used in type serialize root.</para>
    /// </summary>
    /// <param name="obj">Object reference to check, cannot be null</param>
    /// <param name="serialize">The serialize used when processing references, can be the current serialize</param>
    /// <returns>Returns <c>true</c> if it can be skip, when <c>true</c> the serialize should return immediately</returns>
    public bool MarkReference<T, S>(T obj, S serialize) where T : class where S : ISerialize<T>;
}

#endregion

#region Primitive

/// <summary>
/// Hint what format is expected to be serialize, but the serializer does not have to be fully implemented
/// </summary>
[Flags]
public enum SerializerPrimitiveHint : ulong
{
    Unknown = 0,

    /// <summary>
    /// Format numbers in decimal, Use "N"
    /// <code>1234</code>
    /// </summary>
    NumberFormatDecimal = 1 << 0,
    /// <summary>
    /// Format numbers in hexadecimal, Use "X"
    /// <code>FF</code>
    /// </summary>
    NumberFormatHex = 1 << 1,
    /// <summary>
    /// Format numbers in binary, Use "B"
    /// <code>101010</code>
    /// </summary>
    NumberFormatBinary = 1 << 2,


    /// <summary>
    /// Format <see cref="bool"/> as number
    /// </summary>
    BooleanAsNumber = 1 << 10,

    /// <summary>
    /// Format in uppercase if possible
    /// </summary>
    ToUpper = 1 << 8,
    /// <summary>
    /// Format in lowercase if possible
    /// </summary>
    ToLower = 1 << 9,

    /// <summary>
    /// Format date/time as number
    /// </summary>
    DateAsNumber = 1 << 16,
    /// <summary>
    /// Convert <see cref="DateOnly"/> to <see cref="DateTime"/> before formatting
    /// </summary>
    DateOnlyToDateTime = 1 << 17,
    /// <summary>
    /// Convert <see cref="DateOnly"/> to <see cref="DateTimeOffset"/> before formatting
    /// </summary>
    DateOnlyToDateTimeOffset = 1 << 18,
    /// <summary>
    /// Convert <see cref="DateTime"/> to <see cref="DateTimeOffset"/> before formatting
    /// </summary>
    DateTimeToDateTimeOffset = 1 << 19,
    /// <summary>
    /// Convert <see cref="DateTimeOffset"/>'s timezone using <see cref="ISeraOptions.TimeZone"/>
    /// </summary>
    DateTimeOffsetUseTimeZone = 1 << 20,

    /// <summary>
    /// Use "N" to format <see cref="Guid"/>
    /// <code>00000000000000000000000000000000</code>
    /// </summary>
    GuidFormatShort = 1UL << 32,
    /// <summary>
    /// Use "D" to format <see cref="Guid"/>
    /// <code>00000000-0000-0000-0000-000000000000</code>
    /// </summary>
    GuidFormatGuid = 1UL << 33,
    /// <summary>
    /// Use "B" to format <see cref="Guid"/>
    /// <code>(00000000-0000-0000-0000-000000000000)</code>
    /// </summary>
    GuidFormatBraces = 1UL << 34,
    /// <summary>
    /// Use "X" to format <see cref="Guid"/>
    /// <code>{0x00000000，0x0000，0x0000，{0x00，0x00，0x00，0x00，0x00，0x00，0x00，0x00}}</code>
    /// </summary>
    GuidFormatHex = 1UL << 35,
    /// <summary>
    /// When serializing <see cref="Guid"/> in binary serialization, use the Uuid standard
    /// </summary>
    GuidBinaryUuid = 1UL << 36,
    /// <summary>
    /// When serializing <see cref="Guid"/> in binary serialization, use the Guid standard
    /// </summary>
    GuidBinaryGuid = 1UL << 37,
}

public partial interface ISerializer
{
    public void WritePrimitive<T>(T value, SerializerPrimitiveHint? hint);
}

#endregion

#region String

public partial interface ISerializer
{
    public void WriteString(string value) => WriteString(value.AsSpan());
    public void WriteString(char[] value) => WriteString(value.AsSpan());

    public void WriteString(List<char> value) => WriteString(CollectionsMarshal.AsSpan(value));

    public void WriteString(ReadOnlyMemory<char> value) => WriteString(value.Span);
    public void WriteString(ReadOnlySpan<char> value);

    public void WriteStringEncoded(byte[] value, Encoding encoding) =>
        WriteStringEncoded(value.AsSpan(), encoding);

    public void WriteStringEncoded(List<byte> value, Encoding encoding) =>
        WriteStringEncoded(CollectionsMarshal.AsSpan(value), encoding);

    public void WriteStringEncoded(ReadOnlyMemory<byte> value, Encoding encoding) =>
        WriteStringEncoded(value.Span, encoding);

    public void WriteStringEncoded(ReadOnlySpan<byte> value, Encoding encoding);
}

#endregion

#region Bytes

public partial interface ISerializer
{
    public void WriteBytes(byte[] value) => WriteBytes(value.AsSpan());
    public void WriteBytes(List<byte> value) => WriteBytes(CollectionsMarshal.AsSpan(value));
    public void WriteBytes(ReadOnlySequence<byte> value);
    public void WriteBytes(ReadOnlyMemory<byte> value) => WriteBytes(value.Span);
    public void WriteBytes(ReadOnlySpan<byte> value);
}

#endregion

#region Array

public partial interface ISerializer
{
    public void WriteArray<T, S>(T[] value, S serialize) where S : ISerialize<T> =>
        WriteArray<T, S>(value.AsSpan(), serialize);

    public void WriteArray<T, S>(List<T> value, S serialize) where S : ISerialize<T> =>
        WriteArray<T, S>(CollectionsMarshal.AsSpan(value), serialize);

    public void WriteArray<T, S>(ReadOnlySequence<T> value, S serialize) where S : ISerialize<T>;

    public void WriteArray<T, S>(ReadOnlyMemory<T> value, S serialize) where S : ISerialize<T> =>
        WriteArray(value.Span, serialize);

    public void WriteArray<T, S>(ReadOnlySpan<T> value, S serialize) where S : ISerialize<T>;
}

#endregion

#region Unit

public partial interface ISerializer
{
    public void WriteUnit();
}

#endregion

#region Option

public partial interface ISerializer
{
    public void WriteNone();

    public void WriteNone<T>() => WriteNone();

    public void WriteSome<T, S>(T value, S serialize) where S : ISerialize<T>;
}

#endregion

#region Seq

public partial interface ISerializer
{
    public void StartSeq<T, R>(nuint? len, T value, R receiver) where R : ISeqSerializerReceiver<T>;

    public void StartSeq<I, T, R>(nuint? len, T value, R receiver) where R : ISeqSerializerReceiver<T>
        => StartSeq(len, value, receiver);
}

public interface ISeqSerializerReceiver<in T>
{
    public void Receive<S>(T value, S serializer) where S : ISeqSerializer;
}

public interface ISeqSerializer
{
    public void WriteElement<T, S>(T value, S serialize) where S : ISerialize<T>;
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
    public void WriteKey<K, SK>(K key, SK key_serialize) where SK : ISerialize<K>;
    public void WriteValue<V, SV>(V value, SV value_serialize) where SV : ISerialize<V>;


    public void WriteEntry<K, V, SK, SV>(K key, V value, SK key_serialize, SV value_serialize)
        where SK : ISerialize<K> where SV : ISerialize<V>
    {
        WriteKey(key, key_serialize);
        WriteValue(value, value_serialize);
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

public interface IStructSerializerReceiver<in T>
{
    public void Receive<S>(T value, S serializer) where S : IStructSerializer;
}

public interface IStructSerializer
{
    public void WriteField<T, S>(string key, long? int_key, T value, S serialize) where S : ISerialize<T>
        => WriteField(key.AsMemory(), int_key, value, serialize);

    public void WriteField<T, S>(ReadOnlyMemory<char> key, long? int_key, T value, S serialize)
        where S : ISerialize<T>
        => WriteField(key.Span, int_key, value, serialize);

    public void WriteField<T, S>(ReadOnlySpan<char> key, long? int_key, T value, S serialize) where S : ISerialize<T>;
}

#endregion

#region Variant

[Flags]
public enum SerializerVariantHint : uint
{
    Unknown = 0,

    UseNumberTag = 1 << 0,
    UseStringTag = 1 << 1,
}

public partial interface ISerializer
{
    public void WriteEmptyUnion(string? union_name);

    public void WriteVariantUnit(string? union_name, Variant variant, SerializerVariantHint? hint);

    public void WriteVariant<T, S>(string? union_name, Variant variant, T value, S serialize,
        SerializerVariantHint? hint)
        where S : ISerialize<T>;

    public void WriteEmptyUnion<U>(string? union_name) => WriteEmptyUnion(union_name);

    public void WriteVariantUnit<U>(string? union_name, Variant variant, SerializerVariantHint? hint)
        => WriteVariantUnit(union_name, variant, hint);

    public void WriteVariant<U, T, S>(string? union_name, Variant variant, T value, S serialize,
        SerializerVariantHint? hint)
        where S : ISerialize<T>
        => WriteVariant(union_name, variant, value, serialize, hint);
}

#endregion

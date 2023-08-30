using System;
using System.Threading.Tasks;

namespace Sera.Core.De;

#region Basic

/// <summary>Save current position for backtracking later</summary><returns></returns>
public record struct SaveToken(nuint Token);

/// <summary>Record TryXXX api stepping status</summary>
public record struct EatToken(nuint Token);

/// <summary>Hints that the next one might be</summary>
[Flags]
public enum DeserializerHint
{
    Unknown = 0,
    Any = -1,

    Primitive = 1 << 0,
    String = 1 << 1,
    Bytes = 1 << 2,
    Unit = 1 << 3,
    Null = 1 << 4,
    NullableNotNull = 1 << 5,
    Enum = 1 << 6,
    Tuple = 1 << 7,
    Seq = 1 << 8,
    Map = 1 << 9,
    Struct = 1 << 10,
    Variant = 1 << 11,
}

/// <summary>Hints that the primitive might be</summary>
[Flags]
public enum DeserializerPrimitiveHint
{
    Unknown = 0,
    Any = -1,

    Boolean = 1 << 0,
    SByte = 1 << 1,
    Int16 = 1 << 2,
    Int32 = 1 << 3,
    Int64 = 1 << 4,
    Int128 = 1 << 5,
    Byte = 1 << 6,
    UInt16 = 1 << 7,
    UInt32 = 1 << 8,
    UInt64 = 1 << 9,
    UInt128 = 1 << 10,
    IntPtr = 1 << 11,
    UIntPtr = 1 << 12,
    Half = 1 << 13,
    Single = 1 << 14,
    Double = 1 << 15,
    Decimal = 1 << 16,
    BigInteger = 1 << 17,
    Complex = 1 << 18,
    DateOnly = 1 << 19,
    DateTime = 1 << 20,
    DateTimeOffset = 1 << 21,
    Guid = 1 << 22,
    Range = 1 << 23,
    Index = 1 << 24,
    Char = 1 << 25,
    Rune = 1 << 26,

    SInt = SByte | Int16 | Int32 | Int64 | Int128 | IntPtr,
    UInt = Byte | UInt16 | UInt32 | UInt64 | UInt128 | UIntPtr,
    Int = SInt | UInt,
    Float = Half | Single | Double,
    BinaryNumber = Int | Float,
    Number = BinaryNumber | Decimal | BigInteger | Complex,
    Date = DateOnly | DateTime | DateTimeOffset,
}

public partial interface IDeserializer
{
    /// <summary>Random access when deserializing key-value pairs</summary>
    public bool KeyValueRandomAccess => false;

    /// <summary>Is it possible to disambiguate with backtracking</summary>
    public bool Traceable => false;

    /// <summary>Can use TryXXX api</summary>
    public bool CanTry => false;

    /// <summary>Save current position for backtracking later</summary><returns></returns>
    public SaveToken Save() => throw new NotSupportedException();

    /// <summary>Backtracking using saved position</summary>
    public void Back(SaveToken save) => throw new NotSupportedException();

    /// <summary>Complete the stepping of the TryXXX api</summary>
    public void Eat(EatToken token) => throw new NotSupportedException();

    /// <summary>Peek the next hint</summary>
    public DeserializerHint PeekNext() => throw new NotSupportedException();

    /// <summary>Skip once</summary>
    public void Skip() => throw new NotSupportedException();
}

public partial interface IAsyncDeserializer : IDeserializer
{
    /// <summary>Peek the next hint</summary>
    public ValueTask<DeserializerHint> PeekNextAsync()
        => ValueTask.FromResult(PeekNext());

    /// <summary>Skip once</summary>
    public ValueTask SkipAsync()
    {
        Skip();
        return ValueTask.CompletedTask;
    }
}

#endregion

#region Primitive

public partial interface IDeserializer
{
    /// <summary>
    /// Peek the next primitive hint.<br/>
    /// <para>If not a primitive return <see cref="DeserializerPrimitiveHint.Unknown"/></para>
    /// </summary>
    public DeserializerPrimitiveHint PeekPrimitive() => throw new NotSupportedException();

    public T ReadPrimitive<T>(SeraPrimitiveTypes type) => throw new NotSupportedException();

    public bool TryReadPrimitive<T>(SeraPrimitiveTypes type, out T result, out EatToken eat) =>
        throw new NotSupportedException();
}

public partial interface IAsyncDeserializer
{
    /// <inheritdoc cref="IDeserializer.PeekPrimitive()"/>
    public ValueTask<DeserializerPrimitiveHint> PeekPrimitiveAsync() => ValueTask.FromResult(PeekPrimitive());

    public ValueTask<T> ReadPrimitiveAsync<T>(SeraPrimitiveTypes type) => ValueTask.FromResult(ReadPrimitive<T>(type));

    public ValueTask<(T result, EatToken eat)?> TryReadPrimitiveAsync<T>(SeraPrimitiveTypes type)
        => TryReadPrimitive(type, out T result, out var eat)
            ? ValueTask.FromResult<(T result, EatToken eat)?>((result, eat))
            : ValueTask.FromResult<(T result, EatToken eat)?>(null);
}

#endregion

#region String

public partial interface IDeserializer
{
    /// <summary>Peek string length, returns -1 to indicate that the next is not a string.<br/><br/>
    /// Prefer to use <see cref="ReadString()"/>, use this method may cause additional allocation</summary>
    public int PeekStringLength() => throw new NotSupportedException();

    public string ReadString()
    {
        var len = PeekStringLength();
        if (len < 0) throw new DeserializeException("Next is not a string");
        if (len == 0) return string.Empty;
        return string.Create(len, this, (span, self) => self.ReadString(span));
    }

    public void ReadString(Memory<char> value) => ReadString(value.Span);
    public void ReadString(Span<char> value) => throw new NotSupportedException();
}

public partial interface IAsyncDeserializer
{
    /// <summary>Peek string length, returns -1 to indicate that the next is not a string.<br/><br/>
    /// Prefer to use <see cref="ReadStringAsync()"/>, use this method may cause additional allocation</summary>
    public ValueTask<int> PeekStringLengthAsync() => ValueTask.FromResult(PeekStringLength());

    public ValueTask<string> ReadStringAsync() => ValueTask.FromResult(ReadString());

    public ValueTask ReadStringAsync(Memory<char> value)
    {
        ReadString(value);
        return ValueTask.CompletedTask;
    }
}

#endregion

#region Bytes

public partial interface IDeserializer
{
    /// <summary>Peek bytes length, returns -1 to indicate that the next is not bytes.<br/><br/>
    /// Prefer to use <see cref="ReadBytes()"/>, use this method may cause additional allocation</summary>
    public int PeekBytesLength() => throw new NotSupportedException();

    public byte[] ReadBytes()
    {
        var len = PeekBytesLength();
        if (len < 0) throw new DeserializeException("Next is not bytes");
        if (len == 0) return Array.Empty<byte>();
        var arr = new byte[len];
        ReadBytes(arr.AsSpan());
        return arr;
    }

    public void ReadBytes(Memory<byte> value) => ReadBytes(value.Span);
    public void ReadBytes(Span<byte> value) => throw new NotSupportedException();
}

public partial interface IAsyncDeserializer
{
    /// <summary>Peek bytes length, returns -1 to indicate that the next is not bytes.<br/><br/>
    /// Prefer to use <see cref="ReadBytesAsync()"/>, use this method may cause additional allocation</summary>
    public ValueTask<int> PeekBytesLengthAsync() => ValueTask.FromResult(PeekBytesLength());

    public ValueTask<byte[]> ReadBytesAsync() => ValueTask.FromResult(ReadBytes());

    public ValueTask ReadBytesAsync(Memory<byte> value)
    {
        ReadBytes(value);
        return ValueTask.CompletedTask;
    }
}

#endregion

#region Unit

public partial interface IDeserializer
{
    public bool PeekIsUnit() => (PeekNext() & DeserializerHint.Unit) != 0;
    public void ReadUnit() => throw new NotSupportedException();
}

public partial interface IAsyncDeserializer
{
    public ValueTask<bool> PeekIsUnitAsync()
        => ValueTask.FromResult(PeekIsUnit());

    public ValueTask ReadUnitAsync()
    {
        ReadUnit();
        return ValueTask.CompletedTask;
    }
}

#endregion

#region Null

public partial interface IDeserializer
{
    public bool PeekIsNull() => (PeekNext() & DeserializerHint.Null) != 0;
    public void ReadNull() => throw new NotSupportedException();
}

public partial interface IAsyncDeserializer
{
    public ValueTask<bool> PeekIsNullAsync()
        => ValueTask.FromResult(PeekIsNull());

    public ValueTask ReadNullAsync()
    {
        ReadNull();
        return ValueTask.CompletedTask;
    }
}

#endregion

#region NullableNotNull

public partial interface IDeserializer
{
    public void ReadNullableNotNull<T, D>(out T result, D deserialize) where D : IDeserialize<T> =>
        throw new NotSupportedException();

    public bool TryReadNullableNotNull<T, D>(out T result, D deserialize, out EatToken eat) where D : IDeserialize<T> =>
        throw new NotSupportedException();
}

public partial interface IAsyncDeserializer
{
    public ValueTask<T> ReadNullableNotNullAsync<T, D>(D deserialize) where D : IDeserialize<T>
    {
        ReadNullableNotNull(out T r, deserialize);
        return ValueTask.FromResult(r);
    }

    public ValueTask<(T result, EatToken eat)?> TryReadNullableNotNullAsync<T, D>(D deserialize)
        where D : IDeserialize<T>
        => TryReadNullableNotNull(out T result, deserialize, out var eat)
            ? ValueTask.FromResult<(T result, EatToken eat)?>((result, eat))
            : ValueTask.FromResult<(T result, EatToken eat)?>(null);
}

#endregion

#region Enum

public partial interface IDeserializer
{
    public E ReadEnum<E>() => throw new NotSupportedException();

    public void ReadEnum<N, DN>(out string? name, out N? number, DN deserialize)
        where DN : IDeserialize<N>
        => throw new NotSupportedException();

    public bool TryReadEnum<E>(out E result, out EatToken eat) => throw new NotSupportedException();

    public bool TryReadEnum<N, DN>(out string? name, out N? number, DN deserialize, out EatToken eat)
        where DN : IDeserialize<N>
        => throw new NotSupportedException();
}

public partial interface IAsyncDeserializer
{
    public ValueTask<E> ReadEnumAsync<E>() => ValueTask.FromResult(ReadEnum<E>());

    public ValueTask<(string? name, N? number)> ReadEnumAsync<N, DN>(DN deserialize)
        where DN : IDeserialize<N>
    {
        ReadEnum(out var name, out N? number, deserialize);
        return ValueTask.FromResult((name, number));
    }

    public ValueTask<(E result, EatToken eat)?> TryReadEnumAsync<E>()
        => TryReadEnum(out E result, out var eat)
            ? ValueTask.FromResult<(E result, EatToken eat)?>((result, eat))
            : ValueTask.FromResult<(E result, EatToken eat)?>(null);

    public ValueTask<(string? name, N? number, EatToken eat)?> TryReadEnumAsync<N, DN>(DN deserialize)
        where DN : IDeserialize<N>
        => TryReadEnum(out var name, out N? number, deserialize, out var eat)
            ? ValueTask.FromResult<(string? name, N? number, EatToken eat)?>((name, number, eat))
            : ValueTask.FromResult<(string? name, N? number, EatToken eat)?>(null);
}

#endregion

#region Tuple

public partial interface IDeserializer
{
    public nuint? ReadTupleStart(nuint? len) => throw new NotSupportedException();

    public bool PeekTupleHasNext() => throw new NotSupportedException();

    public void ReadTupleElement<T, D>(out T result, D deserialize) where D : IDeserialize<T> =>
        throw new NotSupportedException();

    public bool TryReadTupleElement<T, D>(out T result, D deserialize, out EatToken eat) where D : IDeserialize<T> =>
        throw new NotSupportedException();

    public void ReadTupleEnd() => throw new NotSupportedException();
}

public partial interface IAsyncDeserializer
{
    public ValueTask<nuint?> ReadTupleStartAsync(nuint? len)
        => ValueTask.FromResult(ReadTupleStart(len));

    public ValueTask<bool> PeekTupleHasNextAsync()
        => ValueTask.FromResult(PeekTupleHasNext());

    public ValueTask<T> ReadTupleElementAsync<T, D>(D deserialize) where D : IDeserialize<T>
    {
        ReadTupleElement(out T result, deserialize);
        return ValueTask.FromResult(result);
    }

    public ValueTask<(T result, EatToken eat)?> TryReadTupleElementAsync<T, D>(D deserialize)
        where D : IDeserialize<T>
        => TryReadTupleElement(out T result, deserialize, out var eat)
            ? ValueTask.FromResult<(T result, EatToken eat)?>((result, eat))
            : ValueTask.FromResult<(T result, EatToken eat)?>(null);

    public ValueTask ReadTupleEndAsync()
    {
        ReadTupleEnd();
        return ValueTask.CompletedTask;
    }
}

#endregion

#region Seq

public partial interface IDeserializer
{
    public nuint? ReadSeqStart(nuint? len) => throw new NotSupportedException();

    public nuint? ReadSeqStart<T>(nuint? len) => ReadSeqStart(len);

    public bool PeekSeqHasNext() => throw new NotSupportedException();

    public void ReadSeqElement<T, D>(out T result, D deserialize) where D : IDeserialize<T> =>
        throw new NotSupportedException();

    public bool TryReadSeqElement<T, D>(out T result, D deserialize, out EatToken eat) where D : IDeserialize<T> =>
        throw new NotSupportedException();

    public void ReadSeqEnd() => throw new NotSupportedException();
}

public partial interface IAsyncDeserializer
{
    public ValueTask<nuint?> ReadSeqStartAsync(nuint? len)
        => ValueTask.FromResult(ReadSeqStart(len));

    public ValueTask<nuint?> ReadSeqStartAsync<T>(nuint? len)
        => ValueTask.FromResult(ReadSeqStart<T>(len));

    public ValueTask<bool> PeekSeqHasNextAsync()
        => ValueTask.FromResult(PeekSeqHasNext());

    public ValueTask<T> ReadSeqElementAsync<T, D>(D deserialize) where D : IDeserialize<T>
    {
        ReadSeqElement(out T result, deserialize);
        return ValueTask.FromResult(result);
    }

    public ValueTask<(T result, EatToken eat)?> TryReadSeqElementAsync<T, D>(D deserialize)
        where D : IDeserialize<T>
        => TryReadSeqElement(out T result, deserialize, out var eat)
            ? ValueTask.FromResult<(T result, EatToken eat)?>((result, eat))
            : ValueTask.FromResult<(T result, EatToken eat)?>(null);

    public ValueTask ReadSeqEndAsync()
    {
        ReadSeqEnd();
        return ValueTask.CompletedTask;
    }
}

#endregion

#region Map

public partial interface IDeserializer
{
    public nuint? ReadMapStart(nuint? len) => throw new NotSupportedException();
    public nuint? ReadMapStart<K, V>(nuint? len) => ReadMapStart(len);

    public bool PeekMapHasNext() => throw new NotSupportedException();

    public void ReadMapKey<K, DK>(out K result, DK deserialize) where DK : IDeserialize<K> =>
        throw new NotSupportedException();

    public void ReadMapValue<V, DV>(out V result, DV deserialize) where DV : IDeserialize<V> =>
        throw new NotSupportedException();

    public void ReadMapEntry<K, V, DK, DV>(
        out K key_result,
        out V value_result,
        DK key_deserialize,
        DV value_deserialize
    )
        where DK : IDeserialize<K>
        where DV : IDeserialize<V> =>
        throw new NotSupportedException();

    public bool TryReadMapKey<K, DK>(out K result, DK deserialize, out EatToken eat) where DK : IDeserialize<K> =>
        throw new NotSupportedException();

    public bool TryReadMapValue<V, DV>(out V result, DV deserialize, out EatToken eat) where DV : IDeserialize<V> =>
        throw new NotSupportedException();

    public bool TryReadMapEntry<K, V, DK, DV>(
        out K key_result,
        out V value_result,
        DK key_deserialize,
        DV value_deserialize,
        out EatToken eat
    )
        where DK : IDeserialize<K>
        where DV : IDeserialize<V> =>
        throw new NotSupportedException();

    /// <summary>Need <see cref="KeyValueRandomAccess"/> is <c>true</c></summary>
    public void ReadMapValueByKey<K, V, SK, DV>(in K key, out V result, SK serialize, DV deserialize)
        where SK : ISerialize<V>
        where DV : IDeserialize<V> =>
        throw new NotSupportedException();

    /// <summary>Need <see cref="KeyValueRandomAccess"/> is <c>true</c></summary>
    public void ReadMapValueByKey<V, DV>(string key, out V result, DV deserialize)
        where DV : IDeserialize<V> =>
        throw new NotSupportedException();

    /// <summary>Need <see cref="KeyValueRandomAccess"/> is <c>true</c></summary>
    public bool TryReadMapValueByKey<K, V, SK, DV>(in K key, out V result, SK serialize, DV deserialize,
        out EatToken eat)
        where SK : ISerialize<V>
        where DV : IDeserialize<V> =>
        throw new NotSupportedException();

    /// <summary>Need <see cref="KeyValueRandomAccess"/> is <c>true</c></summary>
    public bool TryReadMapValueByKey<V, DV>(string key, out V result, DV deserialize, out EatToken eat)
        where DV : IDeserialize<V> =>
        throw new NotSupportedException();

    public void ReadMapEnd() => throw new NotSupportedException();
}

public partial interface IAsyncDeserializer
{
    public ValueTask<nuint?> ReadMapStartAsync(nuint? len)
        => ValueTask.FromResult(ReadMapStart(len));

    public ValueTask<nuint?> ReadMapStartAsync<K, V>(nuint? len)
        => ValueTask.FromResult(ReadMapStart<K, V>(len));

    public ValueTask<bool> PeekMapHasNextAsync()
        => ValueTask.FromResult(PeekMapHasNext());

    public ValueTask<K> ReadMapKeyAsync<K, DK>(DK deserialize) where DK : IDeserialize<K>
    {
        ReadMapKey(out K result, deserialize);
        return ValueTask.FromResult(result);
    }

    public ValueTask<V> ReadMapValueAsync<V, DV>(DV deserialize) where DV : IDeserialize<V>
    {
        ReadMapValue(out V result, deserialize);
        return ValueTask.FromResult(result);
    }

    public ValueTask<(K key, V value)> ReadMapEntryAsync<K, V, DK, DV>(DK key_deserialize, DV value_deserialize)
        where DK : IDeserialize<K> where DV : IDeserialize<V>
    {
        ReadMapEntry(out K key, out V value, key_deserialize, value_deserialize);
        return ValueTask.FromResult((key, value));
    }

    public ValueTask<(K result, EatToken eat)?> TryReadMapKeyAsync<K, DK>(DK deserialize) where DK : IDeserialize<K>
        => TryReadMapKey(out K result, deserialize, out var eat)
            ? ValueTask.FromResult<(K result, EatToken eat)?>((result, eat))
            : ValueTask.FromResult<(K result, EatToken eat)?>(null);

    public ValueTask<(V result, EatToken eat)?> TryReadMapValueAsync<V, DV>(DV deserialize) where DV : IDeserialize<V>
        => TryReadMapValue(out V result, deserialize, out var eat)
            ? ValueTask.FromResult<(V result, EatToken eat)?>((result, eat))
            : ValueTask.FromResult<(V result, EatToken eat)?>(null);

    public ValueTask<(K key, V value, EatToken eat)?> TryReadMapEntryAsync<K, V, DK, DV>(DK key_deserialize,
        DV value_deserialize) where DK : IDeserialize<K> where DV : IDeserialize<V>
        => TryReadMapEntry(out K key, out V value, key_deserialize, value_deserialize, out var eat)
            ? ValueTask.FromResult<(K key, V value, EatToken eat)?>((key, value, eat))
            : ValueTask.FromResult<(K key, V value, EatToken eat)?>(null);

    /// <summary>Need <see cref="IDeserializer.KeyValueRandomAccess"/> is <c>true</c></summary>
    public ValueTask<V> ReadMapValueByKeyAsync<K, V, SK, DV>(K key, SK serialize, DV deserialize)
        where SK : ISerialize<V>
        where DV : IDeserialize<V>
    {
        ReadMapValueByKey(in key, out V result, serialize, deserialize);
        return ValueTask.FromResult(result);
    }

    /// <summary>Need <see cref="IDeserializer.KeyValueRandomAccess"/> is <c>true</c></summary>
    public ValueTask<V> ReadMapValueByKeyAsync<V, DV>(string key, DV deserialize)
        where DV : IDeserialize<V>
    {
        ReadMapValueByKey(key, out V result, deserialize);
        return ValueTask.FromResult(result);
    }

    /// <summary>Need <see cref="IDeserializer.KeyValueRandomAccess"/> is <c>true</c></summary>
    public ValueTask<(V result, EatToken eat)?> TryReadMapValueByKeyAsync<K, V, SK, DV>(K key, SK serialize,
        DV deserialize)
        where SK : ISerialize<V>
        where DV : IDeserialize<V>
        => TryReadMapValueByKey(in key, out V result, serialize, deserialize, out var eat)
            ? ValueTask.FromResult<(V result, EatToken eat)?>((result, eat))
            : ValueTask.FromResult<(V result, EatToken eat)?>(null);

    /// <summary>Need <see cref="IDeserializer.KeyValueRandomAccess"/> is <c>true</c></summary>
    public ValueTask<(V result, EatToken eat)?> TryReadMapValueByKeyAsync<V, DV>(string key, DV deserialize)
        where DV : IDeserialize<V>
        => TryReadMapValueByKey(key, out V result, deserialize, out var eat)
            ? ValueTask.FromResult<(V result, EatToken eat)?>((result, eat))
            : ValueTask.FromResult<(V result, EatToken eat)?>(null);

    public ValueTask ReadMapEndAsync()
    {
        ReadMapEnd();
        return ValueTask.CompletedTask;
    }
}

#endregion

#region Struct

public partial interface IDeserializer
{
    public nuint? ReadStructStart(string? name, nuint? len) => throw new NotSupportedException();
    public nuint? ReadStructStart<T>(string? name, nuint? len) => ReadStructStart(name, len);

    public string? ViewStructName() => throw new NotSupportedException();

    public bool PeekStructHasNext() => throw new NotSupportedException();

    public string? PeekStructNextKey() => throw new NotSupportedException();

    public void ReadStructField<T, D>(out T value, D deserialize)
        where D : IDeserialize<T> =>
        throw new NotSupportedException();

    public bool TryReadStructField<T, D>(out T value, D deserialize, out EatToken eat)
        where D : IDeserialize<T> =>
        throw new NotSupportedException();

    /// <summary>Need <see cref="KeyValueRandomAccess"/> is <c>true</c></summary>
    public void ReadStructFieldByKey<T, D>(string key, out T value, D deserialize)
        where D : IDeserialize<T> =>
        throw new NotSupportedException();

    /// <summary>Need <see cref="KeyValueRandomAccess"/> is <c>true</c></summary>
    public bool TryReadStructFieldByKey<T, D>(string key, out T value, D deserialize, out EatToken eat)
        where D : IDeserialize<T> =>
        throw new NotSupportedException();

    public void ReadStructSkipField() => throw new NotSupportedException();

    public void ReadStructEnd() => throw new NotSupportedException();
}

public partial interface IAsyncDeserializer
{
    public ValueTask<nuint?> ReadStructStartAsync(string? name, nuint? len)
        => ValueTask.FromResult(ReadStructStart(name, len));

    public ValueTask<nuint?> ReadStructStartAsync<T>(string? name, nuint? len)
        => ValueTask.FromResult(ReadStructStart<T>(name, len));

    public ValueTask<string?> ViewStructNameAsync()
        => ValueTask.FromResult(ViewStructName());

    public ValueTask<bool> PeekStructHasNextAsync()
        => ValueTask.FromResult(PeekStructHasNext());

    public ValueTask<string?> PeekStructNextKeyAsync()
        => ValueTask.FromResult(PeekStructNextKey());

    public ValueTask<T> ReadStructFieldAsync<T, D>(D deserialize) where D : IDeserialize<T>
    {
        ReadStructField(out T result, deserialize);
        return ValueTask.FromResult(result);
    }

    public ValueTask<(T result, EatToken eat)?> TryReadStructFieldAsync<T, D>(D deserialize) where D : IDeserialize<T>
        => TryReadStructField(out T result, deserialize, out var eat)
            ? ValueTask.FromResult<(T result, EatToken eat)?>((result, eat))
            : ValueTask.FromResult<(T result, EatToken eat)?>(null);

    /// <summary>Need <see cref="IDeserializer.KeyValueRandomAccess"/> is <c>true</c></summary>
    public ValueTask<T> ReadStructFieldByKeyAsync<T, D>(string key, D deserialize) where D : IDeserialize<T>
    {
        ReadStructFieldByKey(key, out T result, deserialize);
        return ValueTask.FromResult(result);
    }

    /// <summary>Need <see cref="IDeserializer.KeyValueRandomAccess"/> is <c>true</c></summary>
    public ValueTask<(T result, EatToken eat)?> TryReadStructFieldByKeyAsync<T, D>(string key, D deserialize)
        where D : IDeserialize<T>
        => TryReadStructFieldByKey(key, out T result, deserialize, out var eat)
            ? ValueTask.FromResult<(T result, EatToken eat)?>((result, eat))
            : ValueTask.FromResult<(T result, EatToken eat)?>(null);

    public ValueTask ReadStructSkipFieldAsync()
    {
        ReadStructSkipField();
        return ValueTask.CompletedTask;
    }

    public ValueTask ReadStructEndAsync()
    {
        ReadStructEnd();
        return ValueTask.CompletedTask;
    }
}

#endregion

#region Variant

public partial interface IDeserializer
{
    public void ReadVariantStart(string? union_name) => throw new NotSupportedException();
    public void ReadVariantStart<U, V>(string? union_name) => ReadVariantStart(union_name);

    public string? ViewUnionName() => throw new NotSupportedException();

    public string? ViewVariantName() => throw new NotSupportedException();

    public nuint? ViewVariantTag() => throw new NotSupportedException();

    public bool PeekVariantIsUnit() => throw new NotSupportedException();

    /// <summary>Each Variant can only read one Value</summary>
    public void ReadVariantValue<T, D>(out T result, D deserialize) where D : IDeserialize<T>
        => throw new NotSupportedException();

    /// <summary>Each Variant can only read one Value</summary>
    public bool TryReadVariantValue<T, D>(out T result, D deserialize, out EatToken eat) where D : IDeserialize<T>
        => throw new NotSupportedException();

    /// <summary>Each Variant can only read one Value</summary>
    public void ReadVariantValueUnit()
        => throw new NotSupportedException();

    /// <summary>Each Variant can only read one Value</summary>
    public bool TryReadVariantValueUnit(out EatToken eat)
        => throw new NotSupportedException();

    public void ReadVariantEnd() => throw new NotSupportedException();
}

public partial interface IAsyncDeserializer
{
    public ValueTask ReadVariantStartAsync(string? union_name)
    {
        ReadVariantStart(union_name);
        return ValueTask.CompletedTask;
    }

    public ValueTask ReadVariantStartAsync<U, V>(string? union_name)
    {
        ReadVariantStart<U, V>(union_name);
        return ValueTask.CompletedTask;
    }

    public ValueTask<string?> ViewUnionNameAsync()
        => ValueTask.FromResult(ViewUnionName());

    public ValueTask<string?> ViewVariantNameAsync()
        => ValueTask.FromResult(ViewVariantName());

    public ValueTask<nuint?> ViewVariantTagAsync()
        => ValueTask.FromResult(ViewVariantTag());

    public ValueTask<bool> PeekVariantIsUnitAsync()
        => ValueTask.FromResult(PeekVariantIsUnit());

    /// <summary>Each Variant can only read one Value</summary>
    public ValueTask<T> ReadVariantValueAsync<T, D>(D deserialize) where D : IDeserialize<T>
    {
        ReadVariantValue(out T result, deserialize);
        return ValueTask.FromResult(result);
    }

    /// <summary>Each Variant can only read one Value</summary>
    public ValueTask<(T result, EatToken eat)?> TryReadVariantValueAsync<T, D>(D deserialize) where D : IDeserialize<T>
        => TryReadVariantValue(out T result, deserialize, out var eat)
            ? ValueTask.FromResult<(T result, EatToken eat)?>((result, eat))
            : ValueTask.FromResult<(T result, EatToken eat)?>(null);

    /// <summary>Each Variant can only read one Value</summary>
    public ValueTask ReadVariantValueUnitAsync()
    {
        ReadVariantValueUnit();
        return ValueTask.CompletedTask;
    }

    /// <summary>Each Variant can only read one Value</summary>
    public ValueTask<EatToken?> TryReadVariantValueUnitAsync()
        => TryReadVariantValueUnit(out var eat)
            ? ValueTask.FromResult<EatToken?>(eat)
            : ValueTask.FromResult<EatToken?>(null);

    public ValueTask ReadVariantEndAsync()
    {
        ReadVariantEnd();
        return ValueTask.CompletedTask;
    }
}

#endregion

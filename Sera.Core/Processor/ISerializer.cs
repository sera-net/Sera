using System;
using System.Threading.Tasks;

namespace Sera.Core.Ser;

#region Basic

public partial interface ISerializer { }

public partial interface IAsyncSerializer : ISerializer { }

#endregion

#region Primitive

public partial interface ISerializer
{
    public void WritePrimitive<T>(T value, SeraPrimitiveTypes type) => throw new NotSupportedException();
}

public partial interface IAsyncSerializer
{
    public ValueTask WritePrimitiveAsync<T>(T value, SeraPrimitiveTypes type)
    {
        WritePrimitive(value, type);
        return ValueTask.CompletedTask;
    }
}

#endregion

#region String

public partial interface ISerializer
{
    public void WriteString(string value) => WriteString(value.AsSpan());
    public void WriteString(ReadOnlyMemory<char> value) => WriteString(value.Span);
    public void WriteString(ReadOnlySpan<char> value) => throw new NotSupportedException();
}

public partial interface IAsyncSerializer
{
    public ValueTask WriteStringAsync(string value)
    {
        WriteString(value);
        return ValueTask.CompletedTask;
    }

    public ValueTask WriteStringAsync(ReadOnlyMemory<char> value)
    {
        WriteString(value);
        return ValueTask.CompletedTask;
    }
}

#endregion

#region Bytes

public partial interface ISerializer
{
    public void WriteBytes(byte[] value) => WriteBytes(value.AsSpan());
    public void WriteBytes(ReadOnlyMemory<byte> value) => WriteBytes(value.Span);
    public void WriteBytes(ReadOnlySpan<byte> value) => throw new NotSupportedException();
}

public partial interface IAsyncSerializer
{
    public ValueTask WriteBytesAsync(byte[] value)
    {
        WriteBytes(value);
        return ValueTask.CompletedTask;
    }

    public ValueTask WriteBytesAsync(ReadOnlyMemory<byte> value)
    {
        WriteBytes(value);
        return ValueTask.CompletedTask;
    }
}

#endregion

#region Unit

public partial interface ISerializer
{
    public void WriteUnit() => throw new NotSupportedException();
}

public partial interface IAsyncSerializer
{
    public ValueTask WriteUnitAsync()
    {
        WriteUnit();
        return ValueTask.CompletedTask;
    }
}

#endregion

#region Null

public partial interface ISerializer
{
    public void WriteNull() => throw new NotSupportedException();
}

public partial interface IAsyncSerializer
{
    public ValueTask WriteNullAsync()
    {
        WriteNull();
        return ValueTask.CompletedTask;
    }
}

#endregion

#region NullableNotNull

public partial interface ISerializer
{
    public void WriteNullableNotNull<T, S>(in T value, S serialize) where S : ISerialize<T>
        => throw new NotSupportedException();
}

public partial interface IAsyncSerializer
{
    public ValueTask WriteNullableNotNullAsync<T, S>(T value, S serialize) where S : ISerialize<T>
    {
        WriteNullableNotNull(value, serialize);
        return ValueTask.CompletedTask;
    }
}

#endregion

#region Enum

public partial interface ISerializer
{
    public void WriteEnum<E>(E e) where E : Enum => throw new NotSupportedException();

    public void WriteEnum<E, N, NS>(E e, N number, NS number_serialize)
        where E : Enum where NS : ISerialize<N>
        => WriteEnum(e, null, number, number_serialize);

    public void WriteEnum<E, N, NS>(E e, string? name, N number, NS number_serialize)
        where E : Enum where NS : ISerialize<N>
        => WriteEnum(e);

    public void WriteEnum<N, NS>(string? name, N number, NS number_serialize)
        where NS : ISerialize<N>
        => throw new NotSupportedException();
}

public partial interface IAsyncSerializer
{
    public ValueTask WriteEnumAsync<E>(E e) where E : Enum
    {
        WriteEnum(e);
        return ValueTask.CompletedTask;
    }

    public ValueTask WriteEnumAsync<E, N, NS>(E e, N number, NS number_serialize)
        where E : Enum where NS : ISerialize<N>
    {
        WriteEnum(e, number, number_serialize);
        return ValueTask.CompletedTask;
    }

    public ValueTask WriteEnumAsync<E, N, NS>(E e, string? name, N number, NS number_serialize)
        where E : Enum where NS : ISerialize<N>
    {
        WriteEnum(e, name, number, number_serialize);
        return ValueTask.CompletedTask;
    }

    public ValueTask WriteEnumAsync<N, NS>(string? name, N number, NS number_serialize)
        where NS : ISerialize<N>
    {
        WriteEnum(name, number, number_serialize);
        return ValueTask.CompletedTask;
    }
}

#endregion

#region Tuple

public partial interface ISerializer
{
    public void WriteTupleStart(nuint len) => throw new NotSupportedException();

    public void WriteTupleElement<T, S>(in T value, S serialize) where S : ISerialize<T> =>
        throw new NotSupportedException();

    public void WriteTupleEnd() => throw new NotSupportedException();
}

public partial interface IAsyncSerializer
{
    public ValueTask WriteTupleStartAsync(nuint len)
    {
        WriteTupleStart(len);
        return ValueTask.CompletedTask;
    }

    public ValueTask WriteTupleElementAsync<T, S>(T value, S serialize) where S : ISerialize<T>
    {
        WriteTupleElement(value, serialize);
        return ValueTask.CompletedTask;
    }

    public ValueTask WriteTupleEndAsync()
    {
        WriteTupleEnd();
        return ValueTask.CompletedTask;
    }
}

#endregion

#region Seq

public partial interface ISerializer
{
    public void WriteSeqStart(nuint? len) => throw new NotSupportedException();
    public void WriteSeqStart<T>(nuint? len) => WriteSeqStart(len);

    public void WriteSeqElement<T, S>(in T value, S serialize) where S : ISerialize<T> =>
        throw new NotSupportedException();

    public void WriteSeqEnd() => throw new NotSupportedException();
}

public partial interface IAsyncSerializer
{
    public ValueTask WriteSeqStartAsync(nuint? len)
    {
        WriteSeqStart(len);
        return ValueTask.CompletedTask;
    }

    public ValueTask WriteSeqStartAsync<T>(nuint? len)
    {
        WriteSeqStart<T>(len);
        return ValueTask.CompletedTask;
    }

    public ValueTask WriteSeqElementAsync<T, S>(T value, S serialize) where S : ISerialize<T>
    {
        WriteSeqElement(value, serialize);
        return ValueTask.CompletedTask;
    }

    public ValueTask WriteSeqEndAsync()
    {
        WriteSeqEnd();
        return ValueTask.CompletedTask;
    }
}

#endregion

#region Map

public partial interface ISerializer
{
    public void WriteMapStart(nuint? len) => throw new NotSupportedException();
    public void WriteMapStart<K, V>(nuint? len) => WriteSeqStart(len);

    public void WriteMapKey<T, S>(in T key, S serialize) where S : ISerialize<T> => throw new NotSupportedException();

    public void WriteMapValue<T, S>(in T value, S serialize) where S : ISerialize<T> =>
        throw new NotSupportedException();

    public void WriteMapEntry<K, V, SK, SV>(in K key, in V value, SK key_serialize, SV value_serialize)
        where SK : ISerialize<K>
        where SV : ISerialize<V>
    {
        WriteMapKey(key, key_serialize);
        WriteMapValue(value, value_serialize);
    }

    public void WriteMapEnd() => throw new NotSupportedException();
}

public partial interface IAsyncSerializer
{
    public ValueTask WriteMapStartAsync(nuint? len)
    {
        WriteMapStart(len);
        return ValueTask.CompletedTask;
    }

    public ValueTask WriteMapStartAsync<K, V>(nuint? len)
    {
        WriteMapStart<K, V>(len);
        return ValueTask.CompletedTask;
    }

    public ValueTask WriteMapKeyAsync<T, S>(T key, S serialize) where S : ISerialize<T>
    {
        WriteMapKey(key, serialize);
        return ValueTask.CompletedTask;
    }

    public ValueTask WriteMapValueAsync<T, S>(T value, S serialize) where S : ISerialize<T>
    {
        WriteMapValue(value, serialize);
        return ValueTask.CompletedTask;
    }

    public ValueTask WriteMapEntryAsync<K, V, SK, SV>(K key, V value, SK key_serialize, SV value_serialize)
        where SK : ISerialize<K>
        where SV : ISerialize<V>
    {
        WriteMapEntry(key, value, key_serialize, value_serialize);
        return ValueTask.CompletedTask;
    }

    public ValueTask WriteMapEndAsync()
    {
        WriteMapEnd();
        return ValueTask.CompletedTask;
    }
}

#endregion

#region Struct

public partial interface ISerializer
{
    public void WriteStructStart(string? name, nuint? len) => throw new NotSupportedException();
    public void WriteStructStart<T>(string? name, nuint? len) => WriteSeqStart(len);

    public void WriteStructField<T, S>(string key, in T value, S serialize) where S : ISerialize<T> =>
        throw new NotSupportedException();

    public void WriteStructSkipField(string key) { }

    public void WriteStructEnd() => throw new NotSupportedException();
}

public partial interface IAsyncSerializer
{
    public ValueTask WriteStructStartAsync(string? name, nuint? len)
    {
        WriteStructStart(name, len);
        return ValueTask.CompletedTask;
    }

    public ValueTask WriteStructStartAsync<T>(string? name, nuint? len)
    {
        WriteStructStart<T>(name, len);
        return ValueTask.CompletedTask;
    }

    public ValueTask WriteStructFieldAsync<T, S>(string key, in T value, S serialize) where S : ISerialize<T>
    {
        WriteStructField(key, value, serialize);
        return ValueTask.CompletedTask;
    }

    public ValueTask WriteStructEndAsync()
    {
        WriteStructEnd();
        return ValueTask.CompletedTask;
    }
}

#endregion

#region Variant

public partial interface ISerializer
{
    public void WriteVariantStart(string? union_name, string? variant_name, nuint variant_tag)
        => throw new NotSupportedException();

    public void WriteVariantStart<U>(string? union_name, string? variant_name, nuint variant_tag)
        => WriteVariantStart(union_name, variant_name, variant_tag);

    public void WriteVariantStart<U, V>(string? union_name, string? variant_name, nuint variant_tag)
        => WriteVariantStart<U>(union_name, variant_name, variant_tag);

    /// <summary>Each Variant can only write one Value</summary>
    public void WriteVariantValueUnit() =>
        throw new NotSupportedException();

    /// <summary>Each Variant can only write one Value</summary>
    public void WriteVariantValue<T, S>(in T value, S serialize) where S : ISerialize<T> =>
        throw new NotSupportedException();

    public void WriteVariantEnd() => throw new NotSupportedException();
}

public partial interface IAsyncSerializer
{
    public ValueTask WriteVariantStartAsync(string? union_name, string? variant_name, nuint variant_tag)
    {
        WriteVariantStart(union_name, variant_name, variant_tag);
        return ValueTask.CompletedTask;
    }

    public ValueTask WriteVariantStartAsync<U>(string? union_name, string? variant_name, nuint variant_tag)
    {
        WriteVariantStart<U>(union_name, variant_name, variant_tag);
        return ValueTask.CompletedTask;
    }

    public ValueTask WriteVariantStartAsync<U, V>(string? union_name, string? variant_name, nuint variant_tag)
    {
        WriteVariantStart<U, V>(union_name, variant_name, variant_tag);
        return ValueTask.CompletedTask;
    }

    /// <summary>Each Variant can only write one Value</summary>
    public ValueTask WriteVariantValueUnitAsync()
    {
        WriteVariantValueUnit();
        return ValueTask.CompletedTask;
    }

    /// <summary>Each Variant can only write one Value</summary>
    public ValueTask WriteVariantValueAsync<T, S>(T value, S serialize)
        where S : ISerialize<T>
    {
        WriteVariantValue(value, serialize);
        return ValueTask.CompletedTask;
    }

    public ValueTask WriteVariantEndAsync()
    {
        WriteVariantEnd();
        return ValueTask.CompletedTask;
    }
}

#endregion

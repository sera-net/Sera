using System;
using System.Threading.Tasks;
using Sera.Core;
using Sera.Core.Ser;

namespace Sera.Json.Ser;

public record JsonSerializer(SeraJsonOptions Options) : ISerializer, IAsyncSerializer
{
    #region RuntimeProvider

    public ISerialize<object?> GetRuntimeSerialize() => Options.RuntimeProvider.GetRuntimeSerialize();

    public ISerialize<T> GetSerialize<T>() => Options.RuntimeProvider.GetSerialize<T>();

    #endregion

    #region Primitive

    public void WritePrimitive<T>(T value, SeraPrimitiveTypes type)
    {
        throw new NotImplementedException();
    }
    
    public ValueTask WritePrimitiveAsync<T>(T value, SeraPrimitiveTypes type)
    {
        throw new NotImplementedException();
    }

    #endregion

    public void WriteStructStart(string? name, UIntPtr? len)
    {
        throw new NotImplementedException();
    }

    public void WriteStructStart<T>(string? name, UIntPtr? len)
    {
        throw new NotImplementedException();
    }

    public void WriteStructField<T, S>(string key, in T value, S serialize) where S : ISerialize<T>
    {
        throw new NotImplementedException();
    }

    public void WriteStructSkipField(string key)
    {
        throw new NotImplementedException();
    }

    public void WriteStructEnd()
    {
        throw new NotImplementedException();
    }

    public void WriteMapStart(UIntPtr? len)
    {
        throw new NotImplementedException();
    }

    public void WriteMapStart<K, V>(UIntPtr? len)
    {
        throw new NotImplementedException();
    }

    public void WriteMapKey<T, S>(in T key, S serialize) where S : ISerialize<T>
    {
        throw new NotImplementedException();
    }

    public void WriteMapValue<T, S>(in T value, S serialize) where S : ISerialize<T>
    {
        throw new NotImplementedException();
    }

    public void WriteMapEntry<K, V, SK, SV>(in K key, in V value, SK key_serialize, SV value_serialize)
        where SK : ISerialize<K> where SV : ISerialize<V>
    {
        throw new NotImplementedException();
    }

    public void WriteMapEnd()
    {
        throw new NotImplementedException();
    }

    public void WriteSeqStart(UIntPtr? len)
    {
        throw new NotImplementedException();
    }

    public void WriteSeqStart<T>(UIntPtr? len)
    {
        throw new NotImplementedException();
    }

    public void WriteSeqElement<T, S>(in T value, S serialize) where S : ISerialize<T>
    {
        throw new NotImplementedException();
    }

    public void WriteSeqEnd()
    {
        throw new NotImplementedException();
    }

    public void WriteTupleStart(UIntPtr len)
    {
        throw new NotImplementedException();
    }

    public void WriteTupleElement<T, S>(in T value, S serialize) where S : ISerialize<T>
    {
        throw new NotImplementedException();
    }

    public void WriteTupleEnd()
    {
        throw new NotImplementedException();
    }

    public void WriteEnum<E>(E e) where E : Enum
    {
        throw new NotImplementedException();
    }

    public void WriteEnum<E, N, NS>(E e, N number, NS number_serialize) where E : Enum where NS : ISerialize<N>
    {
        throw new NotImplementedException();
    }

    public void WriteEnum<E, N, NS>(E e, string? name, N number, NS number_serialize)
        where E : Enum where NS : ISerialize<N>
    {
        throw new NotImplementedException();
    }

    public void WriteNullableNotNull<T, S>(in T value, S serialize) where S : ISerialize<T>
    {
        throw new NotImplementedException();
    }

    public void WriteNull()
    {
        throw new NotImplementedException();
    }

    public void WriteUnit()
    {
        throw new NotImplementedException();
    }

    public void WriteBytes(byte[] value)
    {
        throw new NotImplementedException();
    }

    public void WriteBytes(ReadOnlyMemory<byte> value)
    {
        throw new NotImplementedException();
    }

    public void WriteBytes(ReadOnlySpan<byte> value)
    {
        throw new NotImplementedException();
    }

    public void WriteString(string value)
    {
        throw new NotImplementedException();
    }

    public void WriteString(ReadOnlyMemory<char> value)
    {
        throw new NotImplementedException();
    }

    public void WriteString(ReadOnlySpan<char> value)
    {
        throw new NotImplementedException();
    }


    public void WriteVariantStart(string? union_name, string? variant_name, UIntPtr variant_tag)
    {
        throw new NotImplementedException();
    }

    public void WriteVariantStart<U>(string? union_name, string? variant_name, UIntPtr variant_tag)
    {
        throw new NotImplementedException();
    }

    public void WriteVariantStart<U, V>(string? union_name, string? variant_name, UIntPtr variant_tag)
    {
        throw new NotImplementedException();
    }

    public void WriteVariantValueUnit()
    {
        throw new NotImplementedException();
    }

    public void WriteVariantValue<T, S>(T value, S serialize) where S : ISerialize<T>
    {
        throw new NotImplementedException();
    }

    public void WriteVariantEnd()
    {
        throw new NotImplementedException();
    }

    public ValueTask WriteVariantStartAsync(string? union_name, string? variant_name, UIntPtr variant_tag)
    {
        throw new NotImplementedException();
    }

    public ValueTask WriteVariantStartAsync<U>(string? union_name, string? variant_name, UIntPtr variant_tag)
    {
        throw new NotImplementedException();
    }

    public ValueTask WriteVariantStartAsync<U, V>(string? union_name, string? variant_name, UIntPtr variant_tag)
    {
        throw new NotImplementedException();
    }

    public ValueTask WriteVariantValueUnitAsync()
    {
        throw new NotImplementedException();
    }

    public ValueTask WriteVariantValueAsync<T, S>(T value, S serialize) where S : ISerialize<T>
    {
        throw new NotImplementedException();
    }

    public ValueTask WriteVariantEndAsync()
    {
        throw new NotImplementedException();
    }

    public ValueTask WriteStructStartAsync(string? name, UIntPtr? len)
    {
        throw new NotImplementedException();
    }

    public ValueTask WriteStructStartAsync<T>(string? name, UIntPtr? len)
    {
        throw new NotImplementedException();
    }

    public ValueTask WriteStructFieldAsync<T, S>(string key, in T value, S serialize) where S : ISerialize<T>
    {
        throw new NotImplementedException();
    }

    public ValueTask WriteStructEndAsync()
    {
        throw new NotImplementedException();
    }

    public ValueTask WriteMapStartAsync(UIntPtr? len)
    {
        throw new NotImplementedException();
    }

    public ValueTask WriteMapStartAsync<K, V>(UIntPtr? len)
    {
        throw new NotImplementedException();
    }

    public ValueTask WriteMapKeyAsync<T, S>(T key, S serialize) where S : ISerialize<T>
    {
        throw new NotImplementedException();
    }

    public ValueTask WriteMapValueAsync<T, S>(T value, S serialize) where S : ISerialize<T>
    {
        throw new NotImplementedException();
    }

    public ValueTask WriteMapEntryAsync<K, V, SK, SV>(K key, V value, SK key_serialize, SV value_serialize)
        where SK : ISerialize<K> where SV : ISerialize<V>
    {
        throw new NotImplementedException();
    }

    public ValueTask WriteMapEndAsync()
    {
        throw new NotImplementedException();
    }

    public ValueTask WriteSeqStartAsync(UIntPtr? len)
    {
        throw new NotImplementedException();
    }

    public ValueTask WriteSeqStartAsync<T>(UIntPtr? len)
    {
        throw new NotImplementedException();
    }

    public ValueTask WriteSeqElementAsync<T, S>(T value, S serialize) where S : ISerialize<T>
    {
        throw new NotImplementedException();
    }

    public ValueTask WriteSeqEndAsync()
    {
        throw new NotImplementedException();
    }

    public ValueTask WriteTupleStartAsync(UIntPtr len)
    {
        throw new NotImplementedException();
    }

    public ValueTask WriteTupleElementAsync<T, S>(T value, S serialize) where S : ISerialize<T>
    {
        throw new NotImplementedException();
    }

    public ValueTask WriteTupleEndAsync()
    {
        throw new NotImplementedException();
    }

    public ValueTask WriteEnumAsync<E>(E e) where E : Enum
    {
        throw new NotImplementedException();
    }

    public ValueTask WriteEnumAsync<E, N, NS>(E e, N number, NS number_serialize)
        where E : Enum where NS : ISerialize<N>
    {
        throw new NotImplementedException();
    }

    public ValueTask WriteEnumAsync<E, N, NS>(E e, string? name, N number, NS number_serialize)
        where E : Enum where NS : ISerialize<N>
    {
        throw new NotImplementedException();
    }

    public ValueTask WriteNullableNotNullAsync<T, S>(T value, S serialize) where S : ISerialize<T>
    {
        throw new NotImplementedException();
    }

    public ValueTask WriteNullAsync()
    {
        throw new NotImplementedException();
    }

    public ValueTask WriteUnitAsync()
    {
        throw new NotImplementedException();
    }

    public ValueTask WriteBytesAsync(byte[] value)
    {
        throw new NotImplementedException();
    }

    public ValueTask WriteBytesAsync(ReadOnlyMemory<byte> value)
    {
        throw new NotImplementedException();
    }

    public ValueTask WriteStringAsync(string value)
    {
        throw new NotImplementedException();
    }

    public ValueTask WriteStringAsync(ReadOnlyMemory<char> value)
    {
        throw new NotImplementedException();
    }
}

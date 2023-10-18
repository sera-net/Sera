﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sera.Core.De;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

public readonly struct StringImpl :
    ISerialize<string>, IDeserialize<string>, IAsyncSerialize<string>, IAsyncDeserialize<string>
{
    public static StringImpl Instance { get; } = new();

    public void Write<S>(S serializer, string value, ISeraOptions options) where S : ISerializer
        => serializer.WriteString(value);

    public string Read<D>(D deserializer, ISeraOptions options) where D : IDeserializer
        => deserializer.ReadString();

    public ValueTask WriteAsync<S>(S serializer, string value, ISeraOptions options) where S : IAsyncSerializer
        => serializer.WriteStringAsync(value);

    public ValueTask<string> ReadAsync<D>(D deserializer, ISeraOptions options) where D : IAsyncDeserializer
        => deserializer.ReadStringAsync();
}

public readonly struct CharArrayStringImpl :
    ISerialize<char[]>, IAsyncSerialize<char[]>, IDeserialize<char[]>, IAsyncDeserialize<char[]>
{
    public static CharArrayStringImpl Instance { get; } = new();
    
    public void Write<S>(S serializer, char[] value, ISeraOptions options) where S : ISerializer
        => serializer.WriteString(value);

    public ValueTask WriteAsync<S>(S serializer, char[] value, ISeraOptions options) where S : IAsyncSerializer
        => serializer.WriteStringAsync(value);

    public char[] Read<D>(D deserializer, ISeraOptions options) where D : IDeserializer
        => deserializer.ReadStringAsCharArray();

    public ValueTask<char[]> ReadAsync<D>(D deserializer, ISeraOptions options) where D : IAsyncDeserializer
        => deserializer.ReadStringAsCharArrayAsync();
}

public readonly struct CharListStringImpl :
    ISerialize<List<char>>, IAsyncSerialize<List<char>>, IDeserialize<List<char>>, IAsyncDeserialize<List<char>>
{
    public static CharListStringImpl Instance { get; } = new();
    
    public void Write<S>(S serializer, List<char> value, ISeraOptions options) where S : ISerializer
        => serializer.WriteString(value);

    public ValueTask WriteAsync<S>(S serializer, List<char> value, ISeraOptions options) where S : IAsyncSerializer
        => serializer.WriteStringAsync(value);

    public List<char> Read<D>(D deserializer, ISeraOptions options) where D : IDeserializer
        => deserializer.ReadStringAsCharList();

    public ValueTask<List<char>> ReadAsync<D>(D deserializer, ISeraOptions options) where D : IAsyncDeserializer
        => deserializer.ReadStringAsCharListAsync();
}

public readonly struct MemoryStringImpl :
    ISerialize<Memory<char>>, IAsyncSerialize<Memory<char>>, IDeserialize<Memory<char>>, IAsyncDeserialize<Memory<char>>
{
    public static MemoryStringImpl Instance { get; } = new();
    
    public void Write<S>(S serializer, Memory<char> value, ISeraOptions options) where S : ISerializer
        => serializer.WriteString(value);

    public ValueTask WriteAsync<S>(S serializer, Memory<char> value, ISeraOptions options) where S : IAsyncSerializer
        => serializer.WriteStringAsync(value);

    public Memory<char> Read<D>(D deserializer, ISeraOptions options) where D : IDeserializer
        => deserializer.ReadStringAsMemory();

    public ValueTask<Memory<char>> ReadAsync<D>(D deserializer, ISeraOptions options) where D : IAsyncDeserializer
        => deserializer.ReadStringAsMemoryAsync();
}

public readonly struct ReadOnlyMemoryStringImpl :
    ISerialize<ReadOnlyMemory<char>>, IAsyncSerialize<ReadOnlyMemory<char>>, IDeserialize<ReadOnlyMemory<char>>, IAsyncDeserialize<ReadOnlyMemory<char>>
{
    public static ReadOnlyMemoryStringImpl Instance { get; } = new();
    
    public void Write<S>(S serializer, ReadOnlyMemory<char> value, ISeraOptions options) where S : ISerializer
        => serializer.WriteString(value);

    public ValueTask WriteAsync<S>(S serializer, ReadOnlyMemory<char> value, ISeraOptions options) where S : IAsyncSerializer
        => serializer.WriteStringAsync(value);

    public ReadOnlyMemory<char> Read<D>(D deserializer, ISeraOptions options) where D : IDeserializer
        => deserializer.ReadStringAsReadOnlyMemory();

    public ValueTask<ReadOnlyMemory<char>> ReadAsync<D>(D deserializer, ISeraOptions options) where D : IAsyncDeserializer
        => deserializer.ReadStringAsReadOnlyMemoryAsync();
}

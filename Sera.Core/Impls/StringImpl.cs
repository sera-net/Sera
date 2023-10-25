using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sera.Core.De;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

public readonly struct StringImpl :
    ISerialize<string>
{
    public static StringImpl Instance { get; } = new();

    public void Write<S>(S serializer, string value, ISeraOptions options) where S : ISerializer
        => serializer.WriteString(value);
}

public readonly struct ArrayStringImpl :
    ISerialize<char[]>
{
    public static ArrayStringImpl Instance { get; } = new();

    public void Write<S>(S serializer, char[] value, ISeraOptions options) where S : ISerializer
        => serializer.WriteString(value);
}

public readonly struct ListStringImpl :
    ISerialize<List<char>>
{
    public static ListStringImpl Instance { get; } = new();

    public void Write<S>(S serializer, List<char> value, ISeraOptions options) where S : ISerializer
        => serializer.WriteString(value);
}

public readonly struct MemoryStringImpl :
    ISerialize<Memory<char>>
{
    public static MemoryStringImpl Instance { get; } = new();

    public void Write<S>(S serializer, Memory<char> value, ISeraOptions options) where S : ISerializer
        => serializer.WriteString(value);
}

public readonly struct ReadOnlyMemoryStringImpl :
    ISerialize<ReadOnlyMemory<char>>
{
    public static ReadOnlyMemoryStringImpl Instance { get; } = new();

    public void Write<S>(S serializer, ReadOnlyMemory<char> value, ISeraOptions options) where S : ISerializer
        => serializer.WriteString(value);
}

public readonly struct ListBaseStringSerializeImpl<L> : ISerialize<L>
    where L : List<char>
{
    public static ListBaseStringSerializeImpl<L> Instance { get; } = new();

    public void Write<S>(S serializer, L value, ISeraOptions options) where S : ISerializer
        => serializer.WriteString(value);
}

using System;
using System.Threading.Tasks;
using Sera.Core.De;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

public class ToStringSerializeImpl<T> : ISerialize<T>, IAsyncSerialize<T>
{
    public static ToStringSerializeImpl<T> Instance { get; } = new();

    public void Write<S>(S serializer, T value, ISeraOptions options) where S : ISerializer
    {
        var str = $"{value}";
        serializer.WriteString(str);
    }

    public ValueTask WriteAsync<S>(S serializer, T value, ISeraOptions options) where S : IAsyncSerializer
    {
        var str = $"{value}";
        return serializer.WriteStringAsync(str);
    }
}

public class SpanParsableSerializeImpl<T> : IDeserialize<T>, IStringDeserializerVisitor<T>, IAsyncDeserialize<T>,
    IAsyncStringDeserializerVisitor<T>
    where T : ISpanParsable<T>
{
    public static SpanParsableSerializeImpl<T> Instance { get; } = new();

    public T Read<D>(D deserializer, ISeraOptions options) where D : IDeserializer
        => deserializer.ReadString<T, SpanParsableSerializeImpl<T>>(this);

    public T VisitString<A>(A access) where A : IStringAccess
    {
        var memory = access.ReadStringAsMemory();
        return T.Parse(memory.Span, null);
    }

    public ValueTask<T> ReadAsync<D>(D deserializer, ISeraOptions options) where D : IAsyncDeserializer
        => deserializer.ReadStringAsync<T, SpanParsableSerializeImpl<T>>(this);

    public async ValueTask<T> VisitStringAsync<A>(A access) where A : IAsyncStringAccess
    {
        var memory = await access.ReadStringAsMemoryAsync();
        return T.Parse(memory.Span, null);
    }
}

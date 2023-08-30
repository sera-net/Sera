using System;
using System.Threading.Tasks;
using Sera.Core.De;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

public record SkipImpl<T> : ISerialize<T>, IDeserialize<T>
{
    private static readonly Lazy<SkipImpl<T>> Lazy = new(static () => new());
    public static SkipImpl<T> Instance => Lazy.Value;

    public void Write<S>(S serializer, in T value, SeraOptions options) where S : ISerializer
        => serializer.WriteUnit();

    public ValueTask WriteAsync<S>(S serializer, T value, SeraOptions options) where S : IAsyncSerializer
        => serializer.WriteUnitAsync();

    public void Read<D>(D deserializer, out T value, SeraOptions options) where D : IDeserializer
    {
        deserializer.Skip();
        value = default!;
    }

    public async ValueTask<T> ReadAsync<D>(D deserializer, SeraOptions options) where D : IAsyncDeserializer
    {
        await deserializer.SkipAsync();
        return default!;
    }
}

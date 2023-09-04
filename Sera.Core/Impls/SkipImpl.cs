using System;
using System.Threading.Tasks;
using Sera.Core.De;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

public record SkipImpl<T> : ISerialize<T>, IDeserialize<T>, IAsyncSerialize<T>, IAsyncDeserialize<T>
{
    public static SkipImpl<T> Instance => new();

    public void Write<S>(S serializer, T value, ISeraOptions options) where S : ISerializer
        => serializer.WriteUnit();

    public ValueTask WriteAsync<S>(S serializer, T value, ISeraOptions options) where S : IAsyncSerializer
        => serializer.WriteUnitAsync();

    public T Read<D>(D deserializer, ISeraOptions options) where D : IDeserializer
    {
        deserializer.Skip();
        return default!;
    }

    public async ValueTask<T> ReadAsync<D>(D deserializer, ISeraOptions options) where D : IAsyncDeserializer
    {
        await deserializer.SkipAsync();
        return default!;
    }
}

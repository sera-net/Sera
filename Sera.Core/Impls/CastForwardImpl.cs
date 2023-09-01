using System.Threading.Tasks;
using Sera.Core.De;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

public record CastForwardSerializeImpl<T, ST>(ST Serialize) : ISerialize<object?>
    where ST : ISerialize<T>
{
    public void Write<S>(S serializer, object? value, SeraOptions options) where S : ISerializer
        => Serialize.Write(serializer, (T)value!, options);
}

public record CastForwardDeserializeImpl<T, DT>(DT Deserialize) : IDeserialize<object?>
    where DT : IDeserialize<T>
{
    public object? Read<D>(D deserializer, SeraOptions options) where D : IDeserializer
        => Deserialize.Read(deserializer, options);
}

public record AsyncCastForwardSerializeImpl<T, ST>(ST Serialize) : IAsyncSerialize<object?>
    where ST : IAsyncSerialize<T>
{
    public ValueTask WriteAsync<S>(S serializer, object? value, SeraOptions options) where S : IAsyncSerializer
        => Serialize.WriteAsync(serializer, (T)value!, options);
}

public record AsyncCastForwardDeserializeImpl<T, DT>(DT Deserialize) : IAsyncDeserialize<object?>
    where DT : IAsyncDeserialize<T>
{
    public async ValueTask<object?> ReadAsync<D>(D deserializer, SeraOptions options) where D : IAsyncDeserializer
        => await Deserialize.ReadAsync(deserializer, options);
}

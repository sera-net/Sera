using System.Threading.Tasks;
using Sera.Core.De;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

public record CastForwardSerializeImpl<T, ST>(ST Serialize) : ISerialize<object?>
    where ST : ISerialize<T>
{
    public void Write<S>(S serializer, in object? value, SeraOptions options) where S : ISerializer
        => Serialize.Write(serializer, (T)value!, options);

    public ValueTask WriteAsync<S>(S serializer, object? value, SeraOptions options) where S : IAsyncSerializer
        => Serialize.WriteAsync(serializer, (T)value!, options);
}

public record CastForwardDeserializeImpl<T, DT>(DT Deserialize) : IDeserialize<object?>
    where DT : IDeserialize<T>
{
    public void Read<D>(D deserializer, out object? value, SeraOptions options) where D : IDeserializer
    {
        Deserialize.Read(deserializer, out var r, options);
        value = r;
    }

    public async ValueTask<object?> ReadAsync<D>(D deserializer, SeraOptions options) where D : IAsyncDeserializer
        => await Deserialize.ReadAsync(deserializer, options);
}

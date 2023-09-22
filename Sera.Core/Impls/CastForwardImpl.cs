using System.Threading.Tasks;
using Sera.Core.De;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

public readonly record struct CastForwardSerializeImpl<T, ST>(ST Serialize) : ISerialize<object?>
    where ST : ISerialize<T>
{
    public void Write<S>(S serializer, object? value, ISeraOptions options) where S : ISerializer
        => Serialize.Write(serializer, (T)value!, options);
}

public readonly record struct CastForwardDeserializeImpl<T, DT>(DT Deserialize) : IDeserialize<object?>
    where DT : IDeserialize<T>
{
    public object? Read<D>(D deserializer, ISeraOptions options) where D : IDeserializer
        => Deserialize.Read(deserializer, options);
}

public readonly record struct AsyncCastForwardSerializeImpl<T, ST>(ST Serialize) : IAsyncSerialize<object?>
    where ST : IAsyncSerialize<T>
{
    public ValueTask WriteAsync<S>(S serializer, object? value, ISeraOptions options) where S : IAsyncSerializer
        => Serialize.WriteAsync(serializer, (T)value!, options);
}

public readonly record struct AsyncCastForwardDeserializeImpl<T, DT>(DT Deserialize) : IAsyncDeserialize<object?>
    where DT : IAsyncDeserialize<T>
{
    public async ValueTask<object?> ReadAsync<D>(D deserializer, ISeraOptions options) where D : IAsyncDeserializer
        => await Deserialize.ReadAsync(deserializer, options);
}

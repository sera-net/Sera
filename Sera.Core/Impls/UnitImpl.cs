using System.Threading.Tasks;
using Sera.Core.De;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

public record UnitImpl<T> : ISerialize<T>, IAsyncSerialize<T>, IDeserialize<T>, IAsyncDeserialize<T>
{
    public static UnitImpl<T> Instance { get; } = new();

    public void Write<S>(S serializer, T value, ISeraOptions options) where S : ISerializer
        => serializer.WriteUnit();

    public T Read<D>(D deserializer, ISeraOptions options) where D : IDeserializer
    {
        deserializer.ReadUnit();
        return default!;
    }

    public ValueTask WriteAsync<S>(S serializer, T value, ISeraOptions options) where S : IAsyncSerializer
        => serializer.WriteUnitAsync();

    public async ValueTask<T> ReadAsync<D>(D deserializer, ISeraOptions options) where D : IAsyncDeserializer
    {
        await deserializer.ReadUnitAsync();
        return default!;
    }
}

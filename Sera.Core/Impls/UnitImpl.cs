using System.Threading.Tasks;
using Sera.Core.De;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

public record UnitImpl<T> : ISerialize<T>, IDeserialize<T>
{
    public static UnitImpl<T> Instance { get; } = new();
    public void Write<S>(S serializer, in T value, SeraOptions options) where S : ISerializer
        => serializer.WriteUnit();

    public void Read<D>(D deserializer, out T value, SeraOptions options) where D : IDeserializer
    {
        deserializer.ReadUnit();
        value = default!;
    }

    public ValueTask WriteAsync<S>(S serializer, T value, SeraOptions options) where S : IAsyncSerializer
        => serializer.WriteUnitAsync();

    public async ValueTask<T> ReadAsync<D>(D deserializer, SeraOptions options) where D : IAsyncDeserializer
    {
        await deserializer.ReadUnitAsync();
        return default!;
    }
}

using System.Threading.Tasks;
using Sera.Core.De;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

public record StringImpl : ISerialize<string>, IDeserialize<string>, IAsyncSerialize<string>, IAsyncDeserialize<string>
{
    public static StringImpl Instance { get; } = new();

    public void Write<S>(S serializer, string value, SeraOptions options) where S : ISerializer
        => serializer.WriteString(value);

    public string Read<D>(D deserializer, SeraOptions options) where D : IDeserializer
        => deserializer.ReadString();

    public ValueTask WriteAsync<S>(S serializer, string value, SeraOptions options) where S : IAsyncSerializer
        => serializer.WriteStringAsync(value);

    public ValueTask<string> ReadAsync<D>(D deserializer, SeraOptions options) where D : IAsyncDeserializer
        => deserializer.ReadStringAsync();
}

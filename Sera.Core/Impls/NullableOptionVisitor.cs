using System.Threading.Tasks;
using Sera.Core.De;

namespace Sera.Core.Impls;

public readonly record struct NullableOptionVisitor<R, DR>(DR deserialize) : IOptionDeserializerVisitor<R?>
    where R : struct where DR : IDeserialize<R>
{
    public R? VisitNone() => null;

    public R? VisitSome<D>(D deserializer, ISeraOptions options) where D : IDeserializer
        => deserialize.Read(deserializer, options);
}

public readonly record struct AsyncNullableOptionVisitor<R, DR>(DR deserialize) : IAsyncOptionDeserializerVisitor<R?>
    where R : struct where DR : IAsyncDeserialize<R>
{
    public ValueTask<R?> VisitNoneAsync() => ValueTask.FromResult<R?>(null);

    public async ValueTask<R?> VisitSomeAsync<D>(D deserializer, ISeraOptions options) where D : IAsyncDeserializer
        => await deserialize.ReadAsync(deserializer, options);
}

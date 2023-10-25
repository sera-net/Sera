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

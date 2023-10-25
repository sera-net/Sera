using System.Threading.Tasks;
using Sera.Core.De;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

public struct UnitImpl<T> : ISerialize<T>
{
    public static UnitImpl<T> Instance { get; } = new();

    public void Write<S>(S serializer, T value, ISeraOptions options) where S : ISerializer
        => serializer.WriteUnit();
}

using System.Numerics;
using Sera.Utils;

namespace Sera.Core.Impls.De;

public readonly struct IdentityMapper<T> : ISeraMapper<T, T>
{
    public T Map(T value, InType<T>? u) => value;
}

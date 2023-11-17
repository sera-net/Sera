using Sera.Utils;

namespace Sera.Core.Impls.De.Misc;

public readonly struct IdentityMapper<T> : ISeraMapper<T, T>
{
    public T Map(T value, InType<T>? u) => value;
}

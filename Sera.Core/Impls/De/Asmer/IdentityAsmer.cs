using System.Runtime.CompilerServices;
using Sera.Utils;

namespace Sera.Core.Impls.De;

public struct IdentityAsmer<T> : ISeraAsmer<T>, ISeraValueAsmer<T>
{
    private T Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Provide(T value) => Value = value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Asm(Type<T> t) => Value;
}

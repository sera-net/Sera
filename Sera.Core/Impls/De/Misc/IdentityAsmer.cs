using System.Runtime.CompilerServices;
using Sera.Utils;

namespace Sera.Core.Impls.De;

public struct IdentityAsmer<T> : ISeraAsmer<T>, IValueSeraAsmer<T>
{
    private T Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Provide(T value) => Value = value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Asm() => Value;

    public readonly struct Asmable : ISeraAsmable<IdentityAsmer<T>>
    {
        [AssocType]
        public abstract class A(IdentityAsmer<T> type);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IdentityAsmer<T> Asmer() => new();
    }
}

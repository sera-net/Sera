using System.Runtime.CompilerServices;

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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IdentityAsmer<T> Asmer() => new();
    }
}

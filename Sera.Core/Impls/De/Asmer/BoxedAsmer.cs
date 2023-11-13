using System.Runtime.CompilerServices;
using Sera.Utils;

namespace Sera.Core.Impls.De;

public readonly struct BoxedAsmer<T>() : ISeraAsmer<T>, ISeraValueAsmer<T>
{
    private readonly Box<T> box = new(default!);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Provide(T value) => box.Value = value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Asm(Type<T> t) => box.Value;
}

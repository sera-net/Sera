using System;
using System.Runtime.CompilerServices;

namespace Sera.Core.Impls.Ser;

public readonly struct EnumAsUnderlyingImpl<T, U, D>(D dep) : ISeraVision<T>
    where T : Enum
    where D : ISeraVision<U>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, T value) where V : ASeraVisitor<R>
        => dep.Accept<R, V>(visitor, As(value));

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static U As(T value) => (U)(object)value;
}

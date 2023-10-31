using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace Sera.Core.Impls.Ser;

public readonly struct ArrayImpl<T, D>(D dep) :
    ISeraVision<T[]>, ISeraVision<ReadOnlySequence<T>>,
    ISeraVision<ReadOnlyMemory<T>>, ISeraVision<Memory<T>>
    where D : ISeraVision<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, T[] value) where V : ASeraVisitor<R>
        => visitor.VArray(dep, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, ReadOnlySequence<T> value) where V : ASeraVisitor<R>
        => visitor.VArray(dep, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, ReadOnlyMemory<T> value) where V : ASeraVisitor<R>
        => visitor.VArray(dep, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, Memory<T> value) where V : ASeraVisitor<R>
        => visitor.VArray(dep, (ReadOnlyMemory<T>)value);
}

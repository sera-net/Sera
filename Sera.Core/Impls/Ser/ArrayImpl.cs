using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Sera.Core.Impls.Ser;

public readonly struct ArrayImpl<T, D>(D dep) :
    ITypeVision<T[]>, ITypeVision<List<T>>, ITypeVision<ReadOnlySequence<T>>,
    ITypeVision<ReadOnlyMemory<T>>, ITypeVision<Memory<T>>
    where D : ITypeVision<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, T[] value) where V : ATypeVisitor<R>
        => visitor.VArray(dep, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, List<T> value) where V : ATypeVisitor<R>
        => visitor.VArray(dep, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, ReadOnlySequence<T> value) where V : ATypeVisitor<R>
        => visitor.VArray(dep, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, ReadOnlyMemory<T> value) where V : ATypeVisitor<R>
        => visitor.VArray(dep, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, Memory<T> value) where V : ATypeVisitor<R>
        => visitor.VArray(dep, (ReadOnlyMemory<T>)value);
}

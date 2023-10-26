using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Sera.Core.Impls.Ser;

public readonly struct BytesImpl :
    ITypeVision<byte[]>, ITypeVision<List<byte>>, ITypeVision<ReadOnlySequence<byte>>,
    ITypeVision<ReadOnlyMemory<byte>>, ITypeVision<Memory<byte>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, byte[] value) where V : ATypeVisitor<R>
        => visitor.VBytes(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, List<byte> value) where V : ATypeVisitor<R>
        => visitor.VBytes(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, ReadOnlySequence<byte> value) where V : ATypeVisitor<R>
        => visitor.VBytes(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, ReadOnlyMemory<byte> value) where V : ATypeVisitor<R>
        => visitor.VBytes(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, Memory<byte> value) where V : ATypeVisitor<R>
        => visitor.VBytes(value);
}

using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace Sera.Core.Impls.Ser;

public readonly struct BytesImpl :
    ISeraVision<byte[]>, ISeraVision<ReadOnlySequence<byte>>,
    ISeraVision<ReadOnlyMemory<byte>>, ISeraVision<Memory<byte>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, byte[] value) where V : ASeraVisitor<R>
        => visitor.VBytes(value.NotNull());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, ReadOnlySequence<byte> value) where V : ASeraVisitor<R>
        => visitor.VBytes(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, ReadOnlyMemory<byte> value) where V : ASeraVisitor<R>
        => visitor.VBytes(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, Memory<byte> value) where V : ASeraVisitor<R>
        => visitor.VBytes(value);
}

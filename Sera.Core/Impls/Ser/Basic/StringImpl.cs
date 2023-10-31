using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Sera.Core.Impls.Ser;

public readonly struct StringImpl :
    ISeraVision<string>, ISeraVision<char[]>,
    ISeraVision<ReadOnlyMemory<char>>, ISeraVision<Memory<char>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, string value) where V : ASeraVisitor<R>
        => visitor.VString(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, char[] value) where V : ASeraVisitor<R>
        => visitor.VString(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, ReadOnlyMemory<char> value) where V : ASeraVisitor<R>
        => visitor.VString(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, Memory<char> value) where V : ASeraVisitor<R>
        => visitor.VString(value);
}

public readonly struct StringEncodedImpl(Encoding encoding) :
    ISeraVision<byte[]>,
    ISeraVision<ReadOnlyMemory<byte>>, ISeraVision<Memory<byte>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, byte[] value) where V : ASeraVisitor<R>
        => visitor.VString(value, encoding);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, ReadOnlyMemory<byte> value) where V : ASeraVisitor<R>
        => visitor.VString(value, encoding);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, Memory<byte> value) where V : ASeraVisitor<R>
        => visitor.VString(value, encoding);
}

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Sera.Core.Impls.Ser;

public readonly struct StringImpl :
    ITypeVision<string>, ITypeVision<char[]>,
    ITypeVision<ReadOnlyMemory<char>>, ITypeVision<Memory<char>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, string value) where V : ATypeVisitor<R>
        => visitor.VString(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, char[] value) where V : ATypeVisitor<R>
        => visitor.VString(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, ReadOnlyMemory<char> value) where V : ATypeVisitor<R>
        => visitor.VString(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, Memory<char> value) where V : ATypeVisitor<R>
        => visitor.VString(value);
}

public readonly struct StringEncodedImpl(Encoding encoding) :
    ITypeVision<byte[]>,
    ITypeVision<ReadOnlyMemory<byte>>, ITypeVision<Memory<byte>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, byte[] value) where V : ATypeVisitor<R>
        => visitor.VString(value, encoding);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, ReadOnlyMemory<byte> value) where V : ATypeVisitor<R>
        => visitor.VString(value, encoding);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, Memory<byte> value) where V : ATypeVisitor<R>
        => visitor.VString(value, encoding);
}

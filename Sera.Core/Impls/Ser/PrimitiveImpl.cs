using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using Sera.Utils;

namespace Sera.Core.Impls.Ser;

public readonly struct PrimitiveImpl(SeraFormats? formats = null) :
    ITypeVision<bool>,
    ITypeVision<sbyte>, ITypeVision<byte>,
    ITypeVision<short>, ITypeVision<ushort>,
    ITypeVision<int>, ITypeVision<uint>,
    ITypeVision<long>, ITypeVision<ulong>,
    ITypeVision<Int128>, ITypeVision<UInt128>,
    ITypeVision<nint>, ITypeVision<nuint>,
    ITypeVision<Half>, ITypeVision<float>,
    ITypeVision<double>, ITypeVision<decimal>,
    ITypeVision<BigInteger>, ITypeVision<Complex>,
    ITypeVision<TimeSpan>,
    ITypeVision<DateOnly>, ITypeVision<TimeOnly>,
    ITypeVision<DateTime>, ITypeVision<DateTimeOffset>,
    ITypeVision<Guid>,
    ITypeVision<Range>, ITypeVision<Index>,
    ITypeVision<char>, ITypeVision<Rune>,
    ITypeVision<Uri>, ITypeVision<Version>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, bool value) where V : ATypeVisitor<R>
        => visitor.VPrimitive(value, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, sbyte value) where V : ATypeVisitor<R>
        => visitor.VPrimitive(value, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, byte value) where V : ATypeVisitor<R>
        => visitor.VPrimitive(value, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, short value) where V : ATypeVisitor<R>
        => visitor.VPrimitive(value, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, ushort value) where V : ATypeVisitor<R>
        => visitor.VPrimitive(value, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, int value) where V : ATypeVisitor<R>
        => visitor.VPrimitive(value, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, uint value) where V : ATypeVisitor<R>
        => visitor.VPrimitive(value, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, long value) where V : ATypeVisitor<R>
        => visitor.VPrimitive(value, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, ulong value) where V : ATypeVisitor<R>
        => visitor.VPrimitive(value, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, Int128 value) where V : ATypeVisitor<R>
        => visitor.VPrimitive(value, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, UInt128 value) where V : ATypeVisitor<R>
        => visitor.VPrimitive(value, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, IntPtr value) where V : ATypeVisitor<R>
        => visitor.VPrimitive(value, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, UIntPtr value) where V : ATypeVisitor<R>
        => visitor.VPrimitive(value, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, Half value) where V : ATypeVisitor<R>
        => visitor.VPrimitive(value, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, float value) where V : ATypeVisitor<R>
        => visitor.VPrimitive(value, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, double value) where V : ATypeVisitor<R>
        => visitor.VPrimitive(value, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, decimal value) where V : ATypeVisitor<R>
        => visitor.VPrimitive(value, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, BigInteger value) where V : ATypeVisitor<R>
        => visitor.VPrimitive(value, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, Complex value) where V : ATypeVisitor<R>
        => visitor.VPrimitive(value, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, TimeSpan value) where V : ATypeVisitor<R>
        => visitor.VPrimitive(value, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, DateOnly value) where V : ATypeVisitor<R>
        => visitor.VPrimitive(value, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, TimeOnly value) where V : ATypeVisitor<R>
        => visitor.VPrimitive(value, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, DateTime value) where V : ATypeVisitor<R>
        => visitor.VPrimitive(value, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, DateTimeOffset value) where V : ATypeVisitor<R>
        => visitor.VPrimitive(value, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, Guid value) where V : ATypeVisitor<R>
        => visitor.VPrimitive(value, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, Range value) where V : ATypeVisitor<R>
        => visitor.VPrimitive(value, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, Index value) where V : ATypeVisitor<R>
        => visitor.VPrimitive(value, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, char value) where V : ATypeVisitor<R>
        => visitor.VPrimitive(value, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, Rune value) where V : ATypeVisitor<R>
        => visitor.VPrimitive(value, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, Uri value) where V : ATypeVisitor<R>
        => visitor.VPrimitive(value, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, Version value) where V : ATypeVisitor<R>
        => visitor.VPrimitive(value, formats);
}

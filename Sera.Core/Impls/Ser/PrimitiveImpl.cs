using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using Sera.Utils;

namespace Sera.Core.Impls.Ser;

public readonly struct PrimitiveImpl(SeraFormats? formats = null) :
    ISeraVision<bool>,
    ISeraVision<sbyte>, ISeraVision<byte>,
    ISeraVision<short>, ISeraVision<ushort>,
    ISeraVision<int>, ISeraVision<uint>,
    ISeraVision<long>, ISeraVision<ulong>,
    ISeraVision<Int128>, ISeraVision<UInt128>,
    ISeraVision<nint>, ISeraVision<nuint>,
    ISeraVision<Half>, ISeraVision<float>,
    ISeraVision<double>, ISeraVision<decimal>,
    ISeraVision<BigInteger>, ISeraVision<Complex>,
    ISeraVision<TimeSpan>,
    ISeraVision<DateOnly>, ISeraVision<TimeOnly>,
    ISeraVision<DateTime>, ISeraVision<DateTimeOffset>,
    ISeraVision<Guid>,
    ISeraVision<Range>, ISeraVision<Index>,
    ISeraVision<char>, ISeraVision<Rune>,
    ISeraVision<Uri>, ISeraVision<Version>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, bool value) where V : ASeraVisitor<R>
        => visitor.VPrimitive(value, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, sbyte value) where V : ASeraVisitor<R>
        => visitor.VPrimitive(value, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, byte value) where V : ASeraVisitor<R>
        => visitor.VPrimitive(value, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, short value) where V : ASeraVisitor<R>
        => visitor.VPrimitive(value, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, ushort value) where V : ASeraVisitor<R>
        => visitor.VPrimitive(value, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, int value) where V : ASeraVisitor<R>
        => visitor.VPrimitive(value, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, uint value) where V : ASeraVisitor<R>
        => visitor.VPrimitive(value, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, long value) where V : ASeraVisitor<R>
        => visitor.VPrimitive(value, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, ulong value) where V : ASeraVisitor<R>
        => visitor.VPrimitive(value, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, Int128 value) where V : ASeraVisitor<R>
        => visitor.VPrimitive(value, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, UInt128 value) where V : ASeraVisitor<R>
        => visitor.VPrimitive(value, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, IntPtr value) where V : ASeraVisitor<R>
        => visitor.VPrimitive(value, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, UIntPtr value) where V : ASeraVisitor<R>
        => visitor.VPrimitive(value, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, Half value) where V : ASeraVisitor<R>
        => visitor.VPrimitive(value, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, float value) where V : ASeraVisitor<R>
        => visitor.VPrimitive(value, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, double value) where V : ASeraVisitor<R>
        => visitor.VPrimitive(value, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, decimal value) where V : ASeraVisitor<R>
        => visitor.VPrimitive(value, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, BigInteger value) where V : ASeraVisitor<R>
        => visitor.VPrimitive(value, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, Complex value) where V : ASeraVisitor<R>
        => visitor.VPrimitive(value, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, TimeSpan value) where V : ASeraVisitor<R>
        => visitor.VPrimitive(value, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, DateOnly value) where V : ASeraVisitor<R>
        => visitor.VPrimitive(value, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, TimeOnly value) where V : ASeraVisitor<R>
        => visitor.VPrimitive(value, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, DateTime value) where V : ASeraVisitor<R>
        => visitor.VPrimitive(value, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, DateTimeOffset value) where V : ASeraVisitor<R>
        => visitor.VPrimitive(value, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, Guid value) where V : ASeraVisitor<R>
        => visitor.VPrimitive(value, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, Range value) where V : ASeraVisitor<R>
        => visitor.VPrimitive(value, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, Index value) where V : ASeraVisitor<R>
        => visitor.VPrimitive(value, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, char value) where V : ASeraVisitor<R>
        => visitor.VPrimitive(value, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, Rune value) where V : ASeraVisitor<R>
        => visitor.VPrimitive(value, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, Uri value) where V : ASeraVisitor<R>
        => visitor.VPrimitive(value, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, Version value) where V : ASeraVisitor<R>
        => visitor.VPrimitive(value, formats);
}

using System;
using System.Collections.Frozen;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
    ISeraVision<NFloat>,
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
    public R Accept<R, V>(V visitor, NFloat value) where V : ASeraVisitor<R>
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

    private static readonly FrozenSet<Type> PrimitiveTypes = new[]
    {
        typeof(bool),
        typeof(sbyte),
        typeof(byte),
        typeof(short),
        typeof(ushort),
        typeof(int),
        typeof(uint),
        typeof(long),
        typeof(ulong),
        typeof(Int128),
        typeof(UInt128),
        typeof(nint),
        typeof(nuint),
        typeof(Half),
        typeof(float),
        typeof(double),
        typeof(decimal),
        typeof(NFloat),
        typeof(BigInteger),
        typeof(Complex),
        typeof(TimeSpan),
        typeof(DateOnly),
        typeof(TimeOnly),
        typeof(DateTime),
        typeof(DateTimeOffset),
        typeof(Guid),
        typeof(Range),
        typeof(Index),
        typeof(char),
        typeof(Rune),
        typeof(Uri),
        typeof(Version),
    }.ToFrozenSet();

    public static bool IsPrimitiveType(Type type) => PrimitiveTypes.Contains(type);
}

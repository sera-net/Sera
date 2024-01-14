using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Sera.Utils;

namespace Sera.Core.Impls.De;

public readonly struct PrimitiveImpl(SeraFormats? formats = null) :
    ISeraColion<bool>,
    ISeraColion<sbyte>, ISeraColion<byte>,
    ISeraColion<short>, ISeraColion<ushort>,
    ISeraColion<int>, ISeraColion<uint>,
    ISeraColion<long>, ISeraColion<ulong>,
    ISeraColion<Int128>, ISeraColion<UInt128>,
    ISeraColion<nint>, ISeraColion<nuint>,
    ISeraColion<Half>, ISeraColion<float>,
    ISeraColion<double>, ISeraColion<decimal>,
    ISeraColion<NFloat>,
    ISeraColion<BigInteger>, ISeraColion<Complex>,
    ISeraColion<TimeSpan>, ISeraColion<DateOnly>, ISeraColion<TimeOnly>,
    ISeraColion<DateTime>, ISeraColion<DateTimeOffset>,
    ISeraColion<Guid>,
    ISeraColion<Range>, ISeraColion<Index>,
    ISeraColion<char>, ISeraColion<Rune>,
    ISeraColion<Uri>, ISeraColion<Version>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<bool>? t) where C : ISeraColctor<bool, R>
        => colctor.CPrimitive(new IdentityMapper<bool>(), new Type<bool>(), formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<sbyte>? t = null) where C : ISeraColctor<sbyte, R>
        => colctor.CPrimitive(new IdentityMapper<sbyte>(), new Type<sbyte>(), formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<byte>? t = null) where C : ISeraColctor<byte, R>
        => colctor.CPrimitive(new IdentityMapper<byte>(), new Type<byte>(), formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<short>? t = null) where C : ISeraColctor<short, R>
        => colctor.CPrimitive(new IdentityMapper<short>(), new Type<short>(), formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<ushort>? t = null) where C : ISeraColctor<ushort, R>
        => colctor.CPrimitive(new IdentityMapper<ushort>(), new Type<ushort>(), formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<int>? t = null) where C : ISeraColctor<int, R>
        => colctor.CPrimitive(new IdentityMapper<int>(), new Type<int>(), formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<uint>? t = null) where C : ISeraColctor<uint, R>
        => colctor.CPrimitive(new IdentityMapper<uint>(), new Type<uint>(), formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<long>? t = null) where C : ISeraColctor<long, R>
        => colctor.CPrimitive(new IdentityMapper<long>(), new Type<long>(), formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<ulong>? t = null) where C : ISeraColctor<ulong, R>
        => colctor.CPrimitive(new IdentityMapper<ulong>(), new Type<ulong>(), formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<Int128>? t = null) where C : ISeraColctor<Int128, R>
        => colctor.CPrimitive(new IdentityMapper<Int128>(), new Type<Int128>(), formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<UInt128>? t = null) where C : ISeraColctor<UInt128, R>
        => colctor.CPrimitive(new IdentityMapper<UInt128>(), new Type<UInt128>(), formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<IntPtr>? t = null) where C : ISeraColctor<IntPtr, R>
        => colctor.CPrimitive(new IdentityMapper<IntPtr>(), new Type<IntPtr>(), formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<UIntPtr>? t = null) where C : ISeraColctor<UIntPtr, R>
        => colctor.CPrimitive(new IdentityMapper<UIntPtr>(), new Type<UIntPtr>(), formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<Half>? t = null) where C : ISeraColctor<Half, R>
        => colctor.CPrimitive(new IdentityMapper<Half>(), new Type<Half>(), formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<float>? t) where C : ISeraColctor<float, R>
        => colctor.CPrimitive(new IdentityMapper<float>(), new Type<float>(), formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<double>? t = null) where C : ISeraColctor<double, R>
        => colctor.CPrimitive(new IdentityMapper<double>(), new Type<double>(), formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<decimal>? t = null) where C : ISeraColctor<decimal, R>
        => colctor.CPrimitive(new IdentityMapper<decimal>(), new Type<decimal>(), formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<NFloat>? t = null) where C : ISeraColctor<NFloat, R>
        => colctor.CPrimitive(new IdentityMapper<NFloat>(), new Type<NFloat>(), formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<BigInteger>? t = null) where C : ISeraColctor<BigInteger, R>
        => colctor.CPrimitive(new IdentityMapper<BigInteger>(), new Type<BigInteger>(), formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<Complex>? t = null) where C : ISeraColctor<Complex, R>
        => colctor.CPrimitive(new IdentityMapper<Complex>(), new Type<Complex>(), formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<TimeSpan>? t = null) where C : ISeraColctor<TimeSpan, R>
        => colctor.CPrimitive(new IdentityMapper<TimeSpan>(), new Type<TimeSpan>(), formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<DateOnly>? t = null) where C : ISeraColctor<DateOnly, R>
        => colctor.CPrimitive(new IdentityMapper<DateOnly>(), new Type<DateOnly>(), formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<TimeOnly>? t = null) where C : ISeraColctor<TimeOnly, R>
        => colctor.CPrimitive(new IdentityMapper<TimeOnly>(), new Type<TimeOnly>(), formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<DateTime>? t = null) where C : ISeraColctor<DateTime, R>
        => colctor.CPrimitive(new IdentityMapper<DateTime>(), new Type<DateTime>(), formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<DateTimeOffset>? t = null) where C : ISeraColctor<DateTimeOffset, R>
        => colctor.CPrimitive(new IdentityMapper<DateTimeOffset>(), new Type<DateTimeOffset>(), formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<Guid>? t = null) where C : ISeraColctor<Guid, R>
        => colctor.CPrimitive(new IdentityMapper<Guid>(), new Type<Guid>(), formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<Range>? t = null) where C : ISeraColctor<Range, R>
        => colctor.CPrimitive(new IdentityMapper<Range>(), new Type<Range>(), formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<Index>? t = null) where C : ISeraColctor<Index, R>
        => colctor.CPrimitive(new IdentityMapper<Index>(), new Type<Index>(), formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<char>? t = null) where C : ISeraColctor<char, R>
        => colctor.CPrimitive(new IdentityMapper<char>(), new Type<char>(), formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<Rune>? t = null) where C : ISeraColctor<Rune, R>
        => colctor.CPrimitive(new IdentityMapper<Rune>(), new Type<Rune>(), formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<Uri>? t = null) where C : ISeraColctor<Uri, R>
        => colctor.CPrimitive(new IdentityMapper<Uri>(), new Type<Uri>(), formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<Version>? t = null) where C : ISeraColctor<Version, R>
        => colctor.CPrimitive(new IdentityMapper<Version>(), new Type<Version>(), formats);
}

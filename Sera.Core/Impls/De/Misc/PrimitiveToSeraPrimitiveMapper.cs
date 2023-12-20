using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Sera.Utils;

namespace Sera.Core.Impls.De;

public readonly struct PrimitiveToSeraPrimitiveMapper :
    ISeraMapper<bool, SeraPrimitive>,
    ISeraMapper<sbyte, SeraPrimitive>, ISeraMapper<byte, SeraPrimitive>,
    ISeraMapper<short, SeraPrimitive>, ISeraMapper<ushort, SeraPrimitive>,
    ISeraMapper<int, SeraPrimitive>, ISeraMapper<uint, SeraPrimitive>,
    ISeraMapper<long, SeraPrimitive>, ISeraMapper<ulong, SeraPrimitive>,
    ISeraMapper<Int128, SeraPrimitive>, ISeraMapper<UInt128, SeraPrimitive>,
    ISeraMapper<nint, SeraPrimitive>, ISeraMapper<nuint, SeraPrimitive>,
    ISeraMapper<Half, SeraPrimitive>, ISeraMapper<float, SeraPrimitive>,
    ISeraMapper<double, SeraPrimitive>, ISeraMapper<decimal, SeraPrimitive>,
    ISeraMapper<NFloat, SeraPrimitive>,
    ISeraMapper<BigInteger, SeraPrimitive>, ISeraMapper<Complex, SeraPrimitive>,
    ISeraMapper<TimeSpan, SeraPrimitive>, ISeraMapper<DateOnly, SeraPrimitive>, ISeraMapper<TimeOnly, SeraPrimitive>,
    ISeraMapper<DateTime, SeraPrimitive>, ISeraMapper<DateTimeOffset, SeraPrimitive>,
    ISeraMapper<Guid, SeraPrimitive>,
    ISeraMapper<Range, SeraPrimitive>, ISeraMapper<Index, SeraPrimitive>,
    ISeraMapper<char, SeraPrimitive>, ISeraMapper<Rune, SeraPrimitive>,
    ISeraMapper<Uri, SeraPrimitive>, ISeraMapper<Version, SeraPrimitive>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SeraPrimitive Map(bool value, InType<SeraPrimitive>? u = null)
        => SeraPrimitive.MakeBoolean(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SeraPrimitive Map(sbyte value, InType<SeraPrimitive>? u = null)
        => SeraPrimitive.MakeSByte(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SeraPrimitive Map(byte value, InType<SeraPrimitive>? u = null)
        => SeraPrimitive.MakeByte(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SeraPrimitive Map(short value, InType<SeraPrimitive>? u = null)
        => SeraPrimitive.MakeInt16(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SeraPrimitive Map(ushort value, InType<SeraPrimitive>? u = null)
        => SeraPrimitive.MakeUInt16(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SeraPrimitive Map(int value, InType<SeraPrimitive>? u = null)
        => SeraPrimitive.MakeInt32(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SeraPrimitive Map(uint value, InType<SeraPrimitive>? u = null)
        => SeraPrimitive.MakeUInt32(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SeraPrimitive Map(long value, InType<SeraPrimitive>? u = null)
        => SeraPrimitive.MakeInt64(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SeraPrimitive Map(ulong value, InType<SeraPrimitive>? u = null)
        => SeraPrimitive.MakeUInt64(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SeraPrimitive Map(Int128 value, InType<SeraPrimitive>? u = null)
        => SeraPrimitive.MakeInt128(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SeraPrimitive Map(UInt128 value, InType<SeraPrimitive>? u = null)
        => SeraPrimitive.MakeUInt128(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SeraPrimitive Map(IntPtr value, InType<SeraPrimitive>? u = null)
        => SeraPrimitive.MakeIntPtr(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SeraPrimitive Map(UIntPtr value, InType<SeraPrimitive>? u = null)
        => SeraPrimitive.MakeUIntPtr(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SeraPrimitive Map(Half value, InType<SeraPrimitive>? u = null)
        => SeraPrimitive.MakeHalf(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SeraPrimitive Map(float value, InType<SeraPrimitive>? u = null)
        => SeraPrimitive.MakeSingle(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SeraPrimitive Map(double value, InType<SeraPrimitive>? u = null)
        => SeraPrimitive.MakeDouble(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SeraPrimitive Map(decimal value, InType<SeraPrimitive>? u = null)
        => SeraPrimitive.MakeDecimal(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SeraPrimitive Map(NFloat value, InType<SeraPrimitive>? u = null)
        => SeraPrimitive.MakeNFloat(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SeraPrimitive Map(BigInteger value, InType<SeraPrimitive>? u = null)
        => SeraPrimitive.MakeBigInteger(new(value));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SeraPrimitive Map(Complex value, InType<SeraPrimitive>? u = null)
        => SeraPrimitive.MakeComplex(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SeraPrimitive Map(TimeSpan value, InType<SeraPrimitive>? u = null)
        => SeraPrimitive.MakeTimeSpan(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SeraPrimitive Map(DateOnly value, InType<SeraPrimitive>? u = null)
        => SeraPrimitive.MakeDateOnly(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SeraPrimitive Map(TimeOnly value, InType<SeraPrimitive>? u = null)
        => SeraPrimitive.MakeTimeOnly(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SeraPrimitive Map(DateTime value, InType<SeraPrimitive>? u = null)
        => SeraPrimitive.MakeDateTime(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SeraPrimitive Map(DateTimeOffset value, InType<SeraPrimitive>? u = null)
        => SeraPrimitive.MakeDateTimeOffset(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SeraPrimitive Map(Guid value, InType<SeraPrimitive>? u = null)
        => SeraPrimitive.MakeGuid(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SeraPrimitive Map(Range value, InType<SeraPrimitive>? u = null)
        => SeraPrimitive.MakeRange(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SeraPrimitive Map(Index value, InType<SeraPrimitive>? u = null)
        => SeraPrimitive.MakeIndex(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SeraPrimitive Map(char value, InType<SeraPrimitive>? u = null)
        => SeraPrimitive.MakeChar(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SeraPrimitive Map(Rune value, InType<SeraPrimitive>? u = null)
        => SeraPrimitive.MakeRune(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SeraPrimitive Map(Uri value, InType<SeraPrimitive>? u = null)
        => SeraPrimitive.MakeUri(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SeraPrimitive Map(Version value, InType<SeraPrimitive>? u = null)
        => SeraPrimitive.MakeVersion(value);
}

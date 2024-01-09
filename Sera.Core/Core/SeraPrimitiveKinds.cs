using System;
using System.Runtime.CompilerServices;

namespace Sera.Core;

[Flags]
public enum SeraPrimitiveKinds : ulong
{
    None = 0,
    Boolean = 1UL << 0,
    SByte = 1UL << 1,
    Byte = 1UL << 2,
    Int16 = 1UL << 3,
    UInt16 = 1UL << 4,
    Int32 = 1UL << 5,
    UInt32 = 1UL << 6,
    Int64 = 1UL << 7,
    UInt64 = 1UL << 8,
    Int128 = 1UL << 9,
    UInt128 = 1UL << 10,
    IntPtr = 1UL << 11,
    UIntPtr = 1UL << 12,
    Half = 1UL << 13,
    Single = 1UL << 14,
    Double = 1UL << 15,
    Decimal = 1UL << 16,
    NFloat = 1UL << 17,
    BigInteger = 1UL << 18,
    Complex = 1UL << 19,
    TimeSpan = 1UL << 20,
    DateOnly = 1UL << 21,
    TimeOnly = 1UL << 22,
    DateTime = 1UL << 23,
    DateTimeOffset = 1UL << 24,
    Guid = 1UL << 25,
    Range = 1UL << 26,
    Index = 1UL << 27,
    Char = 1UL << 29,
    Rune = 1UL << 30,
    Uri = 1UL << 31,
    Version = 1UL << 32,
}

public static class SeraPrimitiveKindsEx
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SeraPrimitiveKinds ToKinds(this SeraPrimitiveTypes self) => self switch
    {
        SeraPrimitiveTypes.Boolean => SeraPrimitiveKinds.Boolean,
        SeraPrimitiveTypes.SByte => SeraPrimitiveKinds.SByte,
        SeraPrimitiveTypes.Byte => SeraPrimitiveKinds.Byte,
        SeraPrimitiveTypes.Int16 => SeraPrimitiveKinds.Int16,
        SeraPrimitiveTypes.UInt16 => SeraPrimitiveKinds.UInt16,
        SeraPrimitiveTypes.Int32 => SeraPrimitiveKinds.Int32,
        SeraPrimitiveTypes.UInt32 => SeraPrimitiveKinds.UInt32,
        SeraPrimitiveTypes.Int64 => SeraPrimitiveKinds.Int64,
        SeraPrimitiveTypes.UInt64 => SeraPrimitiveKinds.UInt64,
        SeraPrimitiveTypes.Int128 => SeraPrimitiveKinds.Int128,
        SeraPrimitiveTypes.UInt128 => SeraPrimitiveKinds.UInt128,
        SeraPrimitiveTypes.IntPtr => SeraPrimitiveKinds.IntPtr,
        SeraPrimitiveTypes.UIntPtr => SeraPrimitiveKinds.UIntPtr,
        SeraPrimitiveTypes.Half => SeraPrimitiveKinds.Half,
        SeraPrimitiveTypes.Single => SeraPrimitiveKinds.Single,
        SeraPrimitiveTypes.Double => SeraPrimitiveKinds.Double,
        SeraPrimitiveTypes.Decimal => SeraPrimitiveKinds.Decimal,
        SeraPrimitiveTypes.NFloat => SeraPrimitiveKinds.NFloat,
        SeraPrimitiveTypes.BigInteger => SeraPrimitiveKinds.BigInteger,
        SeraPrimitiveTypes.Complex => SeraPrimitiveKinds.Complex,
        SeraPrimitiveTypes.TimeSpan => SeraPrimitiveKinds.TimeSpan,
        SeraPrimitiveTypes.DateOnly => SeraPrimitiveKinds.DateOnly,
        SeraPrimitiveTypes.TimeOnly => SeraPrimitiveKinds.TimeOnly,
        SeraPrimitiveTypes.DateTime => SeraPrimitiveKinds.DateTime,
        SeraPrimitiveTypes.DateTimeOffset => SeraPrimitiveKinds.DateTimeOffset,
        SeraPrimitiveTypes.Guid => SeraPrimitiveKinds.Guid,
        SeraPrimitiveTypes.Range => SeraPrimitiveKinds.Range,
        SeraPrimitiveTypes.Index => SeraPrimitiveKinds.Index,
        SeraPrimitiveTypes.Char => SeraPrimitiveKinds.Char,
        SeraPrimitiveTypes.Rune => SeraPrimitiveKinds.Rune,
        SeraPrimitiveTypes.Uri => SeraPrimitiveKinds.Uri,
        SeraPrimitiveTypes.Version => SeraPrimitiveKinds.Version,
        _ => throw new ArgumentOutOfRangeException(nameof(self), self, null)
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SeraPrimitiveTypes? TryToTypes(this SeraPrimitiveKinds self) => self switch
    {
        SeraPrimitiveKinds.Boolean => SeraPrimitiveTypes.Boolean,
        SeraPrimitiveKinds.SByte => SeraPrimitiveTypes.SByte,
        SeraPrimitiveKinds.Byte => SeraPrimitiveTypes.Byte,
        SeraPrimitiveKinds.Int16 => SeraPrimitiveTypes.Int16,
        SeraPrimitiveKinds.UInt16 => SeraPrimitiveTypes.UInt16,
        SeraPrimitiveKinds.Int32 => SeraPrimitiveTypes.Int32,
        SeraPrimitiveKinds.UInt32 => SeraPrimitiveTypes.UInt32,
        SeraPrimitiveKinds.Int64 => SeraPrimitiveTypes.Int64,
        SeraPrimitiveKinds.UInt64 => SeraPrimitiveTypes.UInt64,
        SeraPrimitiveKinds.Int128 => SeraPrimitiveTypes.Int128,
        SeraPrimitiveKinds.UInt128 => SeraPrimitiveTypes.UInt128,
        SeraPrimitiveKinds.IntPtr => SeraPrimitiveTypes.IntPtr,
        SeraPrimitiveKinds.UIntPtr => SeraPrimitiveTypes.UIntPtr,
        SeraPrimitiveKinds.Half => SeraPrimitiveTypes.Half,
        SeraPrimitiveKinds.Single => SeraPrimitiveTypes.Single,
        SeraPrimitiveKinds.Double => SeraPrimitiveTypes.Double,
        SeraPrimitiveKinds.Decimal => SeraPrimitiveTypes.Decimal,
        SeraPrimitiveKinds.NFloat => SeraPrimitiveTypes.NFloat,
        SeraPrimitiveKinds.BigInteger => SeraPrimitiveTypes.BigInteger,
        SeraPrimitiveKinds.Complex => SeraPrimitiveTypes.Complex,
        SeraPrimitiveKinds.TimeSpan => SeraPrimitiveTypes.TimeSpan,
        SeraPrimitiveKinds.DateOnly => SeraPrimitiveTypes.DateOnly,
        SeraPrimitiveKinds.TimeOnly => SeraPrimitiveTypes.TimeOnly,
        SeraPrimitiveKinds.DateTime => SeraPrimitiveTypes.DateTime,
        SeraPrimitiveKinds.DateTimeOffset => SeraPrimitiveTypes.DateTimeOffset,
        SeraPrimitiveKinds.Guid => SeraPrimitiveTypes.Guid,
        SeraPrimitiveKinds.Range => SeraPrimitiveTypes.Range,
        SeraPrimitiveKinds.Index => SeraPrimitiveTypes.Index,
        SeraPrimitiveKinds.Char => SeraPrimitiveTypes.Char,
        SeraPrimitiveKinds.Rune => SeraPrimitiveTypes.Rune,
        SeraPrimitiveKinds.Uri => SeraPrimitiveTypes.Uri,
        SeraPrimitiveKinds.Version => SeraPrimitiveTypes.Version,
        _ => null,
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Has(this SeraPrimitiveKinds self, SeraPrimitiveKinds target) => (self & target) != 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Set(this ref SeraPrimitiveKinds self, SeraPrimitiveKinds target) => self |= target;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void UnSet(this ref SeraPrimitiveKinds self, SeraPrimitiveKinds target) => self &= ~target;
}

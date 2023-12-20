using System;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using BetterCollections.Memories;
using Sera.TaggedUnion;

namespace Sera.Core;

[Union(ExternalTags = true, ExternalTagsName = "SeraPrimitiveTypes")]
public readonly partial struct SeraPrimitive
{
    [UnionTemplate]
    private interface Template
    {
        bool Boolean();
        sbyte SByte();
        byte Byte();
        short Int16();
        ushort UInt16();
        int Int32();
        uint UInt32();
        long Int64();
        ulong UInt64();
        Int128 Int128();
        UInt128 UInt128();
        nint IntPtr();
        nuint UIntPtr();
        Half Half();
        float Single();
        double Double();
        decimal Decimal();
        NFloat NFloat();
        Box<BigInteger> BigInteger();
        Complex Complex();
        TimeSpan TimeSpan();
        DateOnly DateOnly();
        TimeOnly TimeOnly();
        DateTime DateTime();
        DateTimeOffset DateTimeOffset();
        Guid Guid();
        Range Range();
        Index Index();
        char Char();
        Rune Rune();
        Uri Uri();
        Version Version();
    }

    public object ToObject() => Tag switch
    {
        SeraPrimitiveTypes.Boolean => Boolean,
        SeraPrimitiveTypes.SByte => SByte,
        SeraPrimitiveTypes.Byte => Byte,
        SeraPrimitiveTypes.Int16 => Int16,
        SeraPrimitiveTypes.UInt16 => UInt16,
        SeraPrimitiveTypes.Int32 => Int32,
        SeraPrimitiveTypes.UInt32 => UInt32,
        SeraPrimitiveTypes.Int64 => Int64,
        SeraPrimitiveTypes.UInt64 => UInt64,
        SeraPrimitiveTypes.Int128 => Int128,
        SeraPrimitiveTypes.UInt128 => UInt128,
        SeraPrimitiveTypes.IntPtr => IntPtr,
        SeraPrimitiveTypes.UIntPtr => UIntPtr,
        SeraPrimitiveTypes.Half => Half,
        SeraPrimitiveTypes.Single => Single,
        SeraPrimitiveTypes.Double => Double,
        SeraPrimitiveTypes.Decimal => Decimal,
        SeraPrimitiveTypes.NFloat => NFloat,
        SeraPrimitiveTypes.BigInteger => BigInteger,
        SeraPrimitiveTypes.Complex => Complex,
        SeraPrimitiveTypes.TimeSpan => TimeSpan,
        SeraPrimitiveTypes.DateOnly => DateOnly,
        SeraPrimitiveTypes.TimeOnly => TimeOnly,
        SeraPrimitiveTypes.DateTime => DateTime,
        SeraPrimitiveTypes.DateTimeOffset => DateTimeOffset,
        SeraPrimitiveTypes.Guid => Guid,
        SeraPrimitiveTypes.Range => Range,
        SeraPrimitiveTypes.Index => Index,
        SeraPrimitiveTypes.Char => Char,
        SeraPrimitiveTypes.Rune => Rune,
        SeraPrimitiveTypes.Uri => Uri,
        SeraPrimitiveTypes.Version => Version,
        _ => throw new ArgumentOutOfRangeException()
    };
}

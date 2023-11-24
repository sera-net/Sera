using System;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using Sera.TaggedUnion;
using Sera.Utils;

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
}

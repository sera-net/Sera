using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using Sera.Core.De;
using Sera.Core.Impls;

namespace Sera.Core;

public enum SeraAnyKind : byte
{
    Unknown,
    Primitive,
    String,
    Bytes,
    Unit,
    Option,
    Seq,
    Map,
    Struct,
    Variant,
    EmptyUnion,
}

public readonly struct SeraAny : IEquatable<SeraAny>,
    ISerializable<SeraAny, AnyImpl>, IDeserializable<SeraAny, AnyImpl>,
    IAsyncSerializable<SeraAny, AnyImpl>, IAsyncDeserializable<SeraAny, AnyImpl>
{
    public static AnyImpl GetSerialize() => AnyImpl.Instance;
    public static AnyImpl GetDeserialize() => AnyImpl.Instance;
    public static AnyImpl GetAsyncSerialize() => AnyImpl.Instance;
    public static AnyImpl GetAsyncDeserialize() => AnyImpl.Instance;

    private readonly _union_ _union;
    private readonly object? _ref_object;
    public SeraPrimitiveTypes PrimitiveType { get; }
    public SeraAnyKind Kind { get; }

    [StructLayout(LayoutKind.Explicit)]
    private struct _union_
    {
        [FieldOffset(0)]
        public bool Boolean;
        [FieldOffset(0)]
        public sbyte SByte;
        [FieldOffset(0)]
        public short Int16;
        [FieldOffset(0)]
        public int Int32;
        [FieldOffset(0)]
        public long Int64;
        [FieldOffset(0)]
        public Int128 Int128;
        [FieldOffset(0)]
        public byte Byte;
        [FieldOffset(0)]
        public ushort UInt16;
        [FieldOffset(0)]
        public uint UInt32;
        [FieldOffset(0)]
        public ulong UInt64;
        [FieldOffset(0)]
        public UInt128 UInt128;
        [FieldOffset(0)]
        public nint IntPtr;
        [FieldOffset(0)]
        public nuint UIntPtr;
        [FieldOffset(0)]
        public Half Half;
        [FieldOffset(0)]
        public float Single;
        [FieldOffset(0)]
        public double Double;
        [FieldOffset(0)]
        public decimal Decimal;
        [FieldOffset(0)]
        public Complex Complex;
        [FieldOffset(0)]
        public TimeSpan TimeSpan;
        [FieldOffset(0)]
        public DateOnly DateOnly;
        [FieldOffset(0)]
        public TimeOnly TimeOnly;
        [FieldOffset(0)]
        public DateTime DateTime;
        [FieldOffset(0)]
        public DateTimeOffset DateTimeOffset;
        [FieldOffset(0)]
        public Guid Guid;
        [FieldOffset(0)]
        public Range Range;
        [FieldOffset(0)]
        public Index Index;
        [FieldOffset(0)]
        public Char Char;
        [FieldOffset(0)]
        public Rune Rune;
    }

    #region Get

    public bool PrimitiveBoolean => _union.Boolean;
    public sbyte PrimitiveSByte => _union.SByte;
    public short PrimitiveInt16 => _union.Int16;
    public int PrimitiveInt32 => _union.Int32;
    public long PrimitiveInt64 => _union.Int64;
    public Int128 PrimitiveInt128 => _union.Int128;
    public byte PrimitiveByte => _union.Byte;
    public ushort PrimitiveUInt16 => _union.UInt16;
    public uint PrimitiveUInt32 => _union.UInt32;
    public ulong PrimitiveUInt64 => _union.UInt64;
    public UInt128 PrimitiveUInt128 => _union.UInt128;
    public nint PrimitiveIntPtr => _union.IntPtr;
    public nuint PrimitiveUIntPtr => _union.UIntPtr;
    public Half PrimitiveHalf => _union.Half;
    public float PrimitiveSingle => _union.Single;
    public double PrimitiveDouble => _union.Double;
    public decimal PrimitiveDecimal => _union.Decimal;
    public BigInteger PrimitiveBigInteger => ((SeraBoxed<BigInteger>)_ref_object!).Value;
    public Complex PrimitiveComplex => _union.Complex;
    public TimeSpan PrimitiveTimeSpan => _union.TimeSpan;
    public DateOnly PrimitiveDateOnly => _union.DateOnly;
    public TimeOnly PrimitiveTimeOnly => _union.TimeOnly;
    public DateTime PrimitiveDateTime => _union.DateTime;
    public DateTimeOffset PrimitiveDateTimeOffset => _union.DateTimeOffset;
    public Guid PrimitiveGuid => _union.Guid;
    public Range PrimitiveRange => _union.Range;
    public Index PrimitiveIndex => _union.Index;
    public char PrimitiveChar => _union.Char;
    public Rune PrimitiveRune => _union.Rune;

    public string String => (string)_ref_object!;
    public byte[] Bytes => (byte[])_ref_object!;
    public SeraBoxed<SeraAny>? Option => (SeraBoxed<SeraAny>?)_ref_object!;
    public List<SeraAny> Seq => (List<SeraAny>)_ref_object!;
    public Dictionary<SeraAny, SeraAny> Map => (Dictionary<SeraAny, SeraAny>)_ref_object!;
    public SeraAnyStruct Struct => (SeraAnyStruct)_ref_object!;
    public SeraAnyVariant Variant => (SeraAnyVariant)_ref_object!;

    #endregion

    private SeraAny(_union_ union, object? refObject, SeraPrimitiveTypes primitiveType, SeraAnyKind kind)
    {
        _ref_object = refObject;
        PrimitiveType = primitiveType;
        Kind = kind;
        _union = union;
    }

    #region MakePrimitive

    public static SeraAny MakeUnknown()
        => new(default, null, SeraPrimitiveTypes.Unknown, SeraAnyKind.Unknown);

    public static SeraAny MakePrimitive()
        => new(default, null, SeraPrimitiveTypes.Unknown, SeraAnyKind.Primitive);

    public static SeraAny MakePrimitive(bool v)
        => new(new() { Boolean = v }, null, SeraPrimitiveTypes.Boolean, SeraAnyKind.Primitive);

    public static SeraAny MakePrimitive(sbyte v)
        => new(new() { SByte = v }, null, SeraPrimitiveTypes.SByte, SeraAnyKind.Primitive);

    public static SeraAny MakePrimitive(short v)
        => new(new() { Int16 = v }, null, SeraPrimitiveTypes.Int16, SeraAnyKind.Primitive);

    public static SeraAny MakePrimitive(int v)
        => new(new() { Int32 = v }, null, SeraPrimitiveTypes.Int32, SeraAnyKind.Primitive);

    public static SeraAny MakePrimitive(long v)
        => new(new() { Int64 = v }, null, SeraPrimitiveTypes.Int64, SeraAnyKind.Primitive);

    public static SeraAny MakePrimitive(Int128 v)
        => new(new() { Int128 = v }, null, SeraPrimitiveTypes.Int128, SeraAnyKind.Primitive);

    public static SeraAny MakePrimitive(byte v)
        => new(new() { Byte = v }, null, SeraPrimitiveTypes.Byte, SeraAnyKind.Primitive);

    public static SeraAny MakePrimitive(ushort v)
        => new(new() { UInt16 = v }, null, SeraPrimitiveTypes.UInt16, SeraAnyKind.Primitive);

    public static SeraAny MakePrimitive(uint v)
        => new(new() { UInt32 = v }, null, SeraPrimitiveTypes.UInt32, SeraAnyKind.Primitive);

    public static SeraAny MakePrimitive(ulong v)
        => new(new() { UInt64 = v }, null, SeraPrimitiveTypes.UInt64, SeraAnyKind.Primitive);

    public static SeraAny MakePrimitive(UInt128 v)
        => new(new() { UInt128 = v }, null, SeraPrimitiveTypes.UInt128, SeraAnyKind.Primitive);

    public static SeraAny MakePrimitive(nint v)
        => new(new() { IntPtr = v }, null, SeraPrimitiveTypes.IntPtr, SeraAnyKind.Primitive);

    public static SeraAny MakePrimitive(nuint v)
        => new(new() { UIntPtr = v }, null, SeraPrimitiveTypes.UIntPtr, SeraAnyKind.Primitive);

    public static SeraAny MakePrimitive(Half v)
        => new(new() { Half = v }, null, SeraPrimitiveTypes.Half, SeraAnyKind.Primitive);

    public static SeraAny MakePrimitive(float v)
        => new(new() { Single = v }, null, SeraPrimitiveTypes.Single, SeraAnyKind.Primitive);

    public static SeraAny MakePrimitive(double v)
        => new(new() { Double = v }, null, SeraPrimitiveTypes.Double, SeraAnyKind.Primitive);

    public static SeraAny MakePrimitive(decimal v)
        => new(new() { Decimal = v }, null, SeraPrimitiveTypes.Decimal, SeraAnyKind.Primitive);

    public static SeraAny MakePrimitive(SeraBoxed<BigInteger> v)
        => new(default, v, SeraPrimitiveTypes.BigInteger, SeraAnyKind.Primitive);

    public static SeraAny MakePrimitive(Complex v)
        => new(new() { Complex = v }, null, SeraPrimitiveTypes.Complex, SeraAnyKind.Primitive);

    public static SeraAny MakePrimitive(TimeSpan v)
        => new(new() { TimeSpan = v }, null, SeraPrimitiveTypes.TimeSpan, SeraAnyKind.Primitive);

    public static SeraAny MakePrimitive(DateOnly v)
        => new(new() { DateOnly = v }, null, SeraPrimitiveTypes.DateOnly, SeraAnyKind.Primitive);

    public static SeraAny MakePrimitive(TimeOnly v)
        => new(new() { TimeOnly = v }, null, SeraPrimitiveTypes.TimeOnly, SeraAnyKind.Primitive);

    public static SeraAny MakePrimitive(DateTime v)
        => new(new() { DateTime = v }, null, SeraPrimitiveTypes.DateTime, SeraAnyKind.Primitive);

    public static SeraAny MakePrimitive(DateTimeOffset v)
        => new(new() { DateTimeOffset = v }, null, SeraPrimitiveTypes.DateTimeOffset, SeraAnyKind.Primitive);

    public static SeraAny MakePrimitive(Guid v)
        => new(new() { Guid = v }, null, SeraPrimitiveTypes.Guid, SeraAnyKind.Primitive);

    public static SeraAny MakePrimitive(Range v)
        => new(new() { Range = v }, null, SeraPrimitiveTypes.Range, SeraAnyKind.Primitive);

    public static SeraAny MakePrimitive(Index v)
        => new(new() { Index = v }, null, SeraPrimitiveTypes.Index, SeraAnyKind.Primitive);

    public static SeraAny MakePrimitive(char v)
        => new(new() { Char = v }, null, SeraPrimitiveTypes.Char, SeraAnyKind.Primitive);

    public static SeraAny MakePrimitive(Rune v)
        => new(new() { Rune = v }, null, SeraPrimitiveTypes.Rune, SeraAnyKind.Primitive);

    #endregion

    #region MakeOther

    public static SeraAny MakeString(string v)
        => new(default, v, SeraPrimitiveTypes.Unknown, SeraAnyKind.String);

    public static SeraAny MakeBytes(byte[] v)
        => new(default, v, SeraPrimitiveTypes.Unknown, SeraAnyKind.Bytes);

    public static SeraAny MakeUnit()
        => new(default, null, SeraPrimitiveTypes.Unknown, SeraAnyKind.Unit);

    public static SeraAny MakeOption(SeraBoxed<SeraAny>? v)
        => new(default, v, SeraPrimitiveTypes.Unknown, SeraAnyKind.Option);

    public static SeraAny MakeSeq(List<SeraAny> v)
        => new(default, v, SeraPrimitiveTypes.Unknown, SeraAnyKind.Seq);

    public static SeraAny MakeMap(Dictionary<SeraAny, SeraAny> v)
        => new(default, v, SeraPrimitiveTypes.Unknown, SeraAnyKind.Map);

    public static SeraAny MakeStruct(SeraAnyStruct v)
        => new(default, v, SeraPrimitiveTypes.Unknown, SeraAnyKind.Struct);

    public static SeraAny MakeVariant(SeraAnyVariant v)
        => new(default, v, SeraPrimitiveTypes.Unknown, SeraAnyKind.Variant);

    public static SeraAny MakeEmptyUnion()
        => new(default, null, SeraPrimitiveTypes.Unknown, SeraAnyKind.EmptyUnion);
    
    #endregion

    #region Equals

    public bool Equals(SeraAny other)
    {
        return Kind == other.Kind && Kind switch
        {
            SeraAnyKind.Unknown => true,
            SeraAnyKind.Primitive => PrimitiveType == other.PrimitiveType && PrimitiveType switch
            {
                SeraPrimitiveTypes.Unknown => true,
                SeraPrimitiveTypes.Boolean => PrimitiveBoolean == other.PrimitiveBoolean,
                SeraPrimitiveTypes.SByte => PrimitiveSByte == other.PrimitiveSByte,
                SeraPrimitiveTypes.Int16 => PrimitiveInt16 == other.PrimitiveInt16,
                SeraPrimitiveTypes.Int32 => PrimitiveInt32 == other.PrimitiveInt32,
                SeraPrimitiveTypes.Int64 => PrimitiveInt64 == other.PrimitiveInt64,
                SeraPrimitiveTypes.Int128 => PrimitiveInt128 == other.PrimitiveInt128,
                SeraPrimitiveTypes.Byte => PrimitiveByte == other.PrimitiveByte,
                SeraPrimitiveTypes.UInt16 => PrimitiveUInt16 == other.PrimitiveUInt16,
                SeraPrimitiveTypes.UInt32 => PrimitiveUInt32 == other.PrimitiveUInt32,
                SeraPrimitiveTypes.UInt64 => PrimitiveUInt64 == other.PrimitiveUInt64,
                SeraPrimitiveTypes.UInt128 => PrimitiveUInt128 == other.PrimitiveUInt128,
                SeraPrimitiveTypes.IntPtr => PrimitiveIntPtr == other.PrimitiveIntPtr,
                SeraPrimitiveTypes.UIntPtr => PrimitiveUIntPtr == other.PrimitiveUIntPtr,
                SeraPrimitiveTypes.Half => PrimitiveHalf.Equals(other.PrimitiveHalf),
                SeraPrimitiveTypes.Single => PrimitiveSingle.Equals(other.PrimitiveSingle),
                SeraPrimitiveTypes.Double => PrimitiveDouble.Equals(other.PrimitiveSingle),
                SeraPrimitiveTypes.Decimal => PrimitiveDecimal == other.PrimitiveDecimal,
                SeraPrimitiveTypes.BigInteger => PrimitiveBigInteger == other.PrimitiveBigInteger,
                SeraPrimitiveTypes.Complex => PrimitiveComplex.Equals(other.PrimitiveComplex),
                SeraPrimitiveTypes.TimeSpan => PrimitiveTimeSpan == other.PrimitiveTimeSpan,
                SeraPrimitiveTypes.DateOnly => PrimitiveDateOnly == other.PrimitiveDateOnly,
                SeraPrimitiveTypes.TimeOnly => PrimitiveTimeOnly == other.PrimitiveTimeOnly,
                SeraPrimitiveTypes.DateTime => PrimitiveDateTime == other.PrimitiveDateTime,
                SeraPrimitiveTypes.DateTimeOffset => PrimitiveDateTimeOffset == other.PrimitiveDateTimeOffset,
                SeraPrimitiveTypes.Guid => PrimitiveGuid == other.PrimitiveGuid,
                SeraPrimitiveTypes.Range => PrimitiveRange.Equals(other.PrimitiveRange),
                SeraPrimitiveTypes.Index => PrimitiveIndex.Equals(other.PrimitiveIndex),
                SeraPrimitiveTypes.Char => PrimitiveChar == other.PrimitiveChar,
                SeraPrimitiveTypes.Rune => PrimitiveRune == other.PrimitiveRune,
                _ => false,
            },
            SeraAnyKind.String => String == other.String,
            SeraAnyKind.Bytes => Bytes.SequenceEqual(other.Bytes),
            SeraAnyKind.Unit => true,
            SeraAnyKind.Option => Option == other.Option,
            SeraAnyKind.Seq => Seq.SequenceEqual(other.Seq),
            SeraAnyKind.Map => other.Map.DictEq(other.Map),
            SeraAnyKind.Struct => Struct == other.Struct,
            SeraAnyKind.Variant => Variant == other.Variant,
            SeraAnyKind.EmptyUnion => true,
            _ => false,
        };
    }

    public override bool Equals(object? obj) => obj is SeraAny other && Equals(other);

    public override int GetHashCode() => Kind switch
    {
        SeraAnyKind.Unknown => HashCode.Combine(Kind),
        SeraAnyKind.Primitive => PrimitiveType switch
        {
            SeraPrimitiveTypes.Unknown => HashCode.Combine(Kind, PrimitiveType),
            SeraPrimitiveTypes.Boolean => HashCode.Combine(Kind, PrimitiveType, PrimitiveBoolean),
            SeraPrimitiveTypes.SByte => HashCode.Combine(Kind, PrimitiveType, PrimitiveSByte),
            SeraPrimitiveTypes.Int16 => HashCode.Combine(Kind, PrimitiveType, PrimitiveInt16),
            SeraPrimitiveTypes.Int32 => HashCode.Combine(Kind, PrimitiveType, PrimitiveInt32),
            SeraPrimitiveTypes.Int64 => HashCode.Combine(Kind, PrimitiveType, PrimitiveInt64),
            SeraPrimitiveTypes.Int128 => HashCode.Combine(Kind, PrimitiveType, PrimitiveInt128),
            SeraPrimitiveTypes.Byte => HashCode.Combine(Kind, PrimitiveType, PrimitiveByte),
            SeraPrimitiveTypes.UInt16 => HashCode.Combine(Kind, PrimitiveType, PrimitiveUInt16),
            SeraPrimitiveTypes.UInt32 => HashCode.Combine(Kind, PrimitiveType, PrimitiveUInt32),
            SeraPrimitiveTypes.UInt64 => HashCode.Combine(Kind, PrimitiveType, PrimitiveUInt64),
            SeraPrimitiveTypes.UInt128 => HashCode.Combine(Kind, PrimitiveType, PrimitiveUInt128),
            SeraPrimitiveTypes.IntPtr => HashCode.Combine(Kind, PrimitiveType, PrimitiveIntPtr),
            SeraPrimitiveTypes.UIntPtr => HashCode.Combine(Kind, PrimitiveType, PrimitiveUIntPtr),
            SeraPrimitiveTypes.Half => HashCode.Combine(Kind, PrimitiveType, PrimitiveHalf),
            SeraPrimitiveTypes.Single => HashCode.Combine(Kind, PrimitiveType, PrimitiveSingle),
            SeraPrimitiveTypes.Double => HashCode.Combine(Kind, PrimitiveType, PrimitiveDouble),
            SeraPrimitiveTypes.Decimal => HashCode.Combine(Kind, PrimitiveType, PrimitiveDecimal),
            SeraPrimitiveTypes.BigInteger => HashCode.Combine(Kind, PrimitiveType, PrimitiveBigInteger),
            SeraPrimitiveTypes.Complex => HashCode.Combine(Kind, PrimitiveType, PrimitiveComplex),
            SeraPrimitiveTypes.TimeSpan => HashCode.Combine(Kind, PrimitiveType, PrimitiveTimeSpan),
            SeraPrimitiveTypes.DateOnly => HashCode.Combine(Kind, PrimitiveType, PrimitiveDateOnly),
            SeraPrimitiveTypes.TimeOnly => HashCode.Combine(Kind, PrimitiveType, PrimitiveTimeOnly),
            SeraPrimitiveTypes.DateTime => HashCode.Combine(Kind, PrimitiveType, PrimitiveDateTime),
            SeraPrimitiveTypes.DateTimeOffset => HashCode.Combine(Kind, PrimitiveType, PrimitiveDateTimeOffset),
            SeraPrimitiveTypes.Guid => HashCode.Combine(Kind, PrimitiveType, PrimitiveGuid),
            SeraPrimitiveTypes.Range => HashCode.Combine(Kind, PrimitiveType, PrimitiveRange),
            SeraPrimitiveTypes.Index => HashCode.Combine(Kind, PrimitiveType, PrimitiveIndex),
            SeraPrimitiveTypes.Char => HashCode.Combine(Kind, PrimitiveType, PrimitiveChar),
            SeraPrimitiveTypes.Rune => HashCode.Combine(Kind, PrimitiveType, PrimitiveRune),
            _ => 0
        },
        SeraAnyKind.String => HashCode.Combine(Kind, String),
        SeraAnyKind.Bytes => HashCode.Combine(Kind, Bytes),
        SeraAnyKind.Unit => HashCode.Combine(Kind),
        SeraAnyKind.Option => HashCode.Combine(Kind, Option),
        SeraAnyKind.Seq => HashCode.Combine(Kind, Seq.SeqHash()),
        SeraAnyKind.Map => HashCode.Combine(Kind, Map.DictHash()),
        SeraAnyKind.Struct => HashCode.Combine(Kind, Struct),
        SeraAnyKind.Variant => HashCode.Combine(Kind, Variant),
        SeraAnyKind.EmptyUnion => HashCode.Combine(Kind),
        _ => 0
    };

    public static bool operator ==(SeraAny left, SeraAny right) => left.Equals(right);

    public static bool operator !=(SeraAny left, SeraAny right) => !left.Equals(right);

    #endregion

    #region ToString

    public override string ToString() => Kind switch
    {
        SeraAnyKind.Unknown => $"{nameof(SeraAny)}.Unknown",
        SeraAnyKind.Primitive => PrimitiveType switch
        {
            SeraPrimitiveTypes.Unknown => $"{nameof(SeraAny)}.Primitive.Unknown",
            SeraPrimitiveTypes.Boolean => $"{PrimitiveBoolean}",
            SeraPrimitiveTypes.SByte => $"{PrimitiveSByte}",
            SeraPrimitiveTypes.Int16 => $"{PrimitiveInt16}",
            SeraPrimitiveTypes.Int32 => $"{PrimitiveInt32}",
            SeraPrimitiveTypes.Int64 => $"{PrimitiveInt64}",
            SeraPrimitiveTypes.Int128 => $"{PrimitiveInt128}",
            SeraPrimitiveTypes.Byte => $"{PrimitiveByte}",
            SeraPrimitiveTypes.UInt16 => $"{PrimitiveUInt16}",
            SeraPrimitiveTypes.UInt32 => $"{PrimitiveUInt32}",
            SeraPrimitiveTypes.UInt64 => $"{PrimitiveUInt64}",
            SeraPrimitiveTypes.UInt128 => $"{PrimitiveUInt128}",
            SeraPrimitiveTypes.IntPtr => $"{PrimitiveIntPtr}",
            SeraPrimitiveTypes.UIntPtr => $"{PrimitiveUIntPtr}",
            SeraPrimitiveTypes.Half => $"{PrimitiveHalf}",
            SeraPrimitiveTypes.Single => $"{PrimitiveSingle}",
            SeraPrimitiveTypes.Double => $"{PrimitiveDouble}",
            SeraPrimitiveTypes.Decimal => $"{PrimitiveDecimal}",
            SeraPrimitiveTypes.BigInteger => $"{PrimitiveBigInteger}",
            SeraPrimitiveTypes.Complex => $"{PrimitiveComplex}",
            SeraPrimitiveTypes.TimeSpan => $"{PrimitiveTimeSpan}",
            SeraPrimitiveTypes.DateOnly => $"{PrimitiveDateOnly}",
            SeraPrimitiveTypes.TimeOnly => $"{PrimitiveTimeOnly}",
            SeraPrimitiveTypes.DateTime => $"{PrimitiveDateTime}",
            SeraPrimitiveTypes.DateTimeOffset => $"{PrimitiveDateTimeOffset}",
            SeraPrimitiveTypes.Guid => $"{PrimitiveGuid}",
            SeraPrimitiveTypes.Range => $"{PrimitiveRange}",
            SeraPrimitiveTypes.Index => $"{PrimitiveIndex}",
            SeraPrimitiveTypes.Char => $"{PrimitiveChar}",
            SeraPrimitiveTypes.Rune => $"{PrimitiveRune}",
            _ => $"{nameof(SeraAny)}.Primitive({PrimitiveType})",
        },
        SeraAnyKind.String => $"{String}",
        SeraAnyKind.Bytes => $"{Bytes}",
        SeraAnyKind.Unit => "Unit",
        SeraAnyKind.Option => Option == null ? "None" : $"Some({Option.Value})",
        SeraAnyKind.Seq => $"[{string.Join(", ", Seq)}]",
        SeraAnyKind.Map => $"{{ {string.Join(", ", Map.Select(static kv => $"{kv.Key}: {kv.Value}"))} }}",
        SeraAnyKind.Struct => $"{Struct}",
        SeraAnyKind.Variant => $"{Variant}",
        SeraAnyKind.EmptyUnion => "union { }",
        _ => $"{nameof(SeraAny)}({Kind})",
    };

    #endregion
}

public record SeraBoxed<T>(T Value) : IEquatable<T>
{
    public readonly T Value = Value;

    public virtual bool Equals(T? other) => EqualityComparer<T>.Default.Equals(Value, other);

    public override string ToString() => $"{Value}";
}

public record SeraAnyStruct(List<(string key, long? int_key, SeraAny value)> Fields)
{
    public List<(string key, long? int_key, SeraAny value)> Fields { get; set; } = Fields;
    public string? StructName { get; set; }

    public virtual bool Equals(SeraAnyStruct? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return StructName == other.StructName && Fields.SequenceEqual(other.Fields);
    }

    public override int GetHashCode()
    {
        // ReSharper disable once NonReadonlyMemberInGetHashCode
        return HashCode.Combine(StructName, 
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            Fields.SeqHash());
    }

    public override string ToString() =>
        $"struct {StructName ?? "_"} {{ {string.Join(", ", Fields.Select(static kv => $"{kv.key} = {kv.value}"))} }}";
}

public record SeraAnyVariant(Variant Variant, SeraAny? Value)
{
    public SeraAny? Value = Value;
    public string? UnionName { get; set; }

    public override string ToString() =>
        $"enum {UnionName ?? "_"}.{Variant}{(Value.HasValue ? $" = {{Value}}" : "")}";
}

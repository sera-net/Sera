using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Sera.Core;

public enum VariantTagKind : byte
{
    SByte = 1,
    Byte,
    Int16,
    UInt16,
    Int32,
    UInt32,
    Int64,
    UInt64,
}

public readonly struct VariantTag : IEquatable<VariantTag>, IComparable<VariantTag>,
    IComparisonOperators<VariantTag, VariantTag, bool>
{
    internal readonly _union_ _union;
    public VariantTagKind Kind { get; }

    [StructLayout(LayoutKind.Explicit)]
    internal struct _union_
    {
        [FieldOffset(0)]
        public sbyte SByte;
        [FieldOffset(0)]
        public byte Byte;
        [FieldOffset(0)]
        public short Int16;
        [FieldOffset(0)]
        public ushort UInt16;
        [FieldOffset(0)]
        public int Int32;
        [FieldOffset(0)]
        public uint UInt32;
        [FieldOffset(0)]
        public long Int64;
        [FieldOffset(0)]
        public ulong UInt64;
    }

    internal VariantTag(_union_ union, VariantTagKind kind)
    {
        _union = union;
        Kind = kind;
    }

    public sbyte SByte => _union.SByte;
    public byte Byte => _union.Byte;
    public short Int16 => _union.Int16;
    public ushort UInt16 => _union.UInt16;
    public int Int32 => _union.Int32;
    public uint UInt32 => _union.UInt32;
    public long Int64 => _union.Int64;
    public ulong UInt64 => _union.UInt64;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static VariantTag Create(sbyte value) => new(new _union_ { SByte = value }, VariantTagKind.SByte);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static VariantTag Create(byte value) => new(new _union_ { Byte = value }, VariantTagKind.Byte);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static VariantTag Create(short value) => new(new _union_ { Int16 = value }, VariantTagKind.Int16);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static VariantTag Create(ushort value) => new(new _union_ { UInt16 = value }, VariantTagKind.UInt16);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static VariantTag Create(int value) => new(new _union_ { Int32 = value }, VariantTagKind.Int32);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static VariantTag Create(uint value) => new(new _union_ { UInt32 = value }, VariantTagKind.UInt32);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static VariantTag Create(long value) => new(new _union_ { Int64 = value }, VariantTagKind.Int64);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static VariantTag Create(ulong value) => new(new _union_ { UInt64 = value }, VariantTagKind.UInt64);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public T? TryAs<T>() where T : unmanaged => Kind switch
    {
        VariantTagKind.SByte => SByte is T v ? v : null,
        VariantTagKind.Byte => Byte is T v ? v : null,
        VariantTagKind.Int16 => Int16 is T v ? v : null,
        VariantTagKind.UInt16 => UInt16 is T v ? v : null,
        VariantTagKind.Int32 => Int32 is T v ? v : null,
        VariantTagKind.UInt32 => UInt32 is T v ? v : null,
        VariantTagKind.Int64 => Int64 is T v ? v : null,
        VariantTagKind.UInt64 => UInt64 is T v ? v : null,
        _ => throw new ArgumentOutOfRangeException()
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public T As<T>() where T : unmanaged => TryAs<T>() ?? throw new ArgumentException();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static VariantTag Parse(VariantTagKind kind, ReadOnlySpan<char> chars) => kind switch
    {
        VariantTagKind.SByte => Create(sbyte.Parse(chars)),
        VariantTagKind.Byte => Create(byte.Parse(chars)),
        VariantTagKind.Int16 => Create(short.Parse(chars)),
        VariantTagKind.UInt16 => Create(ushort.Parse(chars)),
        VariantTagKind.Int32 => Create(int.Parse(chars)),
        VariantTagKind.UInt32 => Create(uint.Parse(chars)),
        VariantTagKind.Int64 => Create(long.Parse(chars)),
        VariantTagKind.UInt64 => Create(ulong.Parse(chars)),
        _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null)
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ToInt() => Kind switch
    {
        VariantTagKind.SByte => SByte,
        VariantTagKind.Byte => Byte,
        VariantTagKind.Int16 => Int16,
        VariantTagKind.UInt16 => UInt16,
        VariantTagKind.Int32 => Int32,
        VariantTagKind.UInt32 => (int)UInt32,
        VariantTagKind.Int64 => (int)Int64,
        VariantTagKind.UInt64 => (int)UInt64,
        _ => throw new ArgumentOutOfRangeException()
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long ToLong() => Kind switch
    {
        VariantTagKind.SByte => SByte,
        VariantTagKind.Byte => Byte,
        VariantTagKind.Int16 => Int16,
        VariantTagKind.UInt16 => UInt16,
        VariantTagKind.Int32 => Int32,
        VariantTagKind.UInt32 => UInt32,
        VariantTagKind.Int64 => Int64,
        VariantTagKind.UInt64 => (long)UInt64,
        _ => throw new ArgumentOutOfRangeException()
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public object ToObject() => Kind switch
    {
        VariantTagKind.SByte => SByte,
        VariantTagKind.Byte => Byte,
        VariantTagKind.Int16 => Int16,
        VariantTagKind.UInt16 => UInt16,
        VariantTagKind.Int32 => Int32,
        VariantTagKind.UInt32 => UInt32,
        VariantTagKind.Int64 => Int64,
        VariantTagKind.UInt64 => UInt64,
        _ => throw new ArgumentOutOfRangeException()
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => Kind switch
    {
        VariantTagKind.SByte => SByte.ToString(),
        VariantTagKind.Byte => Byte.ToString(),
        VariantTagKind.Int16 => Int16.ToString(),
        VariantTagKind.UInt16 => UInt16.ToString(),
        VariantTagKind.Int32 => Int32.ToString(),
        VariantTagKind.UInt32 => UInt32.ToString(),
        VariantTagKind.Int64 => Int64.ToString(),
        VariantTagKind.UInt64 => UInt64.ToString(),
        _ => throw new ArgumentOutOfRangeException()
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(VariantTag other) => Kind == other.Kind && Kind switch
    {
        VariantTagKind.SByte => SByte == other.SByte,
        VariantTagKind.Byte => Byte == other.Byte,
        VariantTagKind.Int16 => Int16 == other.Int16,
        VariantTagKind.UInt16 => UInt16 == other.UInt16,
        VariantTagKind.Int32 => Int32 == other.Int32,
        VariantTagKind.UInt32 => UInt32 == other.UInt32,
        VariantTagKind.Int64 => Int64 == other.Int64,
        VariantTagKind.UInt64 => UInt64 == other.UInt64,
        _ => throw new ArgumentOutOfRangeException()
    };

    public override bool Equals(object? obj) => obj is VariantTag other && Equals(other);

    public override int GetHashCode() => Kind switch
    {
        VariantTagKind.SByte => HashCode.Combine(Kind, SByte),
        VariantTagKind.Byte => HashCode.Combine(Kind, Byte),
        VariantTagKind.Int16 => HashCode.Combine(Kind, Int16),
        VariantTagKind.UInt16 => HashCode.Combine(Kind, UInt16),
        VariantTagKind.Int32 => HashCode.Combine(Kind, Int32),
        VariantTagKind.UInt32 => HashCode.Combine(Kind, UInt32),
        VariantTagKind.Int64 => HashCode.Combine(Kind, Int64),
        VariantTagKind.UInt64 => HashCode.Combine(Kind, UInt64),
        _ => throw new ArgumentOutOfRangeException()
    };

    public static bool operator ==(VariantTag left, VariantTag right) => left.Equals(right);

    public static bool operator !=(VariantTag left, VariantTag right) => !left.Equals(right);

    public int CompareTo(VariantTag other) => Kind switch
    {
        VariantTagKind.SByte => SByte.CompareTo(other.SByte),
        VariantTagKind.Byte => Byte.CompareTo(other.Byte),
        VariantTagKind.Int16 => Int16.CompareTo(other.Int16),
        VariantTagKind.UInt16 => UInt16.CompareTo(other.UInt16),
        VariantTagKind.Int32 => Int32.CompareTo(other.Int32),
        VariantTagKind.UInt32 => UInt32.CompareTo(other.UInt32),
        VariantTagKind.Int64 => Int64.CompareTo(other.Int64),
        VariantTagKind.UInt64 => UInt64.CompareTo(other.UInt64),
        _ => throw new ArgumentOutOfRangeException()
    };

    public static bool operator <(VariantTag left, VariantTag right) => left.CompareTo(right) < 0;

    public static bool operator >(VariantTag left, VariantTag right) => left.CompareTo(right) > 0;

    public static bool operator <=(VariantTag left, VariantTag right) => left.CompareTo(right) <= 0;

    public static bool operator >=(VariantTag left, VariantTag right) => left.CompareTo(right) >= 0;
}

public readonly struct Variant : IEquatable<Variant>, IEqualityOperators<Variant, Variant, bool>
{
    private readonly VariantTag._union_ _tag;
    public string Name { get; }
    public VariantTagKind TagKind { get; }
    public VariantKind Kind { get; }

    public VariantTag Tag => new(_tag, TagKind);

    public Variant(string name)
    {
        Name = name;
        _tag = default;
        TagKind = default;
        Kind = VariantKind.Name;
    }

    public Variant(VariantTag tag)
    {
        Name = default!;
        _tag = tag._union;
        TagKind = tag.Kind;
        Kind = VariantKind.Tag;
    }

    public Variant(string name, VariantTag tag)
    {
        Name = name;
        _tag = tag._union;
        TagKind = tag.Kind;
        Kind = VariantKind.NameAndTag;
    }

    public override string ToString() => Kind switch
    {
        VariantKind.Name => Name,
        VariantKind.Tag => $"[{Tag}]",
        VariantKind.NameAndTag => $"[{Tag}]{Name}",
        _ => $"{nameof(Variant)}({Kind})",
    };

    public bool Equals(Variant other) => Kind == other.Kind && Kind switch
    {
        VariantKind.Name => Name == other.Name,
        VariantKind.Tag => Tag == other.Tag,
        VariantKind.NameAndTag => Tag == other.Tag && Name == other.Name,
        _ => throw new ArgumentOutOfRangeException()
    };

    public override bool Equals(object? obj) => obj is Variant other && Equals(other);

    public override int GetHashCode() => Kind switch
    {
        VariantKind.Name => HashCode.Combine(Kind, Name),
        VariantKind.Tag => HashCode.Combine(Kind, Tag),
        VariantKind.NameAndTag => HashCode.Combine(Kind, Name, Tag),
        _ => throw new ArgumentOutOfRangeException()
    };

    public static bool operator ==(Variant left, Variant right) => left.Equals(right);

    public static bool operator !=(Variant left, Variant right) => !left.Equals(right);
}

public enum VariantKind : byte
{
    Name,
    Tag,
    NameAndTag,
}

public enum VariantPriority : byte
{
    Any,
    TagFirst,
    NameFirst,
}

/// <summary>
/// Union format
/// </summary>
public enum UnionFormat
{
    Any,
    /// <summary>
    /// Json is <code>{ "Tag": Value }</code>
    /// <para>If the variant has no value it will be directly tag, json is <code>"Tag"</code></para>
    /// </summary>
    External,
    /// <summary>
    /// Json is <code>{ "type": Tag, ...Value }</code>
    /// </summary>
    Internal,
    /// <summary>
    /// Json is <code>{ "t": Tag, "c": Value }</code>
    /// </summary>
    Adjacent,
    /// <summary>
    /// Json is <code>[Tag, Value]</code>
    /// </summary>
    Tuple,
    /// <summary>
    /// Json is <code>Value</code>
    /// <para>If the variant has no value it will be directly tag, json is <code>"Tag"</code></para>
    /// </summary>
    Untagged,
}

/// <summary>
/// 
/// </summary>
/// <param name="VariantPriority">Variant tag priority</param>
/// <param name="VariantFormats">The format used when the variant tag as number</param>
/// <param name="Format">Union format</param>
/// <param name="CompactTag">Whether to treat the variant as a tag when it has no value when the format is <see cref="UnionFormat.Internal"/> | <see cref="UnionFormat.Adjacent"/> | <see cref="UnionFormat.Tuple"/></param>
/// <param name="InternalTagName">The name of the tag when the format is <see cref="UnionFormat.Internal"/></param>
/// <param name="InternalValueName">Field name if the variant value cannot be treated as a structure when the format is <see cref="UnionFormat.Internal"/></param>
/// <param name="AdjacentTagName">The name of the tag when the format is <see cref="UnionFormat.Adjacent"/></param>
/// <param name="AdjacentValueName">The name of the value when the format is <see cref="UnionFormat.Adjacent"/></param>
public record UnionStyle(
    VariantPriority VariantPriority = VariantPriority.Any,
    SeraFormats? VariantFormats = null,
    UnionFormat Format = UnionFormat.Any,
    bool CompactTag = false,
    string InternalTagName = "type",
    string InternalValueName = "value",
    string AdjacentTagName = "t",
    string AdjacentValueName = "c"
)
{
    public static UnionStyle Default { get; } = new();

    public static UnionStyle? FromAttr(SeraUnionAttribute? attr, SeraFormatsAttribute? formats) => attr == null
        ? null
        : new()
        {
            VariantPriority = attr.Priority,
            VariantFormats = SeraFormats.FromAttr(formats),
            Format = attr.Format,
            CompactTag = attr.CompactTag,
            InternalTagName = attr.InternalTagName,
            InternalValueName = attr.InternalValueName,
            AdjacentTagName = attr.AdjacentTagName,
            AdjacentValueName = attr.AdjacentValueName,
        };
}

public record VariantStyle(
    VariantPriority Priority = VariantPriority.Any,
    [property: Obsolete("todo remove")] SeraFormats? Formats = null // todo remove Formats
)
{
    public static VariantStyle Default { get; } = new();

    public static VariantStyle? FromAttr(SeraVariantAttribute? attr, SeraFormatsAttribute? formats) => attr == null
        ? null
        : new()
        {
            Priority = attr.Priority,
            // Formats = SeraFormats.FromAttr(formats),
        };
}

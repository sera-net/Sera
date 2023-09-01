using System;
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

public readonly struct VariantTag
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
}

public readonly struct Variant
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
}

public enum VariantKind : byte
{
    Name,
    Tag,
    NameAndTag,
}

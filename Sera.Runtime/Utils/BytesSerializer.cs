using System;
using System.Buffers;
using System.Buffers.Binary;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Sera.Core;
using Sera.Runtime.Emit.Ser;
using Sera.Utils;

namespace Sera.Runtime.Utils;

internal class BytesSerializer(Stream stream, ISeraOptions options) : ASeraVisitor<Unit>, IDisposable, IAsyncDisposable
{
    public Stream Stream { get; } = stream;
    private readonly BinaryWriter Writer = new(stream);

    public override ISeraOptions Options { get; } = options;

    public override string FormatName => "bytes";
    public override string FormatMIME => "application/octet-stream";
    public override SeraFormatType FormatType => SeraFormatType.Binary;
    public override IRuntimeProvider<ISeraVision<object?>> RuntimeProvider => EmitRuntimeProvider.Instance;

    public void Dispose()
    {
        Writer.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await Writer.DisposeAsync();
    }

    public enum TypeToken : byte
    {
        None,
        Some,
        Primitive,
        String,
        Bytes,
        Seq,
        Map,
        Variant,
    }

    public enum SplitToken : byte
    {
        Start = 1,
        End,
        Split,
        Mid,
    }

    public override Unit Flush()
    {
        stream.Flush();
        return default;
    }

    #region Reference

    public override Unit VReference<V, T>(V vision, T value)
        => vision.Accept<Unit, BytesSerializer>(this, value);

    #endregion

    #region Primitive

    public override Unit VPrimitive(bool value, SeraFormats? formats = null)
    {
        Writer.Write((byte)TypeToken.Primitive);
        Writer.Write((byte)SeraPrimitiveTypes.Boolean);
        Writer.Write(value);
        return default;
    }

    public override Unit VPrimitive(sbyte value, SeraFormats? formats = null)
    {
        Writer.Write((byte)TypeToken.Primitive);
        Writer.Write((byte)SeraPrimitiveTypes.SByte);
        Writer.Write(value);
        return default;
    }

    public override Unit VPrimitive(byte value, SeraFormats? formats = null)
    {
        Writer.Write((byte)TypeToken.Primitive);
        Writer.Write((byte)SeraPrimitiveTypes.Byte);
        Writer.Write(value);
        return default;
    }

    public override Unit VPrimitive(short value, SeraFormats? formats = null)
    {
        Writer.Write((byte)TypeToken.Primitive);
        Writer.Write((byte)SeraPrimitiveTypes.Int16);
        Writer.Write(value);
        return default;
    }

    public override Unit VPrimitive(ushort value, SeraFormats? formats = null)
    {
        Writer.Write((byte)TypeToken.Primitive);
        Writer.Write((byte)SeraPrimitiveTypes.UInt16);
        Writer.Write(value);
        return default;
    }

    public override Unit VPrimitive(int value, SeraFormats? formats = null)
    {
        Writer.Write((byte)TypeToken.Primitive);
        Writer.Write((byte)SeraPrimitiveTypes.Int32);
        Writer.Write(value);
        return default;
    }

    public override Unit VPrimitive(uint value, SeraFormats? formats = null)
    {
        Writer.Write((byte)TypeToken.Primitive);
        Writer.Write((byte)SeraPrimitiveTypes.UInt32);
        Writer.Write(value);
        return default;
    }

    public override Unit VPrimitive(long value, SeraFormats? formats = null)
    {
        Writer.Write((byte)TypeToken.Primitive);
        Writer.Write((byte)SeraPrimitiveTypes.Int64);
        Writer.Write(value);
        return default;
    }

    public override Unit VPrimitive(ulong value, SeraFormats? formats = null)
    {
        Writer.Write((byte)TypeToken.Primitive);
        Writer.Write((byte)SeraPrimitiveTypes.UInt64);
        Writer.Write(value);
        return default;
    }

    public override Unit VPrimitive(Int128 value, SeraFormats? formats = null)
    {
        Writer.Write((byte)TypeToken.Primitive);
        Writer.Write((byte)SeraPrimitiveTypes.Int128);
        Span<byte> span = stackalloc byte[Unsafe.SizeOf<Int128>()];
        BinaryPrimitives.WriteInt128LittleEndian(span, value);
        Writer.Write(span);
        return default;
    }

    public override Unit VPrimitive(UInt128 value, SeraFormats? formats = null)
    {
        Writer.Write((byte)TypeToken.Primitive);
        Writer.Write((byte)SeraPrimitiveTypes.UInt128);
        Span<byte> span = stackalloc byte[Unsafe.SizeOf<UInt128>()];
        BinaryPrimitives.WriteUInt128LittleEndian(span, value);
        Writer.Write(span);
        return default;
    }

    public override Unit VPrimitive(IntPtr value, SeraFormats? formats = null)
    {
        Writer.Write((byte)TypeToken.Primitive);
        Writer.Write((byte)SeraPrimitiveTypes.IntPtr);
        Writer.Write(value);
        return default;
    }

    public override Unit VPrimitive(UIntPtr value, SeraFormats? formats = null)
    {
        Writer.Write((byte)TypeToken.Primitive);
        Writer.Write((byte)SeraPrimitiveTypes.UIntPtr);
        Writer.Write(value);
        return default;
    }

    public override Unit VPrimitive(Half value, SeraFormats? formats = null)
    {
        Writer.Write((byte)TypeToken.Primitive);
        Writer.Write((byte)SeraPrimitiveTypes.Half);
        Writer.Write(value);
        return default;
    }

    public override Unit VPrimitive(float value, SeraFormats? formats = null)
    {
        Writer.Write((byte)TypeToken.Primitive);
        Writer.Write((byte)SeraPrimitiveTypes.Single);
        Writer.Write(value);
        return default;
    }

    public override Unit VPrimitive(double value, SeraFormats? formats = null)
    {
        Writer.Write((byte)TypeToken.Primitive);
        Writer.Write((byte)SeraPrimitiveTypes.Double);
        Writer.Write(value);
        return default;
    }

    public override Unit VPrimitive(decimal value, SeraFormats? formats = null)
    {
        Writer.Write((byte)TypeToken.Primitive);
        Writer.Write((byte)SeraPrimitiveTypes.Decimal);
        Writer.Write(value);
        return default;
    }

    public override Unit VPrimitive(NFloat value, SeraFormats? formats = null)
    {
        Writer.Write((byte)TypeToken.Primitive);
        Writer.Write((byte)SeraPrimitiveTypes.NFloat);
        Writer.Write((double)value);
        return default;
    }

    public override Unit VPrimitive(BigInteger value, SeraFormats? formats = null)
    {
        Writer.Write((byte)TypeToken.Primitive);
        Writer.Write((byte)SeraPrimitiveTypes.BigInteger);
        var str = value.ToString();
        Writer.Write(str.Length);
        Writer.Write(str);
        return default;
    }

    public override Unit VPrimitive(Complex value, SeraFormats? formats = null)
    {
        Writer.Write((byte)TypeToken.Primitive);
        Writer.Write((byte)SeraPrimitiveTypes.Complex);
        Writer.Write(value.Real);
        Writer.Write(value.Imaginary);
        return default;
    }

    public override Unit VPrimitive(TimeSpan value, SeraFormats? formats = null)
    {
        Writer.Write((byte)TypeToken.Primitive);
        Writer.Write((byte)SeraPrimitiveTypes.TimeSpan);
        Writer.Write(value.Ticks);
        return default;
    }

    public override Unit VPrimitive(DateOnly value, SeraFormats? formats = null)
    {
        Writer.Write((byte)TypeToken.Primitive);
        Writer.Write((byte)SeraPrimitiveTypes.DateOnly);
        Writer.Write(value.DayNumber);
        return default;
    }

    public override Unit VPrimitive(TimeOnly value, SeraFormats? formats = null)
    {
        Writer.Write((byte)TypeToken.Primitive);
        Writer.Write((byte)SeraPrimitiveTypes.TimeOnly);
        Writer.Write(value.Ticks);
        return default;
    }

    public override Unit VPrimitive(DateTime value, SeraFormats? formats = null)
    {
        Writer.Write((byte)TypeToken.Primitive);
        Writer.Write((byte)SeraPrimitiveTypes.DateTime);
        Writer.Write(value.Ticks);
        return default;
    }

    public override Unit VPrimitive(DateTimeOffset value, SeraFormats? formats = null)
    {
        Writer.Write((byte)TypeToken.Primitive);
        Writer.Write((byte)SeraPrimitiveTypes.DateTimeOffset);
        Writer.Write(value.Ticks);
        return default;
    }

    public override Unit VPrimitive(Guid value, SeraFormats? formats = null)
    {
        Writer.Write((byte)TypeToken.Primitive);
        Writer.Write((byte)SeraPrimitiveTypes.Guid);
        Span<byte> bytes = stackalloc byte[sizeof(ulong) * 2];
        value.TryWriteBytes(bytes);
        Writer.Write(bytes);
        return default;
    }

    public override Unit VPrimitive(Range value, SeraFormats? formats = null)
    {
        Writer.Write((byte)TypeToken.Primitive);
        Writer.Write((byte)SeraPrimitiveTypes.Range);
        Writer.Write(value.Start.Value);
        Writer.Write(value.End.Value);
        return default;
    }

    public override Unit VPrimitive(Index value, SeraFormats? formats = null)
    {
        Writer.Write((byte)TypeToken.Primitive);
        Writer.Write((byte)SeraPrimitiveTypes.Index);
        Writer.Write(value.Value);
        return default;
    }

    public override Unit VPrimitive(char value, SeraFormats? formats = null)
    {
        Writer.Write((byte)TypeToken.Primitive);
        Writer.Write((byte)SeraPrimitiveTypes.Char);
        Writer.Write(value);
        return default;
    }

    public override Unit VPrimitive(Rune value, SeraFormats? formats = null)
    {
        Writer.Write((byte)TypeToken.Primitive);
        Writer.Write((byte)SeraPrimitiveTypes.Rune);
        Writer.Write(value.Value);
        return default;
    }

    public override Unit VPrimitive(Uri value, SeraFormats? formats = null)
    {
        Writer.Write((byte)TypeToken.Primitive);
        Writer.Write((byte)SeraPrimitiveTypes.Uri);
        var str = value.ToString();
        Writer.Write(str.Length);
        Writer.Write(str);
        return default;
    }

    public override Unit VPrimitive(Version value, SeraFormats? formats = null)
    {
        Writer.Write((byte)TypeToken.Primitive);
        Writer.Write((byte)SeraPrimitiveTypes.Version);
        var str = value.ToString();
        Writer.Write(str.Length);
        Writer.Write(str);
        return default;
    }

    #endregion

    #region String

    public override Unit VString(ReadOnlyMemory<char> value)
    {
        var len = value.Length;
        if (len <= 15)
        {
            Writer.Write((byte)((byte)TypeToken.String | (1 << 3) | (value.Length << 4)));
        }
        else
        {
            Writer.Write((byte)TypeToken.String);
            Writer.Write(value.Length);
        }
        Writer.Write(value.Span);
        return default;
    }

    public override Unit VString(ReadOnlyMemory<byte> value, Encoding encoding)
    {
        Writer.Write((byte)((byte)TypeToken.String | (1 << 4)));
        Writer.Write(value.Length);
        Writer.Write(encoding.CodePage);
        Writer.Write(value.Span);
        return default;
    }

    #endregion

    #region Bytes

    public override Unit VBytes(ReadOnlyMemory<byte> value)
    {
        Writer.Write((byte)TypeToken.Bytes);
        Writer.Write(value.Length);
        Writer.Write(value.Span);
        return default;
    }

    public override Unit VBytes(ReadOnlySequence<byte> value)
    {
        var length = value.Length;
        Writer.Write((byte)((byte)TypeToken.Bytes | (1 << 3)));
        Writer.Write(length);
        foreach (var mem in value)
        {
            Writer.Write(mem.Span);
        }
        return default;
    }

    #endregion

    #region Array

    public override Unit VArray<V, T>(V vision, ReadOnlyMemory<T> value)
    {
        Writer.Write((byte)((byte)TypeToken.Seq | (byte)SplitToken.Start << 3));
        var first = true;
        foreach (var item in value)
        {
            if (first) first = false;
            else Writer.Write((byte)((byte)TypeToken.Seq | (byte)SplitToken.Split << 3));
            vision.Accept<Unit, BytesSerializer>(this, item);
        }
        Writer.Write((byte)((byte)TypeToken.Seq | (byte)SplitToken.End << 3));
        return default;
    }

    public override Unit VArray<V, T>(V vision, ReadOnlySequence<T> value)
    {
        Writer.Write((byte)((byte)TypeToken.Seq | (byte)SplitToken.Start << 3));
        bool first = true;
        foreach (var mem in value)
        {
            foreach (var item in mem.Span)
            {
                if (first) first = false;
                else Writer.Write((byte)((byte)TypeToken.Seq | (byte)SplitToken.Split << 3));
                vision.Accept<Unit, BytesSerializer>(this, item);
            }
        }
        Writer.Write((byte)((byte)TypeToken.Seq | (byte)SplitToken.End << 3));
        return default;
    }

    #endregion

    #region Unit

    public override Unit VUnit() => VNone();

    #endregion

    #region Option

    public override Unit VNone()
    {
        Writer.Write((byte)TypeToken.None);
        return default;
    }

    public override Unit VSome<V, T>(V vision, T value)
    {
        Writer.Write((byte)TypeToken.Some);
        return vision.Accept<Unit, BytesSerializer>(this, value);
    }

    #endregion

    #region Entry

    public override Unit VEntry<KV, VV, IK, IV>(KV keyVision, VV valueVision, IK key, IV value)
    {
        Writer.Write((byte)((byte)TypeToken.Seq | (byte)SplitToken.Start << 3));
        keyVision.Accept<Unit, BytesSerializer>(this, key);
        Writer.Write((byte)((byte)TypeToken.Seq | (byte)SplitToken.Split << 3));
        valueVision.Accept<Unit, BytesSerializer>(this, value);
        Writer.Write((byte)((byte)TypeToken.Seq | (byte)SplitToken.End << 3));
        return default;
    }

    #endregion

    #region Tuple

    public override Unit VTuple<V, T>(V vision, T value)
    {
        Writer.Write((byte)((byte)TypeToken.Seq | (byte)SplitToken.Start << 3));
        var first = true;
        var size = vision.Size;
        TupleVisitor ??= new(this);
        for (var i = 0; i < size; i++)
        {
            if (first) first = false;
            else Writer.Write((byte)((byte)TypeToken.Seq | (byte)SplitToken.Split << 3));
            vision.AcceptItem<Unit, TupleSeraVisitor>(TupleVisitor, value, i);
        }
        Writer.Write((byte)((byte)TypeToken.Seq | (byte)SplitToken.End << 3));
        return default;
    }

    private TupleSeraVisitor? TupleVisitor;

    private class TupleSeraVisitor(BytesSerializer Base) : ATupleSeraVisitor<Unit>(Base)
    {
        public override Unit VItem<T, V>(V vision, T value)
            => vision.Accept<Unit, BytesSerializer>(Base, value);

        public override Unit VNone() => default;
    }

    #endregion

    #region Seq

    public override Unit VSeq<V>(V vision)
    {
        Writer.Write((byte)((byte)TypeToken.Seq | (byte)SplitToken.Start << 3));
        var first = true;
        SeqVisitor ??= new(this);
        while (vision.MoveNext())
        {
            if (first) first = false;
            else Writer.Write((byte)((byte)TypeToken.Seq | (byte)SplitToken.Split << 3));
            vision.AcceptNext<Unit, SeqSeraVisitor>(SeqVisitor);
        }
        Writer.Write((byte)((byte)TypeToken.Seq | (byte)SplitToken.End << 3));
        return default;
    }

    private SeqSeraVisitor? SeqVisitor;

    private class SeqSeraVisitor(BytesSerializer Base) : ASeqSeraVisitor<Unit>(Base)
    {
        public override Unit VItem<T, V>(V vision, T value)
            => vision.Accept<Unit, BytesSerializer>(Base, value);

        public override Unit VEnd() => default;
    }

    #endregion

    #region Map

    public override Unit VMap<V>(V vision)
    {
        Writer.Write((byte)((byte)TypeToken.Map | (byte)SplitToken.Start << 3));
        var first = true;
        MapVisitor ??= new(this);
        while (vision.MoveNext())
        {
            if (first) first = false;
            else Writer.Write((byte)((byte)TypeToken.Map | (byte)SplitToken.Split << 3));
            vision.AcceptNext<Unit, MapSeraVisitor>(MapVisitor);
        }
        Writer.Write((byte)((byte)TypeToken.Map | (byte)SplitToken.End << 3));
        return default;
    }

    private MapSeraVisitor? MapVisitor;

    private class MapSeraVisitor(BytesSerializer Base) : AMapSeraVisitor<Unit>(Base)
    {
        public override Unit VEntry<KV, VV, IK, IV>(KV keyVision, VV valueVision, IK key, IV value)
        {
            keyVision.Accept<Unit, BytesSerializer>(Base, key);
            Base.Writer.Write((byte)((byte)TypeToken.Map | (byte)SplitToken.Mid << 3));
            return valueVision.Accept<Unit, BytesSerializer>(Base, value);
        }

        public override Unit VEnd() => default;
    }

    #endregion

    #region Struct

    public override Unit VStruct<V, T>(V vision, T value)
    {
        Writer.Write((byte)((byte)TypeToken.Map | (byte)SplitToken.Start << 3));
        var first = true;
        StructVisitor ??= new(this);
        var size = vision.Count;
        for (var i = 0; i < size; i++)
        {
            if (first) first = false;
            else Writer.Write((byte)((byte)TypeToken.Map | (byte)SplitToken.Split << 3));
            vision.AcceptField<Unit, StructSeraVisitor>(StructVisitor, value, i);
        }
        Writer.Write((byte)((byte)TypeToken.Map | (byte)SplitToken.End << 3));
        return default;
    }

    private StructSeraVisitor? StructVisitor;

    private class StructSeraVisitor(BytesSerializer Base) : AStructSeraVisitor<Unit>(Base)
    {
        public override Unit VField<V, T>(V vision, T value, string name, long key)
        {
            Base.VPrimitive(key);
            Base.Writer.Write((byte)((byte)TypeToken.Map | (byte)SplitToken.Mid << 3));
            return vision.Accept<Unit, BytesSerializer>(Base, value);
        }

        public override Unit VNone() => default;
    }

    #endregion

    #region Union

    public override Unit VUnion<V, T>(V vision, T value)
    {
        UnionVisitor ??= new(this);
        return vision.AcceptUnion<Unit, UnionSeraVisitor>(UnionVisitor, value);
    }

    private UnionSeraVisitor? UnionVisitor;

    private class UnionSeraVisitor(BytesSerializer Base) : AUnionSeraVisitor<Unit>(Base)
    {
        public override Unit VEmpty()
        {
            Base.Writer.Write((byte)((byte)TypeToken.Variant | (byte)SplitToken.Start << 3));
            Base.Writer.Write((byte)((byte)TypeToken.Variant | (byte)SplitToken.End << 3));
            return default;
        }

        public override Unit VNone() => throw new SeraMatchFailureException();

        public override Unit VVariant(Variant variant,
            UnionStyle? union_style = null, VariantStyle? variant_style = null)
        {
            Base.Writer.Write((byte)((byte)TypeToken.Variant | (byte)SplitToken.Start << 3));
            var str = variant.ToString();
            Base.VString(str);
            Base.Writer.Write((byte)((byte)TypeToken.Variant | (byte)SplitToken.End << 3));
            return default;
        }

        public override Unit VVariantValue<V, T>(V vision, T value, Variant variant,
            UnionStyle? union_style = null, VariantStyle? variant_style = null)
        {
            Base.Writer.Write((byte)((byte)TypeToken.Variant | (byte)SplitToken.Start << 3));
            var str = variant.ToString();
            Base.VString(str);
            Base.Writer.Write((byte)((byte)TypeToken.Variant | (byte)SplitToken.Mid << 3));
            vision.Accept<Unit, BytesSerializer>(Base, value);
            Base.Writer.Write((byte)((byte)TypeToken.Variant | (byte)SplitToken.End << 3));
            return default;
        }

        public override Unit VVariantStruct<V, T>(V vision, T value, Variant variant,
            UnionStyle? union_style = null, VariantStyle? variant_style = null)
        {
            Base.Writer.Write((byte)((byte)TypeToken.Variant | (byte)SplitToken.Start << 3));
            var str = variant.ToString();
            Base.VString(str);
            Base.Writer.Write((byte)((byte)TypeToken.Variant | (byte)SplitToken.Mid << 3));
            Base.VStruct(vision, value);
            Base.Writer.Write((byte)((byte)TypeToken.Variant | (byte)SplitToken.End << 3));
            return default;
        }
    }

    #endregion
}

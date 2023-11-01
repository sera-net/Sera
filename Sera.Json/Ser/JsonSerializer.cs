using System;
using System.Buffers;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using Sera.Core;
using Sera.Core.Formats;
using Sera.Core.Impls.Ser;
using Sera.Core.Providers.Ser;
using Sera.Utils;

namespace Sera.Json.Ser;

public class JsonSerializer(SeraJsonOptions options, AJsonFormatter formatter, AJsonWriter writer) : ASeraVisitor<Unit>
{
    public JsonSerializer(AJsonWriter writer) : this(writer.Options, writer.Formatter, writer) { }

    private readonly AJsonWriter writer = writer;
    private readonly AJsonFormatter formatter = formatter;

    public override string FormatName => "json";
    public override string FormatMIME => "application/json";
    public override SeraFormatType FormatType => SeraFormatType.HumanReadableText;
    public override ISeraOptions Options => options;

    public override IRuntimeProvider<ISeraVision<object?>> RuntimeProvider =>
        RuntimeProviderOverride ?? EmptySerRuntimeProvider.Instance;

    public IRuntimeProvider<ISeraVision<object?>>? RuntimeProviderOverride { get; set; }

    private JsonSerializerState state;

    public override Unit Flush()
    {
        writer.Flush();
        return default;
    }
    
    #region Reference

    public override Unit VReference<V, T>(V vision, T value)
    {
        // todo circular reference
        return vision.Accept<Unit, JsonSerializer>(this, value);
    }

    #endregion

    #region Primitive

    #region Number

    private Unit WriteNumber<T>(int size, T v, SeraFormats? formats, bool use_string) where T : ISpanFormattable
    {
        var format = formats?.CustomNumberTextFormat ?? (formats?.NumberTextFormat is { } ntf
            ? (ntf switch
            {
                NumberTextFormat.Decimal or NumberTextFormat.Any => "D",
                NumberTextFormat.Hex => "X",
                NumberTextFormat.Binary => "B",
                _ => throw new ArgumentOutOfRangeException()
            })
            : null);
        if (format == null)
        {
            Span<char> chars = stackalloc char[size];
            if (v.TryFormat(chars, out var len, null, null))
            {
                var span = chars[..len];
                if (use_string) writer.WriteString(span, false);
                else writer.Write(span);
                return default;
            }
        }
        var str = v.ToString(format, null);
        if (use_string) writer.WriteString(str, true);
        else writer.Write(str);
        return default;
    }

    #endregion

    #region Date

    private Unit WriteDate(TimeSpan v, SeraFormats? formats)
    {
        if (formats?.DateTimeFormat.HasFlag(DateTimeFormatFlags.DateAsNumber) ?? false)
        {
            return WriteNumber(32, v.Ticks, formats, formatter.LargeNumberUseString);
        }
        Span<char> chars = stackalloc char[32];
        if (v.TryFormat(chars, out var len, "G"))
        {
            writer.WriteString(chars[..len], false);
        }
        else
        {
            writer.WriteString(v.ToString("G"), false);
        }
        return default;
    }

    private Unit WriteDate(DateOnly v, SeraFormats? formats)
    {
        if (formats?.DateTimeFormat.HasFlag(DateTimeFormatFlags.DateOnlyToDateTime) ?? false)
        {
            var date_time = v.ToDateTime(TimeOnly.MinValue);
            return WriteDate(date_time, formats);
        }
        if (formats?.DateTimeFormat.HasFlag(DateTimeFormatFlags.DateOnlyToDateTimeOffset) ?? false)
        {
            var date_time_offset =
                new DateTimeOffset(TimeZoneInfo.ConvertTime(v.ToDateTime(TimeOnly.MinValue), Options.TimeZone));
            return WriteDate(date_time_offset, formats);
        }
        if (formats?.DateTimeFormat.HasFlag(DateTimeFormatFlags.DateAsNumber) ?? false)
        {
            return WriteNumber(32, v.DayNumber, formats, false);
        }
        Span<char> chars = stackalloc char[16];
        if (v.TryFormat(chars, out var len, "O"))
        {
            writer.WriteString(chars[..len], false);
        }
        else
        {
            writer.WriteString(v.ToString("O"), false);
        }
        return default;
    }

    private Unit WriteDate(TimeOnly v, SeraFormats? formats)
    {
        if (formats?.DateTimeFormat.HasFlag(DateTimeFormatFlags.DateAsNumber) ?? false)
        {
            return WriteNumber(32, v.Ticks, formats, formatter.LargeNumberUseString);
        }
        Span<char> chars = stackalloc char[32];
        if (v.TryFormat(chars, out var len, "O"))
        {
            writer.WriteString(chars[..len], false);
        }
        else
        {
            writer.WriteString(v.ToString("O"), false);
        }
        return default;
    }

    private Unit WriteDate(DateTime v, SeraFormats? formats)
    {
        v = TimeZoneInfo.ConvertTime(v, Options.TimeZone);
        if (formats?.DateTimeFormat.HasFlag(DateTimeFormatFlags.DateTimeToDateTimeOffset) ?? false)
        {
            var date_time_offset = new DateTimeOffset(v);
            return WriteDate(date_time_offset, formats);
        }
        if (formats?.DateTimeFormat.HasFlag(DateTimeFormatFlags.DateAsNumber) ?? false)
        {
            return WriteNumber(32, v.Ticks, formats, formatter.LargeNumberUseString);
        }
        Span<char> chars = stackalloc char[64];
        if (v.TryFormat(chars, out var len, "O"))
        {
            writer.WriteString(chars[..len], false);
        }
        else
        {
            writer.WriteString(v.ToString("O"), false);
        }
        return default;
    }

    private Unit WriteDate(DateTimeOffset v, SeraFormats? formats)
    {
        if (formats?.DateTimeFormat.HasFlag(DateTimeFormatFlags.DateAsNumber) ?? false)
        {
            return WriteNumber(32, v.Ticks, formats, formatter.LargeNumberUseString);
        }
        if (formats?.DateTimeFormat.HasFlag(DateTimeFormatFlags.DateTimeOffsetUseTimeZone) ?? false)
        {
            v = TimeZoneInfo.ConvertTime(v, Options.TimeZone);
        }
        Span<char> chars = stackalloc char[64];
        if (v.TryFormat(chars, out var len, "O"))
        {
            writer.WriteString(chars[..len], false);
        }
        else
        {
            writer.WriteString(v.ToString("O"), false);
        }
        return default;
    }

    #endregion

    #region Guid

    private Unit WriteGuid(Guid v, SeraFormats? formats)
    {
        var format = formats?.CustomGuidTextFormat ?? (formats?.GuidTextFormat is { } gtf
            ? gtf switch
            {
                GuidTextFormat.GuidTextShort or GuidTextFormat.Any => "N",
                GuidTextFormat.GuidTextGuid => "D",
                GuidTextFormat.GuidTextBraces => "B",
                GuidTextFormat.GuidTextParentheses => "P",
                GuidTextFormat.GuidTextHex => "X",
                _ => throw new ArgumentOutOfRangeException()
            }
            : null) ?? "D";
        if (format != "X")
        {
            Span<char> chars = stackalloc char[64];
            if (v.TryFormat(chars, out var len, format))
            {
                writer.WriteString(chars[..len], false);
                return default;
            }
        }
        var str = v.ToString(format);
        writer.WriteString(str, false);
        return default;
    }

    #endregion

    #region Char

    private Unit WriteChar<T>(T v) where T : ISpanFormattable
    {
        Span<char> chars = stackalloc char[8];
        if (v.TryFormat(chars, out var len, null, null))
        {
            writer.WriteString(chars[..len], true);
        }
        else
        {
            writer.WriteString(v.ToString(null, null), true);
        }
        return default;
    }

    #endregion

    public override Unit VPrimitive(bool value, SeraFormats? formats = null)
    {
        if (formats is { BooleanAsNumber: true }) return VPrimitive(value ? 1 : 0, formats);
        writer.Write(value ? "true" : "false");
        return default;
    }

    public override Unit VPrimitive(sbyte value, SeraFormats? formats = null)
        => WriteNumber(4, value, formats, false);

    public override Unit VPrimitive(byte value, SeraFormats? formats = null)
        => WriteNumber(4, value, formats, false);

    public override Unit VPrimitive(short value, SeraFormats? formats = null)
        => WriteNumber(8, value, formats, false);

    public override Unit VPrimitive(ushort value, SeraFormats? formats = null)
        => WriteNumber(8, value, formats, false);

    public override Unit VPrimitive(int value, SeraFormats? formats = null)
        => WriteNumber(16, value, formats, false);

    public override Unit VPrimitive(uint value, SeraFormats? formats = null)
        => WriteNumber(16, value, formats, false);

    public override Unit VPrimitive(long value, SeraFormats? formats = null)
        => WriteNumber(32, value, formats, formatter.LargeNumberUseString);

    public override Unit VPrimitive(ulong value, SeraFormats? formats = null)
        => WriteNumber(32, value, formats, formatter.LargeNumberUseString);

    public override Unit VPrimitive(Int128 value, SeraFormats? formats = null)
        => WriteNumber(64, value, formats, formatter.LargeNumberUseString);

    public override Unit VPrimitive(UInt128 value, SeraFormats? formats = null)
        => WriteNumber(64, value, formats, formatter.LargeNumberUseString);

    public override Unit VPrimitive(IntPtr value, SeraFormats? formats = null)
        => WriteNumber(32, value, formats, formatter.LargeNumberUseString);

    public override Unit VPrimitive(UIntPtr value, SeraFormats? formats = null)
        => WriteNumber(32, value, formats, formatter.LargeNumberUseString);

    public override Unit VPrimitive(Half value, SeraFormats? formats = null)
        => WriteNumber(32, value, formats, false);

    public override Unit VPrimitive(float value, SeraFormats? formats = null)
        => WriteNumber(32, value, formats, false);

    public override Unit VPrimitive(double value, SeraFormats? formats = null)
        => WriteNumber(32, value, formats, false);

    public override Unit VPrimitive(decimal value, SeraFormats? formats = null)
        => WriteNumber(32, value, formats, false);

    public override Unit VPrimitive(NFloat value, SeraFormats? formats = null)
        => WriteNumber(32, value, formats, false);

    public override Unit VPrimitive(BigInteger value, SeraFormats? formats = null)
    {
        writer.WriteString(value.ToString(), false);
        return default;
    }

    public override Unit VPrimitive(Complex value, SeraFormats? formats = null)
    {
        writer.WriteString(value.ToString(), false);
        return default;
    }

    public override Unit VPrimitive(TimeSpan value, SeraFormats? formats = null)
        => WriteDate(value, formats);

    public override Unit VPrimitive(DateOnly value, SeraFormats? formats = null)
        => WriteDate(value, formats);

    public override Unit VPrimitive(TimeOnly value, SeraFormats? formats = null)
        => WriteDate(value, formats);

    public override Unit VPrimitive(DateTime value, SeraFormats? formats = null)
        => WriteDate(value, formats);

    public override Unit VPrimitive(DateTimeOffset value, SeraFormats? formats = null)
        => WriteDate(value, formats);

    public override Unit VPrimitive(Guid value, SeraFormats? formats = null)
        => WriteGuid(value, formats);

    public override Unit VPrimitive(Range value, SeraFormats? formats = null)
    {
        writer.WriteString(value.ToString(), true);
        return default;
    }

    public override Unit VPrimitive(Index value, SeraFormats? formats = null)
    {
        writer.WriteString(value.ToString(), true);
        return default;
    }

    public override Unit VPrimitive(char value, SeraFormats? formats = null)
        => WriteChar(value);

    public override Unit VPrimitive(Rune value, SeraFormats? formats = null)
        => WriteChar(value);

    public override Unit VPrimitive(Uri value, SeraFormats? formats = null)
    {
        writer.WriteString(value.ToString(), true);
        return default;
    }

    public override Unit VPrimitive(Version value, SeraFormats? formats = null)
    {
        writer.WriteString(value.ToString(), true);
        return default;
    }

    #endregion

    #region String

    public override Unit VString(ReadOnlyMemory<char> value)
    {
        writer.WriteString(value.Span, true);
        return default;
    }

    public override Unit VString(ReadOnlyMemory<byte> value, Encoding encoding)
    {
        writer.WriteStringEncoded(value.Span, encoding, true);
        return default;
    }

    #endregion

    #region Bytes

    public override Unit VBytes(ReadOnlySequence<byte> value)
    {
        if (formatter.Base64Bytes)
        {
            var stream = writer.StartBase64();
            try
            {
                foreach (var mem in value)
                {
                    stream.Write(mem.Span);
                }
            }
            finally
            {
                writer.EndBase64();
            }
            return default;
        }
        else
        {
            return VArray(new PrimitiveImpl(), value);
        }
    }

    public override Unit VBytes(ReadOnlyMemory<byte> value)
    {
        if (formatter.Base64Bytes)
        {
            var stream = writer.StartBase64();
            try
            {
                stream.Write(value.Span);
            }
            finally
            {
                writer.EndBase64();
            }
            return default;
        }
        else
        {
            return VArray(new PrimitiveImpl(), value);
        }
    }

    #endregion

    #region Array

    public override Unit VArray<V, T>(V vision, ReadOnlySequence<T> value)
    {
        var len = value.Length;
        if (len is 0)
        {
            writer.Write("[]");
            return default;
        }
        var last_state = state;
        state = JsonSerializerState.None;
        writer.Write("[");
        var isFirst = true;
        foreach (var mem in value)
        {
            foreach (var item in mem.Span)
            {
                if (isFirst) isFirst = false;
                else writer.Write(",");
                vision.Accept<Unit, JsonSerializer>(this, item);
            }
        }
        writer.Write("]");
        state = last_state;
        return default;
    }

    public override Unit VArray<V, T>(V vision, ReadOnlyMemory<T> value)
    {
        var len = value.Length;
        if (len is 0)
        {
            writer.Write("[]");
            return default;
        }
        var last_state = state;
        state = JsonSerializerState.None;
        writer.Write("[");
        var isFirst = true;
        foreach (var item in value)
        {
            if (isFirst) isFirst = false;
            else writer.Write(",");
            vision.Accept<Unit, JsonSerializer>(this, item);
        }
        writer.Write("]");
        state = last_state;
        return default;
    }

    #endregion

    #region Unit

    public override Unit VUnit() => VNone();

    #endregion

    #region Option

    public override Unit VNone()
    {
        writer.Write("null");
        return default;
    }

    public override Unit VSome<V, T>(V vision, T value)
        => vision.Accept<Unit, JsonSerializer>(this, value);

    #endregion

    #region Entry

    public override Unit VEntry<KV, VV, IK, IV>(KV keyVision, VV valueVision, IK key, IV value)
    {
        var last_state = state;
        state = JsonSerializerState.None;
        writer.Write("[");
        keyVision.Accept<Unit, JsonSerializer>(this, key);
        writer.Write(",");
        valueVision.Accept<Unit, JsonSerializer>(this, value);
        writer.Write("]");
        state = last_state;
        return default;
    }

    #endregion

    #region Tuple

    public override Unit VTuple<V, T>(V vision, T value)
    {
        var size = vision.Size;
        if (size == 0)
        {
            writer.Write("[]");
            return default;
        }
        var last_state = state;
        state = JsonSerializerState.None;
        TupleVisitor ??= new(this);
        writer.Write("[");
        var first = true;
        for (var i = 0; i < size; i++)
        {
            if (first) first = false;
            else writer.Write(",");
            var err = vision.AcceptItem<bool, TupleSeraVisitor>(TupleVisitor, value, i);
            if (err) throw new SerializeException($"Unable to get item {i} of tuple {value}");
        }
        writer.Write("]");
        state = last_state;
        return default;
    }

    private TupleSeraVisitor? TupleVisitor;

    private class TupleSeraVisitor(JsonSerializer Base) : ATupleSeraVisitor<bool>(Base)
    {
        public override bool VItem<T, V>(V vision, T value)
        {
            vision.Accept<Unit, JsonSerializer>(Base, value);
            return false;
        }

        public override bool VNone() => true;
    }

    #endregion

    #region Seq

    public override Unit VSeq<V>(V vision)
    {
        var size = vision.Count;
        if (size is 0)
        {
            writer.Write("[]");
            return default;
        }
        var last_state = state;
        state = JsonSerializerState.None;
        SeqVisitor ??= new(this);
        writer.Write("[");
        var first = true;
        while (vision.HasNext)
        {
            if (first) first = false;
            else writer.Write(",");
            vision.AcceptNext<Unit, SeqSeraVisitor>(SeqVisitor);
        }
        writer.Write("]");
        state = last_state;
        return default;
    }

    private SeqSeraVisitor? SeqVisitor;

    private class SeqSeraVisitor(JsonSerializer Base) : ASeqSeraVisitor<Unit>(Base)
    {
        public override Unit VItem<T, V>(V vision, T value)
            => vision.Accept<Unit, JsonSerializer>(Base, value);

        public override Unit VEnd() => default;
    }

    #endregion

    #region Map

    public override Unit VMap<V>(V vision)
    {
        var size = vision.Count;
        if (size is 0)
        {
            writer.Write("[]");
            return default;
        }
        var last_state = state;
        state = JsonSerializerState.None;
        SeqMapVisitor ??= new(this);
        writer.Write("[");
        var first = true;
        while (vision.HasNext)
        {
            if (first) first = false;
            else writer.Write(",");
            vision.AcceptNext<Unit, SeqMapSeraVisitor>(SeqMapVisitor);
        }
        writer.Write("]");
        state = last_state;
        return default;
    }

    private SeqMapSeraVisitor? SeqMapVisitor;

    private class SeqMapSeraVisitor(JsonSerializer Base) : AMapSeraVisitor<Unit>(Base)
    {
        public override Unit VEntry<KV, VV, IK, IV>(KV keyVision, VV valueVision, IK key, IV value)
            => Base.VEntry(keyVision, valueVision, key, value);

        public override Unit VEnd() => default;
    }

    #endregion

    #region Typed Map

    public override Unit VMap<V, T, IK, IV>(V vision)
    {
        if (typeof(IK) != typeof(string)) return VMap(vision);
        var size = vision.Count;
        if (size is 0)
        {
            writer.Write("{}");
            return default;
        }
        var last_state = state;
        state = JsonSerializerState.None;
        MapVisitor ??= new(this);
        writer.Write("{");
        var first = true;
        while (vision.HasNext)
        {
            if (first) first = false;
            else writer.Write(",");
            vision.AcceptNext<Unit, MapSeraVisitor>(MapVisitor);
        }
        writer.Write("}");
        state = last_state;
        return default;
    }

    private MapSeraVisitor? MapVisitor;

    private class MapSeraVisitor(JsonSerializer Base) : AMapSeraVisitor<Unit>(Base)
    {
        public override Unit VEntry<KV, VV, IK, IV>(KV keyVision, VV valueVision, IK key, IV value)
        {
            var str = key is null ? "null" : $"{key}";
            Base.writer.WriteString(str, true);
            Base.writer.Write(":");
            return valueVision.Accept<Unit, JsonSerializer>(Base, value);
        }

        public override Unit VEnd() => default;
    }

    #endregion

    #region Struct

    public override Unit VStruct<V, T>(V vision, T value)
    {
        var size = vision.Count;
        if (size is 0)
        {
            writer.Write("{}");
            return default;
        }
        var last_state = state;
        state = JsonSerializerState.None;
        StructVisitor ??= new(this);
        writer.Write("{");
        var first = true;
        for (var i = 0; i < size; i++)
        {
            if (first) first = false;
            else writer.Write(",");
            var err = vision.AcceptField<bool, StructSeraVisitor>(StructVisitor, value, i);
            if (err) throw new SerializeException($"Unable to get field nth {i} of {value}");
        }
        writer.Write("}");
        state = last_state;
        return default;
    }

    private StructSeraVisitor? StructVisitor;

    private class StructSeraVisitor(JsonSerializer Base) : AStructSeraVisitor<bool>(Base)
    {
        public override bool VField<V, T>(V vision, T value, string name, long key)
        {
            Base.writer.WriteString(name, true);
            Base.writer.Write(":");
            vision.Accept<Unit, JsonSerializer>(Base, value);
            return false;
        }

        public override bool VNone() => true;
    }

    #endregion

    #region Union

    public override Unit VUnion<V, T>(V vision, T value)
    {
        UnionVisitor ??= new(this);
        return vision.Accept<Unit, UnionSeraVisitor>(UnionVisitor, value);
    }

    private UnionSeraVisitor? UnionVisitor;

    private class UnionSeraVisitor(JsonSerializer Base) : AUnionSeraVisitor<Unit>(Base)
    {
        public override Unit VEmpty()
        {
            Base.writer.Write("{}");
            return default;
        }

        private void VVariant(Variant variant, UnionStyle? style, bool mustString)
        {
            switch (variant.Kind)
            {
                case VariantKind.NameAndTag:
                    if (mustString) goto case VariantKind.Name;
                    if (style is { VariantPriority: VariantPriority.TagFirst }) goto case VariantKind.Tag;
                    else goto case VariantKind.Name;
                case VariantKind.Name:
                    Base.writer.WriteString(variant.Name, true);
                    break;
                case VariantKind.Tag:
                    var tag = variant.Tag;
                    if (mustString)
                    {
                        Base.writer.WriteString(tag.ToString(), true);
                        return;
                    }
                    var formats = style?.VariantFormats ?? Base.formatter.DefaultFormats;
                    switch (tag.Kind)
                    {
                        case VariantTagKind.SByte:
                            Base.VPrimitive(tag.SByte, formats);
                            break;
                        case VariantTagKind.Byte:
                            Base.VPrimitive(tag.Byte, formats);
                            break;
                        case VariantTagKind.Int16:
                            Base.VPrimitive(tag.Int16, formats);
                            break;
                        case VariantTagKind.UInt16:
                            Base.VPrimitive(tag.UInt16, formats);
                            break;
                        case VariantTagKind.Int32:
                            Base.VPrimitive(tag.Int32, formats);
                            break;
                        case VariantTagKind.UInt32:
                            Base.VPrimitive(tag.UInt32, formats);
                            break;
                        case VariantTagKind.Int64:
                            Base.VPrimitive(tag.Int64, formats);
                            break;
                        case VariantTagKind.UInt64:
                            Base.VPrimitive(tag.UInt64, formats);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override Unit VVariant(Variant variant, UnionStyle? style = default)
        {
            VVariant(variant, style, false);
            return default;
        }

        public override Unit VVariant<V, T>(V vision, T value, Variant variant, UnionStyle? style = default)
        {
            var s = style ?? Base.formatter.DefaultUnionStyle;
            var format = s.Format is not UnionFormat.Any ? s.Format : Base.formatter.DefaultUnionFormat;
            switch (format)
            {
                case UnionFormat.External:
                    Base.writer.Write("{");
                    VVariant(variant, style, true);
                    Base.writer.Write(":");
                    vision.Accept<Unit, JsonSerializer>(Base, value);
                    Base.writer.Write("}");
                    break;
                case UnionFormat.Internal:
                    goto case UnionFormat.External;
                case UnionFormat.Adjacent:
                    Base.writer.Write("{");
                    Base.writer.WriteString(s.AdjacentTagName, true);
                    Base.writer.Write(":");
                    VVariant(variant, style, false);
                    Base.writer.Write(",");
                    Base.writer.WriteString(s.AdjacentValueName, true);
                    Base.writer.Write(":");
                    vision.Accept<Unit, JsonSerializer>(Base, value);
                    Base.writer.Write("}");
                    break;
                case UnionFormat.Tuple:
                    Base.writer.Write("[");
                    VVariant(variant, style, false);
                    Base.writer.Write(",");
                    vision.Accept<Unit, JsonSerializer>(Base, value);
                    Base.writer.Write("]");
                    break;
                case UnionFormat.Untagged:
                    vision.Accept<Unit, JsonSerializer>(Base, value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(style));
            }
            return default;
        }

        public override Unit VVariantStruct<V, T>(V vision, T value, Variant variant, UnionStyle? style = default)
        {
            var s = style ?? Base.formatter.DefaultUnionStyle;
            var size = vision.Count;
            var last_state = Base.state;
            Base.state = JsonSerializerState.None;
            Base.StructVisitor ??= new(Base);
            Base.writer.Write("{");
            Base.writer.WriteString(s.InternalTagName, true);
            Base.writer.Write(":");
            VVariant(variant, style, false);
            for (var i = 0; i < size; i++)
            {
                Base.writer.Write(",");
                var err = vision.AcceptField<bool, StructSeraVisitor>(Base.StructVisitor, value, i);
                if (err) throw new SerializeException($"Unable to get field nth {i} of {value}");
            }
            Base.writer.Write("}");
            Base.state = last_state;
            return default;
        }
    }

    #endregion
}

using System;
using System.Buffers;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Sera.Core;
using Sera.Core.Formats;
using Sera.Core.Impls.Ser;
using Sera.Core.Providers.Ser;
using Sera.Utils;

namespace Sera.Json.Ser;

public class AsyncJsonSerializer
    (SeraJsonOptions options, AJsonFormatter formatter, AAsyncJsonWriter writer) : ASeraVisitor<ValueTask>
{
    public AsyncJsonSerializer(AAsyncJsonWriter writer) : this(writer.Options, writer.Formatter, writer) { }

    private readonly AAsyncJsonWriter writer = writer;
    private readonly AJsonFormatter formatter = formatter;

    public override string FormatName => "json";
    public override string FormatMIME => "application/json";
    public override SeraFormatType FormatType => SeraFormatType.HumanReadableText;
    public override ISeraOptions Options => options;

    public override IRuntimeProvider<ISeraVision<object?>> RuntimeProvider =>
        RuntimeProviderOverride ?? EmptySerRuntimeProvider.Instance;

    public IRuntimeProvider<ISeraVision<object?>>? RuntimeProviderOverride { get; set; }

    private JsonSerializerState state;

    public override ValueTask Flush() => writer.Flush();

    #region Reference

    public override ValueTask VReference<V, T>(V vision, T value)
    {
        // todo circular reference
        return vision.Accept<ValueTask, AsyncJsonSerializer>(this, value);
    }

    #endregion

    #region Primitive

    #region Number

    private async ValueTask WriteNumber<T>(int size, T v, SeraFormats? formats, bool use_string)
        where T : ISpanFormattable
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
            using var chars = RAIIArrayPool<char>.Get(size);
            if (v.TryFormat(chars.Span, out var len, null, null))
            {
                var mem = chars.Memory[..len];
                if (use_string) await writer.WriteString(mem, false);
                else await writer.Write(mem);
                return;
            }
        }
        var str = v.ToString(format, null);
        if (use_string) await writer.WriteString(str, true);
        else await writer.Write(str);
    }

    #endregion

    #region Date

    private ValueTask WriteDate(TimeSpan v, SeraFormats? formats)
    {
        if (formats?.DateTimeFormat.HasFlag(DateTimeFormatFlags.DateAsNumber) ?? false)
        {
            return WriteNumber(32, v.Ticks, formats, formatter.LargeNumberUseString);
        }
        using var chars = RAIIArrayPool<char>.Get(32);
        if (v.TryFormat(chars.Span, out var len, "G"))
        {
            return writer.WriteString(chars.Memory[..len], false);
        }
        else
        {
            return writer.WriteString(v.ToString("G"), false);
        }
    }

    private ValueTask WriteDate(DateOnly v, SeraFormats? formats)
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
        using var chars = RAIIArrayPool<char>.Get(16);
        if (v.TryFormat(chars.Span, out var len, "O"))
        {
            return writer.WriteString(chars.Memory[..len], false);
        }
        else
        {
            return writer.WriteString(v.ToString("O"), false);
        }
    }

    private ValueTask WriteDate(TimeOnly v, SeraFormats? formats)
    {
        if (formats?.DateTimeFormat.HasFlag(DateTimeFormatFlags.DateAsNumber) ?? false)
        {
            return WriteNumber(32, v.Ticks, formats, formatter.LargeNumberUseString);
        }
        using var chars = RAIIArrayPool<char>.Get(32);
        if (v.TryFormat(chars.Span, out var len, "O"))
        {
            return writer.WriteString(chars.Memory[..len], false);
        }
        else
        {
            return writer.WriteString(v.ToString("O"), false);
        }
    }

    private ValueTask WriteDate(DateTime v, SeraFormats? formats)
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
        using var chars = RAIIArrayPool<char>.Get(64);
        if (v.TryFormat(chars.Span, out var len, "O"))
        {
            return writer.WriteString(chars.Memory[..len], false);
        }
        else
        {
            return writer.WriteString(v.ToString("O"), false);
        }
    }

    private ValueTask WriteDate(DateTimeOffset v, SeraFormats? formats)
    {
        if (formats?.DateTimeFormat.HasFlag(DateTimeFormatFlags.DateAsNumber) ?? false)
        {
            return WriteNumber(32, v.Ticks, formats, formatter.LargeNumberUseString);
        }
        if (formats?.DateTimeFormat.HasFlag(DateTimeFormatFlags.DateTimeOffsetUseTimeZone) ?? false)
        {
            v = TimeZoneInfo.ConvertTime(v, Options.TimeZone);
        }
        using var chars = RAIIArrayPool<char>.Get(64);
        if (v.TryFormat(chars.Span, out var len, "O"))
        {
            return writer.WriteString(chars.Memory[..len], false);
        }
        else
        {
            return writer.WriteString(v.ToString("O"), false);
        }
    }

    #endregion

    #region Guid

    private ValueTask WriteGuid(Guid v, SeraFormats? formats)
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
            using var chars = RAIIArrayPool<char>.Get(64);
            if (v.TryFormat(chars.Span, out var len, format))
            {
                return writer.WriteString(chars.Memory[..len], false);
            }
        }
        var str = v.ToString(format);
        return writer.WriteString(str, false);
    }

    #endregion

    #region Char

    private ValueTask WriteChar<T>(T v) where T : ISpanFormattable
    {
        using var chars = RAIIArrayPool<char>.Get(8);
        if (v.TryFormat(chars.Span, out var len, null, null))
        {
            return writer.WriteString(chars.Memory[..len], true);
        }
        else
        {
            return writer.WriteString(v.ToString(null, null), true);
        }
    }

    #endregion

    public override ValueTask VPrimitive(bool value, SeraFormats? formats = null)
    {
        if (formats is { BooleanAsNumber: true }) return VPrimitive(value ? 1 : 0, formats);
        return writer.Write(value ? "true" : "false");
    }

    public override ValueTask VPrimitive(sbyte value, SeraFormats? formats = null)
        => WriteNumber(4, value, formats, false);

    public override ValueTask VPrimitive(byte value, SeraFormats? formats = null)
        => WriteNumber(4, value, formats, false);

    public override ValueTask VPrimitive(short value, SeraFormats? formats = null)
        => WriteNumber(8, value, formats, false);

    public override ValueTask VPrimitive(ushort value, SeraFormats? formats = null)
        => WriteNumber(8, value, formats, false);

    public override ValueTask VPrimitive(int value, SeraFormats? formats = null)
        => WriteNumber(16, value, formats, false);

    public override ValueTask VPrimitive(uint value, SeraFormats? formats = null)
        => WriteNumber(16, value, formats, false);

    public override ValueTask VPrimitive(long value, SeraFormats? formats = null)
        => WriteNumber(32, value, formats, formatter.LargeNumberUseString);

    public override ValueTask VPrimitive(ulong value, SeraFormats? formats = null)
        => WriteNumber(32, value, formats, formatter.LargeNumberUseString);

    public override ValueTask VPrimitive(Int128 value, SeraFormats? formats = null)
        => WriteNumber(64, value, formats, formatter.LargeNumberUseString);

    public override ValueTask VPrimitive(UInt128 value, SeraFormats? formats = null)
        => WriteNumber(64, value, formats, formatter.LargeNumberUseString);

    public override ValueTask VPrimitive(IntPtr value, SeraFormats? formats = null)
        => WriteNumber(32, value, formats, formatter.LargeNumberUseString);

    public override ValueTask VPrimitive(UIntPtr value, SeraFormats? formats = null)
        => WriteNumber(32, value, formats, formatter.LargeNumberUseString);

    public override ValueTask VPrimitive(Half value, SeraFormats? formats = null)
        => WriteNumber(32, value, formats, false);

    public override ValueTask VPrimitive(float value, SeraFormats? formats = null)
        => WriteNumber(32, value, formats, false);

    public override ValueTask VPrimitive(double value, SeraFormats? formats = null)
        => WriteNumber(32, value, formats, false);

    public override ValueTask VPrimitive(decimal value, SeraFormats? formats = null)
        => WriteNumber(32, value, formats, false);

    public override ValueTask VPrimitive(NFloat value, SeraFormats? formats = null)
        => WriteNumber(32, value, formats, false);

    public override ValueTask VPrimitive(BigInteger value, SeraFormats? formats = null)
        => writer.WriteString(value.ToString(), false);

    public override ValueTask VPrimitive(Complex value, SeraFormats? formats = null)
        => writer.WriteString(value.ToString(), false);

    public override ValueTask VPrimitive(TimeSpan value, SeraFormats? formats = null)
        => WriteDate(value, formats);

    public override ValueTask VPrimitive(DateOnly value, SeraFormats? formats = null)
        => WriteDate(value, formats);

    public override ValueTask VPrimitive(TimeOnly value, SeraFormats? formats = null)
        => WriteDate(value, formats);

    public override ValueTask VPrimitive(DateTime value, SeraFormats? formats = null)
        => WriteDate(value, formats);

    public override ValueTask VPrimitive(DateTimeOffset value, SeraFormats? formats = null)
        => WriteDate(value, formats);

    public override ValueTask VPrimitive(Guid value, SeraFormats? formats = null)
        => WriteGuid(value, formats);

    public override ValueTask VPrimitive(Range value, SeraFormats? formats = null)
        => writer.WriteString(value.ToString(), true);

    public override ValueTask VPrimitive(Index value, SeraFormats? formats = null)
        => writer.WriteString(value.ToString(), true);

    public override ValueTask VPrimitive(char value, SeraFormats? formats = null)
        => WriteChar(value);

    public override ValueTask VPrimitive(Rune value, SeraFormats? formats = null)
        => WriteChar(value);

    public override ValueTask VPrimitive(Uri value, SeraFormats? formats = null)
        => writer.WriteString(value.ToString(), true);

    public override ValueTask VPrimitive(Version value, SeraFormats? formats = null)
        => writer.WriteString(value.ToString(), true);

    #endregion

    #region String

    public override ValueTask VString(ReadOnlyMemory<char> value)
        => writer.WriteString(value, true);

    public override ValueTask VString(ReadOnlyMemory<byte> value, Encoding encoding)
        => writer.WriteStringEncoded(value, encoding, true);

    #endregion

    #region Bytes

    public override async ValueTask VBytes(ReadOnlySequence<byte> value)
    {
        if (formatter.Base64Bytes)
        {
            var stream = await writer.StartBase64();
            try
            {
                foreach (var mem in value)
                {
                    await stream.WriteAsync(mem);
                }
            }
            finally
            {
                await writer.EndBase64();
            }
        }
        else
        {
            await VArray(new PrimitiveImpl(), value);
        }
    }

    public override async ValueTask VBytes(ReadOnlyMemory<byte> value)
    {
        if (formatter.Base64Bytes)
        {
            var stream = await writer.StartBase64();
            try
            {
                await stream.WriteAsync(value);
            }
            finally
            {
                await writer.EndBase64();
            }
        }
        else
        {
            await VArray(new PrimitiveImpl(), value);
        }
    }

    #endregion

    #region Array

    public override async ValueTask VArray<V, T>(V vision, ReadOnlySequence<T> value)
    {
        var len = value.Length;
        if (len is 0)
        {
            await writer.Write("[]");
            return;
        }
        var last_state = state;
        state = JsonSerializerState.None;
        await writer.Write("[");
        var isFirst = true;
        foreach (var mem in value)
        {
            foreach (var item in mem.Iter())
            {
                if (isFirst) isFirst = false;
                else await writer.Write(",");
                await vision.Accept<ValueTask, AsyncJsonSerializer>(this, item);
            }
        }
        await writer.Write("]");
        state = last_state;
    }

    public override async ValueTask VArray<V, T>(V vision, ReadOnlyMemory<T> value)
    {
        var len = value.Length;
        if (len is 0)
        {
            await writer.Write("[]");
        }
        var last_state = state;
        state = JsonSerializerState.None;
        await writer.Write("[");
        var isFirst = true;
        foreach (var item in value.Iter())
        {
            if (isFirst) isFirst = false;
            else await writer.Write(",");
            await vision.Accept<ValueTask, AsyncJsonSerializer>(this, item);
        }
        await writer.Write("]");
        state = last_state;
    }

    #endregion

    #region Unit

    public override ValueTask VUnit() => VNone();

    #endregion

    #region Option

    public override ValueTask VNone()
        => writer.Write("null");

    public override ValueTask VSome<V, T>(V vision, T value)
        => vision.Accept<ValueTask, AsyncJsonSerializer>(this, value);

    #endregion

    #region Entry

    public override async ValueTask VEntry<KV, VV, IK, IV>(KV keyVision, VV valueVision, IK key, IV value)
    {
        var last_state = state;
        state = JsonSerializerState.None;
        await writer.Write("[");
        await keyVision.Accept<ValueTask, AsyncJsonSerializer>(this, key);
        await writer.Write(",");
        await valueVision.Accept<ValueTask, AsyncJsonSerializer>(this, value);
        await writer.Write("]");
        state = last_state;
    }

    #endregion

    #region Tuple

    public override async ValueTask VTuple<V, T>(V vision, T value)
    {
        var size = vision.Size;
        if (size == 0)
        {
            await writer.Write("[]");
            return;
        }
        var last_state = state;
        state = JsonSerializerState.None;
        TupleVisitor ??= new(this);
        await writer.Write("[");
        var first = true;
        for (var i = 0; i < size; i++)
        {
            if (first) first = false;
            else await writer.Write(",");
            var err = await vision.AcceptItem<ValueTask<bool>, TupleSeraVisitor>(TupleVisitor, ref value, i);
            if (err) throw new SerializeException($"Unable to get item {i} of tuple {value}");
        }
        await writer.Write("]");
        state = last_state;
    }

    private TupleSeraVisitor? TupleVisitor;

    private class TupleSeraVisitor(AsyncJsonSerializer Base) : ATupleSeraVisitor<ValueTask<bool>>(Base)
    {
        public override async ValueTask<bool> VItem<V, T>(V vision, T value)
        {
            await vision.Accept<ValueTask, AsyncJsonSerializer>(Base, value);
            return false;
        }

        public override ValueTask<bool> VNone() => ValueTask.FromResult(true);
    }

    #endregion

    #region Seq

    public override async ValueTask VSeq<V>(V vision)
    {
        var size = vision.Count;
        if (size is 0)
        {
            await writer.Write("[]");
            return;
        }
        var last_state = state;
        state = JsonSerializerState.None;
        SeqVisitor ??= new(this);
        await writer.Write("[");
        var first = true;
        while (vision.MoveNext())
        {
            if (first) first = false;
            else await writer.Write(",");
            await vision.AcceptNext<ValueTask, SeqSeraVisitor>(SeqVisitor);
        }
        await writer.Write("]");
        state = last_state;
    }

    private SeqSeraVisitor? SeqVisitor;

    private class SeqSeraVisitor(AsyncJsonSerializer Base) : ASeqSeraVisitor<ValueTask>(Base)
    {
        public override ValueTask VItem<T, V>(V vision, T value)
            => vision.Accept<ValueTask, AsyncJsonSerializer>(Base, value);

        public override ValueTask VEnd() => ValueTask.CompletedTask;
    }

    #endregion

    #region Map

    public override async ValueTask VMap<V>(V vision)
    {
        var size = vision.Count;
        if (size is 0)
        {
            await writer.Write("[]");
        }
        var last_state = state;
        state = JsonSerializerState.None;
        SeqMapVisitor ??= new(this);
        await writer.Write("[");
        var first = true;
        while (vision.MoveNext())
        {
            if (first) first = false;
            else await writer.Write(",");
            await vision.AcceptNext<ValueTask, SeqMapSeraVisitor>(SeqMapVisitor);
        }
        await writer.Write("]");
        state = last_state;
    }

    private SeqMapSeraVisitor? SeqMapVisitor;

    private class SeqMapSeraVisitor(AsyncJsonSerializer Base) : AMapSeraVisitor<ValueTask>(Base)
    {
        public override ValueTask VEntry<KV, VV, IK, IV>(KV keyVision, VV valueVision, IK key, IV value)
            => Base.VEntry(keyVision, valueVision, key, value);

        public override ValueTask VEnd() => ValueTask.CompletedTask;
    }

    #endregion

    #region Typed Map

    public override async ValueTask VMap<V, T, IK, IV>(V vision)
    {
        if (typeof(IK) != typeof(string))
        {
            await VMap(vision);
            return;
        }
        var size = vision.Count;
        if (size is 0)
        {
            await writer.Write("{}");
            return;
        }
        var last_state = state;
        state = JsonSerializerState.None;
        MapVisitor ??= new(this);
        await writer.Write("{");
        var first = true;
        while (vision.MoveNext())
        {
            if (first) first = false;
            else await writer.Write(",");
            await vision.AcceptNext<ValueTask, MapSeraVisitor>(MapVisitor);
        }
        await writer.Write("}");
        state = last_state;
    }

    private MapSeraVisitor? MapVisitor;

    private class MapSeraVisitor(AsyncJsonSerializer Base) : AMapSeraVisitor<ValueTask>(Base)
    {
        public override async ValueTask VEntry<KV, VV, IK, IV>(KV keyVision, VV valueVision, IK key, IV value)
        {
            var str = key is null ? "null" : $"{key}";
            await Base.writer.WriteString(str, true);
            await Base.writer.Write(":");
            await valueVision.Accept<ValueTask, AsyncJsonSerializer>(Base, value);
        }

        public override ValueTask VEnd() => ValueTask.CompletedTask;
    }

    #endregion

    #region Struct

    public override async ValueTask VStruct<V, T>(V vision, T value)
    {
        var size = vision.Count;
        if (size is 0)
        {
            await writer.Write("{}");
            return;
        }
        var last_state = state;
        state = JsonSerializerState.None;
        StructVisitor ??= new(this);
        await writer.Write("{");
        var first = true;
        for (var i = 0; i < size; i++)
        {
            if (first) first = false;
            else await writer.Write(",");
            var err = await vision.AcceptField<ValueTask<bool>, StructSeraVisitor>(StructVisitor, ref value, i);
            if (err) throw new SerializeException($"Unable to get field nth {i} of {value}");
        }
        await writer.Write("}");
        state = last_state;
    }

    private StructSeraVisitor? StructVisitor;

    private class StructSeraVisitor(AsyncJsonSerializer Base) : AStructSeraVisitor<ValueTask<bool>>(Base)
    {
        public override async ValueTask<bool> VField<V, T>(V vision, T value, string name, long key)
        {
            await Base.writer.WriteString(name, true);
            await Base.writer.Write(":");
            await vision.Accept<ValueTask, AsyncJsonSerializer>(Base, value);
            return false;
        }

        public override ValueTask<bool> VNone() => ValueTask.FromResult(true);
    }

    #endregion

    #region Union

    public override ValueTask VUnion<V, T>(V vision, T value)
    {
        UnionVisitor ??= new(this);
        return vision.AcceptUnion<ValueTask, UnionSeraVisitor>(UnionVisitor, ref value);
    }

    private UnionSeraVisitor? UnionVisitor;

    private class UnionSeraVisitor(AsyncJsonSerializer Base) : AUnionSeraVisitor<ValueTask>(Base)
    {
        public override ValueTask VEmpty()
            => Base.writer.Write("{}");

        public override ValueTask VNone() => throw new SeraMatchFailureException();

        private async ValueTask VVariant(Variant variant, UnionStyle? union_style, VariantStyle? variant_style,
            bool mustString)
        {
            var priority = variant_style?.Priority ?? union_style?.VariantPriority;
            switch (variant.Kind)
            {
                case VariantKind.NameAndTag:
                    if (mustString) goto case VariantKind.Name;
                    if (priority is VariantPriority.TagFirst) goto case VariantKind.Tag;
                    else goto case VariantKind.Name;
                case VariantKind.Name:
                    await Base.writer.WriteString(variant.Name, true);
                    break;
                case VariantKind.Tag:
                    var tag = variant.Tag;
                    if (mustString)
                    {
                        await Base.writer.WriteString(tag.ToString(), true);
                        return;
                    }
                    var formats = variant_style?.Formats ??
                                  union_style?.VariantFormats ??
                                  Base.formatter.DefaultFormats;
                    switch (tag.Kind)
                    {
                        case VariantTagKind.SByte:
                            await Base.VPrimitive(tag.SByte, formats);
                            break;
                        case VariantTagKind.Byte:
                            await Base.VPrimitive(tag.Byte, formats);
                            break;
                        case VariantTagKind.Int16:
                            await Base.VPrimitive(tag.Int16, formats);
                            break;
                        case VariantTagKind.UInt16:
                            await Base.VPrimitive(tag.UInt16, formats);
                            break;
                        case VariantTagKind.Int32:
                            await Base.VPrimitive(tag.Int32, formats);
                            break;
                        case VariantTagKind.UInt32:
                            await Base.VPrimitive(tag.UInt32, formats);
                            break;
                        case VariantTagKind.Int64:
                            await Base.VPrimitive(tag.Int64, formats);
                            break;
                        case VariantTagKind.UInt64:
                            await Base.VPrimitive(tag.UInt64, formats);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override async ValueTask VVariant(Variant variant,
            UnionStyle? union_style = null, VariantStyle? variant_style = null)
        {
            var s = union_style ?? Base.formatter.DefaultUnionStyle;
            if (s.CompactTag)
            {
                await VVariant(variant, union_style, variant_style, false);
                return;
            }
            var format = s.Format is not UnionFormat.Any ? s.Format : Base.formatter.DefaultUnionFormat;
            switch (format)
            {
                case UnionFormat.Internal:
                    await Base.writer.Write("{");
                    await Base.writer.WriteString(s.InternalTagName, true);
                    await Base.writer.Write(":");
                    await VVariant(variant, union_style, variant_style, false);
                    await Base.writer.Write("}");
                    break;
                case UnionFormat.Adjacent:
                    await Base.writer.Write("{");
                    await Base.writer.WriteString(s.AdjacentTagName, true);
                    await Base.writer.Write(":");
                    await VVariant(variant, union_style, variant_style, false);
                    await Base.writer.Write("}");
                    break;
                case UnionFormat.Tuple:
                    await Base.writer.Write("[");
                    await VVariant(variant, union_style, variant_style, false);
                    await Base.writer.Write("]");
                    break;
                default:
                    await VVariant(variant, union_style, variant_style, false);
                    break;
            }
        }

        public override async ValueTask VVariantValue<V, T>(V vision, T value, Variant variant,
            UnionStyle? union_style = null, VariantStyle? variant_style = null)
        {
            var s = union_style ?? Base.formatter.DefaultUnionStyle;
            var format = s.Format is not UnionFormat.Any ? s.Format : Base.formatter.DefaultUnionFormat;
            switch (format)
            {
                case UnionFormat.External:
                    await Base.writer.Write("{");
                    await VVariant(variant, union_style, variant_style, true);
                    await Base.writer.Write(":");
                    await vision.Accept<ValueTask, AsyncJsonSerializer>(Base, value);
                    await Base.writer.Write("}");
                    break;
                case UnionFormat.Internal:
                    await Base.writer.Write("{");
                    await Base.writer.WriteString(s.InternalTagName, true);
                    await Base.writer.Write(":");
                    await VVariant(variant, union_style, variant_style, false);
                    await Base.writer.Write(",");
                    await Base.writer.WriteString(s.InternalValueName, true);
                    await Base.writer.Write(":");
                    await vision.Accept<ValueTask, AsyncJsonSerializer>(Base, value);
                    await Base.writer.Write("}");
                    break;
                case UnionFormat.Adjacent:
                    await Base.writer.Write("{");
                    await Base.writer.WriteString(s.AdjacentTagName, true);
                    await Base.writer.Write(":");
                    await VVariant(variant, union_style, variant_style, false);
                    await Base.writer.Write(",");
                    await Base.writer.WriteString(s.AdjacentValueName, true);
                    await Base.writer.Write(":");
                    await vision.Accept<ValueTask, AsyncJsonSerializer>(Base, value);
                    await Base.writer.Write("}");
                    break;
                case UnionFormat.Tuple:
                    await Base.writer.Write("[");
                    await VVariant(variant, union_style, variant_style, false);
                    await Base.writer.Write(",");
                    await vision.Accept<ValueTask, AsyncJsonSerializer>(Base, value);
                    await Base.writer.Write("]");
                    break;
                case UnionFormat.Untagged:
                    await vision.Accept<ValueTask, AsyncJsonSerializer>(Base, value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override ValueTask VVariantTuple<V, T>(V vision, T value, Variant variant,
            UnionStyle? union_style = null,
            VariantStyle? variant_style = null)
            => VVariantValue(ByImpls<T>.ByTuple(vision), value, variant, union_style, variant_style);

        public override async ValueTask VVariantStruct<V, T>(V vision, T value, Variant variant,
            UnionStyle? union_style = null, VariantStyle? variant_style = null)
        {
            var s = union_style ?? Base.formatter.DefaultUnionStyle;
            var format = s.Format is not UnionFormat.Any ? s.Format : Base.formatter.DefaultUnionFormat;
            if (format is not UnionFormat.Internal)
            {
                await VVariantValue(ByImpls<T>.ByStruct(vision), value, variant, union_style, variant_style);
                return;
            }
            var size = vision.Count;
            var last_state = Base.state;
            Base.state = JsonSerializerState.None;
            Base.StructVisitor ??= new(Base);
            await Base.writer.Write("{");
            await Base.writer.WriteString(s.InternalTagName, true);
            await Base.writer.Write(":");
            await VVariant(variant, union_style, variant_style, false);
            for (var i = 0; i < size; i++)
            {
                await Base.writer.Write(",");
                var err = await vision.AcceptField<ValueTask<bool>, StructSeraVisitor>(Base.StructVisitor, ref value,
                    i);
                if (err) throw new SerializeException($"Unable to get field nth {i} of {value}");
            }
            await Base.writer.Write("}");
            Base.state = last_state;
        }
    }

    #endregion
}

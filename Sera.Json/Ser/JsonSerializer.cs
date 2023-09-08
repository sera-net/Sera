using System;
using System.Buffers;
using System.Buffers.Text;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using Sera.Core;
using Sera.Core.Ser;

namespace Sera.Json.Ser;

public record JsonSerializer(SeraJsonOptions Options, AJsonWriter Writer) : ISerializer
{
    public string FormatName => "json";
    public string FormatMIME => "application/json";
    public SeraFormatType FormatType => SeraFormatType.HumanReadableText;

    public IRuntimeProvider RuntimeProvider { get; set; } = Options.RuntimeProvider;

    public IAsyncRuntimeProvider AsyncRuntimeProvider { get; set; } = Options.AsyncRuntimeProvider;

    public AJsonFormatter Formatter => Options.Formatter;

    private JsonSerializerState state;

    #region Reference

    public bool MarkReference<T, S>(T obj, S serialize) where T : class where S : ISerialize<T>
    {
        // todo circular reference
        return false;
    }

    #endregion

    #region Primitive

    public void WritePrimitive<T>(T value, SerializerPrimitiveHint? hint)
    {
        switch (value)
        {
            case bool v:
                if (((hint ?? default) & SerializerPrimitiveHint.BooleanAsNumber) != 0)
                {
                    WriteNumber(4, v ? 1 : 0, hint ?? default, false);
                }
                else
                {
                    Writer.Write(v ? "true" : "false");
                }
                break;
            case sbyte v:
                WriteNumber(4, v, hint ?? default, false);
                break;
            case short v:
                WriteNumber(8, v, hint ?? default, false);
                break;
            case int v:
                WriteNumber(16, v, hint ?? default, false);
                break;
            case long v:
                WriteNumber(32, v, hint ?? default, Formatter.LargeNumberUseString);
                break;
            case Int128 v:
                WriteNumber(64, v, hint ?? default, Formatter.LargeNumberUseString);
                break;
            case byte v:
                WriteNumber(4, v, hint ?? default, false);
                break;
            case ushort v:
                WriteNumber(8, v, hint ?? default, false);
                break;
            case uint v:
                WriteNumber(16, v, hint ?? default, false);
                break;
            case ulong v:
                WriteNumber(32, v, hint ?? default, Formatter.LargeNumberUseString);
                break;
            case UInt128 v:
                WriteNumber(64, v, hint ?? default, Formatter.LargeNumberUseString);
                break;
            case nint v:
                WriteNumber(32, v, hint ?? default, Formatter.LargeNumberUseString);
                break;
            case nuint v:
                WriteNumber(32, v, hint ?? default, Formatter.LargeNumberUseString);
                break;
            case Half v:
                WriteNumber(32, v, hint ?? default, false);
                break;
            case float v:
                WriteNumber(32, v, hint ?? default, false);
                break;
            case double v:
                WriteNumber(32, v, hint ?? default, false);
                break;
            case decimal v:
                WriteNumber(32, v, hint ?? default, Formatter.DecimalUseString);
                break;
            case BigInteger v:
                Writer.WriteString(v.ToString(), false);
                break;
            case Complex v:
                Writer.WriteString(v.ToString(), false);
                break;
            case TimeSpan v:
                WriteDate(v, hint ?? default);
                break;
            case DateOnly v:
                WriteDate(v, hint ?? default);
                break;
            case TimeOnly v:
                WriteDate(v, hint ?? default);
                break;
            case DateTime v:
                WriteDate(v, hint ?? default);
                break;
            case DateTimeOffset v:
                WriteDate(v, hint ?? default);
                break;
            case Guid v:
                WriteGuid(v, hint ?? default);
                break;
            case Range v:
                Writer.WriteString(v.ToString(), false);
                break;
            case Index v:
                Writer.WriteString(v.ToString(), false);
                break;
            case char v:
                WriteChar(v);
                break;
            case Rune v:
                WriteChar(v);
                break;
        }
    }

    #region Number

    private void WriteNumber<T>(int size, T v, SerializerPrimitiveHint hint, bool use_string) where T : ISpanFormattable
    {
        Span<char> chars = stackalloc char[size];
        if (v.TryFormat(chars, out var len, null, null))
        {
            var span = chars[..len];
            if (use_string) Writer.WriteString(span, false);
            else Writer.Write(span);
            return;
        }
        var str = v.ToString(null, null);
        if (use_string) Writer.WriteString(str, false);
        else Writer.Write(str);
    }

    #endregion

    #region Date

    private void WriteDate(TimeSpan v, SerializerPrimitiveHint hint)
    {
        if ((hint & SerializerPrimitiveHint.DateAsNumber) != 0)
        {
            WriteNumber(32, v.Ticks, hint, Formatter.LargeNumberUseString);
            return;
        }
        Span<char> chars = stackalloc char[32];
        if (v.TryFormat(chars, out var len, "G", null))
        {
            Writer.WriteString(chars[..len], false);
        }
        else
        {
            Writer.WriteString(v.ToString("G"), false);
        }
    }

    private void WriteDate(DateOnly v, SerializerPrimitiveHint hint)
    {
        if ((hint & SerializerPrimitiveHint.DateOnlyToDateTime) != 0)
        {
            var date_time = v.ToDateTime(TimeOnly.MinValue);
            WriteDate(date_time, hint);
            return;
        }
        if ((hint & SerializerPrimitiveHint.DateOnlyToDateTimeOffset) != 0)
        {
            var date_time_offset =
                new DateTimeOffset(TimeZoneInfo.ConvertTime(v.ToDateTime(TimeOnly.MinValue), Options.TimeZone));
            WriteDate(date_time_offset, hint);
            return;
        }
        if ((hint & SerializerPrimitiveHint.DateAsNumber) != 0)
        {
            WriteNumber(32, v.DayNumber, hint, false);
            return;
        }
        Span<char> chars = stackalloc char[16];
        if (v.TryFormat(chars, out var len, "O", null))
        {
            Writer.WriteString(chars[..len], false);
        }
        else
        {
            Writer.WriteString(v.ToString("O"), false);
        }
    }

    private void WriteDate(TimeOnly v, SerializerPrimitiveHint hint)
    {
        if ((hint & SerializerPrimitiveHint.DateAsNumber) != 0)
        {
            WriteNumber(32, v.Ticks, hint, Formatter.LargeNumberUseString);
            return;
        }
        Span<char> chars = stackalloc char[32];
        if (v.TryFormat(chars, out var len, "O", null))
        {
            Writer.WriteString(chars[..len], false);
        }
        else
        {
            Writer.WriteString(v.ToString("O"), false);
        }
    }

    private void WriteDate(DateTime v, SerializerPrimitiveHint hint)
    {
        v = TimeZoneInfo.ConvertTime(v, Options.TimeZone);
        if ((hint & SerializerPrimitiveHint.DateTimeToDateTimeOffset) != 0)
        {
            var date_time_offset = new DateTimeOffset(v);
            WriteDate(date_time_offset, hint);
            return;
        }
        if ((hint & SerializerPrimitiveHint.DateAsNumber) != 0)
        {
            WriteNumber(32, v.Ticks, hint, Formatter.LargeNumberUseString);
            return;
        }
        Span<char> chars = stackalloc char[64];
        if (v.TryFormat(chars, out var len, "O", null))
        {
            Writer.WriteString(chars[..len], false);
        }
        else
        {
            Writer.WriteString(v.ToString("O"), false);
        }
    }

    private void WriteDate(DateTimeOffset v, SerializerPrimitiveHint hint)
    {
        if ((hint & SerializerPrimitiveHint.DateAsNumber) != 0)
        {
            WriteNumber(32, v.Ticks, hint, Formatter.LargeNumberUseString);
            return;
        }
        if ((hint & SerializerPrimitiveHint.DateTimeOffsetUseTimeZone) != 0)
        {
            v = TimeZoneInfo.ConvertTime(v, Options.TimeZone);
        }
        Span<char> chars = stackalloc char[64];
        if (v.TryFormat(chars, out var len, "O", null))
        {
            Writer.WriteString(chars[..len], false);
        }
        else
        {
            Writer.WriteString(v.ToString("O"), false);
        }
    }

    #endregion

    #region Guid

    private void WriteGuid(Guid v, SerializerPrimitiveHint hint)
    {
        var format = "D";
        if ((hint & SerializerPrimitiveHint.GuidFormatHex) != 0)
        {
            format = "X";
            goto str;
        }

        if ((hint & SerializerPrimitiveHint.GuidFormatShort) != 0)
        {
            format = "N";
        }
        else if ((hint & SerializerPrimitiveHint.GuidFormatBraces) != 0)
        {
            format = "B";
        }
        Span<char> chars = stackalloc char[64];
        if (v.TryFormat(chars, out var len, format))
        {
            Writer.WriteString(chars[..len], false);
            return;
        }
        str:
        var str = v.ToString(format);
        Writer.WriteString(str, false);
    }

    #endregion

    #region Char

    private void WriteChar<T>(T v) where T : ISpanFormattable
    {
        Span<char> chars = stackalloc char[8];
        if (v.TryFormat(chars, out var len, null, null))
        {
            Writer.WriteString(chars[..len], true);
        }
        else
        {
            Writer.WriteString(v.ToString(null, null), true);
        }
    }

    #endregion

    #endregion

    #region String

    public void WriteString(ReadOnlySpan<char> value)
        => Writer.WriteString(value, true);

    public void WriteStringEncoded(ReadOnlySpan<byte> value, Encoding encoding)
        => Writer.WriteStringEncoded(value, encoding, true);

    #endregion

    #region Bytes

    public void WriteBytes(ReadOnlySpan<byte> value)
    {
        if (Formatter.Base64Bytes)
        {
            var init_len = Base64.GetMaxEncodedToUtf8Length(value.Length);
            var buf = ArrayPool<byte>.Shared.Rent(init_len);
            try
            {
                // ReSharper disable once RedundantAssignment
                var r = Base64.EncodeToUtf8(value, buf, out _, out var len);
                Debug.Assert(r == OperationStatus.Done);
                Writer.WriteStringEncoded(buf.AsSpan(0, len), Encoding.UTF8, false);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buf);
            }
        }
        else
        {
            Writer.Write("[");
            var first = true;
            foreach (var v in value)
            {
                if (first) first = false;
                else Writer.Write(",");
                WriteNumber(4, v, default, false);
            }
            Writer.Write("]");
        }
    }

    #endregion

    #region Unit

    public void WriteUnit()
    {
        WriteNone();
    }

    #endregion

    #region Option

    public void WriteNone()
    {
        Writer.Write("null");
    }

    public void WriteSome<T, S>(T value, S serialize) where S : ISerialize<T>
    {
        serialize.Write(this, value, Options);
    }

    #endregion

    #region Seq

    private SeqSerializer? Seq;

    private void CheckWriteElement()
    {
        if (state is JsonSerializerState.None) state = JsonSerializerState.ArrayItem;
        else if (state is JsonSerializerState.ArrayItem) Writer.Write(",");
        else throw new SerializeException("Serializer status error");
    }

    private record SeqSerializer(JsonSerializer self) : ISeqSerializer
    {
        public void WriteElement<T, S>(T value, S serialize) where S : ISerialize<T>
        {
            self.CheckWriteElement();
            serialize.Write(self, value, self.Options);
        }
    }

    public void StartSeq<T, R>(UIntPtr? len, T value, R receiver) where R : ISeqSerializerReceiver<T>
    {
        if (len is 0)
        {
            Writer.Write("[]");
            return;
        }
        var last_state = state;
        state = JsonSerializerState.None;
        Writer.Write("[");
        Seq ??= new(this);
        receiver.Receive(value, Seq);
        Writer.Write("]");
        state = last_state;
    }

    #endregion

    #region Map

    private SeqMapSerializer? SeqMap;
    private KeyStringMapSerializer? KeyStringMap;
    private KeyStringOnlyJsonSerializer? KeyStringOnly;

    private void CheckWriteKey()
    {
        if (state is JsonSerializerState.None) state = JsonSerializerState.ObjectKey;
        else if (state is JsonSerializerState.ObjectValue) Writer.Write(",");
        else throw new SerializeException("Serializer status error", new SerializeException("Wrong WriteKey order"));
    }

    private void CheckWriteValue()
    {
        if (state is JsonSerializerState.ObjectKey) state = JsonSerializerState.ObjectValue;
        else throw new SerializeException("Serializer status error", new SerializeException("Wrong WriteValue order"));
    }

    private void CheckWriteEntry()
    {
        if (state is JsonSerializerState.None) state = JsonSerializerState.ObjectValue;
        else if (state is JsonSerializerState.ObjectValue) Writer.Write(",");
        else throw new SerializeException("Serializer status error");
    }

    private record SeqMapSerializer(JsonSerializer self) : IMapSerializer
    {
        public void WriteKey<K, SK>(K key, SK key_serialize) where SK : ISerialize<K>
        {
            self.CheckWriteKey();
            self.Writer.Write("[");
            key_serialize.Write(self, key, self.Options);
            self.Writer.Write(",");
        }

        public void WriteValue<V, SV>(V value, SV value_serialize) where SV : ISerialize<V>
        {
            self.CheckWriteValue();
            value_serialize.Write(self, value, self.Options);
            self.Writer.Write("]");
        }

        public void WriteEntry<K, V, SK, SV>(K key, V value, SK key_serialize, SV value_serialize)
            where SK : ISerialize<K> where SV : ISerialize<V>
        {
            self.CheckWriteEntry();
            self.Writer.Write("[");
            key_serialize.Write(self, key, self.Options);
            self.Writer.Write(",");
            value_serialize.Write(self, value, self.Options);
            self.Writer.Write("]");
        }
    }

    private record KeyStringMapSerializer(JsonSerializer self) : IMapSerializer
    {
        public void WriteKey<K, SK>(K key, SK key_serialize) where SK : ISerialize<K>
        {
            self.CheckWriteKey();
            self.Writer.Write("");
            self.KeyStringOnly ??= new(self);
            key_serialize.Write(self.KeyStringOnly, key, self.Options);
            self.Writer.Write(":");
        }

        public void WriteValue<V, SV>(V value, SV value_serialize) where SV : ISerialize<V>
        {
            self.CheckWriteValue();
            value_serialize.Write(self, value, self.Options);
        }

        public void WriteEntry<K, V, SK, SV>(K key, V value, SK key_serialize, SV value_serialize)
            where SK : ISerialize<K> where SV : ISerialize<V>
        {
            self.CheckWriteEntry();
            self.KeyStringOnly ??= new(self);
            key_serialize.Write(self.KeyStringOnly, key, self.Options);
            self.Writer.Write(":");
            value_serialize.Write(self, value, self.Options);
        }
    }

    private record KeyStringOnlyJsonSerializer(JsonSerializer self) : ISerializer
    {
        public string FormatName => self.FormatName;
        public string FormatMIME => self.FormatMIME;
        public SeraFormatType FormatType => self.FormatType;

        public IRuntimeProvider RuntimeProvider => self.RuntimeProvider;

        public IAsyncRuntimeProvider AsyncRuntimeProvider => self.AsyncRuntimeProvider;

        public void WriteString(ReadOnlySpan<char> value)
            => self.WriteString(value);

        public void WriteStringEncoded(ReadOnlySpan<byte> value, Encoding encoding)
            => self.WriteStringEncoded(value, encoding);

        private void Throw() => throw new NotSupportedException("key must be a string");

        #region Other

        public bool MarkReference<T, S>(T obj, S serialize) where T : class where S : ISerialize<T>
            => self.MarkReference(obj, serialize);

        public void WriteVariantUnit(string? union_name, Variant variant, SerializerVariantHint? hint)
            => Throw();

        public void WriteVariant<T, S>(string? union_name, Variant variant, T value, S serializer,
            SerializerVariantHint? hint)
            where S : ISerialize<T>
            => Throw();

        public void StartStruct<T, R>(string? name, UIntPtr len, T value, R receiver)
            where R : IStructSerializerReceiver<T>
            => Throw();

        public void StartMap<T, R>(UIntPtr? len, T value, R receiver) where R : IMapSerializerReceiver<T>
            => Throw();

        public void StartSeq<T, R>(UIntPtr? len, T value, R receiver) where R : ISeqSerializerReceiver<T>
            => Throw();

        public void WriteNone()
            => Throw();

        public void WriteSome<T, S>(T value, S serialize) where S : ISerialize<T>
            => Throw();

        public void WriteUnit()
            => Throw();

        public void WriteBytes(ReadOnlySpan<byte> value)
            => Throw();

        public void WritePrimitive<T>(T value, SerializerPrimitiveHint? hint)
            => Throw();

        #endregion
    }

    public void StartMap<T, R>(UIntPtr? len, T value, R receiver) where R : IMapSerializerReceiver<T>
    {
        if (len is 0)
        {
            Writer.Write("[]");
            return;
        }
        var last_state = state;
        state = JsonSerializerState.None;
        Writer.Write("[");
        SeqMap ??= new(this);
        receiver.Receive(value, SeqMap);
        Writer.Write("]");
        state = last_state;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void StartMap<K, V, T, R>(UIntPtr? len, T value, R receiver) where R : IMapSerializerReceiver<T>
    {
        if (typeof(K) != typeof(string)) StartMap(len, value, receiver);
        else StringKeyMap(len, value, receiver);
    }

    private void StringKeyMap<T, R>(UIntPtr? len, T value, R receiver) where R : IMapSerializerReceiver<T>
    {
        if (len is 0)
        {
            Writer.Write("{}");
            return;
        }
        var last_state = state;
        state = JsonSerializerState.None;
        Writer.Write("{");
        KeyStringMap ??= new(this);
        receiver.Receive(value, KeyStringMap);
        Writer.Write("}");
        state = last_state;
    }

    #endregion

    #region Struct

    private StructSerializer? Struct;

    private void CheckWriteField()
    {
        if (state is JsonSerializerState.None) state = JsonSerializerState.Field;
        else if (state is JsonSerializerState.Field) Writer.Write(",");
        else throw new SerializeException("Serializer status error");
    }

    private record StructSerializer(JsonSerializer self) : IStructSerializer
    {
        public void WriteField<T, S>(ReadOnlySpan<char> key, long? int_key, T value, S serializer) where S : ISerialize<T>
        {
            self.CheckWriteField();
            self.Writer.WriteString(key, true);
            self.Writer.Write(":");
            serializer.Write(self, value, self.Options);
        }
    }

    public void StartStruct<T, R>(string? name, UIntPtr len, T value, R receiver) where R : IStructSerializerReceiver<T>
    {
        if (len is 0)
        {
            Writer.Write("{}");
            return;
        }
        var last_state = state;
        state = JsonSerializerState.None;
        Writer.Write("{");
        Struct ??= new(this);
        receiver.Receive(value, Struct);
        Writer.Write("}");
        state = last_state;
    }

    #endregion

    #region Variant

    public void WriteVariantUnit(string? union_name, Variant variant, SerializerVariantHint? hint)
    {
        switch (variant.Kind)
        {
            case VariantKind.NameAndTag:
                if (hint == SerializerVariantHint.UseNumberTag) goto case VariantKind.Tag;
                else goto case VariantKind.Name;
            case VariantKind.Name:
                Writer.WriteString(variant.Name, true);
                break;
            case VariantKind.Tag:
                var tag = variant.Tag;
                switch (tag.Kind)
                {
                    case VariantTagKind.SByte:
                        WriteNumber(4, tag.SByte, default, false);
                        break;
                    case VariantTagKind.Byte:
                        WriteNumber(4, tag.Byte, default, false);
                        break;
                    case VariantTagKind.Int16:
                        WriteNumber(8, tag.Int16, default, false);
                        break;
                    case VariantTagKind.UInt16:
                        WriteNumber(8, tag.UInt16, default, false);
                        break;
                    case VariantTagKind.Int32:
                        WriteNumber(16, tag.Int32, default, false);
                        break;
                    case VariantTagKind.UInt32:
                        WriteNumber(16, tag.UInt32, default, false);
                        break;
                    case VariantTagKind.Int64:
                        WriteNumber(32, tag.Int64, default, Formatter.LargeNumberUseString);
                        break;
                    case VariantTagKind.UInt64:
                        WriteNumber(32, tag.UInt64, default, Formatter.LargeNumberUseString);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void WriteVariant<T, S>(string? union_name, Variant variant, T value, S serializer,
        SerializerVariantHint? hint) where S : ISerialize<T>
    {
        Writer.Write("{");
        switch (variant.Kind)
        {
            case VariantKind.NameAndTag:
                if (hint == SerializerVariantHint.UseNumberTag) goto case VariantKind.Tag;
                else goto case VariantKind.Name;
            case VariantKind.Name:
                Writer.WriteString(variant.Name, true);
                break;
            case VariantKind.Tag:
                var tag = variant.Tag;
                switch (tag.Kind)
                {
                    case VariantTagKind.SByte:
                        WriteNumber(4, tag.SByte, default, true);
                        break;
                    case VariantTagKind.Byte:
                        WriteNumber(4, tag.Byte, default, true);
                        break;
                    case VariantTagKind.Int16:
                        WriteNumber(8, tag.Int16, default, true);
                        break;
                    case VariantTagKind.UInt16:
                        WriteNumber(8, tag.UInt16, default, true);
                        break;
                    case VariantTagKind.Int32:
                        WriteNumber(16, tag.Int32, default, true);
                        break;
                    case VariantTagKind.UInt32:
                        WriteNumber(16, tag.UInt32, default, true);
                        break;
                    case VariantTagKind.Int64:
                        WriteNumber(32, tag.Int64, default, true);
                        break;
                    case VariantTagKind.UInt64:
                        WriteNumber(32, tag.UInt64, default, true);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        Writer.Write(":");
        serializer.Write(this, value, Options);
        Writer.Write("}");
    }

    #endregion
}

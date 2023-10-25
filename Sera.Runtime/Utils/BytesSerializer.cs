using System;
using System.Buffers;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Sera.Core;
using Sera.Core.Ser;
using Sera.Runtime.Emit;

namespace Sera.Runtime.Utils;

internal class BytesSerializer : ISerializer, IDisposable, IAsyncDisposable
{
    public Stream Stream { get; }
    private readonly BinaryWriter Writer;

    public ISeraOptions Options { get; set; }

    public BytesSerializer(Stream stream, ISeraOptions options)
    {
        Stream = stream;
        Writer = new(stream);
        Options = options;
    }

    public string FormatName => "bytes";
    public string FormatMIME => "application/octet-stream";
    public SeraFormatType FormatType => SeraFormatType.Binary;
    public IRuntimeProvider RuntimeProvider => EmitRuntimeProvider.Instance;

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

    public bool MarkReference<T, S>(T obj, S serialize) where T : class where S : ISerialize<T>
    {
        return false;
    }
    
    public void WritePrimitive<T>(T value, SerializerPrimitiveHint? hint)
    {
        switch (value)
        {
            case bool v:
                Writer.Write((byte)((byte)SeraPrimitiveTypes.Boolean << 3 | (byte)TypeToken.Primitive));
                Writer.Write(v);
                break;
            case sbyte v:
                Writer.Write((byte)((byte)SeraPrimitiveTypes.SByte << 3 | (byte)TypeToken.Primitive));
                Writer.Write(v);
                break;
            case short v:
                Writer.Write((byte)((byte)SeraPrimitiveTypes.Int16 << 3 | (byte)TypeToken.Primitive));
                Writer.Write(v);
                break;
            case int v:
                Writer.Write((byte)((byte)SeraPrimitiveTypes.Int32 << 3 | (byte)TypeToken.Primitive));
                Writer.Write(v);
                break;
            case long v:
                Writer.Write((byte)((byte)SeraPrimitiveTypes.Int64 << 3 | (byte)TypeToken.Primitive));
                Writer.Write(v);
                break;
            case Int128 v:
            {
                Writer.Write((byte)((byte)SeraPrimitiveTypes.Int128 << 3 | (byte)TypeToken.Primitive));
                var span = new Span<Int128>(ref v);
                Writer.Write(MemoryMarshal.AsBytes(span));
                break;
            }
            case byte v:
                Writer.Write((byte)((byte)SeraPrimitiveTypes.Byte << 3 | (byte)TypeToken.Primitive));
                Writer.Write(v);
                break;
            case ushort v:
                Writer.Write((byte)((byte)SeraPrimitiveTypes.UInt16 << 3 | (byte)TypeToken.Primitive));
                Writer.Write(v);
                break;
            case uint v:
                Writer.Write((byte)((byte)SeraPrimitiveTypes.UInt32 << 3 | (byte)TypeToken.Primitive));
                Writer.Write(v);
                break;
            case ulong v:
                Writer.Write((byte)((byte)SeraPrimitiveTypes.UInt64 << 3 | (byte)TypeToken.Primitive));
                Writer.Write(v);
                break;
            case UInt128 v:
            {
                Writer.Write((byte)((byte)SeraPrimitiveTypes.UInt128 << 3 | (byte)TypeToken.Primitive));
                var span = new Span<UInt128>(ref v);
                Writer.Write(MemoryMarshal.AsBytes(span));
                break;
            }
            case nint v:
                Writer.Write((byte)((byte)SeraPrimitiveTypes.IntPtr << 3 | (byte)TypeToken.Primitive));
                Writer.Write(v);
                break;
            case nuint v:
                Writer.Write((byte)((byte)SeraPrimitiveTypes.UIntPtr << 3 | (byte)TypeToken.Primitive));
                Writer.Write(v);
                break;
            case Half v:
                Writer.Write((byte)((byte)SeraPrimitiveTypes.Half << 3 | (byte)TypeToken.Primitive));
                Writer.Write(v);
                break;
            case float v:
                Writer.Write((byte)((byte)SeraPrimitiveTypes.Single << 3 | (byte)TypeToken.Primitive));
                Writer.Write(v);
                break;
            case double v:
                Writer.Write((byte)((byte)SeraPrimitiveTypes.Double << 3 | (byte)TypeToken.Primitive));
                Writer.Write(v);
                break;
            case decimal v:
                Writer.Write((byte)((byte)SeraPrimitiveTypes.Decimal << 3 | (byte)TypeToken.Primitive));
                Writer.Write(v);
                break;
            case BigInteger v:
                Writer.Write((byte)((byte)SeraPrimitiveTypes.BigInteger << 3 | (byte)TypeToken.Primitive));
                WriteString(v.ToString());
                break;
            case Complex v:
                Writer.Write((byte)((byte)SeraPrimitiveTypes.Complex << 3 | (byte)TypeToken.Primitive));
                Writer.Write(v.Real);
                Writer.Write(v.Imaginary);
                break;
            case TimeSpan v:
                Writer.Write((byte)((byte)SeraPrimitiveTypes.TimeSpan << 3 | (byte)TypeToken.Primitive));
                Writer.Write(v.Ticks);
                break;
            case DateOnly v:
                Writer.Write((byte)((byte)SeraPrimitiveTypes.DateOnly << 3 | (byte)TypeToken.Primitive));
                Writer.Write(v.DayNumber);
                break;
            case TimeOnly v:
                Writer.Write((byte)((byte)SeraPrimitiveTypes.TimeOnly << 3 | (byte)TypeToken.Primitive));
                Writer.Write(v.Ticks);
                break;
            case DateTime v:
                Writer.Write((byte)((byte)SeraPrimitiveTypes.DateTime << 3 | (byte)TypeToken.Primitive));
                Writer.Write(v.Ticks);
                break;
            case DateTimeOffset v:
                Writer.Write((byte)((byte)SeraPrimitiveTypes.DateTimeOffset << 3 | (byte)TypeToken.Primitive));
                Writer.Write(v.Ticks);
                break;
            case Guid v:
            {
                Writer.Write((byte)((byte)SeraPrimitiveTypes.Guid << 3 | (byte)TypeToken.Primitive));
                Span<byte> bytes = stackalloc byte[sizeof(ulong) * 2];
                v.TryWriteBytes(bytes);
                Writer.Write(bytes);
                break;
            }
            case Range v:
                Writer.Write((byte)((byte)SeraPrimitiveTypes.Range << 3 | (byte)TypeToken.Primitive));
                Writer.Write(v.Start.Value);
                Writer.Write(v.End.Value);
                break;
            case Index v:
                Writer.Write((byte)((byte)SeraPrimitiveTypes.Index << 3 | (byte)TypeToken.Primitive));
                Writer.Write(v.Value);
                break;
            case char v:
                Writer.Write((byte)((byte)SeraPrimitiveTypes.Char << 3 | (byte)TypeToken.Primitive));
                Writer.Write(v);
                break;
            case Rune v:
                Writer.Write((byte)((byte)SeraPrimitiveTypes.Rune << 3 | (byte)TypeToken.Primitive));
                Writer.Write(v.Value);
                break;
            default:
                Writer.Write((byte)TypeToken.Primitive);
                break;
        }
    }

    public void WriteString(ReadOnlySpan<char> value)
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
        Writer.Write(value);
    }
    
    public void WriteStringEncoded(ReadOnlySpan<byte> value, Encoding encoding)
    {
        Writer.Write((byte)((byte)TypeToken.String | (1 << 4)));
        Writer.Write(value.Length);
        Writer.Write(encoding.CodePage);
        Writer.Write(value);
    }
    
    public void WriteBytes(ReadOnlySpan<byte> value)
    {
        Writer.Write((byte)TypeToken.Bytes);
        Writer.Write(value.Length);
        Writer.Write(value);
    }

    public void WriteBytes(ReadOnlySequence<byte> value)
    {
        var length = value.Length;
        Writer.Write((byte)((byte)TypeToken.Bytes | (1 << 3)));
        Writer.Write(length);
        foreach (var mem in value)
        {
            Writer.Write(mem.Span);
        }
    }

    public void WriteArray<T, S>(ReadOnlySpan<T> value, S serialize) where S : ISerialize<T>
    {
        Writer.Write((byte)((byte)TypeToken.Seq | (byte)SplitToken.Start << 3));
        var first = true;
        foreach (var item in value)
        {
            if (first) first = false;
            else Writer.Write((byte)((byte)TypeToken.Seq | (byte)SplitToken.Split << 3));
            serialize.Write(this, item, Options);
        }
        Writer.Write((byte)((byte)TypeToken.Seq | (byte)SplitToken.End << 3));
    }

    public void WriteArray<T, S>(ReadOnlySequence<T> value, S serialize) where S : ISerialize<T>
    {
        Writer.Write((byte)((byte)TypeToken.Seq | (byte)SplitToken.Start << 3));
        bool first = true;
        foreach (var mem in value)
        {
            foreach (var item in mem.Span)
            {
                if (first) first = false;
                else Writer.Write((byte)((byte)TypeToken.Seq | (byte)SplitToken.Split << 3));
                serialize.Write(this, item, Options);
            }
        }
        Writer.Write((byte)((byte)TypeToken.Seq | (byte)SplitToken.End << 3));
    }

    public void WriteUnit()
    {
        WriteNone();
    }

    public void WriteNone()
    {
        Writer.Write((byte)TypeToken.None);
    }

    public void WriteSome<T, S>(T value, S serialize) where S : ISerialize<T>
    {
        Writer.Write((byte)TypeToken.Some);
        serialize.Write(this, value, Options);
    }

    public void StartSeq<T, R>(UIntPtr? len, T value, R receiver) where R : ISeqSerializerReceiver<T>
    {
        Writer.Write((byte)((byte)TypeToken.Seq | (byte)SplitToken.Start << 3));
        receiver.Receive(value, new SeqSerializer(this));
        Writer.Write((byte)((byte)TypeToken.Seq | (byte)SplitToken.End << 3));
    }

    private record SeqSerializer(BytesSerializer self) : ISeqSerializer
    {
        private bool first = true;

        public void WriteElement<T, S>(T value, S serialize) where S : ISerialize<T>
        {
            if (first) first = false;
            else self.Writer.Write((byte)((byte)TypeToken.Seq | (byte)SplitToken.Split << 3));
            serialize.Write(self, value, self.Options);
        }
    }

    public void StartMap<T, R>(UIntPtr? len, T value, R receiver) where R : IMapSerializerReceiver<T>
    {
        Writer.Write((byte)((byte)TypeToken.Map | (byte)SplitToken.Start << 3));
        receiver.Receive(value, new MapSerializer(this));
        Writer.Write((byte)((byte)TypeToken.Map | (byte)SplitToken.End << 3));
    }

    private record MapSerializer(BytesSerializer self) : IMapSerializer
    {
        private bool first = true;

        public void WriteKey<K, SK>(K key, SK key_serialize) where SK : ISerialize<K>
        {
            if (first) first = false;
            else self.Writer.Write((byte)((byte)TypeToken.Map | (byte)SplitToken.Split << 3));
            key_serialize.Write(self, key, self.Options);
            self.Writer.Write((byte)((byte)TypeToken.Map | (byte)SplitToken.Mid << 3));
        }

        public void WriteValue<V, SV>(V value, SV value_serialize) where SV : ISerialize<V>
        {
            value_serialize.Write(self, value, self.Options);
        }

        public void WriteEntry<K, V, SK, SV>(K key, V value, SK key_serialize, SV value_serialize)
            where SK : ISerialize<K> where SV : ISerialize<V>
        {
            if (first) first = false;
            else self.Writer.Write((byte)((byte)TypeToken.Map | (byte)SplitToken.Split << 3));
            key_serialize.Write(self, key, self.Options);
            self.Writer.Write((byte)((byte)TypeToken.Map | (byte)SplitToken.Mid << 3));
            value_serialize.Write(self, value, self.Options);
        }
    }

    public void StartStruct<T, R>(string? name, UIntPtr len, T value, R receiver) where R : IStructSerializerReceiver<T>
    {
        Writer.Write((byte)((byte)TypeToken.Map | (byte)SplitToken.Start << 3));
        receiver.Receive(value, new StructSerializer(this));
        Writer.Write((byte)((byte)TypeToken.Map | (byte)SplitToken.End << 3));
    }

    private record StructSerializer(BytesSerializer self) : IStructSerializer
    {
        private bool first = true;

        public void WriteField<T, S>(ReadOnlySpan<char> key, long? int_key, T value, S serialize)
            where S : ISerialize<T>
        {
            if (first) first = false;
            else self.Writer.Write((byte)((byte)TypeToken.Map | (byte)SplitToken.Split << 3));
            self.Writer.Write(key);
            self.Writer.Write((byte)((byte)TypeToken.Map | (byte)SplitToken.Mid << 3));
            serialize.Write(self, value, self.Options);
        }

        public void WriteField<T, S>(string key, long? int_key, T value, S serialize) where S : ISerialize<T>
            => WriteField(key.AsMemory(), int_key, value, serialize);

        public void WriteField<T, S>(ReadOnlyMemory<char> key, long? int_key, T value, S serialize)
            where S : ISerialize<T>
            => WriteField(key.Span, int_key, value, serialize);
    }

    public void WriteEmptyUnion(string? union_name)
    {
        Writer.Write((byte)((byte)TypeToken.Variant | (byte)SplitToken.Start << 3));
        WriteUnit();
        Writer.Write((byte)((byte)TypeToken.Variant | (byte)SplitToken.Start << 3));
    }

    private void WriteVariant(Variant variant)
    {
        switch (variant.Kind)
        {
            case VariantKind.Name:
                Writer.Write(variant.Name);
                break;
            case VariantKind.NameAndTag:
                Writer.Write(variant.Name);
                goto case VariantKind.Tag;
            case VariantKind.Tag:
                switch (variant.Tag.Kind)
                {
                    case VariantTagKind.SByte:
                        Writer.Write(variant.Tag.SByte);
                        break;
                    case VariantTagKind.Byte:
                        Writer.Write(variant.Tag.Byte);
                        break;
                    case VariantTagKind.Int16:
                        Writer.Write(variant.Tag.Int16);
                        break;
                    case VariantTagKind.UInt16:
                        Writer.Write(variant.Tag.UInt16);
                        break;
                    case VariantTagKind.Int32:
                        Writer.Write(variant.Tag.Int32);
                        break;
                    case VariantTagKind.UInt32:
                        Writer.Write(variant.Tag.UInt32);
                        break;
                    case VariantTagKind.Int64:
                        Writer.Write(variant.Tag.Int64);
                        break;
                    case VariantTagKind.UInt64:
                        Writer.Write(variant.Tag.UInt64);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void WriteVariantUnit(string? union_name, Variant variant, SerializerVariantHint? hint)
    {
        Writer.Write((byte)((byte)TypeToken.Variant | (byte)SplitToken.Start << 3));
        WriteVariant(variant);
        Writer.Write((byte)((byte)TypeToken.Variant | (byte)SplitToken.Mid << 3));
        WriteUnit();
        Writer.Write((byte)((byte)TypeToken.Variant | (byte)SplitToken.Start << 3));
    }

    public void WriteVariant<T, S>(string? union_name, Variant variant, T value, S serializer,
        SerializerVariantHint? hint) where S : ISerialize<T>
    {
        Writer.Write((byte)((byte)TypeToken.Variant | (byte)SplitToken.Start << 3));
        WriteVariant(variant);
        Writer.Write((byte)((byte)TypeToken.Variant | (byte)SplitToken.Mid << 3));
        serializer.Write(this, value, Options);
        Writer.Write((byte)((byte)TypeToken.Variant | (byte)SplitToken.Start << 3));
    }

    public void Dispose()
    {
        Writer.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await Writer.DisposeAsync();
    }
}

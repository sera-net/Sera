using System;
using System.Buffers;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Sera.Json.Ser;

public record StreamJsonWriter
    (AJsonFormatter Formatter, Stream Stream) : AJsonWriter(Formatter)
{
    public override void WriteShortAscii(ReadOnlySpan<char> str)
    {
        var encode = Formatter.Encoding;
        if (encode.Equals(Encoding.Unicode))
        {
            Stream.Write(MemoryMarshal.AsBytes(str));
        }
        else
        {
            Span<byte> buf = stackalloc byte[Encoding.GetMaxByteCount(str.Length)];
            var count = encode.GetBytes(str, buf);
            var span = buf.Slice(0, count);
            Stream.Write(span);
        }
    }

    public override void Write(ReadOnlySpan<char> str)
    {
        var encode = Formatter.Encoding;
        if (encode.Equals(Encoding.Unicode))
        {
            Stream.Write(MemoryMarshal.AsBytes(str));
        }
        else
        {
            var len = encode.GetMaxByteCount(str.Length);
            var buf = ArrayPool<byte>.Shared.Rent(len);
            try
            {
                var count = encode.GetBytes(str, buf);
                var span = buf.AsSpan(0, count);
                Stream.Write(span);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buf);
            }
        }
    }

    public override void WriteEncoded(ReadOnlySpan<byte> str, Encoding encoding)
    {
        var encode = Formatter.Encoding;
        if (encode.Equals(encoding))
        {
            Stream.Write(str);
        }
        else
        {
            var char_count = encode.GetMaxCharCount(str.Length);
            var chars = ArrayPool<char>.Shared.Rent(char_count);
            try
            {
                var count = encode.GetChars(str, chars);
                var span = chars.AsSpan(0, count);
                Write(span);
            }
            finally
            {
                ArrayPool<char>.Shared.Return(chars);
            }
        }
    }
}

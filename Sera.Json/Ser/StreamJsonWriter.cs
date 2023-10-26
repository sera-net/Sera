using System;
using System.Buffers;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Sera.Json.Builders;
using Sera.Json.Builders.Ser;
using Sera.Utils;

namespace Sera.Json.Ser;

public class StreamJsonWriter
    (SeraJsonOptions Options, AJsonFormatter Formatter, Stream Stream) : AJsonWriter(Options, Formatter)
{
    public static StreamJsonWriter Create(Builder<ToStream> self) =>
        new(self.Options, self.Formatter, self.Value.Stream);

    private Stream? tmpStream;

    public override Stream StartBase64()
    {
        Write("\"");
        if (Equals(Encoding, Encoding.UTF8))
        {
            return tmpStream = new CryptoStream(Stream, new ToBase64Transform(), CryptoStreamMode.Write, true);
        }
        else
        {
            var code_stream = Encoding.CreateTranscodingStream(Stream, Encoding, Encoding.UTF8, true);
            return tmpStream = new CryptoStream(code_stream, new ToBase64Transform(), CryptoStreamMode.Write);
        }
    }

    public override void EndBase64()
    {
        tmpStream!.Flush();
        tmpStream.Dispose();
        tmpStream = null;
        Write("\"");
    }

    public override void Write(ReadOnlySpan<char> str)
    {
        var encode = Options.Encoding;
        if (encode.Equals(Encoding.Unicode))
        {
            Stream.Write(MemoryMarshal.AsBytes(str));
        }
        else
        {
            var len = encode.GetMaxByteCount(str.Length);
            if (len <= 256)
            {
                Span<byte> buf = stackalloc byte[len];
                var count = encode.GetBytes(str, buf);
                var span = buf[..count];
                Stream.Write(span);
            }
            else
            {
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
    }

    public override void WriteEncoded(ReadOnlySpan<byte> str, Encoding encoding)
    {
        var encode = Options.Encoding;
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

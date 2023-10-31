using System;
using System.Buffers;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace Sera.Json.Ser;

public class StringBuilderJsonWriter
    (SeraJsonOptions Options, AJsonFormatter Formatter, StringBuilder Builder) : AJsonWriter(Options, Formatter)
{
    private MemoryStream? tmp_mem;
    private static Stream? tmpStream;

    public override void Flush() { }

    public override Stream StartBase64()
    {
        Write("\"");
        tmp_mem = new MemoryStream();
        return tmpStream = new CryptoStream(tmp_mem, new ToBase64Transform(), CryptoStreamMode.Write, true);
    }

    public override void EndBase64()
    {
        tmpStream!.Flush();
        tmpStream.Dispose();
        tmpStream = null;
        tmp_mem!.Position = 0;
        var str = new StreamReader(tmp_mem, Encoding.UTF8).ReadToEnd();
        Builder.Append(str);
        tmp_mem.Dispose();
        tmp_mem = null;
        Write("\"");
    }

    public override void Write(ReadOnlySpan<char> str)
    {
        Builder.Append(str);
    }

    public override void WriteEncoded(ReadOnlySpan<byte> str, Encoding encoding)
    {
        if (encoding.Equals(Encoding.Unicode))
        {
            Write(MemoryMarshal.Cast<byte, char>(str));
        }
        else
        {
            var len = encoding.GetMaxCharCount(str.Length);
            if (len <= 128)
            {
                Span<char> buf = stackalloc char[len];
                var count = encoding.GetChars(str, buf);
                var span = buf[..count];
                Builder.Append(span);
            }
            else
            {
                var buf = ArrayPool<char>.Shared.Rent(len);
                try
                {
                    var count = encoding.GetChars(str, buf);
                    var span = buf.AsSpan(0, count);
                    Builder.Append(span);
                }
                finally
                {
                    ArrayPool<char>.Shared.Return(buf);
                }
            }
        }
    }
}

using System;
using System.Buffers;
using System.Runtime.InteropServices;
using System.Text;

namespace Sera.Json.Ser;

public record StringBuilderJsonWriter(AJsonFormatter Formatter, StringBuilder Builder) : AJsonWriter(Formatter)
{
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

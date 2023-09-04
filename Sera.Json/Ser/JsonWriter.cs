using System;
using System.Buffers;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using Sera.Json.Utils;

namespace Sera.Json.Ser;

public abstract record AJsonWriter(AJsonFormatter Formatter)
{
    public Encoding Encoding => Formatter.Encoding;

    public abstract void WriteShortAscii(ReadOnlySpan<char> str);

    public abstract void Write(ReadOnlySpan<char> str);
    public abstract void WriteEncoded(ReadOnlySpan<byte> str, Encoding encoding);

    public virtual void WriteString(ReadOnlySpan<char> str, bool escape)
    {
        WriteShortAscii("\"");
        if (escape) WriteEscape(str);
        else Write(str);
        WriteShortAscii("\"");
    }

    public virtual void WriteStringEncoded(ReadOnlySpan<byte> str, Encoding encoding, bool escape)
    {
        WriteShortAscii("\"");
        if (escape) WriteEscapeEncoded(str, encoding);
        else WriteEncoded(str, encoding);
        WriteShortAscii("\"");
    }

    protected virtual void WriteEscapeHex(Rune rune)
    {
        Span<char> span = stackalloc char[6];
        span[0] = '\\';
        span[1] = 'u';
        var hex = span[2..];
        var r = rune.Value.TryFormat(hex, out var len, "X4");
        if (!r || len != 4) throw new ArgumentOutOfRangeException($"{rune}");
        Write(span);
    }

    protected virtual void WriteEscape(ReadOnlySpan<char> str)
    {
        for (var n = 0;;)
        {
            var span = str[n..];
            if (span.IsEmpty)
            {
                if (n > 0) Write(str[..n]);
                return;
            }
            var r = Rune.DecodeFromUtf16(span, out var rune, out var len);
            if (r != OperationStatus.Done) throw new FormatException();
            if (rune.IsAscii && EscapeTable.TryGet(span[0], out var esc))
            {
                if (n > 0) Write(str[..n]);
                Write("\\");
                Write(new ReadOnlySpan<char>(in esc));
                str = str[(n + len)..];
                n = 0;
            }
            else if (
                Formatter.EscapeAllNonAsciiChar && !rune.IsAscii ||
                Rune.GetUnicodeCategory(rune) is
                    UnicodeCategory.Control or
                    UnicodeCategory.Format or
                    UnicodeCategory.LineSeparator or
                    UnicodeCategory.OtherNotAssigned or
                    UnicodeCategory.PrivateUse
            )
            {
                if (n > 0) Write(str[..n]);
                WriteEscapeHex(rune);
                str = str[(n + len)..];
                n = 0;
            }
            else
            {
                n += len;
            }
        }
    }

    protected virtual void WriteEscapeUtf8(ReadOnlySpan<byte> str)
    {
        for (var n = 0;;)
        {
            var span = str[n..];
            if (span.IsEmpty)
            {
                if (n > 0) WriteEncoded(str[..n], Encoding.UTF8);
                return;
            }
            var r = Rune.DecodeFromUtf8(span, out var rune, out var len);
            if (r != OperationStatus.Done) throw new FormatException();
            if (rune.IsAscii && EscapeTable.TryGet((char)span[0], out var esc))
            {
                if (n > 0) WriteEncoded(str[..n], Encoding.UTF8);
                Write("\\");
                Write(new ReadOnlySpan<char>(in esc));
                str = str[(n + len)..];
                n = 0;
            }
            else if (
                Formatter.EscapeAllNonAsciiChar && !rune.IsAscii ||
                Rune.GetUnicodeCategory(rune) is
                    UnicodeCategory.Control or
                    UnicodeCategory.Format or
                    UnicodeCategory.LineSeparator or
                    UnicodeCategory.OtherNotAssigned or
                    UnicodeCategory.PrivateUse
            )
            {
                if (n > 0) WriteEncoded(str[..n], Encoding.UTF8);
                WriteEscapeHex(rune);
                str = str[(n + len)..];
                n = 0;
            }
            else
            {
                n += len;
            }
        }
    }

    protected virtual void WriteEscapeEncoded(ReadOnlySpan<byte> str, Encoding encoding)
    {
        if (encoding.Equals(Encoding.Unicode)) WriteEscape(MemoryMarshal.Cast<byte, char>(str));
        if (encoding.Equals(Encoding.UTF8)) WriteEscapeUtf8(str);
        else
        {
            var encode = Formatter.Encoding;
            var char_count = encode.GetMaxCharCount(str.Length);
            var chars = ArrayPool<char>.Shared.Rent(char_count);
            try
            {
                var count = encode.GetChars(str, chars);
                var span = chars.AsSpan(0, count);
                WriteEscape(span);
            }
            finally
            {
                ArrayPool<char>.Shared.Return(chars);
            }
        }
    }
   
}

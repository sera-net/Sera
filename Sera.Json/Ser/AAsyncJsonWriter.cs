using System;
using System.Buffers;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Sera.Json.Utils;

namespace Sera.Json.Ser;

public abstract class AAsyncJsonWriter(SeraJsonOptions options, AJsonFormatter formatter)
{
    public SeraJsonOptions Options { get; } = options;
    public AJsonFormatter Formatter { get; } = formatter;
    public Encoding Encoding => options.Encoding;

    public abstract ValueTask<Stream> StartBase64();
    public abstract ValueTask EndBase64();
    public abstract ValueTask Write(ReadOnlyMemory<char> str);
    public virtual ValueTask Write(string str) => Write(str.AsMemory());
    public abstract ValueTask WriteEncoded(ReadOnlyMemory<byte> str, Encoding encoding);

    public virtual async ValueTask WriteString(ReadOnlyMemory<char> str, bool escape)
    {
        await Write("\"");
        if (escape) await WriteEscape(str);
        else await Write(str);
        await Write("\"");
    }

    public virtual async ValueTask WriteStringEncoded(ReadOnlyMemory<byte> str, Encoding encoding, bool escape)
    {
        await Write("\"");
        if (escape) await WriteEscapeEncoded(str, encoding);
        else await WriteEncoded(str, encoding);
        await Write("\"");
    }

    [ThreadStatic]
    private static char[]? Chars_2;
    [ThreadStatic]
    private static char[]? Chars_12;

    private static char[] GetChars2() => Chars_2 ??= new char[2];
    private static char[] GetChars12() => Chars_12 ??= new char[12];

    protected virtual async ValueTask WriteEscapeHex(Rune rune)
    {
        var chars = GetChars2();
        var char_count = rune.EncodeToUtf16(chars);


        if (char_count == 1)
        {
            var buf = new Memory<char>(GetChars12(), 0, 6);
            buf.Span[0] = '\\';
            buf.Span[1] = 'u';
            var hex = buf[2..];
            var c = (ushort)chars[0];
            var r = c.TryFormat(hex.Span, out var len, "X4");
            if (!r || len != 4) throw new ArgumentOutOfRangeException($"{rune}");
            await Write(buf);
        }
        else if (char_count == 2)
        {
            var buf = new Memory<char>(GetChars12(), 0, 12);
            buf.Span[0] = '\\';
            buf.Span[1] = 'u';
            buf.Span[6] = '\\';
            buf.Span[7] = 'u';
            var hex = buf[2..];
            var c1 = (ushort)chars[0];
            var r = c1.TryFormat(hex.Span, out var len, "X4");
            if (!r || len != 4) throw new ArgumentOutOfRangeException($"{rune}");
            hex = buf[8..];
            var c2 = (ushort)chars[1];
            r = c2.TryFormat(hex.Span, out len, "X4");
            if (!r || len != 4) throw new ArgumentOutOfRangeException($"{rune}");
            await Write(buf);
        }
        else throw new ArgumentOutOfRangeException($"{rune}");
    }

    protected virtual async ValueTask WriteEscape(ReadOnlyMemory<char> str)
    {
        for (var n = 0;;)
        {
            var buf = str[n..];
            if (buf.IsEmpty)
            {
                if (n > 0) await Write(str[..n]);
                return;
            }
            var r = Rune.DecodeFromUtf16(buf.Span, out var rune, out var len);
            if (r != OperationStatus.Done) throw new FormatException();
            if (rune.IsAscii && EscapeTable.TryGet(buf.Span[0], out var esc))
            {
                if (n > 0) await Write(str[..n]);
                await Write("\\");
                var chars = GetChars2();
                chars[0] = esc;
                await Write(chars.AsMemory(0, 1));
                str = str[(n + len)..];
                n = 0;
            }
            else if (
                (formatter.EscapeAllNonAsciiChar && !rune.IsAscii) ||
                Rune.GetUnicodeCategory(rune) is
                    UnicodeCategory.Control or
                    UnicodeCategory.Format or
                    UnicodeCategory.LineSeparator or
                    UnicodeCategory.OtherNotAssigned or
                    UnicodeCategory.PrivateUse
            )
            {
                if (n > 0) await Write(str[..n]);
                await WriteEscapeHex(rune);
                str = str[(n + len)..];
                n = 0;
            }
            else
            {
                n += len;
            }
        }
    }

    protected virtual async ValueTask WriteEscapeUtf8(ReadOnlyMemory<byte> str)
    {
        for (var n = 0;;)
        {
            var buf = str[n..];
            if (buf.IsEmpty)
            {
                if (n > 0) await WriteEncoded(str[..n], Encoding.UTF8);
                return;
            }
            var r = Rune.DecodeFromUtf8(buf.Span, out var rune, out var len);
            if (r != OperationStatus.Done) throw new FormatException();
            if (rune.IsAscii && EscapeTable.TryGet((char)buf.Span[0], out var esc))
            {
                if (n > 0) await WriteEncoded(str[..n], Encoding.UTF8);
                await Write("\\");
                var chars = GetChars2();
                chars[0] = esc;
                await Write(chars.AsMemory(0, 1));
                str = str[(n + len)..];
                n = 0;
            }
            else if (
                (formatter.EscapeAllNonAsciiChar && !rune.IsAscii) ||
                Rune.GetUnicodeCategory(rune) is
                    UnicodeCategory.Control or
                    UnicodeCategory.Format or
                    UnicodeCategory.LineSeparator or
                    UnicodeCategory.OtherNotAssigned or
                    UnicodeCategory.PrivateUse
            )
            {
                if (n > 0) await WriteEncoded(str[..n], Encoding.UTF8);
                await WriteEscapeHex(rune);
                str = str[(n + len)..];
                n = 0;
            }
            else
            {
                n += len;
            }
        }
    }

    protected virtual async ValueTask WriteEscapeEncoded(ReadOnlyMemory<byte> str, Encoding encoding)
    {
        if (encoding.Equals(Encoding.UTF8)) await WriteEscapeUtf8(str);
        else
        {
            var encode = options.Encoding;
            var char_count = encode.GetMaxCharCount(str.Length);
            var chars = ArrayPool<char>.Shared.Rent(char_count);
            try
            {
                var count = encode.GetChars(str.Span, chars.AsSpan());
                var buf = chars.AsMemory(0, count);
                await WriteEscape(buf);
            }
            finally
            {
                ArrayPool<char>.Shared.Return(chars);
            }
        }
    }
}

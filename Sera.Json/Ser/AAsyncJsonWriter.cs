using System;
using System.Buffers;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Sera.Json.Utils;
using Sera.Utils;

namespace Sera.Json.Ser;

public abstract class AAsyncJsonWriter(SeraJsonOptions options, AJsonFormatter formatter)
{
    public SeraJsonOptions Options { get; } = options;
    public AJsonFormatter Formatter { get; } = formatter;
    public Encoding Encoding => Options.Encoding;

    public abstract ValueTask<Stream> StartBase64();
    public abstract ValueTask EndBase64();
    public abstract ValueTask Write(ReadOnlyMemory<char> str);
    public virtual ValueTask Write(string str) => Write(str.AsMemory());
    public abstract ValueTask WriteEncoded(ReadOnlyMemory<byte> str, Encoding encoding);

    public virtual ValueTask WriteString(string str, bool escape)
        => WriteString(str.AsMemory(), escape);
    
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
    
    protected virtual async ValueTask WriteEscapeHex(Rune rune)
    {
        using var chars = RAIIArrayPool<char>.Get(2);
        var char_count = rune.EncodeToUtf16(chars.Span);


        if (char_count == 1)
        {
            using var buf = RAIIArrayPool<char>.Get(6);
            buf[0] = '\\';
            buf[1] = 'u';
            var hex = buf.Memory[2..];
            var c = (ushort)chars[0];
            var r = c.TryFormat(hex.Span, out var len, "X4");
            if (!r || len != 4) throw new ArgumentOutOfRangeException($"{rune}");
            await Write(buf.Memory);
        }
        else if (char_count == 2)
        {
            using var buf = RAIIArrayPool<char>.Get(12);
            buf[0] = '\\';
            buf[1] = 'u';
            buf[6] = '\\';
            buf[7] = 'u';
            var hex = buf.Memory[2..];
            var c1 = (ushort)chars[0];
            var r = c1.TryFormat(hex.Span, out var len, "X4");
            if (!r || len != 4) throw new ArgumentOutOfRangeException($"{rune}");
            hex = buf.Memory[8..];
            var c2 = (ushort)chars[1];
            r = c2.TryFormat(hex.Span, out len, "X4");
            if (!r || len != 4) throw new ArgumentOutOfRangeException($"{rune}");
            await Write(buf.Memory);
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
                using var chars = RAIIArrayPool<char>.Get(1);
                chars[0] = esc;
                await Write(chars.Memory);
                str = str[(n + len)..];
                n = 0;
            }
            else if (
                (Formatter.EscapeAllNonAsciiChar && !rune.IsAscii) ||
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
                using var chars = RAIIArrayPool<char>.Get(1);
                chars[0] = esc;
                await Write(chars.Memory);
                str = str[(n + len)..];
                n = 0;
            }
            else if (
                (Formatter.EscapeAllNonAsciiChar && !rune.IsAscii) ||
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
            var encode = Options.Encoding;
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

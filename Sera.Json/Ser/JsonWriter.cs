using System;
using System.Buffers;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Sera.Json.Utils;
using Sera.Utils;

namespace Sera.Json.Ser;

public abstract record AJsonWriter(SeraJsonOptions Options, AJsonFormatter Formatter)
{
    public Encoding Encoding => Options.Encoding;

    public abstract Stream StartBase64();
    public abstract void EndBase64();
    public abstract void Write(ReadOnlySpan<char> str);
    public abstract void WriteEncoded(ReadOnlySpan<byte> str, Encoding encoding);
    public abstract void WriteEncoded(ReadOnlySequence<byte> str, Encoding encoding);

    public virtual void WriteString(ReadOnlySpan<char> str, bool escape)
    {
        Write("\"");
        if (escape) WriteEscape(str);
        else Write(str);
        Write("\"");
    }

    public virtual void WriteString(ReadOnlySequence<char> str, bool escape)
    {
        Write("\"");
        if (escape)
        {
            using var stream = new ReadOnlySequenceStream<char>(str);
            using var code_stream = Encoding.CreateTranscodingStream(stream, Encoding.Unicode, Encoding.UTF8, true);
            WriteEscapeUtf8(code_stream);
        }
        else
        {
            // safe because char
            foreach (var mem in str)
            {
                Write(mem.Span);
            }
        }
        Write("\"");
    }

    public virtual void WriteStringEncoded(ReadOnlySpan<byte> str, Encoding encoding, bool escape)
    {
        Write("\"");
        if (escape) WriteEscapeEncoded(str, encoding);
        else WriteEncoded(str, encoding);
        Write("\"");
    }

    public virtual void WriteStringEncoded(ReadOnlySequence<byte> str, Encoding encoding, bool escape)
    {
        Write("\"");
        if (escape) WriteEscapeEncoded(str, encoding);
        else WriteEncoded(str, encoding);
        Write("\"");
    }

    protected virtual void WriteEscapeHex(Rune rune)
    {
        Span<char> chars = stackalloc char[2];
        var char_count = rune.EncodeToUtf16(chars);

        if (char_count == 1)
        {
            Span<char> span = stackalloc char[6];
            span[0] = '\\';
            span[1] = 'u';
            var hex = span[2..];
            var c = (ushort)chars[0];
            var r = c.TryFormat(hex, out var len, "X4");
            if (!r || len != 4) throw new ArgumentOutOfRangeException($"{rune}");
            Write(span);
        }
        else if (char_count == 2)
        {
            Span<char> span = stackalloc char[12];
            span[0] = '\\';
            span[1] = 'u';
            span[6] = '\\';
            span[7] = 'u';
            var hex = span[2..];
            var c1 = (ushort)chars[0];
            var r = c1.TryFormat(hex, out var len, "X4");
            if (!r || len != 4) throw new ArgumentOutOfRangeException($"{rune}");
            hex = span[8..];
            var c2 = (ushort)chars[1];
            r = c2.TryFormat(hex, out len, "X4");
            if (!r || len != 4) throw new ArgumentOutOfRangeException($"{rune}");
            Write(span);
        }
        else throw new ArgumentOutOfRangeException($"{rune}");
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
                Write(new ReadOnlySpan<char>(ref esc));
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
                Write(new ReadOnlySpan<char>(ref esc));
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

    /// <summary>Stream must be utf8</summary>
    protected virtual void WriteEscapeUtf8(Stream str)
    {
        Span<byte> bytes = stackalloc byte[4];

        str.ReadExactly(MemoryMarshal.AsBytes(bytes[..1]));
        
        var byte0 = bytes[0];
        if (byte0 >> 3 == 0b11110) str.ReadExactly(MemoryMarshal.AsBytes(bytes.Slice(1, 3)));
        else if (byte0 >> 4 == 0b1110) str.ReadExactly(MemoryMarshal.AsBytes(bytes.Slice(1, 2)));
        else if (byte0 >> 5 == 0b110) str.ReadExactly(MemoryMarshal.AsBytes(bytes.Slice(1, 1)));

        var r = Rune.DecodeFromUtf8(bytes, out var rune, out _);
        if (r != OperationStatus.Done) throw new FormatException();
        if (rune.IsAscii && EscapeTable.TryGet((char)bytes[0], out var esc))
        {
            Write("\\");
            Write(new ReadOnlySpan<char>(ref esc));
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
            WriteEscapeHex(rune);
        }
        else
        {
            Span<char> chars = stackalloc char[2];
            var n = rune.EncodeToUtf16(chars);
            Write(chars[..n]);
        }
    }

    protected virtual void WriteEscapeEncoded(ReadOnlySpan<byte> str, Encoding encoding)
    {
        if (encoding.Equals(Encoding.Unicode)) WriteEscape(MemoryMarshal.Cast<byte, char>(str));
        if (encoding.Equals(Encoding.UTF8)) WriteEscapeUtf8(str);
        else
        {
            var encode = Options.Encoding;
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

    protected virtual void WriteEscapeEncoded(ReadOnlySequence<byte> str, Encoding encoding)
    {
        var stream = new ReadOnlySequenceStream<byte>(str);
        if (encoding.Equals(Encoding.UTF8)) WriteEscapeUtf8(stream);
        else
        {
            using var code_stream = Encoding.CreateTranscodingStream(stream, encoding, Encoding.UTF8, true);
            WriteEscapeUtf8(code_stream);
        }
    }
}

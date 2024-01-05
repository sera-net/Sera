using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using Sera.Json.Utils;
using Sera.Utils;

namespace Sera.Json.De;

public readonly struct StringJsonReader : IJsonReaderState<StringJsonReader>
{
    #region Create

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static JsonReader<StringJsonReader> Create(SeraJsonOptions options, CompoundString source)
        => new(options, new(source));

    #endregion

    #region Seek

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public StringJsonReader Save() => this;

    #endregion

    #region Fields

    private readonly CompoundString source;
    private readonly ExternalSpan last;

    #endregion

    #region Ctor

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public StringJsonReader(CompoundString source)
    {
        this.source = source;
        last = ExternalSpan.From(source.AsSpan());
        this = MoveNext();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private StringJsonReader(CompoundString source, ExternalSpan last, bool currentHas, JsonToken currentToken,
        SourcePos sourcePos)
    {
        this.source = source;
        this.last = last;
        CurrentHas = currentHas;
        CurrentToken = currentToken;
        SourcePos = sourcePos;
    }

    #endregion

    #region Iter

    public bool CurrentHas
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
    }
    public JsonToken CurrentToken
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
    }
    public SourcePos SourcePos
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public StringJsonReader MoveNext()
        => new Moving(source, last, SourcePos).MoveNext(CurrentToken);

    private struct Moving(CompoundString source, ExternalSpan last, SourcePos pos)
    {
        private readonly CompoundString source = source;
        private ExternalSpan last = last;
        private SourcePos pos = pos;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public StringJsonReader MoveNext(JsonToken lastToken)
        {
            re_try:
            var span = last.GetSpan(source.AsSpan());
            if (span.IsEmpty) return new(source, last, false, lastToken, pos);

            var first = span[0];
            switch (first)
            {
                case '\n':
                    FoundLine(1);
                    goto re_try;
                case '\r':
                {
                    if (span.Length > 1 && span[1] is '\n') FoundLine(2);
                    else FoundLine(1);
                    goto re_try;
                }
                case '\x20' or '\x09':
                {
                    FoundSpace(span.CountLeadingSpace());
                    goto re_try;
                }
                case ',':
                    return Found(JsonTokenKind.Comma, 1);
                case ':':
                    return Found(JsonTokenKind.Colon, 1);
                case '[':
                    return Found(JsonTokenKind.ArrayStart, 1);
                case ']':
                    return Found(JsonTokenKind.ArrayEnd, 1);
                case '{':
                    return Found(JsonTokenKind.ObjectStart, 1);
                case '}':
                    return Found(JsonTokenKind.ObjectEnd, 1);
                case 'n' when span.Length < 4:
                    throw new JsonParseException($"Expected 'null' but found '{span.ToString()}' at {pos}", pos);
                case 'n' when span[1] != 'u' || span[2] != 'l' || span[3] != 'l':
                    throw new JsonParseException($"Expected 'null' but found '{span[..4].ToString()}' at {pos}", pos);
                case 'n':
                    return Found(JsonTokenKind.Null, 4);
                case 't' when span.Length < 4:
                    throw new JsonParseException($"Expected 'true' but found '{span.ToString()}' at {pos}", pos);
                case 't' when span[1] != 'r' || span[2] != 'u' || span[3] != 'e':
                    throw new JsonParseException($"Expected 'true' but found '{span[..4].ToString()}' at {pos}", pos);
                case 't':
                    return Found(JsonTokenKind.True, 4);
                case 'f' when span.Length < 5:
                    throw new JsonParseException($"Expected 'false' but found '{span.ToString()}' at {pos}", pos);
                case 'f' when span[1] != 'a' || span[2] != 'l' || span[3] != 's' || span[4] != 'e':
                    throw new JsonParseException($"Expected 'false' but found '{span[..5].ToString()}' at {pos}", pos);
                case 'f':
                    return Found(JsonTokenKind.False, 5);
                case '-':
                    return FoundNumberNeg(span, 1);
                case '0':
                    return FoundNumberZero(span, 1);
                case '1' or '2' or '3' or '4' or '5' or '6' or '7' or '8' or '9':
                    return FoundNumberDigit(span, 1);
                case '"':
                    return FoundString(span, 1);
            }
            throw new JsonParseException($"Unexpected character at {pos}", pos);
        }


        #region Number

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private StringJsonReader FoundNumberNeg(ReadOnlySpan<char> span, int offset)
        {
            if (offset >= span.Length) goto err;
            var c = span[offset];
            switch (c)
            {
                case '0':
                    return FoundNumberZero(span, offset + 1);
                case '1' or '2' or '3' or '4' or '5' or '6' or '7' or '8' or '9':
                    return FoundNumberDigit(span, offset + 1);
            }
            err:
            var err_pos = pos.AddChar(offset);
            throw new JsonParseException($"No number after minus sign at {err_pos}", err_pos);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private StringJsonReader FoundNumberZero(ReadOnlySpan<char> span, int offset)
        {
            var c = span[offset];
            switch (c)
            {
                case '.':
                    return FoundNumberFraction(span, offset + 1);
                case 'e' or 'E':
                    return FoundNumberExponentE(span, offset + 1);
            }
            return Found(JsonTokenKind.Number, offset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private StringJsonReader FoundNumberDigit(ReadOnlySpan<char> span, int offset)
        {
            while (offset < span.Length)
            {
                var c = span[offset];
                switch (c)
                {
                    case '0' or '1' or '2' or '3' or '4' or '5' or '6' or '7' or '8' or '9':
                        offset++;
                        continue;
                    case '.':
                        return FoundNumberFraction(span, offset + 1);
                    case 'e' or 'E':
                        return FoundNumberExponentE(span, offset + 1);
                }
                break;
            }
            return Found(JsonTokenKind.Number, offset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private StringJsonReader FoundNumberFraction(ReadOnlySpan<char> span, int offset)
        {
            while (offset < span.Length)
            {
                var c = span[offset];
                switch (c)
                {
                    case '0' or '1' or '2' or '3' or '4' or '5' or '6' or '7' or '8' or '9':
                        offset++;
                        continue;
                    case 'e' or 'E':
                        return FoundNumberExponentE(span, offset + 1);
                }
                break;
            }
            return Found(JsonTokenKind.Number, offset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private StringJsonReader FoundNumberExponentE(ReadOnlySpan<char> span, int offset)
        {
            if (offset >= span.Length) goto err;
            var c = span[offset];
            switch (c)
            {
                case '-' or '+':
                    return FoundNumberExponentSign(span, offset + 1);
                case '0' or '1' or '2' or '3' or '4' or '5' or '6' or '7' or '8' or '9':
                    return FoundNumberExponentDigit(span, offset + 1);
            }
            err:
            var err_pos = pos.AddChar(offset);
            throw new JsonParseException($"Exponent part is missing a number at {err_pos}", err_pos);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private StringJsonReader FoundNumberExponentSign(ReadOnlySpan<char> span, int offset)
        {
            if (offset >= span.Length) goto err;
            var c = span[offset];
            if (c is '0' or '1' or '2' or '3' or '4' or '5' or '6' or '7' or '8' or '9')
                return FoundNumberExponentDigit(span, offset + 1);
            err:
            var err_pos = pos.AddChar(offset);
            throw new JsonParseException($"Exponent part is missing a number at {err_pos}", err_pos);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private StringJsonReader FoundNumberExponentDigit(ReadOnlySpan<char> span, int offset)
        {
            while (offset < span.Length)
            {
                var c = span[offset];
                switch (c)
                {
                    case '0' or '1' or '2' or '3' or '4' or '5' or '6' or '7' or '8' or '9':
                        offset++;
                        continue;
                }
                break;
            }
            return Found(JsonTokenKind.Number, offset);
        }

        #endregion

        #region String

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private StringJsonReader FoundString(ReadOnlySpan<char> span, int offset)
        {
            var content_len = span[offset..].CountLeadingStringContent();
            var new_offset = offset + content_len;
            if (new_offset == span.Length)
            {
                var err_pos = pos.AddChar(new_offset);
                throw new JsonParseException($"Unterminated string at {err_pos}", err_pos);
            }
            var len = new_offset + 1;
            var c = span[new_offset];
            if (char.IsControl(c))
            {
                var err_pos = pos.AddChar(new_offset);
                throw new JsonParseException($"Bad control character in string literal at {err_pos}", err_pos);
            }
            switch (c)
            {
                case '"':
                    var token = new JsonToken(JsonTokenKind.String, pos, source.Slice(last.Slice(1, content_len)));
                    MovePos(len);
                    last = last[len..];
                    return new(source, last, true, token, pos);
                case '\\':
                    return FoundStringWithEscape(span, span.Slice(offset, content_len), len);
            }
            throw new Exception("never");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private StringJsonReader FoundStringWithEscape(
            ReadOnlySpan<char> span, ReadOnlySpan<char> first_content, int offset)
        {
            var sb = new StringBuilder();
            sb.Append(first_content);

            #region Escape

            found_escape:
            {
                var c = span[offset];
                switch (c)
                {
                    case '"' or '\\' or '/':
                        sb.Append(c);
                        break;
                    case 'b':
                        sb.Append('\b');
                        break;
                    case 'f':
                        sb.Append('\f');
                        break;
                    case 'n':
                        sb.Append('\n');
                        break;
                    case 'r':
                        sb.Append('\r');
                        break;
                    case 't':
                        sb.Append('\t');
                        break;
                    case 'u':
                    {
                        var last_span = span[(offset + 1)..];
                        if (last_span.Length < 4)
                        {
                            var err_pos = pos.AddChar(offset);
                            throw new JsonParseException($"Hex escape length is insufficient at {err_pos}", err_pos);
                        }
                        var hex = last_span[..4];
                        try
                        {
                            var un_escape = (char)ushort.Parse(hex, NumberStyles.HexNumber);
                            sb.Append(un_escape);
                        }
                        catch (Exception e)
                        {
                            var err_pos = pos.AddChar(offset);
                            throw new JsonParseException($"Illegal escape at {err_pos}", err_pos, e);
                        }
                        offset += 5;
                        goto next_content;
                    }
                    default:
                    {
                        var err_pos = pos.AddChar(offset);
                        throw new JsonParseException($"Illegal escape at {err_pos}", err_pos);
                    }
                }

                offset++;
            }

            #endregion

            #region Content

            next_content:
            {
                var content_len = span[offset..].CountLeadingStringContent();
                var new_offset = offset + content_len;
                if (new_offset == span.Length)
                {
                    var err_pos = pos.AddChar(new_offset);
                    throw new JsonParseException($"Unterminated string at {err_pos}", err_pos);
                }
                var len = new_offset + 1;
                var c = span[new_offset];
                if (char.IsControl(c))
                {
                    var err_pos = pos.AddChar(new_offset);
                    throw new JsonParseException($"Bad control character in string literal at {err_pos}", err_pos);
                }
                switch (c)
                {
                    case '"':
                        sb.Append(span.Slice(offset, content_len));
                        var token = new JsonToken(JsonTokenKind.String, pos, sb.ToString());
                        MovePos(len);
                        last = last[len..];
                        return new(source, last, true, token, pos);
                    case '\\':
                        offset = len;
                        goto found_escape;
                }
                throw new Exception("never");
            }

            #endregion
        }

        #endregion

        #region Found

        [UnscopedRef]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private StringJsonReader Found(JsonTokenKind kind, int len)
        {
            var token = new JsonToken(kind, pos, source.Slice(last[..len]));
            MovePos(len);
            last = last[len..];
            return new(source, last, true, token, pos);
        }

        [UnscopedRef]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void FoundSpace(int len)
        {
            MovePos(len);
            last = last[len..];
        }

        [UnscopedRef]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void FoundLine(int len)
        {
            MovePosToNextLine(len);
            last = last[len..];
        }

        #endregion

        #region MovePos

        [UnscopedRef]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void MovePos(int len) => pos.MutAddChar(len);

        [UnscopedRef]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void MovePosToNextLine(int len) => pos.MutAddLine(len);

        #endregion
    }

    #endregion
}

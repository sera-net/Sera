using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using BetterCollections;
using BetterCollections.Buffers;
using Sera.Utils;
using Sera.Json.Utils;

namespace Sera.Json.De;

public sealed class StreamJsonReader : AJsonReader, IDisposable
{
    #region Fields

    private readonly StreamReader reader;

    private const int BufferInitialSize = 128;
    private const int DiscardThreshold = 512;

    #endregion

    #region Create

    private StreamJsonReader(SeraJsonOptions options, Stream stream) : base(options)
    {
        reader = new(stream, options.Encoding, leaveOpen: true);
    }

    /// <summary>
    /// Does not require ownership of stream, stream's dispose will not be called
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StreamJsonReader Create(SeraJsonOptions options, Stream stream)
    {
        var r = new StreamJsonReader(options, stream);
        r.MoveNext(true);
        return r;
    }

    #endregion

    #region Iter

    private char[] buffer = Array.Empty<char>();
    private ExternalSpan range = new(0, 0);
    private ReadOnlySpan<char> span
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => range.GetSpan(buffer.AsSpan());
    }

    private bool IsEnd;
    private bool IsOnSave
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => SavedTokens != null && savedOffset < SavedTokens.Count;
    }
    public override bool CurrentHas
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => IsOnSave || currentHas;
    }
    public override JsonToken CurrentToken
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => IsOnSave ? SavedTokens![savedOffset] : currentToken;
    }
    public override SourcePos SourcePos
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => IsOnSave ? SavedTokens![savedOffset].Pos : pos;
    }
    private bool currentHas
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set;
    }
    private JsonToken currentToken
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set;
    }
    private SourcePos pos;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void MoveNext()
    {
        if (IsOnSave)
        {
            savedOffset++;
            Version++;
            if (!IsOnSave)
            {
                if (!HasSaves) ClearSaves();
                MoveNext(false);
                Version++;
            }
        }
        else
        {
            if (!CurrentHas) return;
            MoveNext(false);
            Version++;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void MoveNext(bool firstMove)
    {
        var span = this.span;
        re_try:
        if (span.IsEmpty) ReRead(ref span);
        if (span.IsEmpty)
        {
            currentHas = false;
            return;
        }

        if (!firstMove) TryDiscard(ref span);

        var first = span[0];
        switch (first)
        {
            case '\n':
                FoundLine(1);
                MoveRange(ref span, 1);
                goto re_try;
            case '\r':
            {
                MoveRange(ref span, 1);
                if (span.IsEmpty) ReRead(ref span);
                if (span[0] is '\n')
                {
                    FoundLine(2);
                    MoveRange(ref span, 1);
                }
                else FoundLine(1);
                goto re_try;
            }
            case '\x20' or '\x09':
            {
                var count = span.CountLeadingSpace();
                var total = count;
                Debug.Assert(count > 0);
                if (span.Length == count)
                {
                    if (firstMove) ClearDiscard(ref span);
                    else DoDiscard(ref span);
                    count = span.CountLeadingSpace();
                    total += count;
                    while (count != 0 && span.Length == count)
                    {
                        ClearDiscard(ref span);
                        count = span.CountLeadingSpace();
                        total += count;
                    }
                }
                FoundSpace(total);
                MoveRange(ref span, count);
                goto re_try;
            }
            case ',':
                Found(JsonTokenKind.Comma, 1);
                MoveRange(ref span, 1);
                return;
            case ':':
                Found(JsonTokenKind.Colon, 1);
                MoveRange(ref span, 1);
                return;
            case '[':
                Found(JsonTokenKind.ArrayStart, 1);
                MoveRange(ref span, 1);
                return;
            case ']':
                Found(JsonTokenKind.ArrayEnd, 1);
                MoveRange(ref span, 1);
                return;
            case '{':
                Found(JsonTokenKind.ObjectStart, 1);
                MoveRange(ref span, 1);
                return;
            case '}':
                Found(JsonTokenKind.ObjectEnd, 1);
                MoveRange(ref span, 1);
                return;
            case 'n':
                if (span.Length < 4) ReRead(ref span);
                if (span.Length < 4)
                    throw new JsonParseException($"Expected 'null' but found '{span.ToString()}' at {pos}", pos);
                if (span is not [_, 'u', 'l', 'l', ..])
                    throw new JsonParseException($"Expected 'null' but found '{span[..4].ToString()}' at {pos}", pos);
                Found(JsonTokenKind.Null, 4);
                MoveRange(ref span, 4);
                return;
            case 't':
                if (span.Length < 4) ReRead(ref span);
                if (span.Length < 4)
                    throw new JsonParseException($"Expected 'true' but found '{span.ToString()}' at {pos}", pos);
                if (span is not [_, 'r', 'u', 'e', ..])
                    throw new JsonParseException($"Expected 'true' but found '{span[..4].ToString()}' at {pos}", pos);
                Found(JsonTokenKind.True, 4);
                MoveRange(ref span, 4);
                return;
            case 'f':
                if (span.Length < 5) ReRead(ref span);
                if (span.Length < 5)
                    throw new JsonParseException($"Expected 'false' but found '{span.ToString()}' at {pos}", pos);
                if (span is not [_, 'a', 'l', 's', 'e', ..])
                    throw new JsonParseException($"Expected 'false' but found '{span[..4].ToString()}' at {pos}", pos);
                Found(JsonTokenKind.False, 5);
                MoveRange(ref span, 5);
                return;
            case '-':
                FoundNumberNeg(span, 1);
                return;
            case '0':
                FoundNumberZero(span, 1);
                return;
            case '1' or '2' or '3' or '4' or '5' or '6' or '7' or '8' or '9':
                FoundNumberDigit(span, 1);
                return;
            case '"':
                FoundString(span, 1);
                return;
        }
        throw new JsonParseException($"Unexpected character at {pos}", pos);
    }

    #region Read

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void DoRead(bool discard)
    {
        if (IsEnd) return;
        if (discard)
        {
            var count = reader.ReadBlock(buffer.AsSpan());
            if (count != buffer.Length) IsEnd = true;
            range = new(Offset: 0, Length: count);
        }
        else
        {
            var new_buffer = new char[buffer.Length == 0 ? BufferInitialSize : buffer.Length * 2];
            if (buffer.Length != 0) buffer.CopyTo(new_buffer.AsSpan());
            var count = reader.ReadBlock(new_buffer.AsSpan(range.Offset + range.Length));
            if (count != new_buffer.Length - buffer.Length) IsEnd = true;
            buffer = new_buffer;
            range = range with { Length = range.Length + count };
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ReRead(ref ReadOnlySpan<char> span, bool discard = false)
    {
        if (IsEnd) return;
        DoRead(discard);
        span = this.span;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void TryDiscard(ref ReadOnlySpan<char> span)
    {
        if (buffer.Length < DiscardThreshold) return;
        DoDiscard(ref span);
    }

    // ReSharper disable once RedundantAssignment
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void DoDiscard(ref ReadOnlySpan<char> span)
    {
        if (range != new ExternalSpan(0, 0))
        {
            buffer = new char[BufferInitialSize];
            range = new(0, 0);
        }
        ReRead(ref span, discard: true);
    }

    // ReSharper disable once RedundantAssignment
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ClearDiscard(ref ReadOnlySpan<char> span)
    {
        range = new(0, 0);
        span = ReadOnlySpan<char>.Empty;
        ReRead(ref span, discard: true);
    }

    #endregion

    #region Number

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void FoundNumberNeg(ReadOnlySpan<char> span, int offset)
    {
        if (offset >= span.Length) ReRead(ref span);
        if (offset >= span.Length) goto err;
        var c = span[offset];
        switch (c)
        {
            case '0':
                FoundNumberZero(span, offset + 1);
                return;
            case '1' or '2' or '3' or '4' or '5' or '6' or '7' or '8' or '9':
                FoundNumberDigit(span, offset + 1);
                return;
        }
        err:
        var err_pos = pos.AddChar(offset);
        throw new JsonParseException($"No number after minus sign at {err_pos}", err_pos);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void FoundNumberZero(ReadOnlySpan<char> span, int offset)
    {
        var c = span[offset];
        switch (c)
        {
            case '.':
                FoundNumberFraction(span, offset + 1);
                return;
            case 'e' or 'E':
                FoundNumberExponentE(span, offset + 1);
                return;
        }
        Found(JsonTokenKind.Number, offset);
        MoveRange(ref span, offset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void FoundNumberDigit(ReadOnlySpan<char> span, int offset)
    {
        if (offset >= span.Length) ReRead(ref span);
        var count = span[offset..].CountLeadingNumberDigitBody();
        var new_offset = offset + count;
        while (count != 0 && new_offset >= span.Length)
        {
            ReRead(ref span);
            count = span[new_offset..].CountLeadingNumberDigitBody();
            new_offset += count;
        }
        if (new_offset >= span.Length) ReRead(ref span);
        if (new_offset < span.Length)
        {
            var c = span[new_offset];
            switch (c)
            {
                case '.':
                    FoundNumberFraction(span, new_offset + 1);
                    return;
                case 'e' or 'E':
                    FoundNumberExponentE(span, new_offset + 1);
                    return;
            }
        }
        Found(JsonTokenKind.Number, new_offset);
        MoveRange(ref span, new_offset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void FoundNumberFraction(ReadOnlySpan<char> span, int offset)
    {
        if (offset >= span.Length) ReRead(ref span);
        var count = span[offset..].CountLeadingNumberDigitBody();
        var new_offset = offset + count;
        while (count != 0 && new_offset >= span.Length)
        {
            ReRead(ref span);
            count = span[new_offset..].CountLeadingNumberDigitBody();
            new_offset += count;
        }
        if (new_offset >= span.Length) ReRead(ref span);
        if (new_offset < span.Length)
        {
            var c = span[new_offset];
            if (c is 'e' or 'E')
            {
                FoundNumberExponentE(span, new_offset + 1);
                return;
            }
        }
        Found(JsonTokenKind.Number, new_offset);
        MoveRange(ref span, new_offset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void FoundNumberExponentE(ReadOnlySpan<char> span, int offset)
    {
        if (offset >= span.Length) ReRead(ref span);
        if (offset >= span.Length) goto err;
        var c = span[offset];
        switch (c)
        {
            case '-' or '+':
                FoundNumberExponentSign(span, offset + 1);
                return;
            case '0' or '1' or '2' or '3' or '4' or '5' or '6' or '7' or '8' or '9':
                FoundNumberExponentDigit(span, offset + 1);
                return;
        }
        err:
        var err_pos = pos.AddChar(offset);
        throw new JsonParseException($"Exponent part is missing a number at {err_pos}", err_pos);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void FoundNumberExponentSign(ReadOnlySpan<char> span, int offset)
    {
        if (offset >= span.Length) ReRead(ref span);
        if (offset >= span.Length) goto err;
        var c = span[offset];
        if (c is '0' or '1' or '2' or '3' or '4' or '5' or '6' or '7' or '8' or '9')
        {
            FoundNumberExponentDigit(span, offset + 1);
            return;
        }
        err:
        var err_pos = pos.AddChar(offset);
        throw new JsonParseException($"Exponent part is missing a number at {err_pos}", err_pos);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void FoundNumberExponentDigit(ReadOnlySpan<char> span, int offset)
    {
        if (offset >= span.Length) ReRead(ref span);
        var count = span[offset..].CountLeadingNumberDigitBody();
        var new_offset = offset + count;
        while (count != 0 && new_offset >= span.Length)
        {
            ReRead(ref span);
            count = span[new_offset..].CountLeadingNumberDigitBody();
            new_offset += count;
        }
        Found(JsonTokenKind.Number, new_offset);
        MoveRange(ref span, new_offset);
    }

    #endregion

    #region String

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void FoundString(ReadOnlySpan<char> span, int offset)
    {
        var content_len_count = span[offset..].CountLeadingStringContent();
        var content_len_total = content_len_count;
        var new_offset = content_len_total + offset;
        while (new_offset == span.Length)
        {
            ReRead(ref span);
            if (new_offset == span.Length)
            {
                var err_pos = pos.AddChar(new_offset);
                throw new JsonParseException($"Unterminated string at {err_pos}", err_pos);
            }
            content_len_count = span[new_offset..].CountLeadingStringContent();
            content_len_total += content_len_count;
            new_offset = content_len_total + offset;
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
                var token = new JsonToken(JsonTokenKind.String, pos,
                    CompoundString.MakeMemory(buffer).Slice(range.Slice(1, content_len_total)), StringCache);
                MovePos(len);
                MoveRange(ref span, len);
                currentHas = true;
                currentToken = token;
                SavedTokens?.Add(token);
                return;
            case '\\':
                FoundStringWithEscape(span, span.Slice(offset, content_len_total), len);
                return;
        }
        throw new Exception("never");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void FoundStringWithEscape(ReadOnlySpan<char> span, ReadOnlySpan<char> first_content, int offset)
    {
        var sb = new StringBuilder();
        sb.Append(first_content);

        #region Escape

        found_escape:
        {
            if (offset == span.Length)
            {
                var err_pos = pos.AddChar(offset);
                throw new JsonParseException($"Unterminated string at {err_pos}", err_pos);
            }
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
                    if (span.Length < offset + 5) ReRead(ref span);
                    if (span.Length < offset + 5)
                    {
                        var err_pos = pos.AddChar(offset);
                        throw new JsonParseException($"Hex escape length is insufficient at {err_pos}", err_pos);
                    }
                    var hex = span[(offset + 1)..(offset + 5)];
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
            var content_len_count = span[offset..].CountLeadingStringContent();
            var content_len_total = content_len_count;
            var new_offset = content_len_total + offset;
            while (new_offset == span.Length)
            {
                ReRead(ref span);
                if (new_offset == span.Length)
                {
                    var err_pos = pos.AddChar(new_offset);
                    throw new JsonParseException($"Unterminated string at {err_pos}", err_pos);
                }
                content_len_count = span[new_offset..].CountLeadingStringContent();
                content_len_total += content_len_count;
                new_offset = content_len_total + offset;
            }
            var len = new_offset + 1;
            var c = span[new_offset];
            if (char.IsControl(c))
            {
                var err_pos = pos.AddChar(new_offset);
                throw new JsonParseException($"Bad control character in string literal at {err_pos}", err_pos);
            }
            sb.Append(span.Slice(offset, content_len_total));
            switch (c)
            {
                case '"':
                    var token = new JsonToken(JsonTokenKind.String, pos, sb.ToString(), StringCache);
                    MovePos(len);
                    MoveRange(ref span, len);
                    currentHas = true;
                    currentToken = token;
                    SavedTokens?.Add(token);
                    return;
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Found(JsonTokenKind kind, int len)
    {
        var token = new JsonToken(kind, pos, CompoundString.MakeMemory(buffer).Slice(range[..len]), StringCache);
        MovePos(len);
        currentHas = true;
        currentToken = token;
        SavedTokens?.Add(token);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void FoundSpace(int len)
    {
        MovePos(len);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void FoundLine(int len)
    {
        MovePosToNextLine(len);
    }

    #endregion

    #region MovePos

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void MoveRange(ref ReadOnlySpan<char> span, int len)
    {
        range = range[len..];
        span = span[len..];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void MovePos(int len) => pos.MutAddChar(len);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void MovePosToNextLine(int len) => pos.MutAddLine(len);

    #endregion

    #endregion

    #region Seek

    private HashSet<int>? saves;
    private Vec<JsonToken>? SavedTokens;
    private int savedOffset;

    private bool HasSaves
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => saves != null && saves!.Count > 0;
    }

    private void ClearSaves()
    {
        if (SavedTokens != null)
        {
            SavedTokens!.Dispose();
            SavedTokens = null;
            savedOffset = 0;
        }
    }

    public override long Save()
    {
        saves ??= new();
        if (SavedTokens == null)
        {
            SavedTokens = new Vec<JsonToken>(ArrayPoolFactory.Shared);
            savedOffset = 0;
            SavedTokens.Add(CurrentToken);
        }
        saves.Add(savedOffset);
        return savedOffset;
    }

    public override void Load(long savePoint)
    {
        Debug.Assert(SavedTokens != null && savePoint < SavedTokens.Count);
        savedOffset = (int)savePoint;
    }

    public override void UnSave(long savePoint)
    {
        saves!.Remove((int)savePoint);
    }

    #endregion

    #region Dispose

    public void Dispose()
    {
        SavedTokens?.Dispose();
    }

    #endregion
}

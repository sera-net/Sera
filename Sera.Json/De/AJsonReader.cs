using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using Sera.Json.Utils;
using Sera.Utils;

namespace Sera.Json.De;

public abstract class AJsonReader(SeraJsonOptions options)
{
    public SeraJsonOptions Options
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
    } = options;
    public Encoding Encoding
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Options.Encoding;
    }

    protected SourcePos pos;

    #region Seek

    /// <summary>
    /// Can use <see cref="Save"/> <see cref="Load"/>
    /// </summary>
    public abstract bool CanSeek
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
    }

    /// <summary>Returns a save point that can be loaded</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public abstract long Save();

    /// <summary>Load save point</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public abstract void Load(long pos);

    /// <summary>Delete save point</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public abstract void UnSave(long pos);

    #endregion

    #region Cursor

    public abstract ulong Cursor
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual void AssertMove(ulong cursor)
    {
        if (Cursor <= cursor)
            throw new JsonParserStateException(
                "The cursor is abnormal. It is expected to move but does not actually move. Deserialization may be implemented incorrectly.");
    }

    #endregion

    public bool Has
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected set;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void MoveNext()
    {
        Has = MoveNextImpl();
    }

    /// <summary>
    /// This method must set <see cref="CurrentToken"/> <see cref="pos"/> <see cref="Cursor"/> and return <see cref="Has"/>
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public abstract bool MoveNextImpl();

    public JsonToken CurrentToken
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected set;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual void SkipValue()
    {
        var token = CurrentToken;
        if (token.Kind is JsonTokenKind.Null or
            JsonTokenKind.True or JsonTokenKind.False or
            JsonTokenKind.String or JsonTokenKind.Number)
        {
            MoveNext();
            return;
        }
        if (token.Kind is JsonTokenKind.ObjectStart)
        {
            MoveNext();
            var first = true;
            for (; Has;)
            {
                token = CurrentToken;
                if (token.Kind is JsonTokenKind.ObjectEnd) break;
                if (first) first = false;
                else ReadComma();
                ReadStringToken();
                ReadColon();
                SkipValue();
            }
            ReadObjectEnd();
            return;
        }
        if (token.Kind is JsonTokenKind.ArrayStart)
        {
            MoveNext();
            var first = true;
            for (; Has;)
            {
                token = CurrentToken;
                if (token.Kind is JsonTokenKind.ArrayEnd) break;
                if (first) first = false;
                else ReadComma();
                SkipValue();
            }
            ReadArrayEnd();
            return;
        }
        throw new JsonParseException($"Unexpected token {token.Kind} at {token.Pos}", token.Pos);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual JsonAst ReadValue()
    {
        var token = CurrentToken;
        if (token.Kind is JsonTokenKind.Null)
        {
            MoveNext();
            return JsonAst.MakeNull(token);
        }
        if (token.Kind is JsonTokenKind.True or JsonTokenKind.False)
        {
            MoveNext();
            return JsonAst.MakeBool(token);
        }
        if (token.Kind is JsonTokenKind.Number)
        {
            MoveNext();
            return JsonAst.MakeNumber(token);
        }
        if (token.Kind is JsonTokenKind.String)
        {
            MoveNext();
            return JsonAst.MakeString(token);
        }
        if (token.Kind is JsonTokenKind.ObjectStart)
        {
            return JsonAst.MakeObject(ReadObject());
        }
        if (token.Kind is JsonTokenKind.ArrayStart)
        {
            return JsonAst.MakeArray(ReadArray());
        }
        throw new JsonParseException($"Unexpected token {token.Kind} at {token.Pos}", token.Pos);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual JsonAstObject ReadObject()
    {
        var start = ReadObjectStart();
        var map = new JsonAstObject.LinkedMultiMap();
        var first = true;
        for (; Has;)
        {
            var token = CurrentToken;
            if (token.Kind is JsonTokenKind.ObjectEnd) break;
            JsonToken? comma = null;
            if (first) first = false;
            else comma = ReadComma();
            var key = ReadStringToken();
            var colon = ReadColon();
            var val = ReadValue();
            var stringKey = key.AsString();
            var kv = new JsonAstObjectKeyValue(stringKey, key, val, colon, comma);
            map.Add(kv);
        }
        var end = ReadObjectEnd();
        return new JsonAstObject(map, start, end);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual JsonAstArray ReadArray()
    {
        var start = ReadArrayStart();
        var list = new List<JsonAstListValue>();
        var first = true;
        for (; Has;)
        {
            var token = CurrentToken;
            JsonToken? comma = null;
            if (token.Kind is JsonTokenKind.ArrayEnd) break;
            if (first) first = false;
            else comma = ReadComma();
            var val = ReadValue();
            list.Add(new(val, comma));
        }
        var end = ReadArrayEnd();
        return new JsonAstArray(list, start, end);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void MovePos(int len) => pos.MutAddChar(len);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void MovePosToNextLine(int len) => pos.MutAddLine(len);

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual void ThrowExpected(JsonTokenKind kind)
    {
        var token = CurrentToken;
        throw new JsonParseException($"Expected {kind} but found {token.Kind} at {token.Pos}", token.Pos);
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual void ThrowExpected(JsonTokenKind a, JsonTokenKind b)
    {
        var token = CurrentToken;
        throw new JsonParseException($"Expected {a} or {b} but found {token.Kind} at {token.Pos}", token.Pos);
    }

    /// <summary>Read and move next</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual JsonToken ReadOf(JsonTokenKind kind)
    {
        if (!Has) throw new JsonParseException($"Expected {kind} but found eof", pos);
        var token = CurrentToken;
        if (token.Kind != kind)
            throw new JsonParseException($"Expected {kind} but found {token.Kind} at {token.Pos}", token.Pos);
        MoveNext();
        return token;
    }

    /// <summary>Read <c>null</c> and move next</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual void ReadNull() => ReadOf(JsonTokenKind.Null);

    /// <summary>Read <c>null</c> and move next</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual bool ReadBool()
    {
        if (!Has) throw new JsonParseException($"Expected Bool but found eof", pos);
        var token = CurrentToken;
        var r = token.Kind switch
        {
            JsonTokenKind.True => true,
            JsonTokenKind.False => false,
            _ => throw new JsonParseException($"Expected Bool but found {token.Kind} at {token.Pos}", token.Pos)
        };
        MoveNext();
        return r;
    }

    /// <summary>Read <c>number</c> and move next</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual JsonToken ReadNumber() => ReadOf(JsonTokenKind.Number);

    /// <summary>Read <c>string</c> and move next</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual string ReadString() => ReadOf(JsonTokenKind.String).Text.AsString();

    /// <summary>Read <c>string</c> and move next</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual JsonToken ReadStringToken() => ReadOf(JsonTokenKind.String);

    /// <summary>Read <c>,</c> and move next</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual JsonToken ReadComma() => ReadOf(JsonTokenKind.Comma);

    /// <summary>Read <c>:</c> and move next</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual JsonToken ReadColon() => ReadOf(JsonTokenKind.Colon);

    /// <summary>Read <c>[</c> and move next</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual JsonToken ReadArrayStart() => ReadOf(JsonTokenKind.ArrayStart);

    /// <summary>Read <c>]</c> and move next</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual JsonToken ReadArrayEnd() => ReadOf(JsonTokenKind.ArrayEnd);

    /// <summary>Read <c>{</c> and move next</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual JsonToken ReadObjectStart() => ReadOf(JsonTokenKind.ObjectStart);

    /// <summary>Read <c>}</c> and move next</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual JsonToken ReadObjectEnd() => ReadOf(JsonTokenKind.ObjectEnd);
}

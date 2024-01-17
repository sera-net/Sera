using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Sera.Json.Utils;

namespace Sera.Json.De;

public abstract class AAsyncJsonReader(SeraJsonOptions options)
{
    public SeraJsonOptions Options
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
    } = options;
    
    protected readonly ConcurrentDictionary<long, string> StringCache = new();

    #region Seek

    public abstract ValueTask<long> Save();
    public abstract ValueTask Load(long savePoint);
    public abstract ValueTask UnSave(long savePoint);

    #endregion

    #region Iter

    public abstract bool CurrentHas { get; }
    public abstract JsonToken CurrentToken { get; }
    public abstract SourcePos SourcePos { get; }
    public abstract ValueTask MoveNext();

    public ulong Version { get; protected set; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AssertMove(ulong version)
    {
        if (Version == version)
            throw new JsonParserStateException(
                "It is expected to move but does not actually move. Deserialization may be implemented incorrectly.");
    }

    #endregion

    #region Utils

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual async ValueTask SkipValue()
    {
        var token = CurrentToken;
        if (token.Kind is JsonTokenKind.Null or
            JsonTokenKind.True or JsonTokenKind.False or
            JsonTokenKind.String or JsonTokenKind.Number)
        {
            await MoveNext();
            return;
        }
        if (token.Kind is JsonTokenKind.ObjectStart)
        {
            await MoveNext();
            var first = true;
            for (; CurrentHas;)
            {
                token = CurrentToken;
                if (token.Kind is JsonTokenKind.ObjectEnd) break;
                if (first) first = false;
                else await ReadComma();
                await ReadStringToken();
                await ReadColon();
                await SkipValue();
            }
            await ReadObjectEnd();
            return;
        }
        if (token.Kind is JsonTokenKind.ArrayStart)
        {
            await MoveNext();
            var first = true;
            for (; CurrentHas;)
            {
                token = CurrentToken;
                if (token.Kind is JsonTokenKind.ArrayEnd) break;
                if (first) first = false;
                else await ReadComma();
                await SkipValue();
            }
            await ReadArrayEnd();
            return;
        }
        throw new JsonParseException($"Unexpected token {token.Kind} at {token.Pos}", token.Pos);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual async ValueTask<JsonAst> ReadValue()
    {
        var token = CurrentToken;
        if (token.Kind is JsonTokenKind.Null)
        {
            await MoveNext();
            return JsonAst.MakeNull(token);
        }
        if (token.Kind is JsonTokenKind.True or JsonTokenKind.False)
        {
            await MoveNext();
            return JsonAst.MakeBool(token);
        }
        if (token.Kind is JsonTokenKind.Number)
        {
            await MoveNext();
            return JsonAst.MakeNumber(token);
        }
        if (token.Kind is JsonTokenKind.String)
        {
            await MoveNext();
            return JsonAst.MakeString(token);
        }
        if (token.Kind is JsonTokenKind.ObjectStart)
        {
            return JsonAst.MakeObject(await ReadObject());
        }
        if (token.Kind is JsonTokenKind.ArrayStart)
        {
            return JsonAst.MakeArray(await ReadArray());
        }
        throw new JsonParseException($"Unexpected token {token.Kind} at {token.Pos}", token.Pos);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual async ValueTask<JsonAstObject> ReadObject()
    {
        var start = await ReadObjectStart();
        var map = new JsonAstObject.LinkedMultiMap();
        var first = true;
        for (; CurrentHas;)
        {
            var token = CurrentToken;
            if (token.Kind is JsonTokenKind.ObjectEnd) break;
            JsonToken? comma = null;
            if (first) first = false;
            else comma = await ReadComma();
            var key = await ReadStringToken();
            var colon = await ReadColon();
            var val = await ReadValue();
            var stringKey = key.AsString();
            var kv = new JsonAstObjectKeyValue(stringKey, key, val, colon, comma);
            map.Add(kv);
        }
        var end = await ReadObjectEnd();
        return new JsonAstObject(map, start, end);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual async ValueTask<JsonAstArray> ReadArray()
    {
        var start = await ReadArrayStart();
        var list = new List<JsonAstListValue>();
        var first = true;
        for (; CurrentHas;)
        {
            var token = CurrentToken;
            JsonToken? comma = null;
            if (token.Kind is JsonTokenKind.ArrayEnd) break;
            if (first) first = false;
            else comma = await ReadComma();
            var val = await ReadValue();
            list.Add(new(val, comma));
        }
        var end = await ReadArrayEnd();
        return new JsonAstArray(list, start, end);
    }

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
    public virtual async ValueTask<JsonToken> ReadOf(JsonTokenKind kind)
    {
        if (!CurrentHas) throw new JsonParseException($"Expected {kind} but found eof", SourcePos);
        var token = CurrentToken;
        if (token.Kind != kind)
            throw new JsonParseException($"Expected {kind} but found {token.Kind} at {token.Pos}", token.Pos);
        await MoveNext();
        return token;
    }

    /// <summary>Read <c>null</c> and move next</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual async ValueTask ReadNull() => await ReadOf(JsonTokenKind.Null);

    /// <summary>Read <c>null</c> and move next</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual async ValueTask<bool> ReadBool()
    {
        if (!CurrentHas) throw new JsonParseException($"Expected Bool but found eof", SourcePos);
        var token = CurrentToken;
        var r = token.Kind switch
        {
            JsonTokenKind.True => true,
            JsonTokenKind.False => false,
            _ => throw new JsonParseException($"Expected Bool but found {token.Kind} at {token.Pos}", token.Pos)
        };
        await MoveNext();
        return r;
    }

    /// <summary>Read <c>number</c> and move next</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual ValueTask<JsonToken> ReadNumber() => ReadOf(JsonTokenKind.Number);

    /// <summary>Read <c>string</c> and move next</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual async ValueTask<string> ReadString() => (await ReadOf(JsonTokenKind.String)).Text.AsString();

    /// <summary>Read <c>string</c> and move next</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual ValueTask<JsonToken> ReadStringToken() => ReadOf(JsonTokenKind.String);

    /// <summary>Read <c>,</c> and move next</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual ValueTask<JsonToken> ReadComma() => ReadOf(JsonTokenKind.Comma);

    /// <summary>Read <c>:</c> and move next</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual ValueTask<JsonToken> ReadColon() => ReadOf(JsonTokenKind.Colon);

    /// <summary>Read <c>[</c> and move next</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual ValueTask<JsonToken> ReadArrayStart() => ReadOf(JsonTokenKind.ArrayStart);

    /// <summary>Read <c>]</c> and move next</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual ValueTask<JsonToken> ReadArrayEnd() => ReadOf(JsonTokenKind.ArrayEnd);

    /// <summary>Read <c>{</c> and move next</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual ValueTask<JsonToken> ReadObjectStart() => ReadOf(JsonTokenKind.ObjectStart);

    /// <summary>Read <c>}</c> and move next</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual ValueTask<JsonToken> ReadObjectEnd() => ReadOf(JsonTokenKind.ObjectEnd);

    #endregion
}

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Sera.Json.Utils;

namespace Sera.Json.De;

public abstract class AJsonReader(SeraJsonOptions options)
{
    public SeraJsonOptions Options
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
    } = options;

    #region Seek

    public abstract long Save();
    public abstract void Load(long savePoint);
    public abstract void UnSave(long savePoint);

    #endregion

    #region Iter

    public abstract bool CurrentHas { get; }
    public abstract JsonToken CurrentToken { get; }
    public abstract SourcePos SourcePos { get; }
    public abstract void MoveNext();

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
            for (; CurrentHas;)
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
            for (; CurrentHas;)
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
        for (; CurrentHas;)
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
        for (; CurrentHas;)
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
        if (!CurrentHas) throw new JsonParseException($"Expected {kind} but found eof", SourcePos);
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
        if (!CurrentHas) throw new JsonParseException($"Expected Bool but found eof", SourcePos);
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

    #endregion
}

public class JsonReader<State> : AJsonReader where State : IJsonReaderState<State>
{
    #region Fields

    protected State state;

    #endregion

    #region Ctor

    public JsonReader(SeraJsonOptions options, State state) : base(options)
    {
        this.state = state;
    }

    protected JsonReader(SeraJsonOptions options) : base(options)
    {
        Unsafe.SkipInit(out state);
    }

    #endregion

    #region Seek

    private Dictionary<long, State>? saves;
    private long save_inc;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public sealed override long Save()
    {
        saves ??= new();
        var savePoint = save_inc++;
        saves[savePoint] = state.Save();
        return savePoint;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public sealed override void Load(long savePoint)
    {
        state = saves![savePoint];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public sealed override void UnSave(long savePoint)
    {
        saves!.Remove(savePoint);
    }

    #endregion

    #region Iter

    public sealed override bool CurrentHas
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => state.CurrentHas;
    }
    public sealed override JsonToken CurrentToken
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => state.CurrentToken;
    }
    public override SourcePos SourcePos
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => state.SourcePos;
    }

    public sealed override void MoveNext()
    {
        if (!state.CurrentHas) return;
        state = state.MoveNext();
        Version++;
    }

    #endregion
}

// ReSharper disable once TypeParameterCanBeVariant
public interface IJsonReaderState<S> where S : IJsonReaderState<S>
{
    #region Seek

    public S Save();

    #endregion

    #region Iter

    public bool CurrentHas { get; }
    public JsonToken CurrentToken { get; }
    public SourcePos SourcePos { get; }

    public S MoveNext();

    #endregion
}

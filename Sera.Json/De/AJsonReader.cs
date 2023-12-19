using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
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
        private set;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void MoveNext()
    {
        Has = MoveNextImpl();
    }

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
        throw new NotImplementedException();
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
    public virtual void ReadComma() => ReadOf(JsonTokenKind.Comma);

    /// <summary>Read <c>:</c> and move next</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual void ReadColon() => ReadOf(JsonTokenKind.Colon);

    /// <summary>Read <c>[</c> and move next</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual void ReadArrayStart() => ReadOf(JsonTokenKind.ArrayStart);

    /// <summary>Read <c>]</c> and move next</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual void ReadArrayEnd() => ReadOf(JsonTokenKind.ArrayEnd);

    /// <summary>Read <c>{</c> and move next</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual void ReadObjectStart() => ReadOf(JsonTokenKind.ObjectStart);

    /// <summary>Read <c>}</c> and move next</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual void ReadObjectEnd() => ReadOf(JsonTokenKind.ObjectEnd);
}

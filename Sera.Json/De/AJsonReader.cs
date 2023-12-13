using System;
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

    /// <summary>Read <c>string</c> and move next</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual string ReadString() => ReadOf(JsonTokenKind.String).Text.AsString();

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

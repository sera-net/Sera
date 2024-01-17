using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Sera.Utils;

namespace Sera.Json.De;

public readonly struct JsonToken(
    JsonTokenKind Kind,
    SourcePos Pos,
    CompoundString Text,
    ConcurrentDictionary<long, string> StringCache)
{
    public JsonTokenKind Kind
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
    } = Kind;
    public SourcePos Pos
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
    } = Pos;
    public CompoundString Text
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
    } = Text;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => $"({Kind.ToString(),-12}) at {Pos.ToString(),-12} \"{AsString()}\"";

    private long CacheKey
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Unsafe.As<int, long>(ref ((Span<int>) [Pos.Index, Text.Length])[0]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string AsString()
        => StringCache.GetOrAdd(CacheKey, static (_, Text) => Text.AsString(), Text);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<char> AsSpan() => Text.AsSpan();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyMemory<char> AsMemory() => Text.Tag switch {
        CompoundString.Tags.Memory => Text.Memory,
        CompoundString.Tags.String => Text.String.AsMemory(),
        CompoundString.Tags.Span => AsString().AsMemory(),
        _ => throw new ArgumentOutOfRangeException()
    };
}

public enum JsonTokenKind
{
    EndOfFile,
    Null,
    True,
    False,
    Number,
    String,
    /// <summary>,</summary>
    Comma,
    /// <summary>:</summary>
    Colon,
    /// <summary>[</summary>
    ArrayStart,
    /// <summary>]</summary>
    ArrayEnd,
    /// <summary>{</summary>
    ObjectStart,
    /// <summary>}</summary>
    ObjectEnd,
}

public record struct SourcePos(int Index, int Line, int Char)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => $"{Line + 1}:{Char + 1}";

    [UnscopedRef]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SourcePos AddChar(int c) => new(Index + c, Line, Char + c);

    [UnscopedRef]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SourcePos AddLine(int c) => new(Index + c, Line + c, 0);

    [UnscopedRef]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void MutAddChar(int len)
    {
        Index += len;
        Char += len;
    }

    [UnscopedRef]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void MutAddLine(int len)
    {
        Line += 1;
        Index += len;
        Char = 0;
    }
}

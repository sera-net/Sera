using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Sera.Utils;

namespace Sera.Json.De;

public readonly struct JsonToken(
    JsonTokenKind kind,
    SourcePos pos,
    CompoundString Text)
{
    public JsonTokenKind Kind { get; } = kind;
    public SourcePos Pos { get; } = pos;
    public CompoundString Text { get; } = Text;

    public override string ToString() => $"( {Kind.ToString(),-12}) at {Pos.ToString(),-12} \"{Text.AsString()}\"";

    public string AsString() => Text.AsString();
    public ReadOnlySpan<char> AsSpan() => Text.AsSpan();
    public ReadOnlyMemory<char> AsMemory() => Text.AsMemory();
}

public enum JsonTokenKind
{
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

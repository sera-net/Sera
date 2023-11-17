using System;

namespace Sera.Json.De;

public readonly struct JsonToken(JsonTokenKind kind, SourcePos pos, ReadOnlyMemory<char> Text)
{
    public JsonTokenKind Kind { get; } = kind;
    public SourcePos Pos { get; } = pos;
    public ReadOnlyMemory<char> Text { get; } = Text;

    public ReadOnlyMemory<char> AsString => Text[1..^1];
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

public record struct SourcePos(int Index, int Line, int Char);

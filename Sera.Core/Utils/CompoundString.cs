using System;
using System.Runtime.CompilerServices;
using Sera.TaggedUnion;

namespace Sera.Utils;

/// <summary>
/// Compound of <c>char ReadOnlyMemory | string | char ReadOnlyFixedSpan</c>
/// <para><br/><b>Warning</b>: No escape safety, moving out of the current call stack may result in wild pointers</para>
/// </summary>
[Union]
public readonly partial struct CompoundString
{
    [UnionTemplate]
    private interface Template
    {
        ReadOnlyMemory<char> Memory();
        string String();
        ReadOnlyFixedSpan<char> Span();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public CompoundString() => this = MakeString(string.Empty);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public CompoundString(ReadOnlyMemory<char> memory) => this = MakeMemory(memory);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public CompoundString(string str) => this = MakeString(str);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public CompoundString(ReadOnlyFixedSpan<char> span) => this = MakeSpan(span);

    /// <summary>
    /// Gets the number of characters in the current <see cref="CompoundString"/>.
    /// </summary>
    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Tag switch
        {
            Tags.Memory => Memory.Length,
            Tags.String => String.Length,
            Tags.Span => Span.Length,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    /// <summary>
    /// AsString may incur heap and copy overhead
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string AsString() => Tag switch
    {
        Tags.Memory => Memory.ToString(),
        Tags.String => String,
        Tags.Span => Span.ToString(),
        _ => throw new ArgumentOutOfRangeException()
    };

    /// <summary>
    /// AsMemory may incur heap and copy overhead
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyMemory<char> AsMemory() => Tag switch
    {
        Tags.Memory => Memory,
        Tags.String => String.AsMemory(),
        Tags.Span => Span.ToString().AsMemory(),
        _ => throw new ArgumentOutOfRangeException()
    };

    /// <summary>
    /// AsSpan has no heap or copy overhead
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<char> AsSpan() => Tag switch
    {
        Tags.Memory => Memory.Span,
        Tags.String => String.AsSpan(),
        Tags.Span => Span.AsSpan(),
        _ => throw new ArgumentOutOfRangeException()
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator CompoundString(ReadOnlyMemory<char> memory) => MakeMemory(memory);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator CompoundString(string str) => MakeString(str);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator CompoundString(ReadOnlyFixedSpan<char> span) => MakeSpan(span);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public CompoundString Slice(int start) => Tag switch
    {
        Tags.Memory => MakeMemory(Memory[start..]),
        Tags.String => MakeMemory(String.AsMemory(start)),
        Tags.Span => MakeSpan(Span[start..]),
        _ => throw new ArgumentOutOfRangeException()
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public CompoundString Slice(int start, int length) => Tag switch
    {
        Tags.Memory => MakeMemory(Memory.Slice(start, length)),
        Tags.String => MakeMemory(String.AsMemory(start, length)),
        Tags.Span => MakeSpan(Span.Slice(start, length)),
        _ => throw new ArgumentOutOfRangeException()
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public CompoundString Slice(ExternalSpan span) => Slice(span.Offset, span.Length);
}

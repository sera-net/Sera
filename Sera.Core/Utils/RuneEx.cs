using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Sera.Utils;

public static class RuneEx
{
    public static RunesEnumerable Runes(this string chars) => Runes(chars.AsMemory());
    public static RunesEnumerable Runes(this ReadOnlyMemory<char> chars) => new(chars);
    public static RunesEnumerableSpan Runes(this ReadOnlySpan<char> chars) => new(chars);
}

public readonly ref struct RunesEnumerableSpan(ReadOnlySpan<char> chars)
{
    private readonly ReadOnlySpan<char> chars = chars;
    public RunesEnumeratorSpan GetEnumerator() => new(chars);
}

public readonly struct RunesEnumerable(ReadOnlyMemory<char> chars) : IEnumerable<Rune>
{
    public RunesEnumeratorSpan GetEnumerator() => new(chars.Span);

    IEnumerator<Rune> IEnumerable<Rune>.GetEnumerator() => new RunesEnumeratorClass(chars);

    IEnumerator IEnumerable.GetEnumerator() => new RunesEnumeratorClass(chars);
}

public ref struct RunesEnumeratorSpan(ReadOnlySpan<char> chars)
{
    private ReadOnlySpan<char> chars = chars;
    public Rune Current { get; private set; }

    public bool MoveNext()
    {
        if (chars.IsEmpty) return false;
        if (Rune.DecodeFromUtf16(chars, out var rune, out var count) != OperationStatus.Done)
            throw new ArgumentException("Illegal string", nameof(chars));
        Current = rune;
        chars = chars[count..];
        return true;
    }
}

public class RunesEnumeratorClass(ReadOnlyMemory<char> chars) : IEnumerator<Rune>
{
    public void Reset() => throw new NotSupportedException();
    public Rune Current { get; private set; }

    object IEnumerator.Current => Current;

    public bool MoveNext()
    {
        if (chars.IsEmpty) return false;
        if (Rune.DecodeFromUtf16(chars.Span, out var rune, out var count) != OperationStatus.Done)
            throw new ArgumentException("Illegal string", nameof(chars));
        Current = rune;
        chars = chars[count..];
        return true;
    }

    public void Dispose() { }
}

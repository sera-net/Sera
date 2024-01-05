using System;
using System.Runtime.CompilerServices;

namespace Sera.Utils;

public readonly record struct ExternalSpan(int Offset, int Length)
{
    public bool IsEmpty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Length == 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ExternalSpan From<T>(Span<T> source) => new(0, source.Length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ExternalSpan From<T>(ReadOnlySpan<T> source) => new(0, source.Length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> GetSpan<T>(Span<T> source) => source.Slice(Offset, Length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<T> GetSpan<T>(ReadOnlySpan<T> source) => source.Slice(Offset, Length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Memory<T> GetMemory<T>(Memory<T> source) => source.Slice(Offset, Length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyMemory<T> GetMemory<T>(ReadOnlyMemory<T> source) => source.Slice(Offset, Length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ExternalSpan Slice(int start)
    {
        if (start > Length) throw new ArgumentOutOfRangeException();

        return new(Offset + start, Length - start);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ExternalSpan Slice(int start, int length)
    {
        if (start > Length || length > (Length - start)) throw new ArgumentOutOfRangeException();

        return new(Offset + start, length);
    }
}

public static class ExternalSpanEx
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> Slice<T>(this Span<T> self, ExternalSpan span) =>
        self.Slice(span.Offset, span.Length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> Slice<T>(this ReadOnlySpan<T> self, ExternalSpan span) =>
        self.Slice(span.Offset, span.Length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Memory<T> Slice<T>(this Memory<T> self, ExternalSpan span) =>
        self.Slice(span.Offset, span.Length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyMemory<T> Slice<T>(this ReadOnlyMemory<T> self, ExternalSpan span) =>
        self.Slice(span.Offset, span.Length);
}

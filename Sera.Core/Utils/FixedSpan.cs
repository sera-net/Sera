using System;
using System.Runtime.CompilerServices;

namespace Sera.Utils;

#pragma warning disable CS8500

/// <summary>
/// A Span that can be passed unsafely on the heap
/// </summary>
/// <param name="ptr">The lifetime of this pointer needs to be manually guaranteed to be greater than this Span</param>
/// <param name="len">Length</param>
public readonly unsafe struct ReadOnlyFixedSpan<T>(T* ptr, nuint len) : IEquatable<ReadOnlyFixedSpan<T>>
{
    /// <inheritdoc cref="ReadOnlySpan{T}.Empty"/>
    public static ReadOnlySpan<T> Empty => default;

    private readonly nuint ptr = (nuint)ptr;
    private readonly nuint len = len;

    public ReadOnlySpan<T> AsSpan() => new((T*)ptr, (int)len);

    /// <inheritdoc cref="ReadOnlySpan{T}.this[int]"/>
    public ref readonly T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if ((nuint)index >= len) throw new IndexOutOfRangeException();
            return ref ((T*)ptr)[index];
        }
    }

    /// <inheritdoc cref="ReadOnlySpan{T}.this[int]"/>
    public ref readonly T this[nint index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if ((nuint)index >= len) throw new IndexOutOfRangeException();
            return ref ((T*)ptr)[index];
        }
    }

    /// <inheritdoc cref="ReadOnlySpan{T}.this[int]"/>
    public ref readonly T this[nuint index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if (index >= len) throw new IndexOutOfRangeException();
            return ref ((T*)ptr)[index];
        }
    }

    /// <inheritdoc cref="ReadOnlySpan{T}.Length"/>
    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (int)len;
    }

    /// <inheritdoc cref="ReadOnlySpan{T}.Length"/>
    public nuint RawLength
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => len;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(ReadOnlyFixedSpan<T> left, ReadOnlyFixedSpan<T> right) =>
        left.len == right.len && left.ptr == right.ptr;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(ReadOnlyFixedSpan<T> left, ReadOnlyFixedSpan<T> right) =>
        !(left == right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(ReadOnlyFixedSpan<T> other) => ptr == other.ptr && len == other.len;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals(object? obj) => obj is ReadOnlyFixedSpan<T> other && Equals(other);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => HashCode.Combine(unchecked((int)(long)ptr), len);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString()
    {
        if (typeof(T) == typeof(char)) return AsSpan().ToString();
        return $"ReadOnlyFixedSpan<{typeof(T).Name}>[{len}]";
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyFixedSpan<T> Slice(int start)
    {
        if ((nuint)start > len) throw new ArgumentOutOfRangeException();

        return new(((T*)ptr) + start, len - (nuint)start);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyFixedSpan<T> Slice(int start, int length)
    {
        if ((nuint)start > len || (nuint)length > (len - (nuint)start)) throw new ArgumentOutOfRangeException();

        return new(((T*)ptr) + start, (nuint)length);
    }
}

#pragma warning restore CS8500

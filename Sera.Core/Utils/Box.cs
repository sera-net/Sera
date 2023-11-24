using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Sera.Utils;

public class Box<T>(T Value) : IRef<T>, IEquatable<T>, IEquatable<Box<T>>, IComparable<T>, IComparable<Box<T>>
{
    public T Value = Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T GetRef() => ref Value;

    #region ToString

    public override string ToString() => $"{Value}";

    #endregion

    #region Equals

    public bool Equals(T? other) => EqualityComparer<T>.Default.Equals(Value, other);

    public bool Equals(Box<T>? other) => ReferenceEquals(other, null) && Equals(other!.Value);

    public override bool Equals(object? obj) => obj is Box<T> box ? Equals(box) : obj is T v && Equals(v);

    // ReSharper disable once NonReadonlyMemberInGetHashCode
    public override int GetHashCode() => Value?.GetHashCode() ?? 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Box<T> left, Box<T> right) => left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Box<T> left, Box<T> right) => !left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Box<T> left, T right) => left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Box<T> left, T right) => !left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(T left, Box<T> right) => right.Equals(left);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(T left, Box<T> right) => !right.Equals(left);

    public int CompareTo(T? other) => Comparer<T>.Default.Compare(Value, other);

    public int CompareTo(Box<T>? other) => ReferenceEquals(other, null) ? -1 : CompareTo(other.Value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(Box<T> left, Box<T> right) => left.CompareTo(right) < 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(Box<T> left, Box<T> right) => left.CompareTo(right) > 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(Box<T> left, Box<T> right) => left.CompareTo(right) <= 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(Box<T> left, Box<T> right) => left.CompareTo(right) >= 0;

    #endregion
}

public interface IRef<T>
{
    public ref T GetRef();
}

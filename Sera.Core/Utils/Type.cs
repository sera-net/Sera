using System;
using System.Globalization;
using System.Reflection;

namespace Sera.Utils;

public readonly struct Type<T> : IEquatable<Type<T>>, IEquatable<Type>
{
    public Type TypeOf => typeof(T);

    public static implicit operator Type(Type<T> _) => typeof(T);

    public override string ToString() => typeof(T).ToString();

    #region Equals

    public bool Equals(Type<T> other) => true;

    public bool Equals(Type? other) => other != null && typeof(T) == other;

    public override bool Equals(object? obj) => obj is Type<T> other && Equals(other);

    public override int GetHashCode() => typeof(T).GetHashCode();

    public static bool operator ==(Type<T> left, Type<T> right) => left.Equals(right);

    public static bool operator !=(Type<T> left, Type<T> right) => !left.Equals(right);

    #endregion
}

// ReSharper disable once UnusedTypeParameter
public interface InType<in T>;

// ReSharper disable once UnusedTypeParameter
public interface OutType<out T>;

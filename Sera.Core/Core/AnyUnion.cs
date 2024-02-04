using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Sera.TaggedUnion;

namespace Sera.Core;

public record AnyUnion(string? Name, (Variant variant, AnyVariantValue value)? Variant);

[Union(TagsName = "Kind", GenerateEquals = false, GenerateCompareTo = false)]
public readonly partial struct AnyVariantValue : IEquatable<AnyVariantValue>
{
    [UnionTemplate]
    private interface Template
    {
        void None();
        Any Value();
        List<Any> Tuple();
        AnyStruct Struct();
    }

    #region Equals

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(AnyVariantValue other) => Tag == other.Tag && Tag switch
    {
        Kind.None => true,
        Kind.Value => Value.Equals(other.Value),
        Kind.Tuple => Tuple.SequenceEqual(other.Tuple),
        Kind.Struct => Struct.Equals(other.Struct),
        _ => throw new ArgumentOutOfRangeException()
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals(object? obj) => obj is AnyVariantValue other && Equals(other);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => Tag switch
    {
        Kind.None => HashCode.Combine(Tag),
        Kind.Value => HashCode.Combine(Tag, Value),
        Kind.Tuple => GetTupleHash(Tag),
        Kind.Struct => HashCode.Combine(Tag, Struct),
        _ => throw new ArgumentOutOfRangeException()
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int GetTupleHash(Kind tag)
    {
        var hash = new HashCode();
        hash.Add(tag);
        foreach (var item in Tuple)
        {
            hash.Add(item);
        }
        return hash.ToHashCode();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(AnyVariantValue left, AnyVariantValue right) => left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(AnyVariantValue left, AnyVariantValue right) => !left.Equals(right);

    #endregion
}

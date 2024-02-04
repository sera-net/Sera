using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using BetterCollections.Memories;
using Sera.TaggedUnion;

namespace Sera.Core;

public record AnyEntry(Any Key, Any Value);

[Union(TagsName = "Kind", GenerateEquals = false, GenerateCompareTo = false)]
public readonly partial struct Any : IEquatable<Any>
{
    [UnionTemplate]
    private interface Template
    {
        Box<SeraPrimitive> Primitive();
        string String();
        byte[] Bytes();
        Any[] Array();
        void Unit();
        Box<Any?> Option();
        AnyEntry Entry();
        List<Any> Tuple();
        List<Any> Seq();
        Dictionary<Any, Any> Map();
        AnyStruct Struct();
        AnyUnion Union();
    }

    #region Equals

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Any other) => Tag == other.Tag && Tag switch
    {
        Kind.Primitive => EqualityComparer<Box<SeraPrimitive>>.Default.Equals(Primitive, other.Primitive),
        Kind.String => string.Equals(String, other.String),
        Kind.Bytes => Bytes.SequenceEqual(other.Bytes),
        Kind.Array => Array.SequenceEqual(other.Array),
        Kind.Unit => true,
        Kind.Option => EqualityComparer<Box<Any?>>.Default.Equals(Option, other.Option),
        Kind.Entry => EqualityComparer<AnyEntry>.Default.Equals(Entry, other.Entry),
        Kind.Tuple => Tuple.SequenceEqual(other.Tuple),
        Kind.Seq => Seq.SequenceEqual(other.Seq),
        Kind.Map => Map.SequenceEqual(other.Map),
        Kind.Struct => EqualityComparer<AnyStruct>.Default.Equals(Struct, other.Struct),
        Kind.Union => EqualityComparer<AnyUnion>.Default.Equals(Union, other.Union),
        _ => throw new ArgumentOutOfRangeException()
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals(object? obj) => obj is Any other && Equals(other);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => Tag switch
    {
        Kind.Primitive => HashCode.Combine(Tag, Primitive),
        Kind.String => HashCode.Combine(Tag, String),
        Kind.Bytes => SeqHash(Tag, Bytes),
        Kind.Array => SeqHash(Tag, Array),
        Kind.Unit => Tag.GetHashCode(),
        Kind.Option => HashCode.Combine(Tag, Option),
        Kind.Entry => HashCode.Combine(Tag, Entry),
        Kind.Tuple => SeqHash(Tag, Tuple),
        Kind.Seq => SeqHash(Tag, Seq),
        Kind.Map => SeqHash(Tag, Map),
        Kind.Struct => HashCode.Combine(Tag, Struct),
        Kind.Union => HashCode.Combine(Tag, Union),
        _ => throw new ArgumentOutOfRangeException()
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int SeqHash(Kind tag, byte[] arr)
    {
        var hash = new HashCode();
        hash.Add(tag);
        hash.AddBytes(arr);
        return hash.ToHashCode();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int SeqHash(Kind tag, Any[] arr)
    {
        var hash = new HashCode();
        hash.Add(tag);
        foreach (var item in arr)
        {
            hash.Add(item);
        }
        return hash.ToHashCode();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int SeqHash(Kind tag, List<Any> arr)
    {
        var hash = new HashCode();
        hash.Add(tag);
        foreach (var item in arr)
        {
            hash.Add(item);
        }
        return hash.ToHashCode();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int SeqHash(Kind tag, Dictionary<Any, Any> arr)
    {
        var hash = new HashCode();
        hash.Add(tag);
        foreach (var item in arr)
        {
            hash.Add(item);
        }
        return hash.ToHashCode();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Any left, Any right) => left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Any left, Any right) => !left.Equals(right);

    #endregion

    public override string ToString() => Tag switch
    {
        Kind.Primitive => $"{nameof(Any)}.{nameof(Primitive)} {{ {Primitive} }}",
        Kind.String => $"{nameof(Any)}.{nameof(String)} {{ \"{String.Replace("\"", "\\\"")}\" }}",
        Kind.Bytes => $"{nameof(Any)}.{nameof(Bytes)} {{ Length = {Bytes.Length} }}",
        Kind.Array => $"{nameof(Any)}.{nameof(Array)} {{ {string.Join(", ", Array)} }}",
        Kind.Unit => $"{nameof(Any)}.{nameof(Unit)}",
        Kind.Option => $"{nameof(Any)}.{nameof(Option)} {{ {Option} }}",
        Kind.Entry => $"{nameof(Any)}.{nameof(Entry)} {{ Key = {Entry.Key}, Value = {Entry.Value} }}",
        Kind.Tuple => $"{nameof(Any)}.{nameof(Tuple)} {{ {string.Join(", ", Tuple)} }}",
        Kind.Seq => $"{nameof(Any)}.{nameof(Seq)} {{ {string.Join(", ", Seq)} }}",
        Kind.Map => $"{nameof(Any)}.{nameof(Map)} {{ {string.Join(", ", Map.Select(kv => $"{kv.Key} => {kv.Value}"))} }}",
        Kind.Struct => $"{nameof(Any)}.{nameof(Struct)} {{ {Struct} }}",
        Kind.Union => $"{nameof(Any)}.{nameof(Union)} {{ {Union} }}",
        _ => throw new ArgumentOutOfRangeException()
    };
}

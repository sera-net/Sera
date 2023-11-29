using System.Collections.Generic;
using BetterCollections.Memories;
using Sera.TaggedUnion;

namespace Sera.Core;

[Union(TagsName = "Kind")]
public readonly partial struct Any
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
        Any[] Tuple();
        List<Any> Seq();
        Dictionary<Any, Any> Map();
        AnyStruct Struct();
        AnyUnion Union();
    }
}

public record AnyEntry(Any Key, Any Value);
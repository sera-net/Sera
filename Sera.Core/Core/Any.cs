using System.Collections.Generic;
using Sera.TaggedUnion;
using Sera.Utils;

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
        Dictionary<string, Any> Struct();
        AnyUnion Union();
    }
}

public record AnyEntry(Any Key, Any Value);

public record AnyUnion(Variant Variant, Any? Value);

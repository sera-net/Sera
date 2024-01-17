using System.Collections.Generic;
using Sera.TaggedUnion;

namespace Sera.Core;

public record AnyUnion(string? Name, (Variant variant, AnyVariantValue value)? Variant);

[Union(TagsName = "Kind")]
public readonly partial struct AnyVariantValue
{
    [UnionTemplate]
    private interface Template
    {
        void None();
        Any Value();
        List<Any> Tuple();
        AnyStruct Struct();
    }
}

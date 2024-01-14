using System;
using System.Collections.Frozen;
using System.Linq;

namespace Sera.Core;

public record SeraVariantInfos
{
    public Variant[] Variants { get; }
    public VariantTagKind TagKind { get; }
    public FrozenDictionary<string, int> NameToIndex { get; }
    public FrozenDictionary<VariantTag, int> TagToIndex { get; }

    public SeraVariantInfos(Variant[] variants, VariantTagKind tagKind)
    {
        Variants = variants;
        TagKind = tagKind;
        NameToIndex = variants.Select((a, b) => (a, b))
            .ToFrozenDictionary(a => a.a.Name, a => a.b);
        TagToIndex = variants.Select((a, b) => (a, b))
            .ToFrozenDictionary(a => a.a.Tag, a => a.b);
    }

    public static SeraVariantInfos Empty { get; } = new(Array.Empty<Variant>(), VariantTagKind.Int32);

    public VariantTag ParseTag(ReadOnlySpan<char> chars) => VariantTag.Parse(TagKind, chars);

    public bool TryGet(string name, out (Variant variant, int index) info)
    {
        if (!NameToIndex.TryGetValue(name, out var i))
        {
            info = default;
            return false;
        }
        else
        {
            info = (Variants[i], i);
            return true;
        }
    }

    public bool TryGet(VariantTag tag, out (Variant variant, int index) info)
    {
        if (!TagToIndex.TryGetValue(tag, out var i))
        {
            info = default;
            return false;
        }
        else
        {
            info = (Variants[i], i);
            return true;
        }
    }
}

using Sera.Core;

namespace Sera.Runtime;

public readonly record struct SeraStyles(
    SeraFormats? Formats = null,
    SeraAs As = SeraAs.None
)
{
    public static readonly SeraStyles Default = default!;

    public static SeraStyles FromAttr(SeraAttribute? sera_attr, SeraFormatsAttribute? sera_format_attr)
        => new(
            Formats: SeraFormats.FromAttr(sera_format_attr),
            As: sera_attr?.As ?? SeraAs.None
        );
}

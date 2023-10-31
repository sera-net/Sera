using Sera.Core;

namespace Sera.Runtime;

public readonly record struct SeraStyles(
    SeraFormats? Formats = null,
    SeraAs As = SeraAs.None
)
{
    public static readonly SeraStyles Default = default!;
}

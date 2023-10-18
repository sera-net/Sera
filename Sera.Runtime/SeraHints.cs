using Sera.Core.Ser;

namespace Sera.Runtime;

public readonly record struct SeraHints(
    SerializerPrimitiveHint? Primitive = null,
    SeraAs As = SeraAs.None
)
{
    public static readonly SeraHints Default = default!;
}

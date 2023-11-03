using Sera.Core;

namespace Sera.Runtime.Emit.Ser.Internal;

public readonly struct VariantMeta(string name, VariantStyle? style)
{
    public readonly string Name = name;
    public readonly VariantStyle? Style = style;
}

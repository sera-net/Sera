using Sera.Core;

namespace Sera.Json.Ser;

public abstract record AJsonFormatter : ACommonTextFormatter
{
    public bool LargeNumberUseString { get; set; } = true;
    public bool DecimalUseString { get; set; } = true;
}

public record CompactJsonFormatter : AJsonFormatter
{
    public static CompactJsonFormatter Default { get; } = new();
}

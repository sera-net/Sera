using System.Text;

namespace Sera.Json.Ser;

public abstract record AJsonFormatter
{
    public bool LargeNumberUseString { get; set; } = true;
    public bool DecimalUseString { get; set; } = true;
    public bool Base64Bytes { get; set; } = true;
    public bool EscapeAllNonAsciiChar { get; set; } = false;
}

public record CompactJsonFormatter : AJsonFormatter
{
    public static CompactJsonFormatter Default { get; } = new();
}

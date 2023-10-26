namespace Sera.Core;

public abstract record ACommonFormatter
{
    public SeraFormats? DefaultFormats { get; set; } = null;
    public UnionStyle DefaultUnionStyle { get; set; } = UnionStyle.Default;
    public UnionFormat DefaultUnionFormat { get; set; } = UnionFormat.External;
}

public abstract record ACommonTextFormatter : ACommonFormatter
{
    public bool Base64Bytes { get; set; } = true;
    public bool EscapeAllNonAsciiChar { get; set; } = false;
}

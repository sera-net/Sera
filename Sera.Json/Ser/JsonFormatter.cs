﻿using System.Text;

namespace Sera.Json.Ser;

public abstract class AJsonFormatter
{
    public Encoding Encoding { get; set; } = Encoding.UTF8;
    public bool LargeNumberUseString { get; set; } = true;
    public bool DecimalUseString { get; set; } = true;
    public bool Base64Bytes { get; set; } = true;
    public bool EscapeAllNonAsciiChar { get; set; } = false;
}

public class CompactJsonFormatter : AJsonFormatter
{
    public static readonly CompactJsonFormatter Default = new();
}

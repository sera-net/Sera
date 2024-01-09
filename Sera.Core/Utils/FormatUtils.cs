using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using Sera.Core;
using Sera.Core.Formats;

namespace Sera.Utils;

public static class FormatUtils
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? GetNumberFormat(this SeraFormats? formats, bool integer)
        => formats?.CustomNumberTextFormat ?? (formats?.CustomNumberTextFormat ?? (formats?.NumberTextFormat is { } ntf
            ? integer
                ? ntf switch
                {
                    NumberTextFormat.Any or NumberTextFormat.Generic => "G",
                    NumberTextFormat.Decimal or NumberTextFormat.Exponent => "D",
                    NumberTextFormat.Hex => "X",
                    NumberTextFormat.Binary => "B",
                    _ => throw new ArgumentOutOfRangeException()
                }
                : ntf switch
                {
                    NumberTextFormat.Any or NumberTextFormat.Generic => "G",
                    NumberTextFormat.Decimal or NumberTextFormat.Hex or NumberTextFormat.Binary => "N",
                    NumberTextFormat.Exponent => "E",
                    _ => throw new ArgumentOutOfRangeException()
                }
            : null));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetGuidFormat(this SeraFormats? formats)
        => formats?.CustomGuidTextFormat ?? (formats?.CustomGuidTextFormat ?? (formats?.GuidTextFormat is { } gtf
            ? gtf switch
            {
                GuidTextFormat.GuidTextShort or GuidTextFormat.Any => "N",
                GuidTextFormat.GuidTextGuid => "D",
                GuidTextFormat.GuidTextBraces => "B",
                GuidTextFormat.GuidTextParentheses => "P",
                GuidTextFormat.GuidTextHex => "X",
                _ => throw new ArgumentOutOfRangeException()
            }
            : null)) ?? "D";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static NumberStyles GetNumberStyles(this SeraFormats? formats)
    {
        if (formats == null) return NumberStyles.None;
        var style = formats.NumberStyles;
        switch (formats.NumberTextFormat)
        {
            case NumberTextFormat.Generic:
            case NumberTextFormat.Decimal:
                style |= NumberStyles.Number;
                break;
            case NumberTextFormat.Hex:
                style |= NumberStyles.HexNumber;
                break;
            case NumberTextFormat.Binary:
                style |= NumberStyles.BinaryNumber;
                break;
            case NumberTextFormat.Exponent:
                style |= NumberStyles.AllowExponent;
                break;
        }
        return style;
    }
}

﻿using System;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using Sera.Core;

namespace Sera.Utils;

public static class ParseUtils
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParseComplex(this ReadOnlySpan<char> str, NumberStyles styles, out Complex complex)
    {
        if (str.Length < 5) goto failed;
        if (str[0] != '<') goto failed;
        if (str[^1] != '>') goto failed;
        var mid = str.IndexOf(';');
        if (mid < 0) goto failed;
        var real_str = str[1..mid];
        var imaginary_str = str[(mid + 1)..^1];
        styles |= NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite;
        if (!double.TryParse(real_str, styles, null, out var real)) goto failed;
        if (!double.TryParse(imaginary_str, styles, null, out var imaginary)) goto failed;
        complex = new(real, imaginary);
        return true;
        failed:
        complex = default;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParseIndex(this ReadOnlySpan<char> str, out Index index)
    {
        var num = str;
        var inv = false;
        if (str.Length > 0 && str[0] == '^')
        {
            inv = true;
            num = str[1..];
        }
        if (long.TryParse(num, out var value))
        {
            index = new((int)value, inv);
            return true;
        }
        index = default;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParseRange(this ReadOnlySpan<char> str, out Range range)
    {
        if (str.Length <= 4) goto failed;
        var split_atr = str.IndexOf('.');
        if (split_atr < 0 || split_atr + 2 > str.Length) goto failed;
        if (str[split_atr + 1] != '.') goto failed;
        var left = str[..split_atr];
        var right = str[(split_atr + 2)..];
        if (!TryParseIndex(left, out var start)) goto failed;
        if (!TryParseIndex(right, out var end)) goto failed;
        range = new(start, end);
        return true;
        failed:
        range = default;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParseVariantTag(this ReadOnlySpan<char> chars, VariantTagKind tagKind, SeraFormats? formats,
        out VariantTag tag)
    {
        var style = formats.GetNumberStyles();
        switch (tagKind)
        {
            case VariantTagKind.SByte:
            {
                if (!sbyte.TryParse(chars, style, null, out var v)) goto failed;
                tag = VariantTag.Create(v);
                return true;
            }
            case VariantTagKind.Byte:
            {
                if (!byte.TryParse(chars, style, null, out var v)) goto failed;
                tag = VariantTag.Create(v);
                return true;
            }
            case VariantTagKind.Int16:
            {
                if (!short.TryParse(chars, style, null, out var v)) goto failed;
                tag = VariantTag.Create(v);
                return true;
            }
            case VariantTagKind.UInt16:
            {
                if (!ushort.TryParse(chars, style, null, out var v)) goto failed;
                tag = VariantTag.Create(v);
                return true;
            }
            case VariantTagKind.Int32:
            {
                if (!int.TryParse(chars, style, null, out var v)) goto failed;
                tag = VariantTag.Create(v);
                return true;
            }
            case VariantTagKind.UInt32:
            {
                if (!uint.TryParse(chars, style, null, out var v)) goto failed;
                tag = VariantTag.Create(v);
                return true;
            }
            case VariantTagKind.Int64:
            {
                if (!long.TryParse(chars, style, null, out var v)) goto failed;
                tag = VariantTag.Create(v);
                return true;
            }
            case VariantTagKind.UInt64:
            {
                if (!ulong.TryParse(chars, style, null, out var v)) goto failed;
                tag = VariantTag.Create(v);
                return true;
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(tagKind), tagKind, null);
        }
        failed:
        tag = default;
        return false;
    }
}

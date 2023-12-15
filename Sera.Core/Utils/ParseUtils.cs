using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Sera.Utils;

public static class ParseUtils
{
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
}

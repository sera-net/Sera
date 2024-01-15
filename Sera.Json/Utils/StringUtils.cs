using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;

namespace Sera.Json.Utils;

public static class StringUtils
{
    /// <summary>Count the number of leading whitespace(0x20) and tab(0x09) characters</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CountLeadingSpace(this ReadOnlySpan<char> str)
        => CountLeadingChar2(str, '\x20', '\x09', ne: true);

    /// <summary>Count the number of leading characters</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CountLeadingChar2(this ReadOnlySpan<char> str, char a, char b, bool ne)
    {
        var len = 0;
        var span = str;
        if (Vector512.IsHardwareAccelerated)
        {
            for (; span.Length >= 32; span = span[32..])
            {
                var vec = Vector512.LoadUnsafe(
                    ref Unsafe.As<char, ushort>(ref Unsafe.AsRef(in span.GetPinnableReference())));
                var cmp_a_v = Vector512.Equals(vec, Vector512.Create((ushort)a));
                var cmp_b_v = Vector512.Equals(vec, Vector512.Create((ushort)b));
                var cmp_a = cmp_a_v.ExtractMostSignificantBits();
                var cmp_b = cmp_b_v.ExtractMostSignificantBits();
                var cmp = cmp_a | cmp_b;
                if (ne)
                {
                    cmp = ~cmp;
                    Debug.Assert(cmp != 0);
                    var count = (int)ulong.TrailingZeroCount(cmp);
                    len += count;
                    if (count != 32) return len;
                }
                else
                {
                    if (cmp == 0) len += 32;
                    else
                    {
                        var count = (int)ulong.TrailingZeroCount(cmp);
                        return len + count;
                    }
                }
            }
        }
        if (Vector256.IsHardwareAccelerated)
        {
            for (; span.Length >= 16; span = span[16..])
            {
                var vec = Vector256.LoadUnsafe(
                    ref Unsafe.As<char, ushort>(ref Unsafe.AsRef(in span.GetPinnableReference())));
                var cmp_a_v = Vector256.Equals(vec, Vector256.Create((ushort)a));
                var cmp_b_v = Vector256.Equals(vec, Vector256.Create((ushort)b));
                var cmp_a = cmp_a_v.ExtractMostSignificantBits();
                var cmp_b = cmp_b_v.ExtractMostSignificantBits();
                var cmp = cmp_a | cmp_b;
                if (ne)
                {
                    cmp = ~cmp;
                    Debug.Assert(cmp != 0);
                    var count = (int)uint.TrailingZeroCount(cmp);
                    len += count;
                    if (count != 16) return len;
                }
                else
                {
                    if (cmp == 0) len += 16;
                    else
                    {
                        var count = (int)uint.TrailingZeroCount(cmp);
                        return len + count;
                    }
                }
            }
        }
        if (Vector128.IsHardwareAccelerated)
        {
            for (; span.Length >= 8; span = span[8..])
            {
                var vec = Vector128.LoadUnsafe(
                    ref Unsafe.As<char, ushort>(ref Unsafe.AsRef(in span.GetPinnableReference())));
                var cmp_a_v = Vector128.Equals(vec, Vector128.Create((ushort)a));
                var cmp_b_v = Vector128.Equals(vec, Vector128.Create((ushort)b));
                var cmp_a = cmp_a_v.ExtractMostSignificantBits();
                var cmp_b = cmp_b_v.ExtractMostSignificantBits();
                var cmp = cmp_a | cmp_b;
                if (ne)
                {
                    cmp = ~cmp;
                    Debug.Assert(cmp != 0);
                    var count = (int)uint.TrailingZeroCount(cmp);
                    len += count;
                    if (count != 8) return len;
                }
                else
                {
                    if (cmp == 0) len += 8;
                    else
                    {
                        var count = (int)uint.TrailingZeroCount(cmp);
                        return len + count;
                    }
                }
            }
        }
        if (Vector64.IsHardwareAccelerated)
        {
            for (; span.Length >= 4; span = span[4..])
            {
                var vec = Vector64.LoadUnsafe(
                    ref Unsafe.As<char, ushort>(ref Unsafe.AsRef(in span.GetPinnableReference())));
                var cmp_a_v = Vector64.Equals(vec, Vector64.Create((ushort)a));
                var cmp_b_v = Vector64.Equals(vec, Vector64.Create((ushort)b));
                var cmp_a = cmp_a_v.ExtractMostSignificantBits();
                var cmp_b = cmp_b_v.ExtractMostSignificantBits();
                var cmp = cmp_a | cmp_b;
                if (ne)
                {
                    cmp = ~cmp;
                    Debug.Assert(cmp != 0);
                    var count = (int)uint.TrailingZeroCount(cmp);
                    len += count;
                    if (count != 4) return len;
                }
                else
                {
                    if (cmp == 0) len += 4;
                    else
                    {
                        var count = (int)uint.TrailingZeroCount(cmp);
                        return len + count;
                    }
                }
            }
        }
        foreach (var c in span)
        {
            if (ne)
            {
                if (c != a && c != b) return len;
            }
            else
            {
                if (c == a || c == b) return len;
            }
            len++;
        }
        return len;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int FindFirstControlCharacterIndex(this ReadOnlySpan<char> str)
    {
        var len = 0;
        var span = str;
        if (Vector512.IsHardwareAccelerated)
        {
            for (; span.Length >= 32; span = span[32..])
            {
                var vec = Vector512.LoadUnsafe(
                    ref Unsafe.As<char, ushort>(ref Unsafe.AsRef(in span.GetPinnableReference())));
                // See comments in Rune.IsControl for more information.
                var tmp_1 = (vec + Vector512<ushort>.One) & Vector512.Create(unchecked((ushort)~0x80));
                var cmp_v = Vector512.LessThanOrEqual(tmp_1, Vector512.Create((ushort)0x20));
                var cmp = cmp_v.ExtractMostSignificantBits();
                if (cmp == 0) len += 32;
                else
                {
                    var nth = (int)ulong.TrailingZeroCount(cmp);
                    return len + nth;
                }
            }
        }
        if (Vector256.IsHardwareAccelerated)
        {
            for (; span.Length >= 16; span = span[16..])
            {
                var vec = Vector256.LoadUnsafe(
                    ref Unsafe.As<char, ushort>(ref Unsafe.AsRef(in span.GetPinnableReference())));
                // See comments in Rune.IsControl for more information.
                var tmp_1 = (vec + Vector256<ushort>.One) & Vector256.Create(unchecked((ushort)~0x80));
                var cmp_v = Vector256.LessThanOrEqual(tmp_1, Vector256.Create((ushort)0x20));
                var cmp = cmp_v.ExtractMostSignificantBits();
                if (cmp == 0) len += 16;
                else
                {
                    var nth = (int)uint.TrailingZeroCount(cmp);
                    return len + nth;
                }
            }
        }
        if (Vector128.IsHardwareAccelerated)
        {
            for (; span.Length >= 8; span = span[8..])
            {
                var vec = Vector128.LoadUnsafe(
                    ref Unsafe.As<char, ushort>(ref Unsafe.AsRef(in span.GetPinnableReference())));
                // See comments in Rune.IsControl for more information.
                var tmp_1 = (vec + Vector128<ushort>.One) & Vector128.Create(unchecked((ushort)~0x80));
                var cmp_v = Vector128.LessThanOrEqual(tmp_1, Vector128.Create((ushort)0x20));
                var cmp = cmp_v.ExtractMostSignificantBits();
                if (cmp == 0) len += 8;
                else
                {
                    var nth = (int)uint.TrailingZeroCount(cmp);
                    return len + nth;
                }
            }
        }
        if (Vector64.IsHardwareAccelerated)
        {
            for (; span.Length >= 4; span = span[4..])
            {
                var vec = Vector64.LoadUnsafe(
                    ref Unsafe.As<char, ushort>(ref Unsafe.AsRef(in span.GetPinnableReference())));
                // See comments in Rune.IsControl for more information.
                var tmp_1 = (vec + Vector64<ushort>.One) & Vector64.Create(unchecked((ushort)~0x80));
                var cmp_v = Vector64.LessThanOrEqual(tmp_1, Vector64.Create((ushort)0x20));
                var cmp = cmp_v.ExtractMostSignificantBits();
                if (cmp == 0) len += 8;
                else
                {
                    var nth = (int)uint.TrailingZeroCount(cmp);
                    return len + nth;
                }
            }
        }
        foreach (var c in span)
        {
            if (char.IsControl(c)) return len;
            len++;
        }
        return -1;
    }

    /// <summary>Count the number of leading whitespace(0x20) and tab(0x09) characters</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CountLeadingStringContent(this ReadOnlySpan<char> str)
    {
        var len = 0;
        var span = str;
        if (Vector512.IsHardwareAccelerated)
        {
            for (; span.Length >= 32; span = span[32..])
            {
                var vec = Vector512.LoadUnsafe(
                    ref Unsafe.As<char, ushort>(ref Unsafe.AsRef(in span.GetPinnableReference())));
                var cmp_a_v = Vector512.Equals(vec, Vector512.Create((ushort)'"'));
                var cmp_b_v = Vector512.Equals(vec, Vector512.Create((ushort)'\\'));
                // See comments in Rune.IsControl for more information.
                var tmp_c_1 = (vec + Vector512<ushort>.One) & Vector512.Create(unchecked((ushort)~0x80));
                var cmp_c_v = Vector512.LessThanOrEqual(tmp_c_1, Vector512.Create((ushort)0x20));
                var cmp_a = cmp_a_v.ExtractMostSignificantBits();
                var cmp_b = cmp_b_v.ExtractMostSignificantBits();
                var cmp_c = cmp_c_v.ExtractMostSignificantBits();
                var cmp = cmp_a | cmp_b | cmp_c;
                if (cmp == 0) len += 32;
                else
                {
                    var nth = (int)ulong.TrailingZeroCount(cmp);
                    return len + nth;
                }
            }
        }
        if (Vector256.IsHardwareAccelerated)
        {
            for (; span.Length >= 16; span = span[16..])
            {
                var vec = Vector256.LoadUnsafe(
                    ref Unsafe.As<char, ushort>(ref Unsafe.AsRef(in span.GetPinnableReference())));
                var cmp_a_v = Vector256.Equals(vec, Vector256.Create((ushort)'"'));
                var cmp_b_v = Vector256.Equals(vec, Vector256.Create((ushort)'\\'));
                // See comments in Rune.IsControl for more information.
                var tmp_c_1 = (vec + Vector256<ushort>.One) & Vector256.Create(unchecked((ushort)~0x80));
                var cmp_c_v = Vector256.LessThanOrEqual(tmp_c_1, Vector256.Create((ushort)0x20));
                var cmp_a = cmp_a_v.ExtractMostSignificantBits();
                var cmp_b = cmp_b_v.ExtractMostSignificantBits();
                var cmp_c = cmp_c_v.ExtractMostSignificantBits();
                var cmp = cmp_a | cmp_b | cmp_c;
                if (cmp == 0) len += 16;
                else
                {
                    var nth = (int)uint.TrailingZeroCount(cmp);
                    return len + nth;
                }
            }
        }
        if (Vector128.IsHardwareAccelerated)
        {
            for (; span.Length >= 8; span = span[8..])
            {
                var vec = Vector128.LoadUnsafe(
                    ref Unsafe.As<char, ushort>(ref Unsafe.AsRef(in span.GetPinnableReference())));
                var cmp_a_v = Vector128.Equals(vec, Vector128.Create((ushort)'"'));
                var cmp_b_v = Vector128.Equals(vec, Vector128.Create((ushort)'\\'));
                // See comments in Rune.IsControl for more information.
                var tmp_c_1 = (vec + Vector128<ushort>.One) & Vector128.Create(unchecked((ushort)~0x80));
                var cmp_c_v = Vector128.LessThanOrEqual(tmp_c_1, Vector128.Create((ushort)0x20));
                var cmp_a = cmp_a_v.ExtractMostSignificantBits();
                var cmp_b = cmp_b_v.ExtractMostSignificantBits();
                var cmp_c = cmp_c_v.ExtractMostSignificantBits();
                var cmp = cmp_a | cmp_b | cmp_c;
                if (cmp == 0) len += 8;
                else
                {
                    var nth = (int)uint.TrailingZeroCount(cmp);
                    return len + nth;
                }
            }
        }
        if (Vector64.IsHardwareAccelerated)
        {
            for (; span.Length >= 4; span = span[4..])
            {
                var vec = Vector64.LoadUnsafe(
                    ref Unsafe.As<char, ushort>(ref Unsafe.AsRef(in span.GetPinnableReference())));
                var cmp_a_v = Vector64.Equals(vec, Vector64.Create((ushort)'"'));
                var cmp_b_v = Vector64.Equals(vec, Vector64.Create((ushort)'\\'));
                // See comments in Rune.IsControl for more information.
                var tmp_c_1 = (vec + Vector64<ushort>.One) & Vector64.Create(unchecked((ushort)~0x80));
                var cmp_c_v = Vector64.LessThanOrEqual(tmp_c_1, Vector64.Create((ushort)0x20));
                var cmp_a = cmp_a_v.ExtractMostSignificantBits();
                var cmp_b = cmp_b_v.ExtractMostSignificantBits();
                var cmp_c = cmp_c_v.ExtractMostSignificantBits();
                var cmp = cmp_a | cmp_b | cmp_c;
                if (cmp == 0) len += 8;
                else
                {
                    var nth = (int)uint.TrailingZeroCount(cmp);
                    return len + nth;
                }
            }
        }
        foreach (var c in span)
        {
            if (c is '"' or '\\' || char.IsControl(c)) return len;
            len++;
        }
        return len;
    }

    /// <summary>Count the number of leading 0..9 characters</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CountLeadingNumberDigitBody(this ReadOnlySpan<char> str)
    {
        var len = 0;
        var span = str;
        if (Vector512.IsHardwareAccelerated)
        {
            for (; span.Length >= 32; span = span[32..])
            {
                var vec = Vector512.LoadUnsafe(
                    ref Unsafe.As<char, ushort>(ref Unsafe.AsRef(in span.GetPinnableReference())));
                var a = vec - Vector512.Create((ushort)'0');
                var cmp_v = Vector512.GreaterThan(a, Vector512.Create((ushort)('9' - '0')));
                var cmp = cmp_v.ExtractMostSignificantBits();
                if (cmp == 0) len += 32;
                else
                {
                    var nth = (int)ulong.TrailingZeroCount(cmp);
                    return len + nth;
                }
            }
        }
        if (Vector256.IsHardwareAccelerated)
        {
            for (; span.Length >= 16; span = span[16..])
            {
                var vec = Vector256.LoadUnsafe(
                    ref Unsafe.As<char, ushort>(ref Unsafe.AsRef(in span.GetPinnableReference())));
                var a = vec - Vector256.Create((ushort)'0');
                var cmp_v = Vector256.GreaterThan(a, Vector256.Create((ushort)('9' - '0')));
                var cmp = cmp_v.ExtractMostSignificantBits();
                if (cmp == 0) len += 16;
                else
                {
                    var nth = (int)ulong.TrailingZeroCount(cmp);
                    return len + nth;
                }
            }
        }
        if (Vector128.IsHardwareAccelerated)
        {
            for (; span.Length >= 8; span = span[8..])
            {
                var vec = Vector128.LoadUnsafe(
                    ref Unsafe.As<char, ushort>(ref Unsafe.AsRef(in span.GetPinnableReference())));
                var a = vec - Vector128.Create((ushort)'0');
                var cmp_v = Vector128.GreaterThan(a, Vector128.Create((ushort)('9' - '0')));
                var cmp = cmp_v.ExtractMostSignificantBits();
                if (cmp == 0) len += 8;
                else
                {
                    var nth = (int)ulong.TrailingZeroCount(cmp);
                    return len + nth;
                }
            }
        }
        if (Vector64.IsHardwareAccelerated)
        {
            for (; span.Length >= 4; span = span[4..])
            {
                var vec = Vector64.LoadUnsafe(
                    ref Unsafe.As<char, ushort>(ref Unsafe.AsRef(in span.GetPinnableReference())));
                var a = vec - Vector64.Create((ushort)'0');
                var cmp_v = Vector64.GreaterThan(a, Vector64.Create((ushort)('9' - '0')));
                var cmp = cmp_v.ExtractMostSignificantBits();
                if (cmp == 0) len += 4;
                else
                {
                    var nth = (int)ulong.TrailingZeroCount(cmp);
                    return len + nth;
                }
            }
        }
        foreach (var c in span)
        {
            if (!char.IsAsciiDigit(c)) return len;
            len++;
        }
        return len;
    }
}

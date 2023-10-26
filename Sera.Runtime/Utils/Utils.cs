using System;
using System.Runtime.CompilerServices;
using Sera.Core;

namespace Sera.Runtime.Utils;

internal static class Utils
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static VariantTag MakeVariantTag<V>(this V value) => value switch
    {
        byte v => VariantTag.Create(v),
        sbyte v => VariantTag.Create(v),
        short v => VariantTag.Create(v),
        ushort v => VariantTag.Create(v),
        int v => VariantTag.Create(v),
        uint v => VariantTag.Create(v),
        long v => VariantTag.Create(v),
        ulong v => VariantTag.Create(v),
        _ => throw new ArgumentException($"{typeof(V)} is not a valid underlying type of enum"),
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Int128 PrimitiveToInt128<V>(this V value) => value switch
    {
        bool v => v ? 1 : 0,
        sbyte v => v,
        byte v => v,
        short v => v,
        ushort v => v,
        int v => v,
        uint v => v,
        long v => v,
        ulong v => v,
        float v => (Int128)v,
        double v => (Int128)v,
        decimal v => (Int128)v,
        char v => v,
        _ => throw new ArgumentException($"{typeof(V)} is not a primitive type"),
    };
}

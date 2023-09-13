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
}

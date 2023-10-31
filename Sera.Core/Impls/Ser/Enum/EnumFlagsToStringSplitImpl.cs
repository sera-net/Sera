using System;
using System.Runtime.CompilerServices;
using static Sera.Core.Impls.Ser.EnumFlagsToStringSplitImpl;

namespace Sera.Core.Impls.Ser;

internal static class EnumFlagsToStringSplitImpl
{
    internal static readonly ArrayImpl<string, StringImpl> dep = new();
}

public readonly struct EnumFlagsToStringSplitImpl<T> : ISeraVision<T>
    where T : Enum
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, T value) where V : ASeraVisitor<R>
    {
        var arr = $"{value}".Split(", ");
        return dep.Accept<R, V>(visitor, arr);
    }
}

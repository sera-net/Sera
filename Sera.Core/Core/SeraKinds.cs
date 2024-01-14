using System;
using System.Runtime.CompilerServices;

namespace Sera.Core;

[Flags]
public enum SeraKinds : uint
{
    None = 0,
    Primitive = 1 << 0,
    String = 1 << 1,
    Bytes = 1 << 2,
    Array = 1 << 3,
    Unit = 1 << 4,
    Option = 1 << 5,
    Entry = 1 << 6,
    Tuple = 1 << 7,
    Seq = 1 << 8,
    Map = 1 << 9,
    Struct = 1 << 10,
    Union = 1 << 11,
}

public static class SeraKindsEx
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SeraKinds ToKinds(this Any.Kind self) => self switch
    {
        Any.Kind.Primitive => SeraKinds.Primitive,
        Any.Kind.String => SeraKinds.String,
        Any.Kind.Bytes => SeraKinds.Bytes,
        Any.Kind.Array => SeraKinds.Array,
        Any.Kind.Unit => SeraKinds.Unit,
        Any.Kind.Option => SeraKinds.Option,
        Any.Kind.Entry => SeraKinds.Entry,
        Any.Kind.Tuple => SeraKinds.Tuple,
        Any.Kind.Seq => SeraKinds.Seq,
        Any.Kind.Map => SeraKinds.Map,
        Any.Kind.Struct => SeraKinds.Struct,
        Any.Kind.Union => SeraKinds.Union,
        _ => throw new ArgumentOutOfRangeException(nameof(self), self, null)
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Has(this SeraKinds self, SeraKinds target) => (self & target) != 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Set(this ref SeraKinds self, SeraKinds target) => self |= target;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void UnSet(this ref SeraKinds self, SeraKinds target) => self &= ~target;
}

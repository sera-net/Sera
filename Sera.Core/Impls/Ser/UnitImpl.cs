using System.Runtime.CompilerServices;

namespace Sera.Core.Impls.Ser;

public readonly struct UnitImpl<T> : ITypeVision<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, T value) where V : ATypeVisitor<R>
        => visitor.VUnit();
}

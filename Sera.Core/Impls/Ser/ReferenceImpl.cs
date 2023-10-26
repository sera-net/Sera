using System.Runtime.CompilerServices;

namespace Sera.Core.Impls.Ser;

public readonly struct ReferenceImpl<T, D>(D dep) : ITypeVision<T>
    where T : class where D : ITypeVision<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, T value) where V : ATypeVisitor<R>
        => visitor.VReference(dep, value);
}

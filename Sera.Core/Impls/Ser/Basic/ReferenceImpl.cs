using System.Runtime.CompilerServices;

namespace Sera.Core.Impls.Ser;

public readonly struct ReferenceImpl<T, D>(D dep) : ISeraVision<T>
    where T : class where D : ISeraVision<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, T value) where V : ASeraVisitor<R>
        => visitor.VReference(dep, value.NotNull());
}

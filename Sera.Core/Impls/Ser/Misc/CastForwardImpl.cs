using System.Runtime.CompilerServices;

namespace Sera.Core.Impls.Ser;

public readonly struct CastForwardImpl<T, B>(B impl) : ISeraVision<object>
    where B : ISeraVision<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, object value) where V : ASeraVisitor<R>
        => impl.Accept<R, V>(visitor, (T)value);
}

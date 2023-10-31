using System.Runtime.CompilerServices;

namespace Sera.Core.Impls.Ser;

public readonly struct NullableImpl<T, D>(D dep) : ISeraVision<T?>
    where T : struct where D : ISeraVision<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, T? value) where V : ASeraVisitor<R>
        => value switch
        {
            null => visitor.VNone<T>(),
            { } v => visitor.VSome(dep, v),
        };
}

public readonly struct NullableClassImpl<T, D>(D dep) : ISeraVision<T?>
    where T : class where D : ISeraVision<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, T? value) where V : ASeraVisitor<R>
        => value switch
        {
            null => visitor.VNone<T>(),
            _ => visitor.VSome(dep, value),
        };
}

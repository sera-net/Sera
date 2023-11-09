using System.Runtime.CompilerServices;

namespace Sera.Core.Impls.Ser;

public static class ByImpls<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ByTupleImpl<T, D> ByTuple<D>(D dep) where D : ITupleSeraVision<T>
        => new(dep);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ByStructImpl<T, D> ByStruct<D>(D dep) where D : IStructSeraVision<T>
        => new(dep);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ByUnionImpl<T, D> ByUnion<D>(D dep) where D : IUnionSeraVision<T>
        => new(dep);
}

public readonly struct ByTupleImpl<T, D>(D dep) : ISeraVision<T>
    where D : ITupleSeraVision<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, T value) where V : ASeraVisitor<R>
        => visitor.VTuple(dep, value);
}

public readonly struct ByStructImpl<T, D>(D dep) : ISeraVision<T>
    where D : IStructSeraVision<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, T value) where V : ASeraVisitor<R>
        => visitor.VStruct(dep, value);
}

public readonly struct ByUnionImpl<T, D>(D dep) : ISeraVision<T>
    where D : IUnionSeraVision<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, T value) where V : ASeraVisitor<R>
        => visitor.VUnion(dep, value);
}

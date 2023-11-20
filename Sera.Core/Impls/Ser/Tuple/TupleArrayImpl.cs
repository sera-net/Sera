using System.Runtime.CompilerServices;

namespace Sera.Core.Impls.Ser;

public readonly struct TupleArrayImpl<T, D>(D dep, int size) : ISeraVision<T[]>, ITupleSeraVision<T[]>
    where D : ISeraVision<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, T[] value) where V : ASeraVisitor<R>
        => visitor.VTuple(this, value);

    public int Size
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => size;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R AcceptItem<R, V>(V visitor, ref T[] value, int index) where V : ATupleSeraVisitor<R>
        => visitor.VItem(dep, value[index]);
}

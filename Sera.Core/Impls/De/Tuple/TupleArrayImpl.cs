using System.Runtime.CompilerServices;
using Sera.Core.Impls.De;
using Sera.Utils;

namespace Sera.Core.Impls.De;

public readonly struct TupleArrayImpl<T, D>(D dep, int size)
    : ISeraColion<T[]>, ITupleSeraColion<T[]>
    where D : ISeraColion<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<T[]>? t = null) where C : ISeraColctor<T[], R>
        => colctor.CTuple(this, new IdentityMapper<T[]>(), new Type<T[]>());

    public int Size
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => size;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T[] Builder(Type<T[]> b = default) => new T[size];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R CollectItem<R, C>(ref C colctor, int index, Type<T[]> b = default) where C : ITupleSeraColctor<T[], R>
        => colctor.CItem(dep, new TupleArrayEffector<T>(index), new Type<T>());
}

public readonly struct TupleArrayEffector<T>(int index) : ISeraEffector<T[], T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Effect(ref T[] target, T value) => target[index] = value;
}

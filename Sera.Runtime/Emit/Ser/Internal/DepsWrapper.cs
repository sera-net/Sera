using System.Runtime.CompilerServices;
using Sera.Core;
using Sera.Core.Impls.Deps;
using Sera.Runtime.Utils;

namespace Sera.Runtime.Emit.Ser.Internal;

public readonly struct DepsSerWrapper<T, D, C> : ISeraVision<T>
    where D : ISeraVision<T>
    where C : IDepsContainer<D>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, T value) where V : ASeraVisitor<R>
        => C.Impl1!.Accept<R, V>(visitor, value);
}

public readonly struct BoxedDepsSerWrapper<T, D, C> : ISeraVision<T>
    where D : ISeraVision<T>
    where C : IDepsContainer<Box<D>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, T value) where V : ASeraVisitor<R>
        => C.Impl1!.Value.Accept<R, V>(visitor, value);
}

public readonly struct DepsSerTupleWrapper<T, D, C> : ITupleSeraVision<T>
    where D : ITupleSeraVision<T>
    where C : IDepsContainer<D>
{
    public int Size => C.Impl1!.Size;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R AcceptItem<R, V>(V visitor, T value, int index) where V : ATupleSeraVisitor<R>
        => C.Impl1!.AcceptItem<R, V>(visitor, value, index);
}

public readonly struct BoxedDepsSerTupleWrapper<T, D, C> : ITupleSeraVision<T>
    where D : ITupleSeraVision<T>
    where C : IDepsContainer<Box<D>>
{
    public int Size => C.Impl1!.Value.Size;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R AcceptItem<R, V>(V visitor, T value, int index) where V : ATupleSeraVisitor<R>
        => C.Impl1!.Value.AcceptItem<R, V>(visitor, value, index);
}

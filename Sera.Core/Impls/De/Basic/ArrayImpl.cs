using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using Sera.Core.Impls.De;
using Sera.Utils;

namespace Sera.Core.Impls.De;

public readonly struct ArrayImpl<T, D>(D dep) :
    ISeraColion<T[]>,
    ISeraColion<ReadOnlyMemory<T>>, ISeraColion<Memory<T>>
    where D : ISeraColion<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<T[]>? t = null)
        where C : ISeraColctor<T[], R>
        => colctor.CArray(dep, new IdentityMapper<T[]>(), new Type<T[]>(), new Type<T>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<ReadOnlyMemory<T>>? t = null)
        where C : ISeraColctor<ReadOnlyMemory<T>, R>
        => colctor.CArray(dep, new IdentityMapper<ReadOnlyMemory<T>>(), new Type<ReadOnlyMemory<T>>(), new Type<T>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<Memory<T>>? t = null)
        where C : ISeraColctor<Memory<T>, R>
        => colctor.CArray(dep, new IdentityMapper<Memory<T>>(), new Type<Memory<T>>(), new Type<T>());
}

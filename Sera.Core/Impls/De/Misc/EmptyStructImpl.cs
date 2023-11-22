using System;
using System.Runtime.CompilerServices;
using Sera.Utils;

namespace Sera.Core.Impls.De;

public readonly struct EmptyStructImpl<T, N>(N ctor) :
    ISeraColion<T>, IStructSeraColion<T>
    where N : ISeraCtor<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<T>? t = null) where C : ISeraColctor<T, R>
        => colctor.CStruct(this, new IdentityMapper<T>(), new Type<T>());

    public SeraFieldInfos Fields
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => SeraFieldInfos.Empty;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Builder(Type<T> b = default) => ctor.Ctor();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R CollectField<R, C>(ref C colctor, int field, Type<T> b = default) where C : IStructSeraColctor<T, R>
        => colctor.CNone();
}

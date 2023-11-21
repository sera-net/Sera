using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sera.Utils;

namespace Sera.Core.Impls.De;

public readonly struct MapImpl<T, IK, IV, DK, DV, B, M, N, E>(DK dk, DV dv, M mapper, N ctor, E effector) :
    ISeraColion<T>, IMapSeraColion<B, IK, IV>
    where DK : ISeraColion<IK>
    where DV : ISeraColion<IV>
    where M : ISeraMapper<B, T>
    where N : ICapSeraCtor<B>
    where E : ISeraEffector<B, KeyValuePair<IK, IV>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<T>? t = null) where C : ISeraColctor<T, R>
        => colctor.CMap(this, mapper, new Type<B>(), new Type<IK>(), new Type<IV>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public B Builder(int? cap, Type<B> b = default) => ctor.Ctor(cap);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R CollectItem<R, C>(ref C colctor, Type<B> b = default) where C : IMapSeraColctor<B, IK, IV, R>
        => colctor.CItem(dk, dv, effector);
}

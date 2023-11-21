using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sera.Utils;

namespace Sera.Core.Impls.De;

public readonly struct MapAsSeqImpl<T, IK, IV, DK, DV, B, M, D, E>(DK dk, DV dv, M mapper, D d, E effector) :
    ISeraColion<T>, IMapSeraColion<B, IK, IV>
    where DK : ISeraColion<IK>
    where DV : ISeraColion<IV>
    where D : ISeqSeraColion<B, KeyValuePair<IK, IV>>
    where M : ISeraMapper<B, T>
    where E : ISeraEffector<B, KeyValuePair<IK, IV>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<T>? t = null) where C : ISeraColctor<T, R>
        => colctor.CMap(this, mapper, new Type<B>(), new Type<IK>(), new Type<IV>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public B Builder(int? cap, Type<B> b = default) => d.Builder(cap);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R CollectItem<R, C>(ref C colctor, Type<B> b = default) where C : IMapSeraColctor<B, IK, IV, R>
        => colctor.CItem(dk, dv, effector);
}

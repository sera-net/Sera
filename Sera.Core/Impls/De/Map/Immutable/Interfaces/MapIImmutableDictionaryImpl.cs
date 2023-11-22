using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using Sera.Core.Impls.De;
using Sera.Utils;

namespace Sera.Core.Impls.De;

public readonly struct MapIImmutableDictionaryImpl<T, IK, IV, DK, DV, N>(DK dk, DV dv, N ctor) :
    ISeraColion<T>, IMapSeraColion<T, IK, IV>
    where T : IImmutableDictionary<IK, IV>
    where DK : ISeraColion<IK>
    where DV : ISeraColion<IV>
    where N : ICapSeraCtor<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<T>? t = null) where C : ISeraColctor<T, R>
        => colctor.CMap(this, new IdentityMapper<T>(), new Type<T>(), new Type<IK>(), new Type<IV>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Builder(int? cap, Type<T> b = default) => ctor.Ctor(cap);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R CollectItem<R, C>(ref C colctor, Type<T> b = default) where C : IMapSeraColctor<T, IK, IV, R>
        => colctor.CItem(dk, dv, new MapIImmutableDictionaryEffector<T, IK, IV>());
}

public readonly struct MapIImmutableDictionaryEffector<T, IK, IV> : ISeraEffector<T, KeyValuePair<IK, IV>>
    where T : IImmutableDictionary<IK, IV>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Effect(ref T target, KeyValuePair<IK, IV> value) => target = (T)target.Add(value.Key, value.Value);
}

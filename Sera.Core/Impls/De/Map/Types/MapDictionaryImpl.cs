using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sera.Core.Abstract;
using Sera.Core.Impls.De;
using Sera.Utils;

namespace Sera.Core.Impls.De;

public readonly struct MapDictionaryImpl<IK, IV, DK, DV>(DK dk, DV dv, bool useAdd = false, IKeyAbility? keyAbility = null) :
    ISeraColion<Dictionary<IK, IV>>,
    IMapSeraColion<Dictionary<IK, IV>, IK, IV>
    where DK : ISeraColion<IK>
    where DV : ISeraColion<IV>
    where IK : notnull
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<Dictionary<IK, IV>>? t = null)
        where C : ISeraColctor<Dictionary<IK, IV>, R>
        => colctor.CMap(this, new IdentityMapper<Dictionary<IK, IV>>(),
            new Type<Dictionary<IK, IV>>(), new Type<IK>(), new Type<IV>(), keyAbility);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Dictionary<IK, IV> Builder(int? cap, Type<Dictionary<IK, IV>> b = default)
        => new DictCapCtor<IK, IV>().Ctor(cap, (InType<Dictionary<IK, IV>>?)null);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R CollectItem<R, C>(ref C colctor, Type<Dictionary<IK, IV>> b = default)
        where C : IMapSeraColctor<Dictionary<IK, IV>, IK, IV, R>
        => colctor.CItem(dk, dv, new MapDictionaryEffector<IK, IV>(useAdd));
}

public readonly struct MapDictionaryEffector<IK, IV>(bool useAdd = false)
    : ISeraEffector<Dictionary<IK, IV>, KeyValuePair<IK, IV>>
    where IK : notnull
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Effect(ref Dictionary<IK, IV> target, KeyValuePair<IK, IV> value)
    {
        if (useAdd) target.Add(value.Key, value.Value);
        else target[value.Key] = value.Value;
    }
}

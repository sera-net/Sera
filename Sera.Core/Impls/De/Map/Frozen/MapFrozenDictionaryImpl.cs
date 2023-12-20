using System.Collections.Frozen;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sera.Core.Abstract;
using Sera.Utils;

namespace Sera.Core.Impls.De;

public readonly struct MapFrozenDictionaryImpl<IK, IV, DK, DV>(DK dk, DV dv, IKeyAbility? keyAbility = null)
    : ISeraColion<FrozenDictionary<IK, IV>>
    where DK : ISeraColion<IK>
    where DV : ISeraColion<IV>
    where IK : notnull
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<FrozenDictionary<IK, IV>>? t = null)
        where C : ISeraColctor<FrozenDictionary<IK, IV>, R>
        => colctor.CMap(new MapListImpl<IK, IV, DK, DV>(dk, dv), new ListToFrozenDictionaryMapper<IK, IV>(),
            new Type<List<KeyValuePair<IK, IV>>>(), new Type<IK>(), new Type<IV>(), keyAbility);
}

public readonly struct ListToFrozenDictionaryMapper<IK, IV>
    : ISeraMapper<List<KeyValuePair<IK, IV>>, FrozenDictionary<IK, IV>>
    where IK : notnull
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FrozenDictionary<IK, IV> Map(List<KeyValuePair<IK, IV>> value, InType<FrozenDictionary<IK, IV>>? u = null)
        => value.ToFrozenDictionary();
}

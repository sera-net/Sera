using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sera.Core.Abstract;
using Sera.Core.Impls.De;
using Sera.Utils;

namespace Sera.Core.Impls.De;

public readonly struct MapListImpl<IK, IV, DK, DV>(DK dk, DV dv, ASeraTypeAbility? keyAbility = null) :
    ISeraColion<List<KeyValuePair<IK, IV>>>,
    IMapSeraColion<List<KeyValuePair<IK, IV>>, IK, IV>
    where DK : ISeraColion<IK>
    where DV : ISeraColion<IV>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<List<KeyValuePair<IK, IV>>>? t = null)
        where C : ISeraColctor<List<KeyValuePair<IK, IV>>, R>
        => colctor.CMap(this, new IdentityMapper<List<KeyValuePair<IK, IV>>>(),
            new Type<List<KeyValuePair<IK, IV>>>(), new Type<IK>(), new Type<IV>(), keyAbility);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public List<KeyValuePair<IK, IV>> Builder(int? cap, Type<List<KeyValuePair<IK, IV>>> b = default)
        => new SeqCapCtor<KeyValuePair<IK, IV>>().Ctor(cap, (InType<List<KeyValuePair<IK, IV>>>?)null);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R CollectItem<R, C>(ref C colctor, Type<List<KeyValuePair<IK, IV>>> b = default)
        where C : IMapSeraColctor<List<KeyValuePair<IK, IV>>, IK, IV, R>
        => colctor.CItem(dk, dv, new SeqListEffector<KeyValuePair<IK, IV>>());
}

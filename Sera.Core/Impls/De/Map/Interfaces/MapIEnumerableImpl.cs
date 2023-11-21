using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sera.Utils;

namespace Sera.Core.Impls.De;

public readonly struct MapIEnumerableImpl<IK, IV, DK, DV>(DK dk, DV dv)
    : ISeraColion<IEnumerable<KeyValuePair<IK, IV>>>
    where DK : ISeraColion<IK>
    where DV : ISeraColion<IV>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<IEnumerable<KeyValuePair<IK, IV>>>? t = null)
        where C : ISeraColctor<IEnumerable<KeyValuePair<IK, IV>>, R>
        => colctor.CMap(new MapListImpl<IK, IV, DK, DV>(dk, dv), new List2IEnumerableMapper<KeyValuePair<IK, IV>>(),
            new Type<List<KeyValuePair<IK, IV>>>(), new Type<IK>(), new Type<IV>());
}

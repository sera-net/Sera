using System.Collections.Generic;

namespace Sera.Core.Impls.Ser;

public readonly struct EntryImpl<IK, IV, DK, DV>(DK dk, DV dv) :
    ITypeVision<KeyValuePair<IK, IV>>
    where DK : ITypeVision<IK> where DV : ITypeVision<IV>
{
    public R Accept<R, V>(V visitor, KeyValuePair<IK, IV> value) where V : ATypeVisitor<R>
        => visitor.VEntry(dk, dv, value.Key, value.Value);
    
    public R AcceptInMap<R, V>(V visitor, KeyValuePair<IK, IV> value) where V : AMapTypeVisitor<R>
        => visitor.VEntry(dk, dv, value.Key, value.Value);
}

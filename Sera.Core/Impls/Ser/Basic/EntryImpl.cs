using System.Collections.Generic;

namespace Sera.Core.Impls.Ser;

public readonly struct EntryImpl<IK, IV, DK, DV>(DK dk, DV dv) :
    ISeraVision<KeyValuePair<IK, IV>>
    where DK : ISeraVision<IK> where DV : ISeraVision<IV>
{
    public R Accept<R, V>(V visitor, KeyValuePair<IK, IV> value) where V : ASeraVisitor<R>
        => visitor.VEntry(dk, dv, value.Key, value.Value);
}

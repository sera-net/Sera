using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sera.Utils;

namespace Sera.Core.Impls.De;

public readonly struct EntryImpl<IK, IV, DK, DV>(DK dk, DV dv) :
    ISeraColion<KeyValuePair<IK, IV>>, IEntrySeraColion<(IK, IV)>
    where DK : ISeraColion<IK> where DV : ISeraColion<IV>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<KeyValuePair<IK, IV>>? t = null)
        where C : ISeraColctor<KeyValuePair<IK, IV>, R>
        => colctor.CEntry(this, new ValueTupleToKeyValuePairMapper<IK, IV>(), new Type<(IK, IV)>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public (IK, IV) Builder(Type<(IK, IV)> b = default) => default;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R CollectKey<R, C>(ref C colctor, Type<(IK, IV)> b = default) where C : IEntrySeraColctor<(IK, IV), R>
        => colctor.CItem(dk, new ValueTupleEffector<IK, IV>.Item1(), new Type<IK>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R CollectValue<R, C>(ref C colctor, Type<(IK, IV)> b = default) where C : IEntrySeraColctor<(IK, IV), R>
        => colctor.CItem(dv, new ValueTupleEffector<IK, IV>.Item2(), new Type<IV>());
}

public readonly struct ValueTupleToKeyValuePairMapper<IK, IV> : ISeraMapper<(IK, IV), KeyValuePair<IK, IV>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public KeyValuePair<IK, IV> Map((IK, IV) value, InType<KeyValuePair<IK, IV>>? u = null)
        => new(value.Item1, value.Item2);
}

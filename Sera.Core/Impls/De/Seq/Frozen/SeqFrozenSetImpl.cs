using System.Collections.Frozen;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sera.Utils;

namespace Sera.Core.Impls.De;

public readonly struct SeqFrozenSetImpl<I, D>(D d) : ISeraColion<FrozenSet<I>>
    where D : ISeraColion<I>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<FrozenSet<I>>? t = null) where C : ISeraColctor<FrozenSet<I>, R>
        => colctor.CSeq(new SeqListImpl<I, D>(d), new ListToFrozenSetMapper<I>(), new Type<List<I>>(), new Type<I>());
}

public readonly struct ListToFrozenSetMapper<I> : ISeraMapper<List<I>, FrozenSet<I>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FrozenSet<I> Map(List<I> value, InType<FrozenSet<I>>? u = null)
        => value.ToFrozenSet();
}

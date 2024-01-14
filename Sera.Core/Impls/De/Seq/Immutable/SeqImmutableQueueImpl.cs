using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Sera.Utils;

namespace Sera.Core.Impls.De;

public readonly struct SeqImmutableQueueImpl<I, D>(D d) :
    ISeraColion<ImmutableQueue<I>>
    where D : ISeraColion<I>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<ImmutableQueue<I>>? t = null)
        where C : ISeraColctor<ImmutableQueue<I>, R>
        => colctor.CSeq(new SeqListImpl<I, D>(d), new ListToImmutableQueueMapper<I>(),
            new Type<List<I>>(), new Type<I>());
}

public readonly struct ListToImmutableQueueMapper<I>
    : ISeraMapper<List<I>, ImmutableQueue<I>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ImmutableQueue<I> Map(List<I> value, InType<ImmutableQueue<I>>? u = null)
        => ImmutableQueue.Create<I>(CollectionsMarshal.AsSpan(value));
}

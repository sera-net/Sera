using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Sera.Utils;

namespace Sera.Core.Impls.De;

public readonly struct SeqImmutableStackImpl<I, D>(D d) :
    ISeraColion<ImmutableStack<I>>
    where D : ISeraColion<I>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<ImmutableStack<I>>? t = null)
        where C : ISeraColctor<ImmutableStack<I>, R>
        => colctor.CSeq(new SeqListImpl<I, D>(d), new ListToImmutableStackMapper<I>(),
            new Type<List<I>>(), new Type<I>());
}

public readonly struct ListToImmutableStackMapper<I>
    : ISeraMapper<List<I>, ImmutableStack<I>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ImmutableStack<I> Map(List<I> value, InType<ImmutableStack<I>>? u = null)
        => ImmutableStack.Create<I>(CollectionsMarshal.AsSpan(value));
}

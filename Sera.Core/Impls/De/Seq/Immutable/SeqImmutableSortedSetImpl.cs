using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using Sera.Utils;

namespace Sera.Core.Impls.De;

public readonly struct SeqImmutableSortedSetImpl<I, D>(D d) :
    ISeraColion<ImmutableSortedSet<I>>, ISeqSeraColion<ImmutableSortedSet<I>.Builder, I>
    where D : ISeraColion<I>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<ImmutableSortedSet<I>>? t = null)
        where C : ISeraColctor<ImmutableSortedSet<I>, R>
        => colctor.CSeq(this, new ImmutableSortedSetBuilderToImmutableSortedSetMapper<I>(),
            new Type<ImmutableSortedSet<I>.Builder>(), new Type<I>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ImmutableSortedSet<I>.Builder Builder(int? cap, Type<ImmutableSortedSet<I>.Builder> b = default) =>
        ImmutableSortedSet.CreateBuilder<I>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R CollectItem<R, C>(ref C colctor, Type<ImmutableSortedSet<I>.Builder> b = default)
        where C : ISeqSeraColctor<ImmutableSortedSet<I>.Builder, I, R>
        => colctor.CItem(d, new SeqImmutableSortedSetBuilderEffector<I>());
}

public readonly struct ImmutableSortedSetBuilderToImmutableSortedSetMapper<I>
    : ISeraMapper<ImmutableSortedSet<I>.Builder, ImmutableSortedSet<I>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ImmutableSortedSet<I> Map(ImmutableSortedSet<I>.Builder value, InType<ImmutableSortedSet<I>>? u = null)
        => value.ToImmutable();
}

public readonly struct SeqImmutableSortedSetBuilderEffector<I> : ISeraEffector<ImmutableSortedSet<I>.Builder, I>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Effect(ref ImmutableSortedSet<I>.Builder target, I value) => target.Add(value);
}

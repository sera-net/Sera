using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using Sera.Utils;

namespace Sera.Core.Impls.De;

public readonly struct SeqImmutableHashSetImpl<I, D>(D d) :
    ISeraColion<ImmutableHashSet<I>>, ISeqSeraColion<ImmutableHashSet<I>.Builder, I>
    where D : ISeraColion<I>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<ImmutableHashSet<I>>? t = null)
        where C : ISeraColctor<ImmutableHashSet<I>, R>
        => colctor.CSeq(this, new ImmutableHashSetBuilderToImmutableHashSetMapper<I>(),
            new Type<ImmutableHashSet<I>.Builder>(), new Type<I>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ImmutableHashSet<I>.Builder Builder(int? cap, Type<ImmutableHashSet<I>.Builder> b = default) =>
        ImmutableHashSet.CreateBuilder<I>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R CollectItem<R, C>(ref C colctor, Type<ImmutableHashSet<I>.Builder> b = default)
        where C : ISeqSeraColctor<ImmutableHashSet<I>.Builder, I, R>
        => colctor.CItem(d, new SeqImmutableHashSetBuilderEffector<I>());
}

public readonly struct ImmutableHashSetBuilderToImmutableHashSetMapper<I>
    : ISeraMapper<ImmutableHashSet<I>.Builder, ImmutableHashSet<I>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ImmutableHashSet<I> Map(ImmutableHashSet<I>.Builder value, InType<ImmutableHashSet<I>>? u = null)
        => value.ToImmutable();
}

public readonly struct SeqImmutableHashSetBuilderEffector<I> : ISeraEffector<ImmutableHashSet<I>.Builder, I>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Effect(ref ImmutableHashSet<I>.Builder target, I value) => target.Add(value);
}

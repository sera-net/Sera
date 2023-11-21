using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using Sera.Utils;

namespace Sera.Core.Impls.De;

public readonly struct SeqImmutableArrayImpl<I, D>(D d) :
    ISeraColion<ImmutableArray<I>>, ISeqSeraColion<ImmutableArray<I>.Builder, I>
    where D : ISeraColion<I>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<ImmutableArray<I>>? t = null)
        where C : ISeraColctor<ImmutableArray<I>, R>
        => colctor.CSeq(this, new ImmutableArrayBuilderToImmutableArrayMapper<I>(),
            new Type<ImmutableArray<I>.Builder>(), new Type<I>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ImmutableArray<I>.Builder Builder(int? cap, Type<ImmutableArray<I>.Builder> b = default) =>
        ImmutableArray.CreateBuilder<I>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R CollectItem<R, C>(ref C colctor, Type<ImmutableArray<I>.Builder> b = default)
        where C : ISeqSeraColctor<ImmutableArray<I>.Builder, I, R>
        => colctor.CItem(d, new SeqImmutableArrayBuilderEffector<I>());
}

public readonly struct ImmutableArrayBuilderToImmutableArrayMapper<I>
    : ISeraMapper<ImmutableArray<I>.Builder, ImmutableArray<I>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ImmutableArray<I> Map(ImmutableArray<I>.Builder value, InType<ImmutableArray<I>>? u = null)
        => value.ToImmutable();
}

public readonly struct SeqImmutableArrayBuilderEffector<I> : ISeraEffector<ImmutableArray<I>.Builder, I>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Effect(ref ImmutableArray<I>.Builder target, I value) => target.Add(value);
}

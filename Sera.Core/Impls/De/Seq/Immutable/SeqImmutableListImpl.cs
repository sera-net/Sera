using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using Sera.Utils;

namespace Sera.Core.Impls.De;

public readonly struct SeqImmutableListImpl<I, D>(D d) :
    ISeraColion<ImmutableList<I>>, ISeqSeraColion<ImmutableList<I>.Builder, I>
    where D : ISeraColion<I>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<ImmutableList<I>>? t = null)
        where C : ISeraColctor<ImmutableList<I>, R>
        => colctor.CSeq(this, new ImmutableListBuilderToImmutableListMapper<I>(),
            new Type<ImmutableList<I>.Builder>(), new Type<I>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ImmutableList<I>.Builder Builder(int? cap, Type<ImmutableList<I>.Builder> b = default) =>
        ImmutableList.CreateBuilder<I>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R CollectItem<R, C>(ref C colctor, Type<ImmutableList<I>.Builder> b = default)
        where C : ISeqSeraColctor<ImmutableList<I>.Builder, I, R>
        => colctor.CItem(d, new SeqImmutableListBuilderEffector<I>());
}

public readonly struct ImmutableListBuilderToImmutableListMapper<I>
    : ISeraMapper<ImmutableList<I>.Builder, ImmutableList<I>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ImmutableList<I> Map(ImmutableList<I>.Builder value, InType<ImmutableList<I>>? u = null)
        => value.ToImmutable();
}

public readonly struct SeqImmutableListBuilderEffector<I> : ISeraEffector<ImmutableList<I>.Builder, I>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Effect(ref ImmutableList<I>.Builder target, I value) => target.Add(value);
}

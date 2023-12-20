using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using Sera.Core.Abstract;
using Sera.Utils;

namespace Sera.Core.Impls.De;

public readonly struct MapImmutableSortedDictionaryImpl<IK, IV, DK, DV>(DK dk, DV dv, IKeyAbility? keyAbility = null)
    : ISeraColion<ImmutableSortedDictionary<IK, IV>>, IMapSeraColion<ImmutableSortedDictionary<IK, IV>.Builder, IK, IV>
    where DK : ISeraColion<IK>
    where DV : ISeraColion<IV>
    where IK : notnull
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<ImmutableSortedDictionary<IK, IV>>? t = null)
        where C : ISeraColctor<ImmutableSortedDictionary<IK, IV>, R>
        => colctor.CMap(this, new ImmutableSortedDictionaryBuilderToImmutableSortedDictionaryMapper<IK, IV>(),
            new Type<ImmutableSortedDictionary<IK, IV>.Builder>(), new Type<IK>(), new Type<IV>(), keyAbility);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ImmutableSortedDictionary<IK, IV>.Builder Builder(int? cap,
        Type<ImmutableSortedDictionary<IK, IV>.Builder> b = default)
        => ImmutableSortedDictionary.CreateBuilder<IK, IV>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R CollectItem<R, C>(ref C colctor, Type<ImmutableSortedDictionary<IK, IV>.Builder> b = default)
        where C : IMapSeraColctor<ImmutableSortedDictionary<IK, IV>.Builder, IK, IV, R>
        => colctor.CItem(dk, dv, new MapImmutableSortedDictionaryBuilderEffector<IK, IV>());
}

public readonly struct ImmutableSortedDictionaryBuilderToImmutableSortedDictionaryMapper<IK, IV>
    : ISeraMapper<ImmutableSortedDictionary<IK, IV>.Builder, ImmutableSortedDictionary<IK, IV>>
    where IK : notnull
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ImmutableSortedDictionary<IK, IV> Map(ImmutableSortedDictionary<IK, IV>.Builder value,
        InType<ImmutableSortedDictionary<IK, IV>>? u = null)
        => value.ToImmutableSortedDictionary();
}

public readonly struct MapImmutableSortedDictionaryBuilderEffector<IK, IV>
    : ISeraEffector<ImmutableSortedDictionary<IK, IV>.Builder, KeyValuePair<IK, IV>>
    where IK : notnull
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Effect(ref ImmutableSortedDictionary<IK, IV>.Builder target, KeyValuePair<IK, IV> value) =>
        target.Add(value.Key, value.Value);
}

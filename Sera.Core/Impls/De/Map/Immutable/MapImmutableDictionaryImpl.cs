using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using Sera.Core.Abstract;
using Sera.Utils;

namespace Sera.Core.Impls.De;

public readonly struct MapImmutableDictionaryImpl<IK, IV, DK, DV>(DK dk, DV dv, IKeyAbility? keyAbility = null)
    : ISeraColion<ImmutableDictionary<IK, IV>>, IMapSeraColion<ImmutableDictionary<IK, IV>.Builder, IK, IV>
    where DK : ISeraColion<IK>
    where DV : ISeraColion<IV>
    where IK : notnull
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<ImmutableDictionary<IK, IV>>? t = null)
        where C : ISeraColctor<ImmutableDictionary<IK, IV>, R>
        => colctor.CMap(this, new ImmutableDictionaryBuilderToImmutableDictionaryMapper<IK, IV>(),
            new Type<ImmutableDictionary<IK, IV>.Builder>(), new Type<IK>(), new Type<IV>(), keyAbility);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ImmutableDictionary<IK, IV>.Builder Builder(int? cap, Type<ImmutableDictionary<IK, IV>.Builder> b = default)
        => ImmutableDictionary.CreateBuilder<IK, IV>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R CollectItem<R, C>(ref C colctor, Type<ImmutableDictionary<IK, IV>.Builder> b = default)
        where C : IMapSeraColctor<ImmutableDictionary<IK, IV>.Builder, IK, IV, R>
        => colctor.CItem(dk, dv, new MapImmutableDictionaryBuilderEffector<IK, IV>());
}

public readonly struct ImmutableDictionaryBuilderToImmutableDictionaryMapper<IK, IV>
    : ISeraMapper<ImmutableDictionary<IK, IV>.Builder, ImmutableDictionary<IK, IV>>
    where IK : notnull
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ImmutableDictionary<IK, IV> Map(ImmutableDictionary<IK, IV>.Builder value,
        InType<ImmutableDictionary<IK, IV>>? u = null)
        => value.ToImmutableDictionary();
}

public readonly struct MapImmutableDictionaryBuilderEffector<IK, IV>
    : ISeraEffector<ImmutableDictionary<IK, IV>.Builder, KeyValuePair<IK, IV>>
    where IK : notnull
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Effect(ref ImmutableDictionary<IK, IV>.Builder target, KeyValuePair<IK, IV> value) =>
        target.Add(value.Key, value.Value);
}

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Text;
using Sera.Utils;

namespace Sera.Core.Impls.De;

public readonly struct DynamicImpl : ISeraColion<object?>, ISelectSeraColion<object?>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<object?>? t = null) where C : ISeraColctor<object?, R>
        => colctor.CSelect(this, new IdentityMapper<object?>(), new Type<object?>());

    public ReadOnlyMemory<Any.Kind>? Priorities
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R SelectPrimitive<R, C>(ref C colctor, InType<object?>? t = null) where C : ISelectSeraColctor<object?, R>
        => colctor.CSome(new SeraPrimitiveImpl(), new SeraPrimitiveToObjectMapper(), new Type<SeraPrimitive>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R SelectString<R, C>(ref C colctor, InType<object?>? t = null)
        where C : ISelectSeraColctor<object?, R>
        => colctor.CSome(new StringImpl(), new ToObjectMapper<string>(), new Type<string>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R SelectBytes<R, C>(ref C colctor, InType<object?>? t = null) where C : ISelectSeraColctor<object?, R>
        => colctor.CSome(new BytesImpl(), new ToObjectMapper<byte[]>(), new Type<byte[]>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R SelectArray<R, C>(ref C colctor, InType<object?>? t = null) where C : ISelectSeraColctor<object?, R>
        => colctor.CSome(new ArrayImpl<object?, DynamicImpl>(this), new ToObjectMapper<object?[]>(),
            new Type<object?[]>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R SelectUnit<R, C>(ref C colctor, InType<object?>? t = null) where C : ISelectSeraColctor<object?, R>
        => colctor.CSome(new UnitAsNullImpl<object>(), new IdentityMapper<object?>(), new Type<object?>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R SelectOption<R, C>(ref C colctor, InType<object?>? t = null) where C : ISelectSeraColctor<object?, R>
        => colctor.CSome(new NullableNullableClassImpl<object, DynamicImpl>(this), new IdentityMapper<object?>(),
            new Type<object?>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R SelectEntry<R, C>(ref C colctor, InType<object?>? t = null) where C : ISelectSeraColctor<object?, R>
        => colctor.CSome(new DynamicEntryImpl(), new IdentityMapper<object>(), new Type<object>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R SelectTuple<R, C>(ref C colctor, int? size, InType<object?>? t = null)
        where C : ISelectSeraColctor<object?, R>
        => colctor.CSome(new TupleListImpl<object?, DynamicImpl>(new(), null), new ToObjectMapper<List<object?>>(),
            new Type<List<object?>>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R SelectSeq<R, C>(ref C colctor, int? size, InType<object?>? t = null)
        where C : ISelectSeraColctor<object?, R>
        => colctor.CSome(new SeqListImpl<object?, DynamicImpl>(new()), new ToObjectMapper<List<object?>>(),
            new Type<List<object?>>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R SelectMap<R, C>(ref C colctor, int? size, InType<object?>? t = null)
        where C : ISelectSeraColctor<object?, R>
        => colctor.CSome(
            new MapIDictionaryImpl<DynamicMap, object?, object?, DynamicImpl, DynamicImpl, NewCtor<DynamicMap>>(
                new(), new(), new()), new ToObjectMapper<DynamicMap>(), new Type<DynamicMap>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R SelectStruct<R, C>(ref C colctor, string? name, int? size, InType<object?>? t = null)
        where C : ISelectSeraColctor<object?, R>
        => colctor.CSome(new DynamicStructImpl(), new IdentityMapper<object?>(), new Type<object?>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R SelectUnion<R, C>(ref C colctor, string? name, InType<object?>? t = null)
        where C : ISelectSeraColctor<object?, R>
    {
        throw new NotImplementedException();
    }
}

public readonly struct DynamicEntryImpl : ISeraColion<ExpandoObject>, IEntrySeraColion<ExpandoObject>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<ExpandoObject>? t = null) where C : ISeraColctor<ExpandoObject, R>
        => colctor.CEntry(this, new IdentityMapper<ExpandoObject>(), new Type<ExpandoObject>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ExpandoObject Builder(Type<ExpandoObject> b = default) => new();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R CollectKey<R, C>(ref C colctor, Type<ExpandoObject> b = default)
        where C : IEntrySeraColctor<ExpandoObject, R>
        => colctor.CItem(new DynamicImpl(), new KeyEffector(), new Type<object?>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R CollectValue<R, C>(ref C colctor, Type<ExpandoObject> b = default)
        where C : IEntrySeraColctor<ExpandoObject, R>
        => colctor.CItem(new DynamicImpl(), new ValueEffector(), new Type<object?>());

    private readonly struct KeyEffector : ISeraEffector<ExpandoObject, object?>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Effect(ref ExpandoObject target, object? value)
            => ((IDictionary<string, object?>)target)["Key"] = value;
    }

    private readonly struct ValueEffector : ISeraEffector<ExpandoObject, object?>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Effect(ref ExpandoObject target, object? value)
            => ((IDictionary<string, object?>)target)["Value"] = value;
    }
}

public readonly struct DynamicStructImpl : ISeraColion<ExpandoObject>, IStructSeraColion<ExpandoObject>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<ExpandoObject>? t = null) where C : ISeraColctor<ExpandoObject, R>
        => colctor.CStruct(this, new IdentityMapper<ExpandoObject>(), new Type<ExpandoObject>());

    public SeraFieldInfos? Fields
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ExpandoObject Builder(string? name, Type<ExpandoObject> b = default) => new();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R CollectField<R, C>(ref C colctor, int field, string? name, long? key, Type<ExpandoObject> b = default)
        where C : IStructSeraColctor<ExpandoObject, R>
        => colctor.CField(new DynamicImpl(), new DynamicFieldEffector(name ?? key?.ToString() ?? $"{field}"),
            new Type<object?>());

    private readonly struct DynamicFieldEffector(string name) : ISeraEffector<ExpandoObject, object?>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Effect(ref ExpandoObject target, object? value)
            => ((IDictionary<string, object?>)target)[name] = value;
    }
}

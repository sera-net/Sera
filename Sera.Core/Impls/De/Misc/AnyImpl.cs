using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Sera.Core.Abstract;
using Sera.Utils;

namespace Sera.Core.Impls.De;

public readonly struct AnyImpl(SeraFormats? formats = null) : ISeraColion<Any>, ISelectSeraColion<Any>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<Any>? t = null) where C : ISeraColctor<Any, R>
        => colctor.CSelect(this, new IdentityMapper<Any>(), new Type<Any>());

    public ReadOnlyMemory<Any.Kind>? Priorities
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R SelectPrimitive<R, C>(ref C colctor, InType<Any>? t = null)
        where C : ISelectSeraColctor<Any, R>
        => colctor.CSome(new SeraPrimitiveImpl(), new PrimitiveMapper(), new Type<SeraPrimitive>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R SelectString<R, C>(ref C colctor, Encoding encoding, InType<Any>? t = null)
        where C : ISelectSeraColctor<Any, R>
        => colctor.CSome(new StringImpl(), new StringMapper(), new Type<string>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R SelectBytes<R, C>(ref C colctor, InType<Any>? t = null)
        where C : ISelectSeraColctor<Any, R>
        => colctor.CSome(new BytesImpl(), new BytesMapper(), new Type<byte[]>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R SelectArray<R, C>(ref C colctor, InType<Any>? t = null)
        where C : ISelectSeraColctor<Any, R>
        => colctor.CSome(new ArrayImpl<Any, AnyImpl>(this), new ArrayMapper(), new Type<Any[]>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R SelectUnit<R, C>(ref C colctor, InType<Any>? t = null)
        where C : ISelectSeraColctor<Any, R>
        => colctor.CSome(new UnitImpl<Unit>(), new UnitMapper(), new Type<Unit>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R SelectOption<R, C>(ref C colctor, InType<Any>? t = null)
        where C : ISelectSeraColctor<Any, R>
        => colctor.CSome(new NullableImpl<Any, AnyImpl>(this), new OptionMapper(), new Type<Any?>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R SelectEntry<R, C>(ref C colctor, InType<Any>? t = null)
        where C : ISelectSeraColctor<Any, R>
        => colctor.CSome(new EntryImpl<Any, Any, AnyImpl, AnyImpl>(this, this),
            new EntryMapper(), new Type<KeyValuePair<Any, Any>>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R SelectTuple<R, C>(ref C colctor, int? size, InType<Any>? t = null)
        where C : ISelectSeraColctor<Any, R>
        => colctor.CSome(new TupleListImpl<Any, AnyImpl>(this, size),
            new TupleMapper(), new Type<List<Any>>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R SelectSeq<R, C>(ref C colctor, int? size, InType<Any>? t = null)
        where C : ISelectSeraColctor<Any, R>
        => colctor.CSome(new SeqListImpl<Any, AnyImpl>(this), new SeqMapper(), new Type<List<Any>>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R SelectMap<R, C>(ref C colctor, int? size, InType<Any>? t = null)
        where C : ISelectSeraColctor<Any, R>
        => colctor.CSome(
            new MapDictionaryImpl<Any, Any, AnyImpl, AnyImpl>(this, this, keyAbility: StringSeraTypeAbility.Instance),
            new MapMapper(), new Type<Dictionary<Any, Any>>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R SelectStruct<R, C>(ref C colctor, string? name, int? size, InType<Any>? t = null)
        where C : ISelectSeraColctor<Any, R>
        => colctor.CSome(new AnyStructImpl(this), new StructMapper(), new Type<AnyStruct>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R SelectUnion<R, C>(ref C colctor, string? name, InType<Any>? t = null)
        where C : ISelectSeraColctor<Any, R>
        => throw new NotImplementedException();

    public readonly struct PrimitiveMapper : ISeraMapper<SeraPrimitive, Any>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Any Map(SeraPrimitive value, InType<Any>? u = null)
            => Any.MakePrimitive(new(value));
    }

    public readonly struct OptionMapper : ISeraMapper<Any?, Any>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Any Map(Any? value, InType<Any>? u = null) => Any.MakeOption(new(value));
    }

    public readonly struct StringMapper : ISeraMapper<string, Any>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Any Map(string value, InType<Any>? u = null) => Any.MakeString(value);
    }

    public readonly struct BytesMapper : ISeraMapper<byte[], Any>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Any Map(byte[] value, InType<Any>? u = null) => Any.MakeBytes(value);
    }

    public readonly struct ArrayMapper : ISeraMapper<Any[], Any>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Any Map(Any[] value, InType<Any>? u = null) => Any.MakeArray(value);
    }

    public readonly struct UnitMapper : ISeraMapper<Unit, Any>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Any Map(Unit value, InType<Any>? u = null) => Any.MakeUnit();
    }

    public readonly struct EntryMapper : ISeraMapper<KeyValuePair<Any, Any>, Any>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Any Map(KeyValuePair<Any, Any> value, InType<Any>? u = null) =>
            Any.MakeEntry(new AnyEntry(value.Key, value.Value));
    }

    public readonly struct TupleMapper : ISeraMapper<List<Any>, Any>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Any Map(List<Any> value, InType<Any>? u = null) => Any.MakeTuple(value);
    }

    public readonly struct SeqMapper : ISeraMapper<List<Any>, Any>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Any Map(List<Any> value, InType<Any>? u = null) => Any.MakeSeq(value);
    }

    public readonly struct MapMapper : ISeraMapper<Dictionary<Any, Any>, Any>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Any Map(Dictionary<Any, Any> value, InType<Any>? u = null) => Any.MakeMap(value);
    }

    public readonly struct StructMapper : ISeraMapper<AnyStruct, Any>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Any Map(AnyStruct value, InType<Any>? u = null) => Any.MakeStruct(value);
    }
}

public readonly struct AnyStructImpl(AnyImpl impl) : ISeraColion<AnyStruct>, IStructSeraColion<AnyStruct.Builder>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<AnyStruct>? t = null) where C : ISeraColctor<AnyStruct, R>
        => colctor.CStruct(this, new BuilderMapper(), new Type<AnyStruct.Builder>());

    public SeraFieldInfos? Fields
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public AnyStruct.Builder Builder(string? name, Type<AnyStruct.Builder> b = default) => new(name);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R CollectField<R, C>(ref C colctor, int field, string? name, long? key, Type<AnyStruct.Builder> b = default)
        where C : IStructSeraColctor<AnyStruct.Builder, R>
        => colctor.CField(impl, new BuilderEffector(field, name, key), new Type<Any>());

    public readonly struct BuilderMapper : ISeraMapper<AnyStruct.Builder, AnyStruct>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AnyStruct Map(AnyStruct.Builder value, InType<AnyStruct>? u = null)
            => value.Build();
    }

    public readonly struct BuilderEffector(int field, string? name, long? key) : ISeraEffector<AnyStruct.Builder, Any>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Effect(ref AnyStruct.Builder target, Any value)
            => target.Add((name, key, value));
    }
}

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sera.Utils;

namespace Sera.Core.Impls.De;

public readonly struct AnyImpl(SeraFormats? formats = null) : ISeraColion<Any>, ISelectSeraColion<Any>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<Any>? t = null) where C : ISeraColctor<Any, R>
        => colctor.CSelect(this, new IdentityMapper<Any>(), new Type<Any>());

    public ReadOnlyMemory<SelectKind>? Priority => null;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R CollectSelect<R, C>(ref C colctor, SeraSelect select, InType<Any>? t = null)
        where C : ISelectSeraColctor<Any, R>
        => select switch
        {
            { Tag: SelectKind.Primitive } => throw new NotImplementedException(),
            { Tag: SelectKind.String } =>
                colctor.CSome(new StringImpl(), new StringMapper(), new Type<string>()),
            { Tag: SelectKind.Bytes } => throw new NotImplementedException(),
            // colctor.CSome(new BytesImpl(), new BytesMapper(), new Type<byte[]>()),
            { Tag: SelectKind.Array } =>
                colctor.CSome(new ArrayImpl<Any, AnyImpl>(this), new ArrayMapper(), new Type<Any[]>()),
            { Tag: SelectKind.Unit } =>
                colctor.CSome(new UnitImpl<Unit>(), new UnitMapper(), new Type<Unit>()),
            { Tag: SelectKind.Option } =>
                colctor.CSome(new NullableImpl<Any, AnyImpl>(this), new OptionMapper(), new Type<Any?>()),
            { Tag: SelectKind.Entry } =>
                colctor.CSome(new EntryImpl<Any, Any, AnyImpl, AnyImpl>(this, this),
                    new EntryMapper(), new Type<KeyValuePair<Any, Any>>()),
            { Tag: SelectKind.Tuple, Tuple.Size: var size } =>
                colctor.CSome(new TupleArrayImpl<Any, AnyImpl>(this, size),
                    new TupleMapper(), new Type<Any[]>()),
            { Tag: SelectKind.Seq } =>
                colctor.CSome(new SeqListImpl<Any, AnyImpl>(this), new SeqMapper(), new Type<List<Any>>()),
            { Tag: SelectKind.Map } =>
                colctor.CSome(new MapDictionaryImpl<Any, Any, AnyImpl, AnyImpl>(this, this), new MapMapper(),
                    new Type<Dictionary<Any, Any>>()),
            { Tag: SelectKind.Struct } =>
                colctor.CSome(new AnyStructImpl(this), new StructMapper(), new Type<AnyStruct>()),
            { Tag: SelectKind.Union } => throw new NotImplementedException(),
            _ => colctor.CNone(),
        };

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

    public readonly struct TupleMapper : ISeraMapper<Any[], Any>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Any Map(Any[] value, InType<Any>? u = null) => Any.MakeTuple(value);
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

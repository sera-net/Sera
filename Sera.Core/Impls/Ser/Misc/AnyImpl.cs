using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Sera.Core.Impls.Ser;

public readonly struct AnyImpl(SeraFormats? formats = null) : ISeraVision<Any>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, Any value) where V : ASeraVisitor<R>
        => value switch
        {
            { Tag: Any.Kind.Primitive, Primitive.Value: var Primitive } =>
                new SeraPrimitiveImpl().Accept<R, V>(visitor, Primitive),

            { Tag: Any.Kind.String, String: var String } =>
                new StringImpl().Accept<R, V>(visitor, String),

            { Tag: Any.Kind.Bytes, Bytes: var Bytes } =>
                new BytesImpl().Accept<R, V>(visitor, Bytes),

            { Tag: Any.Kind.Array, Array: var Array } =>
                new ArrayImpl<Any, AnyImpl>(this).Accept<R, V>(visitor, Array),

            { Tag: Any.Kind.Unit } =>
                visitor.VUnit(),

            { Tag: Any.Kind.Option, Option.Value: var Option } =>
                new NullableImpl<Any, AnyImpl>(this).Accept<R, V>(visitor, Option),

            { Tag: Any.Kind.Entry, Entry: var Entry } =>
                new AnyEntryImpl(this).Accept<R, V>(visitor, Entry),

            { Tag: Any.Kind.Tuple, Tuple: var Tuple } =>
                new TupleIListImpl<List<Any>, Any, AnyImpl>(this, value.Tuple.Count).Accept<R, V>(visitor, Tuple),

            { Tag: Any.Kind.Seq, Seq: var Seq } =>
                new SeqICollectionImpl<List<Any>, Any, AnyImpl>(this).Accept<R, V>(visitor, Seq),

            { Tag: Any.Kind.Map, Map: var Map } =>
                new MapICollectionImpl<Dictionary<Any, Any>, Any, Any, AnyImpl, AnyImpl>(this, this)
                    .Accept<R, V>(visitor, Map),

            { Tag: Any.Kind.Struct, Struct: var Struct } =>
                visitor.VStruct(new AnyStructImpl(this, Struct), Struct),

            { Tag: Any.Kind.Union, Union: var Union } =>
                visitor.VUnion(new AnyUnionImpl(this, Union), Union),

            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
}

public readonly struct AnyEntryImpl(AnyImpl impl) : ISeraVision<AnyEntry>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, AnyEntry value) where V : ASeraVisitor<R>
        => visitor.VEntry(impl, impl, value.Key, value.Value);
}

public readonly struct AnyStructImpl(AnyImpl impl, AnyStruct value)
    : ISeraVision<AnyStruct>, IStructSeraVision<AnyStruct>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, AnyStruct _value) where V : ASeraVisitor<R>
        => visitor.VStruct(this, value);

    public string? Name => value.Name;
    public int Count => value.Count;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R AcceptField<R, V>(V visitor, ref AnyStruct _value, int field) where V : AStructSeraVisitor<R>
        => value.TryGet(field, out var info, out var item)
            ? visitor.VField(impl, item, info.Name, info.Key)
            : visitor.VNone();
}

public readonly struct AnyUnionImpl(AnyImpl impl, AnyUnion value) : IUnionSeraVision<AnyUnion>
{
    public string? Name => value.Name;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R AcceptUnion<R, V>(V visitor, ref AnyUnion value) where V : AUnionSeraVisitor<R>
        => value.Variant is var (variant, val)
            ? val switch
            {
                { Tag: AnyVariantValue.Kind.None } =>
                    visitor.VVariant(variant),

                { Tag: AnyVariantValue.Kind.Value, Value: var Value } =>
                    visitor.VVariantValue(impl, Value, variant),

                { Tag: AnyVariantValue.Kind.Tuple, Tuple: var Tuple } =>
                    visitor.VVariantTuple(
                        new TupleIListImpl<List<Any>, Any, AnyImpl>(impl, Tuple.Count), Tuple, variant),

                { Tag: AnyVariantValue.Kind.Struct, Struct: var Struct } =>
                    visitor.VVariantStruct(new AnyStructImpl(impl, Struct), Struct, variant),

                _ => visitor.VNone(),
            }
            : visitor.VEmpty();
}

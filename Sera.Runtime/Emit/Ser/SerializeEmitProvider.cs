using System;
using System.Buffers;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Sera.Core;
using Sera.Core.Impls.Ser;
using Sera.Runtime.Emit.Ser.Internal;
using Sera.Runtime.Utils;

namespace Sera.Runtime.Emit.Ser;

internal class SerializeEmitProvider : AEmitProvider
{
    public static readonly EmitTransform[] NullableClassImplTransforms =
        { new Transforms._NullableClassImpl() };

    public static readonly EmitTransform[] ReferenceTypeTransforms =
        { new Transforms._ReferenceImpl() };

    #region impls

    private static readonly object BytesImpl = new BytesImpl();
    private static readonly object StringImpl = new StringImpl();

    private static readonly FrozenDictionary<Type, object> bytes_impl = new Dictionary<Type, object>
    {
        { typeof(byte[]), BytesImpl },
        { typeof(Memory<byte>), BytesImpl },
        { typeof(ReadOnlyMemory<byte>), BytesImpl },
        { typeof(ReadOnlySequence<byte>), BytesImpl },
    }.ToFrozenDictionary();

    private static readonly FrozenDictionary<Type, object> string_impl = new Dictionary<Type, object>
    {
        { typeof(string), StringImpl },
        { typeof(char[]), StringImpl },
        { typeof(Memory<char>), StringImpl },
        { typeof(ReadOnlyMemory<char>), StringImpl },
    }.ToFrozenDictionary();

    #endregion

    private bool TryGetStaticImpl(Type type, out object? impl)
    {
        var method = ReflectionUtils.StaticSerImpls_TryGet.MakeGenericMethod(type);
        var args = new object?[] { null };
        var r = (bool)method.Invoke(null, args)!;
        impl = args[0];
        return r;
    }

    public ISeraVision<T> Get<T>(SeraStyles styles)
    {
        var stub = Emit(new(TypeMetas.GetTypeMeta(typeof(T)), styles));
        return (ISeraVision<T>)stub.GetResult()!;
    }

    protected override EmitJob CreateJob(EmitMeta target)
    {
        if (target.Type.IsByRefLike || target.Type.IsByRef)
            throw new ArgumentException($"ByRefType is not support; {target.Type}");
        if (PrimitiveImpl.IsPrimitiveType(target.Type)) return new Jobs._Primitive();
        if (target.Type == typeof(object))
            return new Jobs._Static(typeof(ReferenceImpl<object, EmptyStructImpl<object>>),
                new ReferenceImpl<object, EmptyStructImpl<object>>());
        if (TryGetStaticImpl(target.Type, out var inst)) return new Jobs._Static(inst!.GetType(), inst);
        if (target.Styles.As is SeraAs.Bytes && bytes_impl.TryGetValue(target.Type, out inst))
            return new Jobs._Static(inst.GetType(), inst);
        if (target.Styles.As is SeraAs.String && string_impl.TryGetValue(target.Type, out inst))
            return new Jobs._Static(inst.GetType(), inst);
        var struct_sera_attr = target.Type.GetCustomAttribute<SeraAttribute>();
        if (target.IsEnum) return CreateEnumJob(target, struct_sera_attr);
        if (target.IsArray) return CreateArrayJob(target);
        if (target.IsTuple(out var is_value_tuple)) return CreateTupleJob(target, is_value_tuple);
        if (target.Type.IsGenericType)
        {
            var generic_def = target.Type.GetGenericTypeDefinition();
            if (generic_def == typeof(ReadOnlySequence<>))
                return CreateOtherArrayJob(target, target.Type.GetGenericArguments()[0]);
            if (generic_def == typeof(ReadOnlyMemory<>))
                return CreateOtherArrayJob(target, target.Type.GetGenericArguments()[0]);
            if (generic_def == typeof(Memory<>))
                return CreateOtherArrayJob(target, target.Type.GetGenericArguments()[0]);
            if (generic_def == typeof(Nullable<>)) return CreateNullableJob(target);
            if (generic_def == typeof(KeyValuePair<,>)) return CreateKeyValuePairJob(target);
        }
        // var job = CreateJobCollectionLike(target);
        // job ??= CreateJobFSharpTypes(target);
        // return job ?? CreateStructJob(target);
        return CreateStructJob(target, struct_sera_attr);
    }

    // private EmitJob? CreateJobCollectionLike(EmitMeta target)
    // {
    //     if (
    //         target.Type.IsCollectionLike(
    //             out var CollectionKind, out var item_type, out var key_type, out var interface_mapping
    //         )
    //     )
    //     {
    //         switch (CollectionKind)
    //         {
    //             case CollectionLikeKind.IEnumerable:
    //                 if (target.Styles.As is SeraAs.Map)
    //                 {
    //                     if (item_type.IsOpenTypeEq(typeof(KeyValuePair<,>)))
    //                     {
    //                         var kv = item_type.GetGenericArguments();
    //                         return CreateIEnumerableMapJob(target, kv[0], kv[1], interface_mapping);
    //                     }
    //                 }
    //                 return CreateIEnumerableJob(target, item_type, interface_mapping);
    //             case CollectionLikeKind.ICollection:
    //                 if (target.Styles.As is SeraAs.Map)
    //                 {
    //                     if (item_type.IsOpenTypeEq(typeof(KeyValuePair<,>)))
    //                     {
    //                         var kv = item_type.GetGenericArguments();
    //                         return CreateICollectionMapJob(target, kv[0], kv[1], interface_mapping);
    //                     }
    //                 }
    //                 return CreateICollectionJob(target, item_type, interface_mapping);
    //             case CollectionLikeKind.IReadOnlyCollection:
    //                 if (target.Styles.As is SeraAs.Map)
    //                 {
    //                     if (item_type.IsOpenTypeEq(typeof(KeyValuePair<,>)))
    //                     {
    //                         var kv = item_type.GetGenericArguments();
    //                         return CreateIReadOnlyCollectionMapJob(target, kv[0], kv[1], interface_mapping);
    //                     }
    //                 }
    //                 return CreateIReadOnlyCollectionJob(target, item_type, interface_mapping);
    //             case CollectionLikeKind.IDictionary:
    //                 if (target.Styles.As is SeraAs.Seq)
    //                     return CreateICollectionJob(target,
    //                         typeof(KeyValuePair<,>).MakeGenericType(key_type!, item_type), interface_mapping);
    //                 return CreateIDictionaryJob(target, key_type!, item_type, interface_mapping);
    //             case CollectionLikeKind.IReadOnlyDictionary:
    //                 if (target.Styles.As is SeraAs.Seq)
    //                     return CreateIReadOnlyCollectionJob(target,
    //                         typeof(KeyValuePair<,>).MakeGenericType(key_type!, item_type), interface_mapping);
    //                 return CreateIReadOnlyDictionaryJob(target, key_type!, item_type, interface_mapping);
    //             default:
    //                 throw new ArgumentOutOfRangeException(nameof(CollectionKind),
    //                     $"Unknown CollectionKind {CollectionKind}");
    //         }
    //     }
    //     if (target.Type.IsIDictionary()) return CreateIDictionaryLegacyJob();
    //     if (target.Type.IsICollection()) return CreateICollectionLegacyJob();
    //     if (target.Type.IsIEnumerable()) return CreateIEnumerableLegacyJob();
    //     return null;
    // }
    //
    // private EmitJob? CreateJobFSharpTypes(EmitMeta target)
    // {
    //     if (target.Type.IsGenericType)
    //     {
    //         var generic_def = target.Type.GetGenericTypeDefinition();
    //         if (generic_def.FullName == "Microsoft.FSharp.Core.FSharpOption`1")
    //             return CreateFSharpOptionJob(target, generic_def);
    //     }
    //     return null;
    // }

    private EmitJob CreateStructJob(EmitMeta target, SeraAttribute? struct_sera_attr)
    {
        var struct_attr = target.Type.GetCustomAttribute<SeraStructAttribute>();
        var name = struct_sera_attr?.Name ?? target.Type.Name; // todo rename
        var members = StructReflectionUtils.GetStructMembers(target.Type, SerOrDe.Ser, struct_sera_attr, struct_attr);
        if (target.Type.IsVisible && members.All(m => m.Type.IsVisible))
        {
            return new Jobs._Struct._Public(name, members);
        }
        else
        {
            return new Jobs._Struct._Private(name, members);
        }
    }

    private EmitJob CreateEnumJob(EmitMeta target, SeraAttribute? struct_sera_attr)
    {
        var underlying_type = target.Type.GetEnumUnderlyingType();
        var name = struct_sera_attr?.Name ?? target.Type.Name; // todo rename
        var flags = target.Type.GetCustomAttribute<FlagsAttribute>() != null;
        if (flags)
        {
            var flags_attr = target.Type.GetCustomAttribute<SeraFlagsAttribute>();
            var mode = flags_attr?.Mode ?? SeraFlagsMode.Seq;
            return mode switch
            {
                SeraFlagsMode.Seq => new Jobs._Enum._Flags_Seq(underlying_type,
                    EnumUtils.GetEnumInfo(target.Type, underlying_type, distinct: false)),
                SeraFlagsMode.Number => new Jobs._Enum._Flags_Number(underlying_type),
                SeraFlagsMode.String => new Jobs._Enum._Flags_String(underlying_type),
                SeraFlagsMode.StringSplit => new Jobs._Enum._Flags_StringSplit(underlying_type),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        else
        {
            var union_attr = target.Type.GetCustomAttribute<SeraUnionAttribute>();
            var format_attr = target.Type.GetCustomAttribute<SeraFormatsAttribute>();
            var mode = union_attr?.Mode ?? SeraUnionMode.None;
            var style = UnionStyle.FromAttr(union_attr, format_attr);
            var items = EnumUtils.GetEnumInfo(target.Type, underlying_type, distinct: true);
            if (target.Type.IsVisible)
            {
                var jump_table = EnumUtils.TryMakeJumpTable(underlying_type, items);
                return new Jobs._Enum._Variant_Public(name, underlying_type, items, jump_table, style, mode);
            }
            else
            {
                return new Jobs._Enum._Variant_Private(underlying_type, items, style, mode);
            }
        }
    }

    private EmitJob CreateArrayJob(EmitMeta target)
    {
        var item_type = target.Type.GetElementType()!;
        if (target.IsSZArray)
        {
            return new Jobs._Array(item_type);
        }
        throw new NotSupportedException("Multidimensional and non-zero lower bound arrays are not supported");
    }

    private EmitJob CreateOtherArrayJob(EmitMeta target, Type item_type)
        => new Jobs._Array(item_type);

    private EmitJob CreateTupleJob(EmitMeta target, bool is_value_tuple)
    {
        var item_types = target.Type.GetGenericArguments();
        return new Jobs._Tuples(is_value_tuple, item_types);
    }

    private EmitJob CreateNullableJob(EmitMeta target)
    {
        var item_type = target.Type.GetGenericArguments()[0];
        return new Jobs._Nullable(item_type);
    }

    private EmitJob CreateKeyValuePairJob(EmitMeta target)
    {
        var generic = target.Type.GetGenericArguments();
        var key_type = generic[0];
        var val_type = generic[1];
        return new Jobs._KeyValuePair(key_type, val_type);
    }

    // private MethodInfo? GetDirectGetEnumerator(EmitMeta target)
    //     => target.Type.GetMethod(nameof(IEnumerable<int>.GetEnumerator),
    //         BindingFlags.Public | BindingFlags.Instance,
    //         Array.Empty<Type>());
    //
    // private EmitJob CreateIEnumerableJob(EmitMeta target, Type item_type, InterfaceMapping? mapping)
    // {
    //     var direct_get_enumerator = GetDirectGetEnumerator(target);
    //     if (target.Type.IsVisible && item_type.IsVisible)
    //         return new Jobs._IEnumerable._Generic._Public(item_type, mapping, direct_get_enumerator);
    //     return new Jobs._IEnumerable._Generic._Private(item_type);
    // }
    //
    // private EmitJob CreateIEnumerableLegacyJob()
    //     => new Jobs._IEnumerable._Legacy();
    //
    // private EmitJob CreateICollectionJob(EmitMeta target, Type item_type, InterfaceMapping? mapping)
    // {
    //     var direct_get_enumerator = GetDirectGetEnumerator(target);
    //     if (target.Type.IsVisible && item_type.IsVisible)
    //         return new Jobs._ICollection._Generic._Mutable_Public(item_type, mapping, direct_get_enumerator);
    //     return new Jobs._ICollection._Generic._Mutable_Private(item_type);
    // }
    //
    // private EmitJob CreateIReadOnlyCollectionJob(EmitMeta target, Type item_type, InterfaceMapping? mapping)
    // {
    //     var direct_get_enumerator = GetDirectGetEnumerator(target);
    //     if (target.Type.IsVisible && item_type.IsVisible)
    //         return new Jobs._ICollection._Generic._ReadOnly_Public(item_type, mapping, direct_get_enumerator);
    //     return new Jobs._ICollection._Generic._ReadOnly_Private(item_type);
    // }
    //
    // private EmitJob CreateICollectionLegacyJob()
    //     => new Jobs._ICollection._Legacy();
    //
    // private EmitJob CreateIDictionaryJob(EmitMeta target, Type key_type, Type item_type, InterfaceMapping? mapping)
    // {
    //     var direct_get_enumerator = GetDirectGetEnumerator(target);
    //     if (target.Type.IsVisible && key_type.IsVisible && item_type.IsVisible)
    //         return new Jobs._IDictionary._Generic._Mutable_Public(key_type, item_type, mapping, direct_get_enumerator);
    //     return new Jobs._IDictionary._Generic._Mutable_Private(key_type, item_type);
    // }
    //
    // private EmitJob CreateIReadOnlyDictionaryJob(EmitMeta target, Type key_type, Type item_type,
    //     InterfaceMapping? mapping)
    // {
    //     var direct_get_enumerator = GetDirectGetEnumerator(target);
    //     if (target.Type.IsVisible && key_type.IsVisible && item_type.IsVisible)
    //         return new Jobs._IDictionary._Generic._ReadOnly_Public(key_type, item_type, mapping, direct_get_enumerator);
    //     return new Jobs._IDictionary._Generic._ReadOnly_Private(key_type, item_type);
    // }
    //
    // private EmitJob CreateIDictionaryLegacyJob()
    //     => new Jobs._IDictionary._Legacy();
    //
    // private EmitJob CreateIEnumerableMapJob(EmitMeta target, Type key_type, Type item_type, InterfaceMapping? mapping)
    // {
    //     var direct_get_enumerator = GetDirectGetEnumerator(target);
    //     if (target.Type.IsVisible && key_type.IsVisible && item_type.IsVisible)
    //         return new Jobs._IDictionary._Generic._IEnumerable_Public(
    //             key_type, item_type, mapping, direct_get_enumerator
    //         );
    //     return new Jobs._IDictionary._Generic._IEnumerable_Private(key_type, item_type);
    // }
    //
    // private EmitJob CreateICollectionMapJob(EmitMeta target, Type key_type, Type item_type, InterfaceMapping? mapping)
    // {
    //     var direct_get_enumerator = GetDirectGetEnumerator(target);
    //     if (target.Type.IsVisible && key_type.IsVisible && item_type.IsVisible)
    //         return new Jobs._IDictionary._Generic._Mutable_Public(key_type, item_type, mapping, direct_get_enumerator);
    //     return new Jobs._IDictionary._Generic._ICollection_Private(key_type, item_type);
    // }
    //
    // private EmitJob CreateIReadOnlyCollectionMapJob(EmitMeta target, Type key_type, Type item_type,
    //     InterfaceMapping? mapping)
    // {
    //     var direct_get_enumerator = GetDirectGetEnumerator(target);
    //     if (target.Type.IsVisible && key_type.IsVisible && item_type.IsVisible)
    //         return new Jobs._IDictionary._Generic._ReadOnly_Public(key_type, item_type, mapping, direct_get_enumerator);
    //     return new Jobs._IDictionary._Generic._IReadOnlyCollection_Private(key_type, item_type);
    // }
    //
    // private EmitJob CreateFSharpOptionJob(EmitMeta target, Type generic_def)
    // {
    //     var item_type = target.Type.GetGenericArguments()[0];
    //     if (target.Type.IsVisible) return new Jobs._FSharpOption._Class_Public(item_type);
    //     return new Jobs._FSharpOption._Class_Private(generic_def, item_type);
    // }
}

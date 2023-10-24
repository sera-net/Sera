using System;
using System.Buffers;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Sera.Core;
using Sera.Core.Impls;
using Sera.Runtime.Emit.Ser.Internal;
using Sera.Runtime.Utils;

namespace Sera.Runtime.Emit.Ser;

internal class SerializeEmitProvider : AEmitProvider
{
    public static readonly EmitTransform[] NullableReferenceTypeTransforms =
        { new Transforms._NullableReferenceTypeSerializeImpl() };

    public static readonly EmitTransform[] ReferenceTypeTransforms =
        { new Transforms._ReferenceTypeWrapperSerializeImpl() };

    #region impls

    private static readonly FrozenDictionary<Type, object> bytes_impl = new Dictionary<Type, object>
    {
        { typeof(byte[]), BytesImpl.Instance },
        { typeof(List<byte>), ListBytesImpl.Instance },
        { typeof(Memory<byte>), MemoryBytesImpl.Instance },
        { typeof(ReadOnlyMemory<byte>), ReadOnlyMemoryBytesImpl.Instance },
        { typeof(ReadOnlySequence<byte>), ReadOnlySequenceBytesImpl.Instance },
    }.ToFrozenDictionary();

    private static readonly FrozenDictionary<Type, object> string_impl = new Dictionary<Type, object>
    {
        { typeof(char[]), ArrayStringImpl.Instance },
        { typeof(List<char>), ListStringImpl.Instance },
        { typeof(Memory<char>), MemoryStringImpl.Instance },
        { typeof(ReadOnlyMemory<char>), ReadOnlyMemoryStringImpl.Instance },
    }.ToFrozenDictionary();

    #endregion

    private bool TryGetStaticImpl(Type type, out object? impl)
    {
        var method = ReflectionUtils.StaticRuntimeProvider_TryGetSerialize.MakeGenericMethod(type);
        var args = new object?[] { null };
        var r = (bool)method.Invoke(StaticRuntimeProvider.Instance, args)!;
        impl = args[0];
        return r;
    }

    public ISerialize<T> GetSerialize<T>(SeraHints hints)
    {
        var stub = Emit(new(TypeMetas.GetTypeMeta(typeof(T)), hints));
        return (ISerialize<T>)stub.GetResult()!;
    }

    private readonly ConditionalWeakTable<Type, ISerialize<object>> RuntimeImplCache = new();

    public ISerialize<object> GetRuntimeSerialize(Type type) => RuntimeImplCache.GetValue(type, type =>
    {
        var stub = Emit(new(TypeMetas.GetTypeMeta(type), SeraHints.Default));
        var res = stub.GetResult()!;
        var impl = typeof(RuntimeSerializeImplWrapper<,>).MakeGenericType(type, res.GetType());
        var ctor = impl.GetConstructor(BindingFlags.Public | BindingFlags.Instance, new[] { res.GetType() })!;
        var inst = ctor.Invoke(new[] { res });
        return (ISerialize<object>)inst;
    });

    protected override EmitJob CreateJob(EmitMeta target)
    {
        if (target.Type.IsByRefLike || target.Type.IsByRef)
            throw new ArgumentException($"ByRefType is not support; {target.Type}");
        if (PrimitiveImpls.IsPrimitiveType(target.Type)) return new Jobs._Primitive();
        if (TryGetStaticImpl(target.Type, out var inst)) return new Jobs._Static(inst!.GetType(), inst);
        if (target.Data.As is SeraAs.Bytes && bytes_impl.TryGetValue(target.Type, out inst))
            return new Jobs._Static(inst.GetType(), inst);
        if (target.Data.As is SeraAs.String && string_impl.TryGetValue(target.Type, out inst))
            return new Jobs._Static(inst.GetType(), inst);
        if (target.IsArray) return CreateArrayJob(target);
        if (target.IsEnum) return CreateEnumJob(target);
        if (target.IsTuple(out var is_value_tuple)) return CreateTupleJob(target, is_value_tuple);
        if (target.Type.IsGenericType)
        {
            var generic_def = target.Type.GetGenericTypeDefinition();
            if (generic_def == typeof(ReadOnlySequence<>)) return CreateReadOnlySequenceJob(target);
            if (generic_def == typeof(ReadOnlyMemory<>)) return CreateReadOnlyMemoryJob(target);
            if (generic_def == typeof(Memory<>)) return CreateMemoryJob(target);
            if (generic_def == typeof(Nullable<>)) return CreateNullableJob(target);
            if (generic_def == typeof(KeyValuePair<,>)) return CreateKeyValuePairJob(target);
        }
        if (target.Type.IsListBase(out var item_type)) return CreateListJob(target, item_type);
        if (
            target.Type.IsCollectionLike(
                out var CollectionKind, out item_type, out var key_type, out var interface_mapping
            )
        )
        {
            switch (CollectionKind)
            {
                // todo kv seq as map
                case CollectionLikeKind.IEnumerable:
                    return CreateIEnumerableJob(target, item_type, interface_mapping);
                case CollectionLikeKind.ICollection:
                    return CreateICollectionJob(target, item_type, interface_mapping);
                case CollectionLikeKind.IReadOnlyCollection:
                    return CreateIReadOnlyCollectionJob(target, item_type, interface_mapping);
                case CollectionLikeKind.IDictionary:
                    if (target.Data.As is SeraAs.Seq)
                        return CreateICollectionJob(target,
                            typeof(KeyValuePair<,>).MakeGenericType(key_type!, item_type), interface_mapping);
                    return CreateIDictionaryJob(target, key_type!, item_type, interface_mapping);
                case CollectionLikeKind.IReadOnlyDictionary:
                    if (target.Data.As is SeraAs.Seq)
                        return CreateIReadOnlyCollectionJob(target,
                            typeof(KeyValuePair<,>).MakeGenericType(key_type!, item_type), interface_mapping);
                    return CreateIReadOnlyDictionaryJob(target, key_type!, item_type, interface_mapping);
                default:
                    throw new ArgumentOutOfRangeException(nameof(CollectionKind),
                        $"Unknown CollectionKind {CollectionKind}");
            }
        }
        if (target.Type.IsICollection()) return CreateICollectionLegacyJob();
        if (target.Type.IsIEnumerable()) return CreateIEnumerableLegacyJob();
        // todo other type
        return CreateStructJob(target);
    }

    private EmitJob CreateStructJob(EmitMeta target)
    {
        var members = StructReflectionUtils.GetStructMembers(target.Type, SerOrDe.Ser);
        if (target.Type.IsVisible && members.All(m => m.Type.IsVisible))
        {
            return new Jobs._Struct._Public(members);
        }
        else
        {
            return new Jobs._Struct._Private(members);
        }
    }

    private EmitJob CreateEnumJob(EmitMeta target)
    {
        var underlying_type = target.Type.GetEnumUnderlyingType();
        var flags = target.Type.GetCustomAttribute<FlagsAttribute>() != null;
        if (flags)
        {
            var flags_attr = target.Type.GetCustomAttribute<SeraFlagsAttribute>();
            var mode = flags_attr?.Mode ?? SeraFlagsMode.Array;
            return mode switch
            {
                SeraFlagsMode.Array => new Jobs._Enum._Flags_Array(underlying_type,
                    EnumUtils.GetEnumInfo(target.Type, underlying_type, distinct: false)),
                SeraFlagsMode.Number => new Jobs._Enum._Flags_Number(underlying_type),
                SeraFlagsMode.String => new Jobs._Enum._Flags_String(underlying_type),
                SeraFlagsMode.StringSplit => new Jobs._Enum._Flags_StringSplit(underlying_type),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        else
        {
            var enum_attr = target.Type.GetCustomAttribute<SeraEnumAttribute>();
            var items = EnumUtils.GetEnumInfo(target.Type, underlying_type, distinct: true);
            if (target.Type.IsVisible)
            {
                var jump_table = EnumUtils.TryMakeJumpTable(underlying_type, items);
                return new Jobs._Enum._Variant_Public(underlying_type, items, jump_table, enum_attr);
            }
            else
            {
                return new Jobs._Enum._Variant_Private(underlying_type, items, enum_attr);
            }
        }
    }

    private EmitJob CreateTupleJob(EmitMeta target, bool is_value_tuple)
    {
        var item_types = target.Type.GetGenericArguments();
        if (item_types.All(t => t.IsVisible))
        {
            return new Jobs._Tuples._Public(is_value_tuple, item_types);
        }
        else
        {
            return new Jobs._Tuples._Private(is_value_tuple, item_types);
        }
    }

    private EmitJob CreateArrayJob(EmitMeta target)
    {
        var item_type = target.Type.GetElementType()!;
        if (target.IsSZArray)
        {
            if (target.Type.IsVisible) return new Jobs._Array._SZ_Public(item_type);
            else return new Jobs._Array._SZ_Private(item_type);
        }
        throw new NotSupportedException("Multidimensional and non-zero lower bound arrays are not supported");
    }

    private EmitJob CreateListJob(EmitMeta target, Type item_type)
    {
        if (item_type == typeof(byte) && target.Data.As is SeraAs.Bytes) return new Jobs._Bytes_List();
        if (item_type == typeof(char) && target.Data.As is SeraAs.String) return new Jobs._String_List();
        if (target.Type.IsVisible && item_type.IsVisible) return new Jobs._Array._List_Public(item_type);
        return new Jobs._Array._List_Private(item_type);
    }

    private EmitJob CreateReadOnlySequenceJob(EmitMeta target)
    {
        var item_type = target.Type.GetGenericArguments()[0];
        if (target.Type.IsVisible) return new Jobs._Array._ReadOnlySequence_Public(item_type);
        return new Jobs._Array._ReadOnlySequence_Private(item_type);
    }

    private EmitJob CreateReadOnlyMemoryJob(EmitMeta target)
    {
        var item_type = target.Type.GetGenericArguments()[0];
        if (target.Type.IsVisible) return new Jobs._Array._ReadOnlyMemory_Public(item_type);
        return new Jobs._Array._ReadOnlyMemory_Private(item_type);
    }

    private EmitJob CreateMemoryJob(EmitMeta target)
    {
        var item_type = target.Type.GetGenericArguments()[0];
        if (target.Type.IsVisible) return new Jobs._Array._Memory_Public(item_type);
        return new Jobs._Array._Memory_Private(item_type);
    }

    private EmitJob CreateNullableJob(EmitMeta target)
    {
        var item_type = target.Type.GetGenericArguments()[0];
        if (target.Type.IsVisible) return new Jobs._Nullable._Public(item_type);
        return new Jobs._Nullable._Private(item_type);
    }

    private EmitJob CreateKeyValuePairJob(EmitMeta target)
    {
        var generic = target.Type.GetGenericArguments();
        var key_type = generic[0];
        var val_type = generic[1];
        if (target.Type.IsVisible) return new Jobs._KeyValuePair._Public(key_type, val_type);
        return new Jobs._KeyValuePair._Private(key_type, val_type);
    }

    private EmitJob CreateIEnumerableJob(EmitMeta target, Type item_type, InterfaceMapping? mapping)
    {
        var direct_get_enumerator = target.Type.GetMethod(nameof(IEnumerable<int>.GetEnumerator),
            BindingFlags.Public | BindingFlags.Instance,
            Array.Empty<Type>());
        if (target.Type.IsVisible && item_type.IsVisible)
            return new Jobs._IEnumerable._Generic._Public(item_type, mapping, direct_get_enumerator);
        return new Jobs._IEnumerable._Generic._Private(item_type);
    }

    private EmitJob CreateIEnumerableLegacyJob()
        => new Jobs._IEnumerable._Legacy();

    private EmitJob CreateICollectionJob(EmitMeta target, Type item_type, InterfaceMapping? mapping)
    {
        var direct_get_enumerator = target.Type.GetMethod(nameof(IEnumerable<int>.GetEnumerator),
            BindingFlags.Public | BindingFlags.Instance,
            Array.Empty<Type>());
        if (target.Type.IsVisible && item_type.IsVisible)
            return new Jobs._ICollection._Generic._Mutable_Public(item_type, mapping, direct_get_enumerator);
        return new Jobs._ICollection._Generic._Mutable_Private(item_type);
    }

    private EmitJob CreateIReadOnlyCollectionJob(EmitMeta target, Type item_type, InterfaceMapping? mapping)
    {
        var direct_get_enumerator = target.Type.GetMethod(nameof(IEnumerable<int>.GetEnumerator),
            BindingFlags.Public | BindingFlags.Instance,
            Array.Empty<Type>());
        if (target.Type.IsVisible && item_type.IsVisible)
            return new Jobs._ICollection._Generic._ReadOnly_Public(item_type, mapping, direct_get_enumerator);
        return new Jobs._ICollection._Generic._ReadOnly_Private(item_type);
    }

    private EmitJob CreateICollectionLegacyJob()
        => new Jobs._ICollection._Legacy();

    private EmitJob CreateIDictionaryJob(EmitMeta target, Type key_type, Type item_type, InterfaceMapping? mapping)
    {
        var direct_get_enumerator = target.Type.GetMethod(nameof(IEnumerable<int>.GetEnumerator),
            BindingFlags.Public | BindingFlags.Instance,
            Array.Empty<Type>());
        if (target.Type.IsVisible && key_type.IsVisible && item_type.IsVisible)
            return new Jobs.IDictionary._Generic._Mutable_Public(key_type, item_type, mapping, direct_get_enumerator);
        return new Jobs.IDictionary._Generic._Mutable_Private(key_type, item_type);
    }
    
    private EmitJob CreateIReadOnlyDictionaryJob(EmitMeta target, Type key_type, Type item_type, InterfaceMapping? mapping)
    {
        var direct_get_enumerator = target.Type.GetMethod(nameof(IEnumerable<int>.GetEnumerator),
            BindingFlags.Public | BindingFlags.Instance,
            Array.Empty<Type>());
        if (target.Type.IsVisible && key_type.IsVisible && item_type.IsVisible)
            return new Jobs.IDictionary._Generic._ReadOnly_Public(key_type, item_type, mapping, direct_get_enumerator);
        return new Jobs.IDictionary._Generic._ReadOnly_Private(key_type, item_type);
    }
}

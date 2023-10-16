using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sera.Core;
using Sera.Core.Impls;
using Sera.Runtime.Emit.Transform;
using Sera.Runtime.Utils;

namespace Sera.Runtime.Emit.Ser;

internal class SerializeEmitProvider : AEmitProvider
{
    #region Deps

    public static readonly EmitTransform[] NullableReferenceTypeTransforms =
        { new EmitTransformNullableReferenceTypeImpl() };

    public static readonly EmitTransform[] ReferenceTypeTransforms =
        { new EmitTransformReferenceTypeWrapperSerializeImpl() };

    private bool TryGetStaticImpl(Type type, out object? impl)
    {
        var method = ReflectionUtils.StaticRuntimeProvider_TryGetSerialize.MakeGenericMethod(type);
        var args = new object?[] { null };
        var r = (bool)method.Invoke(StaticRuntimeProvider.Instance, args)!;
        impl = args[0];
        return r;
    }

    #endregion

    public ISerialize<T> GetSerialize<T>()
    {
        var stub = Emit(new(TypeMetas.GetTypeMeta(typeof(T)), EmitData.Default));
        return (ISerialize<T>)stub.GetResult()!;
    }

    protected override EmitJob CreateJob(EmitMeta target)
    {
        // todo support span or custom impl
        if (target.Type.IsByRefLike || target.Type.IsByRef)
            throw new ArgumentException($"ByRefType is not support; {target.Type}");
        if (PrimitiveImpls.IsPrimitiveType(target.Type)) return new Jobs._Primitive();
        if (TryGetStaticImpl(target.Type, out var inst)) return new Jobs._Static(inst!.GetType(), inst);
        if (target.IsArray) return CreateArrayJob(target);
        if (target.IsEnum) return CreateEnumJob(target);
        if (target.IsTuple(out var is_value_tuple)) return CreateTupleJob(target, is_value_tuple);
        if (target.Type.IsAssignableTo2(typeof(List<>))) return CreateListJob(target);
        if (target.Type.IsAssignableTo2(typeof(ReadOnlySequence<>))) return CreateReadOnlySequenceJob(target);
        if (target.Type.IsAssignableTo2(typeof(ReadOnlyMemory<>))) return CreateReadOnlyMemoryJob(target);
        if (target.Type.IsAssignableTo2(typeof(Memory<>))) return CreateMemoryJob(target);
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
            if (item_type.IsVisible) return new Jobs._Array._SZ_Public(item_type);
            else return new Jobs._Array._SZ_Private(item_type);
        }
        throw new NotSupportedException("Multidimensional and non-zero lower bound arrays are not supported");
    }

    private EmitJob CreateListJob(EmitMeta target)
    {
        var item_type = target.Type.GetGenericArguments()[0];
        if (item_type.IsVisible) return new Jobs._Array._List_Public(item_type);
        return new Jobs._Array._List_Private(item_type);
    }

    private EmitJob CreateReadOnlySequenceJob(EmitMeta target)
    {
        var item_type = target.Type.GetGenericArguments()[0];
        if (item_type.IsVisible) return new Jobs._Array._ReadOnlySequence_Public(item_type);
        return new Jobs._Array._ReadOnlySequence_Private(item_type);
    }

    private EmitJob CreateReadOnlyMemoryJob(EmitMeta target)
    {
        var item_type = target.Type.GetGenericArguments()[0];
        if (item_type.IsVisible) return new Jobs._Array._ReadOnlyMemory_Public(item_type);
        return new Jobs._Array._ReadOnlyMemory_Private(item_type);
    }

    private EmitJob CreateMemoryJob(EmitMeta target)
    {
        var item_type = target.Type.GetGenericArguments()[0];
        if (item_type.IsVisible) return new Jobs._Array._Memory_Public(item_type);
        return new Jobs._Array._Memory_Private(item_type);
    }
}

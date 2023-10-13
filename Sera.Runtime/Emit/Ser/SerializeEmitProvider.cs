using System;
using System.Linq;
using System.Reflection;
using Sera.Core;
using Sera.Core.Impls;
using Sera.Runtime.Emit.Deps;
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
        if (PrimitiveImpls.IsPrimitiveType(target.Type)) return new Jobs._Primitive();
        if (TryGetStaticImpl(target.Type, out var inst)) return new Jobs._Static(inst!.GetType(), inst);
        if (target.IsArray) return CreateArrayJob(target);
        if (target.IsEnum) return CreateEnumJob(target);
        if (target.IsTuple(out var is_value_tuple)) return CreateTupleJob(target, is_value_tuple);
        // todo other type
        return CreateStructJob(target);
    }

    private EmitJob CreateStructJob(EmitMeta target)
    {
        var members = StructReflectionUtils.GetStructMembers(target.Type, SerOrDe.Ser);
        if (target.Type.IsVisible && members.All(m => m.Type.IsVisible))
        {
            return new Jobs._Struct_Public(members);
        }
        else
        {
            return new Jobs._Struct_Private(members);
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
                SeraFlagsMode.Array => new Jobs._Enum_Flags_Array(underlying_type,
                    EnumUtils.GetEnumInfo(target.Type, underlying_type, distinct: false)),
                SeraFlagsMode.Number => new Jobs._Enum_Flags_Number(underlying_type),
                SeraFlagsMode.String => new Jobs._Enum_Flags_String(underlying_type),
                SeraFlagsMode.StringSplit => new Jobs._Enum_Flags_StringSplit(underlying_type),
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
                return new Jobs._Enum_Variant_Public(underlying_type, items, jump_table, enum_attr);
            }
            else
            {
                return new Jobs._Enum_Variant_Private(underlying_type, items, enum_attr);
            }
        }
    }

    private EmitJob CreateArrayJob(EmitMeta target)
    {
        var item_type = target.Type.GetElementType()!;
        if (target.IsSZArray)
        {
            if (item_type.IsVisible) return new Jobs._Array_SZ_Public(item_type);
            else return new Jobs._Array_SZ_Private(item_type);
        }
        throw new NotSupportedException("Multidimensional and non-zero lower bound arrays are not supported");
    }

    private EmitJob CreateTupleJob(EmitMeta target, bool is_value_tuple)
    {
        var item_types = target.Type.GetGenericArguments();
        if (item_types.All(t => t.IsVisible))
        {
            return new Jobs._Tuples_Public(is_value_tuple, item_types);
        }
        else
        {
            return new Jobs._Tuples_Private(is_value_tuple, item_types);
        }
    }
}

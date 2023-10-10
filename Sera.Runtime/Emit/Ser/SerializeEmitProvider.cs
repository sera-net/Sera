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

    protected override DepMeta MakeRootDep(EmitMeta target)
        => new(target, target.IsValueType ? EmitTransform.EmptyTransforms : NullableReferenceTypeTransforms);

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
        if (target.IsEnum) return CreateEnumJob(target);
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
            throw new NotImplementedException();
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
}

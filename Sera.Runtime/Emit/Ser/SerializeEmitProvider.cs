using System;
using System.Linq;
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
        { new NullableReferenceTypeImplEmitTransform() };

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
        if (PrimitiveImpls.IsPrimitiveType(target.Type)) return new EmitPrimitiveSerJob();
        if (TryGetStaticImpl(target.Type, out var inst)) return new EmitStaticSerJob(inst!.GetType(), inst);
        // todo other type
        return CreateStructJob(target);
    }


    private EmitJob CreateStructJob(EmitMeta target)
    {
        var members = StructReflectionUtils.GetStructMembers(target.Type, SerOrDe.Ser);
        if (target.Type.IsVisible && members.All(m => m.Type.IsVisible))
        {
            return new EmitPublicStructSerJob(members);
        }
        else
        {
            return new EmitPrivateStructSerJob(members);
        }
    }
}

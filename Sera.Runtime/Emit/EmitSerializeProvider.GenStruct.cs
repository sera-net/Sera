using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using Sera.Core.Impls;
using Sera.Runtime.Utils;

namespace Sera.Runtime.Emit;

internal partial class EmitSerializeProvider
{
    private void GenStruct(TypeMeta target, CacheStub stub)
    {
        var members = StructReflectionUtils.GetStructMembers(target.Type, SerOrDe.Ser);
        if (target.Type.IsVisible && members.All(m => m.Type.IsVisible))
        {
            GenPublicStruct(target, members, stub);
        }
        else
        {
            GenPrivateStruct(target, members, stub);
        }
    }

    /// <returns>CacheStubDeps.Place is Field</returns>
    private Dictionary<Type, CacheStubDeps> GetSerDeps(
        StructMember[] members, TypeBuilder dep_container_type_builder, Thread current_thread
    )
    {
        var ser_impl_field_names = members.AsParallel()
            .Select(m =>
            {
                var ref_nullable = false;
                var null_meta = m.Kind switch
                {
                    PropertyOrField.Property => TypeMetas.GetNullabilityMeta(m.Property!),
                    PropertyOrField.Field => TypeMetas.GetNullabilityMeta(m.Field!),
                    _ => throw new ArgumentOutOfRangeException()
                };
                if (!m.Type.IsValueType)
                {
                    ref_nullable = null_meta?.NullabilityInfo?.ReadState != NullabilityState.NotNull;
                }
                return (m.Type, null_meta, ref_nullable);
            })
            .DistinctBy(m => (m.Type, m.null_meta, m.ref_nullable))
            .Select((m, i) => (i, m.Type, m.null_meta, m.ref_nullable))
            .ToDictionary(a => (a.Type, a.null_meta, a.ref_nullable), a => (a.i, $"_ser_impl_{a.i}"));

        var ser_deps = new Dictionary<Type, CacheStubDeps>();

        foreach (var ((value_type, null_meta, ref_nullable), (index, field_name)) in ser_impl_field_names)
        {
            var type_meta = TypeMetas.GetTypeMeta(value_type, null_meta);
            var (impl_type, impl_cell, impl) = GetSerImpl(type_meta, current_thread);
            var raw_impl_type = impl_type;
            if (ref_nullable)
            {
                impl_type = typeof(NullableReferenceTypeImpl<,>).MakeGenericType(value_type, impl_type);
            }
            var boxed = false;
            Type? boxed_type = null;
            if (impl_type.IsValueType && impl == null)
            {
                boxed_type = typeof(Box<>).MakeGenericType(impl_type);
                boxed = true;
            }
            var field = dep_container_type_builder.DefineField(field_name, boxed ? boxed_type! : impl_type,
                FieldAttributes.Public | FieldAttributes.Static);
            ser_deps.Add(
                value_type,
                new(index, new[] { index },
                    DepPlace.MakeField(field), 
                    impl_type, raw_impl_type, value_type, impl_cell, impl, ref_nullable,
                    boxed_type, boxed
                )
            );
        }

        return ser_deps;
    }
}

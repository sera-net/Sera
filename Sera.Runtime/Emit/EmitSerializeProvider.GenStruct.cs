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
    private void GenStruct(Type target, CacheStub stub)
    {
        var members = StructReflectionUtils.GetStructMembers(target, SerOrDe.Ser);
        if (target.IsVisible && members.All(m => m.Type.IsVisible))
        {
            GenPublicStruct(target, members, stub);
        }
        else
        {
            GenPrivateStruct(target, members, stub);
        }
    }
    
    /// <returns>CacheStubDeps.Field is not null</returns>
    private Dictionary<Type, CacheStubDeps> GetSerDeps(
        StructMember[] members, TypeBuilder dep_container_type_builder, Thread current_thread
    )
    {
        var ser_impl_field_names = members.AsParallel()
            .Select(m =>
            {
                var ref_nullable = false;
                if (!m.Type.IsValueType)
                {
                    var null_info = m.Kind switch
                    {
                        PropertyOrField.Property => nullabilityInfoContext.Create(m.Property!),
                        PropertyOrField.Field => nullabilityInfoContext.Create(m.Field!),
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    ref_nullable = null_info.ReadState != NullabilityState.NotNull;
                }
                return (m.Type, ref_nullable);
            })
            .DistinctBy(m => (m.Type, m.ref_nullable))
            .Select((m, i) => (i, m.Type, m.ref_nullable))
            .ToDictionary(a => (a.Type, a.ref_nullable), a => $"_ser_impl_{a.i}");

        var ser_deps = new Dictionary<Type, CacheStubDeps>();

        foreach (var ((value_type, ref_nullable), field_name) in ser_impl_field_names)
        {
            var (impl_type, impl_cell, impl) = GetSerImpl(value_type, current_thread);
            var raw_impl_type = impl_type;
            if (ref_nullable)
            {
                impl_type = typeof(NullableReferenceTypeImpl<,>).MakeGenericType(value_type, impl_type);
            }
            var field = dep_container_type_builder.DefineField(field_name, impl_type,
                FieldAttributes.Public | FieldAttributes.Static);
            ser_deps.Add(
                value_type,
                new(field, null, impl_type, raw_impl_type, value_type, impl_cell, impl, ref_nullable)
            );
        }

        return ser_deps;
    }
}

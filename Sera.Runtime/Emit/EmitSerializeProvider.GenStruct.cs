using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using Sera.Core;
using Sera.Core.Impls;
using Sera.Runtime.Utils;

namespace Sera.Runtime.Emit;

internal partial class EmitSerializeProvider
{
    /// <returns>Actual type: (Type, CacheStub | object)</returns>
    internal (Type impl_type, CacheStub? stub, object? impl) GetSerImpl(Type target, Thread thread)
    {
        Type impl_type;
        CacheStub? stub;
        if (TryGetStaticImpl(target, out var impl))
        {
            impl_type = impl!.GetType();
            stub = null;
        }
        else
        {
            stub = GetSerializeStub(target, thread);
            if (stub.CreateThread != thread)
            {
                stub.WaitType.WaitOne();
            }
            impl_type = stub.ser_type!;
        }
        return (impl_type, stub, impl);
    }

    internal bool TryGetStaticImpl(Type type, out object? impl)
    {
        var method = ReflectionUtils.StaticRuntimeProvider_TryGetSerialize.MakeGenericMethod(type);
        var args = new object?[] { null };
        var r = (bool)method.Invoke(StaticRuntimeProvider.Instance, args)!;
        impl = args[0];
        return r;
    }

    private static readonly NullabilityInfoContext nullabilityInfoContext = new();

    internal Dictionary<Type, CacheCellDeps> GetSerDeps(StructMember[] members, TypeBuilder dep_container_type_builder,
        Thread current_thread)
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

        var ser_deps = new Dictionary<Type, CacheCellDeps>();

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
                new(field, impl_type, raw_impl_type, value_type, impl_cell, impl, ref_nullable)
            );
        }

        return ser_deps;
    }
}

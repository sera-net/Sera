using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Sera.Core;
using Sera.Core.Impls;
using Sera.Runtime.Utils;

namespace Sera.Runtime.Emit;

internal partial class EmitSerializeProvider
{
    /// <summary>Actual type: (Type, CacheStub | object)</summary>
    internal record struct DepInfo(Type impl_type, CacheStub? stub, object? impl);

    internal record TheDep(Type dep_container_type, CacheStubDeps[] deps);

    internal record TheStaticDep(Type dep_container_type, CacheStubDeps[] deps) : TheDep(dep_container_type, deps)
    {
        public Dictionary<int, (int index, CacheStubDeps deps)> IndexMap { get; } = deps
            .SelectMany(a => a.rawIndexes.Select(i => (i, a.index, a)))
            .DistinctBy(a => a.i)
            .ToDictionary(a => a.i, a => (a.index, a.a));


        public Type GetSerWarp(int i)
        {
            var (mapped, dep) = IndexMap[i];
            var type = ReflectionUtils.DepsSerWraps[mapped]
                .MakeGenericType(dep.ValueType, dep.ImplType, dep_container_type);
            return type;
        }

        public Type GetSeqSerReceiverWarp(int i)
        {
            var (mapped, dep) = IndexMap[i];
            var type = ReflectionUtils.DepsSeqSerReceiverWraps[mapped]
                .MakeGenericType(dep.ValueType, dep.ImplType, dep_container_type);
            return type;
        }
    }

    private DepInfo GetSerImpl(TypeMeta target, Thread thread)
    {
        Type impl_type;
        CacheStub? stub;
        if (TryGetStaticImpl(target.Type, out var impl))
        {
            impl_type = impl!.GetType();
            stub = null;
        }
        else
        {
            stub = GetSerializeStub(target, thread);
            if (stub.CreateThread != thread)
            {
                stub.WaitTypeProvided();
            }
            impl_type = stub.SerType;
        }
        return new(impl_type, stub, impl);
    }

    private bool TryGetStaticImpl(Type type, out object? impl)
    {
        var method = ReflectionUtils.StaticRuntimeProvider_TryGetSerialize.MakeGenericMethod(type);
        var args = new object?[] { null };
        var r = (bool)method.Invoke(StaticRuntimeProvider.Instance, args)!;
        impl = args[0];
        return r;
    }

    /// <summary>Make dep with less than 8 generics</summary>
    private TheStaticDep MakeDepContainer(GenericMeta generics, Thread thread)
    {
        if (generics.Length is > 8 or <= 0) throw new NotSupportedException();

        var tmp1 = generics.Metas.AsParallel().AsOrdered()
            .Select((meta, i) => (i, value_type: generics.RawTypes[i], meta))
            .Select(a =>
            {
                var (i, value_type, meta) = a;
                var ref_nullable = !meta.KeepRaw && !value_type.IsValueType && meta.Nullability is not
                    { NullabilityInfo.ReadState: NullabilityState.NotNull };
                return (i, value_type, meta, ref_nullable);
            })
            .ToArray();
        var grouped = tmp1
            .GroupBy(a => (a.value_type, a.meta.Nullability, a.ref_nullable))
            .Select(g =>
            {
                var (_, value_type, meta, ref_nullable) = g.First();
                var indexes = g.Select(a => a.i).ToArray();
                return (value_type, meta, ref_nullable, indexes);
            })
            .ToArray();
        var infos = grouped.AsParallel().AsOrdered()
            .Select(g =>
            {
                var (value_type, meta, ref_nullable, indexes) = g;
                var (impl_type, impl_cell, impl) = GetSerImpl(meta, thread);
                var raw_impl_type = impl_type;
                if (ref_nullable)
                {
                    impl_type = typeof(NullableReferenceTypeImpl<,>).MakeGenericType(value_type, impl_type);
                }
                return (dep: new DepInfo(impl_type, impl_cell, impl), value_type, ref_nullable, raw_impl_type, indexes);
            })
            .ToArray();

        var deps_type = ReflectionUtils.DepsContainers[infos.Length]
            .MakeGenericType(infos.Select(a => a.dep.impl_type).ToArray());

        var deps = infos.AsParallel().AsOrdered()
            .Select((a, i) =>
            {
                var ((impl_type, impl_cell, impl), value_type, ref_nullable, raw_impl_type, indexes) = a;
                return new CacheStubDeps(i, indexes,
                    DepPlace.MakeLateProperty($"Impl{i + 1}"),
                    impl_type, raw_impl_type, value_type, impl_cell, impl,
                    ref_nullable, null, false);
            })
            .ToArray();

        return new(deps_type, deps);
    }
}

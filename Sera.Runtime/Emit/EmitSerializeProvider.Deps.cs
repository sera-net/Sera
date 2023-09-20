using System;
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

    internal record struct TheDep(Type dep_container_type, IEnumerable<CacheStubDeps> deps);

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

    /// <summary>Make dep with less than 8 quantities</summary>
    private TheDep MakeDepContainer(DepInfo[] depInfos, GenericMeta generics, out Type[] impl_types)
    {
        if (depInfos.Length != generics.Length) throw new ArgumentException($"{nameof(depInfos)}.Length must == {nameof(generics)}.Length");
        if (depInfos.Length > 8) throw new NotSupportedException();

        var items = new (DepInfo dep, bool ref_nullable, Type raw_impl_type)[depInfos.Length];
        impl_types = new Type[depInfos.Length];
        for (var i = 0; i < depInfos.Length; i++)
        {
            var value_type = generics.RawTypes[i];
            var generic_meta = generics.Metas[i];
            var (impl_type, impl_cell, impl) = depInfos[i];
            var raw_impl_type = impl_type;
            var ref_nullable = !generic_meta.KeepRaw && !value_type.IsValueType;
            if (ref_nullable)
            {
                impl_type = typeof(NullableReferenceTypeImpl<,>).MakeGenericType(value_type, impl_type);
            }
            impl_types[i] = impl_type;
            items[i] = (new(impl_type, impl_cell, impl), ref_nullable, raw_impl_type);
        }

        var deps_type = ReflectionUtils.DepsContainers[depInfos.Length]
            .MakeGenericType(items.Select(a => a.dep.impl_type).ToArray());

        var deps = new List<CacheStubDeps>();
        for (var i = 0; i < depInfos.Length; i++)
        {
            var value_type = generics.RawTypes[i];
            var ((impl_type, impl_cell, impl), ref_nullable, raw_impl_type) = items[i];
            var prop = deps_type.GetProperty($"Impl{i + 1}", BindingFlags.Public | BindingFlags.Static);
            deps.Add(
                new(null, prop, impl_type, raw_impl_type, value_type, impl_cell, impl, ref_nullable)
            );
        }

        return new(deps_type, deps);
    }
}

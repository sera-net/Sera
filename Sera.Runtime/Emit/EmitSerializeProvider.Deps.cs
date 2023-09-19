using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Sera.Core;
using Sera.Runtime.Utils;

namespace Sera.Runtime.Emit;

internal partial class EmitSerializeProvider
{
    /// <summary>Actual type: (Type, CacheStub | object)</summary>
    internal record struct DepInfo(Type impl_type, CacheStub? stub, object? impl);

    internal record struct TheDep(Type dep_container_type, IEnumerable<CacheStubDeps> deps);

    private DepInfo GetSerImpl(Type target, Thread thread)
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
    private TheDep MakeDepContainer(DepInfo[] depInfos, Type[] types)
    {
        if (depInfos.Length != types.Length) throw new ArgumentException($"{nameof(depInfos)} must == {nameof(types)}");
        if (depInfos.Length > 8) throw new NotSupportedException();
        var deps_type = ReflectionUtils.DepsContainers[depInfos.Length]
            .MakeGenericType(depInfos.Select(a => a.impl_type).ToArray());

        var deps = new List<CacheStubDeps>();
        for (var i = 0; i < depInfos.Length; i++)
        {
            var value_type = types[i];
            var (impl_type, impl_cell, impl) = depInfos[i];
            var prop = deps_type.GetProperty($"Impl{i + 1}", BindingFlags.Public | BindingFlags.Static);
            deps.Add(
                new(null, prop, impl_type, impl_type, value_type, impl_cell, impl, false)
            );
        }

        return new(deps_type, deps);
    }
}

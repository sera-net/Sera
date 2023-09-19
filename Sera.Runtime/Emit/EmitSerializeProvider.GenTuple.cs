using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Sera.Core;
using Sera.Core.Impls;
using Sera.Core.Impls.Tuples;
using Sera.Runtime.Utils;

namespace Sera.Runtime.Emit;

internal partial class EmitSerializeProvider
{
    private void GenTuple(Type target, CacheStub stub)
    {
        var generics = target.GenericTypeArguments;
        if (generics.Length is not (> 0 and <= 8)) throw new ArgumentException($"{target} is not a tuple");
        if (generics.Length is 8)
        {
            var rest = generics[7];
            if (ReflectionUtils.IsTuple(rest))
            {
                GenTupleRest(target, generics, stub);
                return;
            }
        }
        GenTuple(target, generics, stub);
    }

    private void GenTuple(Type target, Type[] generics, CacheStub stub)
    {
        #region ready base type

        var is_value_tuple = ReflectionUtils.ValueTuples.Contains(target.GetGenericTypeDefinition());

        var base_type = (is_value_tuple
                ? ReflectionUtils.ValueTupleSerBaseImpls[generics.Length]
                : ReflectionUtils.ClassTupleSerBaseImpls[generics.Length])
            .MakeGenericType(generics);
        stub.ProvideType(base_type);

        #endregion

        #region ready deps

        var deps = generics.AsParallel().AsOrdered()
            .Select(t => GetSerImpl(t, stub.CreateThread))
            .ToArray();

        var the_dep = MakeDepContainer(deps, generics);
        stub.ProvideDeps(the_dep);

        var (deps_type, _) = the_dep;

        #endregion

        #region ready type

        var type_args = generics
            .Concat(deps.Select(a => a.impl_type))
            .Append(deps_type)
            .ToArray();
        var type = (is_value_tuple
                ? ReflectionUtils.ValueTupleSerImpls[generics.Length]
                : ReflectionUtils.ClassTupleSerImpls[generics.Length])
            .MakeGenericType(type_args);
        stub.ProvideType(type);

        #endregion

        #region create inst

        var inst = Activator.CreateInstance(type)!;
        stub.ProvideInst(inst);

        #endregion
    }

    private void GenTupleRest(Type target, Type[] generics, CacheStub stub)
    {
        #region ready base type

        var is_value_tuple = ReflectionUtils.ValueTuples.Contains(target.GetGenericTypeDefinition());
        var base_type = (is_value_tuple
                ? typeof(ValueTupleRestSerializeImplBase<,,,,,,,>)
                : typeof(TupleRestSerializeImplBase<,,,,,,,>))
            .MakeGenericType(generics);
        stub.ProvideType(base_type);

        #endregion

        #region ready size

        var size = GetTupleSize(generics[7], is_value_tuple, 7);

        #endregion

        #region ready deps

        var deps = generics.AsParallel().AsOrdered()
            .Select(t => GetSerImpl(t, stub.CreateThread))
            .ToArray();

        var the_dep = MakeDepContainer(deps, generics);
        stub.ProvideDeps(the_dep);

        var (deps_type, _) = the_dep;

        #endregion

        #region ready type

        var type_args = generics
            .Concat(deps.Select(a => a.impl_type))
            .Append(deps_type)
            .ToArray();
        var type = (is_value_tuple
                ? typeof(ValueTupleRestSerializeDepsImpl<,,,,,,,,,,,,,,,,>)
                : typeof(TupleRestSerializeDepsImpl<,,,,,,,,,,,,,,,,>))
            .MakeGenericType(type_args);
        stub.ProvideType(type);

        #endregion

        #region create inst

        var inst_ctor = type.GetConstructor(
            BindingFlags.Public | BindingFlags.Instance,
            new[] { typeof(nuint) }
        )!;
        var inst = inst_ctor.Invoke(new object[] { (nuint)size });
        stub.ProvideInst(inst);

        #endregion
    }

    private int GetTupleSize(Type target, bool is_value_tuple, int size = 0)
    {
        for (;;)
        {
            if (!target.IsGenericType) return size;
            var generics = target.GenericTypeArguments;
            if (generics.Length < 8) return generics.Length + size;
            target = generics[7];
            if (!(is_value_tuple ? ReflectionUtils.ValueTuples : ReflectionUtils.ClassTuples).Contains(
                    target.GetGenericTypeDefinition()))
                return generics.Length + size;
            size += 7;
        }
    }
}

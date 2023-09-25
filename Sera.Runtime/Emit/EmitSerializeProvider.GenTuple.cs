using System;
using System.Linq;
using System.Reflection;
using Sera.Core.Impls.Tuples;
using Sera.Runtime.Utils;

namespace Sera.Runtime.Emit;

internal partial class EmitSerializeProvider
{
    private void GenTuple(TypeMeta target, bool is_value_tuple, CacheStub stub)
    {
        var generics = target.Generics ?? TypeMetas.GetGenericMeta(target.Type.GenericTypeArguments);
        if (generics.Length is not (> 0 and <= 8)) throw new ArgumentException($"{target} is not a tuple");
        if (generics.Length is 8)
        {
            var rest = generics.RawTypes[7];
            generics.Metas[7].KeepRaw = true;
            if (!(is_value_tuple ? ReflectionUtils.IsValueTuple(rest) : ReflectionUtils.IsClassTuple(rest)))
            {
                throw new ArgumentException($"The eighth element of a tuple must be a tuple");
            }
            GenTupleRest(generics, is_value_tuple, stub);
        }
        else
        {
            GenTuple(generics, is_value_tuple, stub);
        }
    }

    private void GenTuple(GenericMeta generics, bool is_value_tuple, CacheStub stub)
    {
        #region ready base type

        var base_type = (is_value_tuple
                ? ReflectionUtils.ValueTupleSerBaseImpls[generics.Length]
                : ReflectionUtils.ClassTupleSerBaseImpls[generics.Length])
            .MakeGenericType(generics.RawTypes);
        stub.ProvideType(base_type);

        #endregion

        #region ready deps

        var dep_count = generics.Length;

        var the_dep = MakeDepContainer(generics, stub.CreateThread);
        stub.ProvideDeps(the_dep);

        var wraps = ParallelEnumerable.Range(0, dep_count).AsOrdered().Select(the_dep.GetSerWarp).ToArray();
        var wrapTypes = wraps.Select(a => a.type).ToArray();
        var wrapInsts = wraps.Select(a => a.inst).ToArray();

        #endregion

        #region ready type

        var type_args = generics.RawTypes
            .Concat(wrapTypes)
            .ToArray();
        var type = (is_value_tuple
                ? ReflectionUtils.ValueTupleSerImpls[generics.Length]
                : ReflectionUtils.ClassTupleSerImpls[generics.Length])
            .MakeGenericType(type_args);
        stub.ProvideType(type);

        #endregion

        #region create inst

        var ctor = type.GetConstructor(
            BindingFlags.Public | BindingFlags.Instance,
            wrapTypes
        )!;
        var inst = ctor.Invoke(wrapInsts);
        stub.ProvideInst(inst);

        #endregion
    }

    private void GenTupleRest(GenericMeta generics, bool is_value_tuple, CacheStub stub)
    {
        #region ready base type

        var base_type = (is_value_tuple
                ? typeof(ValueTupleRestSerializeImplBase<,,,,,,,>)
                : typeof(TupleRestSerializeImplBase<,,,,,,,>))
            .MakeGenericType(generics.RawTypes);
        stub.ProvideType(base_type);

        #endregion

        #region ready size

        var size = GetTupleSize(generics.RawTypes[7], is_value_tuple, 7);

        #endregion

        #region ready deps

        var dep_count = generics.Length;

        var the_dep = MakeDepContainer(generics, stub.CreateThread);
        stub.ProvideDeps(the_dep);

        var wraps = ParallelEnumerable.Range(0, dep_count - 1).AsOrdered().Select(the_dep.GetSerWarp).ToArray();
        var wrapTypes = wraps.Select(a => a.type).ToArray();
        var wrapInsts = wraps.Select(a => a.inst).ToArray();

        var (seq_wrap_type, seq_wrap_inst) = the_dep.GetSeqSerReceiverWarp(7);

        #endregion

        #region ready type

        var type_args = generics.RawTypes
            .Concat(wrapTypes)
            .Append(seq_wrap_type)
            .ToArray();
        var type = (is_value_tuple
                ? typeof(ValueTupleRestSerializeImpl<,,,,,,,,,,,,,,,>)
                : typeof(TupleRestSerializeImpl<,,,,,,,,,,,,,,,>))
            .MakeGenericType(type_args);
        stub.ProvideType(type);

        #endregion

        #region create inst

        var inst_ctor = type.GetConstructor(
            BindingFlags.Public | BindingFlags.Instance,
            wrapTypes.Append(seq_wrap_type).Append(typeof(nuint)).ToArray()
        )!;
        var inst = inst_ctor.Invoke(wrapInsts.Append(seq_wrap_inst).Append((nuint)size).ToArray());
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

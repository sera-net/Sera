using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Sera.Core;
using Sera.Core.Impls.Deps;
using Sera.Core.Impls.Tuples;
using Sera.Core.Ser;
using Sera.Runtime.Emit;

namespace Sera.Runtime.Utils;

internal static class ReflectionUtils
{
    public static ConstructorInfo NullReferenceException_ctor { get; } =
        typeof(NullReferenceException).GetConstructor(BindingFlags.Public | BindingFlags.Instance,
            Array.Empty<Type>())!;

    public static ConstructorInfo Nullable_UInt64_ctor { get; } = typeof(long?).GetConstructor(new[] { typeof(long) })!;

    public static MethodInfo StaticRuntimeProvider_TryGetSerialize { get; } =
        typeof(StaticRuntimeProvider).GetMethod(nameof(StaticRuntimeProvider.TryGetSerialize))!;

    public static MethodInfo[] ISerializerMethods { get; } = typeof(ISerializer).GetMethods();

    public static MethodInfo ISerializer_StartStruct_3generic { get; } = ISerializerMethods
        .AsParallel()
        .Single(m =>
            m is { Name: nameof(ISerializer.StartStruct), IsGenericMethod: true }
            && m.GetGenericArguments() is { Length: 3 }
        );

    public static MethodInfo ISerializer_WriteVariantUnit_1generic { get; } = ISerializerMethods
        .AsParallel()
        .Single(m =>
            m is { Name: nameof(ISerializer.WriteVariantUnit), IsGenericMethod: true }
            && m.GetGenericArguments() is { Length: 1 }
        );

    public static MethodInfo ISerializer_WriteEmptyUnion_1generic { get; } = ISerializerMethods
        .AsParallel()
        .Single(m =>
            m is { Name: nameof(ISerializer.WriteEmptyUnion), IsGenericMethod: true }
            && m.GetGenericArguments() is { Length: 1 }
        );

    public static MethodInfo[] IStructSerializerMethods { get; } = typeof(IStructSerializer).GetMethods();

    public static MethodInfo IStructSerializer_WriteField_2generic_3arg_string_t_s { get; } = IStructSerializerMethods
        .Single(m =>
            m is { Name: nameof(IStructSerializer.WriteField), IsGenericMethod: true }
            && m.GetGenericArguments() is { Length: 2 }
            && m.GetParameters() is { Length: 4 } p && p[0].ParameterType == typeof(string)
        );

    public static readonly ConstructorInfo IsReadOnlyAttributeCtor =
        typeof(IsReadOnlyAttribute).GetConstructor(BindingFlags.Instance | BindingFlags.Public, Array.Empty<Type>())!;

    public static void MarkReadonly(this MethodBuilder builder) =>
        builder.SetCustomAttribute(new CustomAttributeBuilder(IsReadOnlyAttributeCtor, Array.Empty<object>()));

    public static void MarkReadonly(this TypeBuilder builder) =>
        builder.SetCustomAttribute(new CustomAttributeBuilder(IsReadOnlyAttributeCtor, Array.Empty<object>()));

    public static readonly MethodInfo ToFrozenDictionary_2generic_2arg__IEnumerable_KeyValuePair__IEqualityComparer =
        typeof(FrozenDictionary)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(static a =>
                a is { Name: nameof(FrozenDictionary.ToFrozenDictionary), IsGenericMethod: true } &&
                a.GetGenericArguments() is { Length: 2 } && a.GetParameters() is [var arg1, var arg2]
                && arg1.ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                && arg1.ParameterType.GetGenericArguments() is [var arg1_g1]
                && arg1_g1.GetGenericTypeDefinition() == typeof(KeyValuePair<,>)
                && arg2.ParameterType.GetGenericTypeDefinition() == typeof(IEqualityComparer<>)
            );

    public static string GetAsmName(string name) => $"{nameof(Sera)}.{nameof(Runtime)}.{nameof(Emit)}.Runtime.{name}";

    public static ModuleBuilder CreateAssembly(string name)
    {
        var asm_name = new AssemblyName(GetAsmName(name));
        AssemblyBuilder asm;
        if (Debugger.IsAttached)
        {
            var con = typeof(DebuggableAttribute).GetConstructor(new[] { typeof(bool), typeof(bool) })!;
            var debug_attr = new CustomAttributeBuilder(con, new object[] { true, true });
            asm = AssemblyBuilder.DefineDynamicAssembly(asm_name, AssemblyBuilderAccess.RunAndCollect,
                new[] { debug_attr });
        }
        else
        {
            asm = AssemblyBuilder.DefineDynamicAssembly(asm_name, AssemblyBuilderAccess.RunAndCollect);
        }
        return asm.DefineDynamicModule(asm_name.Name!);
    }

    public static Dictionary<int, Type> DepsContainers { get; } = new()
    {
        { 0, typeof(DepsContainer) },
        { 1, typeof(DepsContainer<>) },
        { 2, typeof(DepsContainer<,>) },
        { 3, typeof(DepsContainer<,,>) },
        { 4, typeof(DepsContainer<,,,>) },
        { 5, typeof(DepsContainer<,,,,>) },
        { 6, typeof(DepsContainer<,,,,,>) },
        { 7, typeof(DepsContainer<,,,,,,>) },
        { 8, typeof(DepsContainer<,,,,,,,>) },
    };

    public static Dictionary<int, Type> DepsSerWraps { get; } = new()
    {
        { 0, typeof(DepsSerializerWrapper1<,,>) },
        { 1, typeof(DepsSerializerWrapper2<,,>) },
        { 2, typeof(DepsSerializerWrapper3<,,>) },
        { 3, typeof(DepsSerializerWrapper4<,,>) },
        { 4, typeof(DepsSerializerWrapper5<,,>) },
        { 5, typeof(DepsSerializerWrapper6<,,>) },
        { 6, typeof(DepsSerializerWrapper7<,,>) },
        { 7, typeof(DepsSerializerWrapper8<,,>) },
    };

    public static Dictionary<int, Type> DepsSeqSerReceiverWraps { get; } = new()
    {
        { 0, typeof(DepsSeqSerializerReceiverWrapper1<,,>) },
        { 1, typeof(DepsSeqSerializerReceiverWrapper2<,,>) },
        { 2, typeof(DepsSeqSerializerReceiverWrapper3<,,>) },
        { 3, typeof(DepsSeqSerializerReceiverWrapper4<,,>) },
        { 4, typeof(DepsSeqSerializerReceiverWrapper5<,,>) },
        { 5, typeof(DepsSeqSerializerReceiverWrapper6<,,>) },
        { 6, typeof(DepsSeqSerializerReceiverWrapper7<,,>) },
        { 7, typeof(DepsSeqSerializerReceiverWrapper8<,,>) },
    };

    public static HashSet<Type> ValueTuples { get; } = new()
    {
        typeof(ValueTuple<>),
        typeof(ValueTuple<,>),
        typeof(ValueTuple<,,>),
        typeof(ValueTuple<,,,>),
        typeof(ValueTuple<,,,,>),
        typeof(ValueTuple<,,,,,>),
        typeof(ValueTuple<,,,,,,>),
        typeof(ValueTuple<,,,,,,,>),
    };

    public static HashSet<Type> ClassTuples { get; } = new()
    {
        typeof(Tuple<>),
        typeof(Tuple<,>),
        typeof(Tuple<,,>),
        typeof(Tuple<,,,>),
        typeof(Tuple<,,,,>),
        typeof(Tuple<,,,,,>),
        typeof(Tuple<,,,,,,>),
        typeof(Tuple<,,,,,,,>),
    };

    public static bool IsTuple(this TypeMeta target)
        => IsTuple(target.Type, out _);

    public static bool IsTuple(this TypeMeta target, out bool is_value_tuple)
        => IsTuple(target.Type, out is_value_tuple);

    public static bool IsTuple(Type target) => IsTuple(target, out _);

    public static bool IsTuple(Type target, out bool is_value_tuple)
    {
        if (!target.IsGenericType)
        {
            is_value_tuple = false;
            return false;
        }
        var t = target.GetGenericTypeDefinition();
        if (ValueTuples.Contains(t))
        {
            is_value_tuple = true;
            return true;
        }
        else if (ClassTuples.Contains(t))
        {
            is_value_tuple = false;
            return true;
        }
        else
        {
            is_value_tuple = false;
            return false;
        }
    }

    public static bool IsValueTuple(this EmitMeta target)
        => IsValueTuple(target.Type);

    public static bool IsValueTuple(this TypeMeta target)
        => IsValueTuple(target.Type);

    public static bool IsValueTuple(Type target)
    {
        if (!target.IsGenericType) return false;
        var t = target.GetGenericTypeDefinition();
        return ValueTuples.Contains(t);
    }

    public static bool IsClassTuple(this EmitMeta target)
        => IsClassTuple(target.Type);

    public static bool IsClassTuple(this TypeMeta target)
        => IsClassTuple(target.Type);

    public static bool IsClassTuple(Type target)
    {
        if (!target.IsGenericType) return false;
        var t = target.GetGenericTypeDefinition();
        return ClassTuples.Contains(t);
    }

    public static Dictionary<int, Type> ValueTupleSerImpls { get; } = new()
    {
        { 1, typeof(ValueTupleSerializeImpl<,>) },
        { 2, typeof(ValueTupleSerializeImpl<,,,>) },
        { 3, typeof(ValueTupleSerializeImpl<,,,,,>) },
        { 4, typeof(ValueTupleSerializeImpl<,,,,,,,>) },
        { 5, typeof(ValueTupleSerializeImpl<,,,,,,,,,>) },
        { 6, typeof(ValueTupleSerializeImpl<,,,,,,,,,,,>) },
        { 7, typeof(ValueTupleSerializeImpl<,,,,,,,,,,,,,>) },
    };

    public static Dictionary<int, Type> ClassTupleSerImpls { get; } = new()
    {
        { 1, typeof(TupleSerializeImpl<,>) },
        { 2, typeof(TupleSerializeImpl<,,,>) },
        { 3, typeof(TupleSerializeImpl<,,,,,>) },
        { 4, typeof(TupleSerializeImpl<,,,,,,,>) },
        { 5, typeof(TupleSerializeImpl<,,,,,,,,,>) },
        { 6, typeof(TupleSerializeImpl<,,,,,,,,,,,>) },
        { 7, typeof(TupleSerializeImpl<,,,,,,,,,,,,,>) },
    };

    public static Dictionary<int, Type> ValueTupleSerBaseImpls { get; } = new()
    {
        { 1, typeof(ValueTupleSerializeImplBase<>) },
        { 2, typeof(ValueTupleSerializeImplBase<,>) },
        { 3, typeof(ValueTupleSerializeImplBase<,,>) },
        { 4, typeof(ValueTupleSerializeImplBase<,,,>) },
        { 5, typeof(ValueTupleSerializeImplBase<,,,,>) },
        { 6, typeof(ValueTupleSerializeImplBase<,,,,,>) },
        { 7, typeof(ValueTupleSerializeImplBase<,,,,,,>) },
    };

    public static Dictionary<int, Type> ClassTupleSerBaseImpls { get; } = new()
    {
        { 1, typeof(TupleSerializeImplBase<>) },
        { 2, typeof(TupleSerializeImplBase<,>) },
        { 3, typeof(TupleSerializeImplBase<,,>) },
        { 4, typeof(TupleSerializeImplBase<,,,>) },
        { 5, typeof(TupleSerializeImplBase<,,,,>) },
        { 6, typeof(TupleSerializeImplBase<,,,,,>) },
        { 7, typeof(TupleSerializeImplBase<,,,,,,>) },
    };
}

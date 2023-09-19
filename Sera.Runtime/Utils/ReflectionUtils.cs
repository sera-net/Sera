using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Sera.Core;
using Sera.Core.Impls;
using Sera.Core.Impls.Deps;
using Sera.Core.Impls.Tuples;
using Sera.Core.Ser;

namespace Sera.Runtime.Utils;

internal static class ReflectionUtils
{
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

    public static string GetAsmName(string name) => $"{nameof(Sera)}.{nameof(Runtime)}.{nameof(Emit)}.Impls.{name}";

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

    public static bool IsTuple(Type target)
    {
        if (!target.IsGenericType) return false;
        var t = target.GetGenericTypeDefinition();
        return ValueTuples.Contains(t) || ClassTuples.Contains(t);
    }

    public static Dictionary<int, Type> ValueTupleSerImpls { get; } = new()
    {
        { 1, typeof(ValueTupleSerializeDepsImpl<,,>) },
        { 2, typeof(ValueTupleSerializeDepsImpl<,,,,>) },
        { 3, typeof(ValueTupleSerializeDepsImpl<,,,,,,>) },
        { 4, typeof(ValueTupleSerializeDepsImpl<,,,,,,,,>) },
        { 5, typeof(ValueTupleSerializeDepsImpl<,,,,,,,,,,>) },
        { 6, typeof(ValueTupleSerializeDepsImpl<,,,,,,,,,,,,>) },
        { 7, typeof(ValueTupleSerializeDepsImpl<,,,,,,,,,,,,,,>) },
        { 8, typeof(ValueTupleSerializeDepsImpl<,,,,,,,,,,,,,,,,>) },
    };

    public static Dictionary<int, Type> ClassTupleSerImpls { get; } = new()
    {
        { 1, typeof(TupleSerializeDepsImpl<,,>) },
        { 2, typeof(TupleSerializeDepsImpl<,,,,>) },
        { 3, typeof(TupleSerializeDepsImpl<,,,,,,>) },
        { 4, typeof(TupleSerializeDepsImpl<,,,,,,,,>) },
        { 5, typeof(TupleSerializeDepsImpl<,,,,,,,,,,>) },
        { 6, typeof(TupleSerializeDepsImpl<,,,,,,,,,,,,>) },
        { 7, typeof(TupleSerializeDepsImpl<,,,,,,,,,,,,,,>) },
        { 8, typeof(TupleSerializeDepsImpl<,,,,,,,,,,,,,,,,>) },
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
        { 8, typeof(ValueTupleSerializeImplBase<,,,,,,,>) },
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
        { 8, typeof(TupleSerializeImplBase<,,,,,,,>) },
    };
}

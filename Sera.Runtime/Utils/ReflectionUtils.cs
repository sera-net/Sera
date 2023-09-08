using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Sera.Core;
using Sera.Core.Ser;

namespace Sera.Runtime.Utils;

internal static class ReflectionUtils
{
    public static ConstructorInfo Nullable_UInt64_ctor { get; } = typeof(long?).GetConstructor(new[] { typeof(long) })!;

    public static MethodInfo StaticRuntimeProvider_TryGetSerialize { get; } =
        typeof(StaticRuntimeProvider).GetMethod("TryGetSerialize")!;

    public static MethodInfo[] ISerializerMethods { get; } = typeof(ISerializer).GetMethods();

    public static MethodInfo ISerializer_StartStruct_3generic { get; } = ISerializerMethods
        .Single(m =>
            m is { Name: "StartStruct", IsGenericMethod: true }
            && m.GetGenericArguments() is { Length: 3 }
        );

    public static MethodInfo[] IStructSerializerMethods { get; } = typeof(IStructSerializer).GetMethods();

    public static MethodInfo IStructSerializer_WriteField_2generic_3arg_string_t_s { get; } = IStructSerializerMethods
        .Single(m =>
            m is { Name: "WriteField", IsGenericMethod: true }
            && m.GetGenericArguments() is { Length: 2 }
            && m.GetParameters() is { Length: 4 } p && p[0].ParameterType == typeof(string)
        );

    public static ModuleBuilder CreateAssembly(string name)
    {
        var asm_name = new AssemblyName($"{nameof(Sera)}.{nameof(Runtime)}.{nameof(Emit)}.Impls.{name}");
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
}

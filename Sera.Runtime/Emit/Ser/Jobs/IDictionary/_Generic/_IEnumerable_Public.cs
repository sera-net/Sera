// using System;
// using System.Reflection;
// using System.Reflection.Emit;
// using Sera.Core;
// using Sera.Core.Ser;
// using Sera.Runtime.Utils;
//
// namespace Sera.Runtime.Emit.Ser.Jobs._IDictionary._Generic;
//
// internal class _IEnumerable_Public(Type KeyType, Type ValueType,
//         InterfaceMapping? mapping, MethodInfo? DirectGetEnumerator)
//     : _Public(KeyType, ValueType, mapping, DirectGetEnumerator)
// {
//     protected override MethodInfo GetGetCount() => throw new NotSupportedException();
//
//     protected override void EmitWrite(EmitMeta target)
//     {
//         var write_method = TypeBuilder.DefineMethod(nameof(ISerialize<object>.Write),
//             MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.NewSlot);
//         var generic_parameters = write_method.DefineGenericParameters("S");
//         var TS = generic_parameters[0];
//         TS.SetInterfaceConstraints(typeof(ISerializer));
//         write_method.SetParameters(TS, target.Type, typeof(ISeraOptions));
//         write_method.DefineParameter(1, ParameterAttributes.None, "serializer");
//         write_method.DefineParameter(2, ParameterAttributes.None, "value");
//         write_method.DefineParameter(3, ParameterAttributes.None, "options");
//         write_method.SetImplementationFlags(MethodImplAttributes.AggressiveInlining);
//
//         var ilg = write_method.GetILGenerator();
//
//         var not_null_label = ilg.DefineLabel();
//
//         if (!target.Type.IsValueType)
//         {
//             #region if (value == null) throw new NullReferenceException();
//
//             ilg.Emit(OpCodes.Ldarg_2);
//             ilg.Emit(OpCodes.Brtrue_S, not_null_label);
//             ilg.Emit(OpCodes.Newobj, ReflectionUtils.NullReferenceException_ctor);
//             ilg.Emit(OpCodes.Throw);
//
//             #endregion
//         }
//
//         #region serializer.StartSeq<I, T, Self>(value.Count, value, this);
//
//         ilg.MarkLabel(not_null_label);
//
//         var nullable_tmp = ilg.DeclareLocal(typeof(nuint?));
//         ilg.Emit(OpCodes.Ldloca, nullable_tmp);
//         ilg.Emit(OpCodes.Initobj, typeof(nuint?));
//
//         ilg.Emit(OpCodes.Ldarga, 1);
//         ilg.Emit(OpCodes.Ldloc, nullable_tmp);
//         ilg.Emit(OpCodes.Ldarg_2);
//         ilg.Emit(OpCodes.Ldarg_0);
//         ilg.Emit(OpCodes.Ldobj, TypeBuilder);
//         ilg.Emit(OpCodes.Constrained, TS);
//         ilg.Emit(OpCodes.Callvirt, StartMap);
//
//         #endregion
//
//         #region return;
//
//         ilg.Emit(OpCodes.Ret);
//
//         #endregion
//
//         var interface_type = typeof(ISerialize<>).MakeGenericType(target.Type);
//         TypeBuilder.AddInterfaceImplementation(interface_type);
//         TypeBuilder.DefineMethodOverride(write_method,
//             interface_type.GetMethod(nameof(ISerialize<object>.Write))!);
//     }
// }

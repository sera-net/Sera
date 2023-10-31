// using System;
// using System.Reflection;
// using System.Reflection.Emit;
// using System.Runtime.CompilerServices;
// using Sera.Core;
// using Sera.Core.Ser;
// using Sera.Runtime.Utils;
//
// namespace Sera.Runtime.Emit.Ser.Jobs._FSharpOption;
//
// internal static class FSharpOptionPrivateImpl
// {
//     private static readonly ConditionalWeakTable<Type, Box<(Type TypeWrapper, Type TypeBase, Type TypeImpl)>>
//         cache = new();
//
//     public static (Type TypeWrapper, Type TypeBase, Type TypeImpl) Get(Type FSharpOption)
//         => cache.GetValue(
//             FSharpOption,
//             static FSharpOption => new(Create(FSharpOption))
//         ).Value;
//
//     /// <param name="FSharpOption">typeof(FSharpOption&lt;&gt;)</param>
//     private static (Type TypeWrapper, Type TypeBase, Type TypeImpl) Create(Type FSharpOption)
//     {
//         var guid = Guid.NewGuid();
//         var module = ReflectionUtils.CreateAssembly($"_{guid:N}_");
//
//         var TypeWrapper = module.DefineType(
//             $"{module.Assembly.GetName().Name}.Ser_FSharpOption_Wrapper`1",
//             TypeAttributes.Public | TypeAttributes.Sealed,
//             typeof(ValueType)
//         );
//         var TypeBase = module.DefineType(
//             $"{module.Assembly.GetName().Name}.Ser_FSharpOption_Base`1",
//             TypeAttributes.Public | TypeAttributes.Abstract
//         );
//         var TypeImpl = module.DefineType(
//             $"{module.Assembly.GetName().Name}.Ser_FSharpOption`2",
//             TypeAttributes.Public | TypeAttributes.Sealed
//         );
//
//         MethodBuilder base_write;
//
//         TypeWrapper.MarkReadonly();
//
//         #region base type
//
//         {
//             var T = TypeBase.DefineGenericParameters("T")[0];
//             var T_Option = FSharpOption.MakeGenericType(T);
//
//             #region base write
//
//             base_write = TypeBase.DefineMethod(nameof(ISerialize<int>.Write),
//                 MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot |
//                 MethodAttributes.Abstract | MethodAttributes.Virtual
//             );
//             var TS = base_write.DefineGenericParameters("S")[0];
//             TS.SetInterfaceConstraints(typeof(ISerializer));
//             base_write.SetParameters(TS, T_Option, typeof(ISeraOptions));
//             base_write.DefineParameter(1, ParameterAttributes.None, "serializer");
//             base_write.DefineParameter(2, ParameterAttributes.None, "value");
//             base_write.DefineParameter(3, ParameterAttributes.None, "options");
//             base_write.SetImplementationFlags(MethodImplAttributes.AggressiveInlining);
//
//             var interface_type = typeof(ISerialize<>).MakeGenericType(T_Option);
//             var interface_write = typeof(ISerialize<>).GetMethod(nameof(ISerialize<object>.Write))!;
//             TypeBase.AddInterfaceImplementation(interface_type);
//             TypeBase.DefineMethodOverride(base_write, TypeBuilder.GetMethod(interface_type, interface_write));
//
//             #endregion
//         }
//
//         #endregion
//
//         #region impl type
//
//         {
//             var generics = TypeImpl.DefineGenericParameters("T", "ST");
//             var T = generics[0];
//             var ST = generics[1];
//             ST.SetInterfaceConstraints(typeof(ISerialize<>).MakeGenericType(T));
//
//             var T_Option = FSharpOption.MakeGenericType(T);
//             var parent_type = TypeBase.MakeGenericType(T);
//             TypeImpl.SetParent(parent_type);
//
//             var serialize_field = TypeImpl.DefineField("serialize", ST, FieldAttributes.Private);
//
//             #region impl ctor
//
//             var ctor = TypeImpl.DefineConstructor(
//                 MethodAttributes.Public, CallingConventions.Standard, new Type[] { ST });
//             var ctor_ilg = ctor.GetILGenerator();
//
//             ctor_ilg.Emit(OpCodes.Ldarg_0);
//             ctor_ilg.Emit(OpCodes.Ldarg_1);
//             ctor_ilg.Emit(OpCodes.Stfld, serialize_field);
//             ctor_ilg.Emit(OpCodes.Ret);
//
//             #endregion
//
//             #region impl write
//
//             var impl_write = TypeImpl.DefineMethod(nameof(ISerialize<int>.Write),
//                 MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual
//             );
//             var TS = impl_write.DefineGenericParameters("S")[0];
//             TS.SetInterfaceConstraints(typeof(ISerializer));
//             impl_write.SetParameters(TS, T_Option, typeof(ISeraOptions));
//             impl_write.DefineParameter(1, ParameterAttributes.None, "serializer");
//             impl_write.DefineParameter(2, ParameterAttributes.None, "value");
//             impl_write.DefineParameter(3, ParameterAttributes.None, "options");
//             impl_write.SetImplementationFlags(MethodImplAttributes.AggressiveInlining);
//
//             #region ready
//
//             var get_value = TypeBuilder.GetMethod(
//                 T_Option,
//                 FSharpOption.GetProperty("Value", BindingFlags.Public | BindingFlags.Instance)!.GetMethod!
//             );
//
//             var write_none = ReflectionUtils.ISerializer_WriteNone_1generic
//                 .MakeGenericMethod(T);
//
//             var write_some = ReflectionUtils.ISerializer_WriteSome_2generic
//                 .MakeGenericMethod(T, ST);
//
//             #endregion
//
//             var ilg = impl_write.GetILGenerator();
//
//             var not_null_label = ilg.DefineLabel();
//
//             #region if (value == null)
//
//             ilg.Emit(OpCodes.Ldarg_2);
//             ilg.Emit(OpCodes.Brtrue_S, not_null_label);
//
//             #endregion
//
//             #region Write None
//
//             ilg.Emit(OpCodes.Ldarga, 1);
//             ilg.Emit(OpCodes.Constrained, TS);
//             ilg.Emit(OpCodes.Callvirt, write_none);
//
//             ilg.Emit(OpCodes.Ret);
//
//             #endregion
//
//             #region Write Some
//
//             #region load serializer
//
//             ilg.MarkLabel(not_null_label);
//             ilg.Emit(OpCodes.Ldarga, 1);
//
//             #endregion
//
//             #region value.Value
//
//             ilg.Emit(OpCodes.Ldarg, 2);
//             ilg.Emit(OpCodes.Callvirt, get_value);
//
//             #endregion
//
//             #region get dep
//
//             ilg.Emit(OpCodes.Ldarg_0);
//             ilg.Emit(OpCodes.Ldfld, serialize_field);
//
//             #endregion
//
//             #region serializer.WriteSome<T, S>(value.Value, serialize)
//
//             ilg.Emit(OpCodes.Constrained, TS);
//             ilg.Emit(OpCodes.Callvirt, write_some);
//
//             #endregion
//
//             #endregion
//
//             #region return
//
//             ilg.Emit(OpCodes.Ret);
//
//             #endregion
//
//             TypeImpl.DefineMethodOverride(impl_write, TypeBuilder.GetMethod(parent_type, base_write));
//
//             #endregion
//         }
//
//         #endregion
//
//         #region wrapper type
//
//         {
//             var T = TypeWrapper.DefineGenericParameters("T")[0];
//             var T_Option = FSharpOption.MakeGenericType(T);
//
//             var base_type = TypeBase.MakeGenericType(T);
//
//             var serialize_field = TypeWrapper.DefineField("serialize", base_type, FieldAttributes.Private);
//
//             #region ctor
//
//             var ctor = TypeWrapper.DefineConstructor(
//                 MethodAttributes.Public, CallingConventions.Standard, new[] { base_type });
//             var ctor_ilg = ctor.GetILGenerator();
//
//             ctor_ilg.Emit(OpCodes.Ldarg_0);
//             ctor_ilg.Emit(OpCodes.Ldarg_1);
//             ctor_ilg.Emit(OpCodes.Stfld, serialize_field);
//             ctor_ilg.Emit(OpCodes.Ret);
//
//             #endregion
//
//             #region wrapper write
//
//             var wrapper_write = TypeWrapper.DefineMethod(nameof(ISerialize<int>.Write),
//                 MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.HideBySig |
//                 MethodAttributes.NewSlot | MethodAttributes.Virtual
//             );
//             var TS = wrapper_write.DefineGenericParameters("S")[0];
//             TS.SetInterfaceConstraints(typeof(ISerializer));
//             wrapper_write.SetParameters(TS, T_Option, typeof(ISeraOptions));
//             wrapper_write.DefineParameter(1, ParameterAttributes.None, "serializer");
//             wrapper_write.DefineParameter(2, ParameterAttributes.None, "value");
//             wrapper_write.DefineParameter(3, ParameterAttributes.None, "options");
//             wrapper_write.SetImplementationFlags(MethodImplAttributes.AggressiveInlining);
//
//             var ilg = wrapper_write.GetILGenerator();
//
//             ilg.Emit(OpCodes.Ldarg_0);
//             ilg.Emit(OpCodes.Ldfld);
//             ilg.Emit(OpCodes.Ldarg_1);
//             ilg.Emit(OpCodes.Ldarg_2);
//             ilg.Emit(OpCodes.Ldarg_3);
//             ilg.Emit(OpCodes.Callvirt, TypeBuilder.GetMethod(base_type, base_write));
//             ilg.Emit(OpCodes.Ret);
//
//             var interface_type = typeof(ISerialize<>).MakeGenericType(T_Option);
//             var interface_write = typeof(ISerialize<>).GetMethod(nameof(ISerialize<object>.Write))!;
//             TypeWrapper.AddInterfaceImplementation(interface_type);
//             TypeWrapper.DefineMethodOverride(wrapper_write, TypeBuilder.GetMethod(interface_type, interface_write));
//
//             #endregion
//         }
//
//         #endregion
//
//         var rt_base = TypeBase.CreateType();
//         var rt_impl = TypeImpl.CreateType();
//         var rt_wrapper = TypeWrapper.CreateType();
//         return (rt_wrapper, rt_base, rt_impl);
//     }
// }

// using System;
// using System.Reflection;
// using System.Reflection.Emit;
// using Sera.Core;
// using Sera.Core.Ser;
// using Sera.Runtime.Emit.Deps;
// using Sera.Runtime.Utils;
//
// namespace Sera.Runtime.Emit.Ser.Jobs._Nullable;
//
// internal class _Public(Type UnderlyingType) : _Nullable(UnderlyingType)
// {
//     public TypeBuilder TypeBuilder { get; set; } = null!;
//     public Type RuntimeType { get; set; } = null!;
//
//     public override void Init(EmitStub stub, EmitMeta target)
//     {
//         TypeBuilder = CreateTypeBuilderStruct($"Ser_{target.Type.Name}");
//         TypeBuilder.MarkReadonly();
//     }
//
//     public override EmitTransform[] CollectTransforms(EmitStub stub, EmitMeta target)
//         => EmitTransform.EmptyTransforms;
//
//     public override Type GetEmitPlaceholderType(EmitStub stub, EmitMeta target)
//         => TypeBuilder;
//
//     public override Type GetRuntimePlaceholderType(EmitStub stub, EmitMeta target)
//         => RuntimeType;
//
//     public override Type GetEmitType(EmitStub stub, EmitMeta target, EmitDeps deps)
//         => TypeBuilder;
//
//     public override Type GetRuntimeType(EmitStub stub, EmitMeta target, RuntimeDeps deps)
//         => RuntimeType;
//
//     public override void Emit(EmitStub stub, EmitMeta target, EmitDeps deps)
//     {
//         EmitWrite(target, deps);
//         RuntimeType = TypeBuilder.CreateType();
//     }
//
//     public override object CreateInst(EmitStub stub, EmitMeta target, RuntimeDeps deps)
//     {
//         return Activator.CreateInstance(RuntimeType)!;
//     }
//
//     private void EmitWrite(EmitMeta target, EmitDeps deps)
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
//         #region misc
//
//         var dep = deps.Get(0);
//
//         var has_value =
//             target.Type.GetProperty(nameof(Nullable<int>.HasValue), BindingFlags.Public | BindingFlags.Instance)!;
//         var get_has_value = has_value.GetMethod!;
//
//         var write_none = ReflectionUtils.ISerializer_WriteNone_1generic
//             .MakeGenericMethod(UnderlyingType);
//
//         var value =
//             target.Type.GetProperty(nameof(Nullable<int>.Value), BindingFlags.Public | BindingFlags.Instance)!;
//         var get_value = value.GetMethod!;
//         
//         var write_some = ReflectionUtils.ISerializer_WriteSome_2generic
//             .MakeGenericMethod(UnderlyingType, dep.TransformedType);
//
//         #endregion
//
//         var ilg = write_method.GetILGenerator();
//
//         var not_null_label = ilg.DefineLabel();
//
//         #region if value.HasValue
//
//         ilg.Emit(OpCodes.Ldarga, 2);
//         ilg.Emit(OpCodes.Call, get_has_value);
//         ilg.Emit(OpCodes.Brtrue_S, not_null_label);
//
//         #endregion
//
//         #region None
//
//         ilg.Emit(OpCodes.Ldarga, 1);
//         ilg.Emit(OpCodes.Constrained, TS);
//         ilg.Emit(OpCodes.Callvirt, write_none);
//
//         ilg.Emit(OpCodes.Ret);
//
//         #endregion
//
//         #region Some
//
//         #region load serializer
//
//         ilg.MarkLabel(not_null_label);
//         ilg.Emit(OpCodes.Ldarga, 1);
//
//         #endregion
//
//         #region value.Value
//
//         ilg.Emit(OpCodes.Ldarga, 2);
//         ilg.Emit(OpCodes.Call, get_value);
//
//         #endregion
//
//         #region get dep
//
//         ilg.Emit(OpCodes.Call, dep.GetDepMethodInfo);
//         if (dep.Boxed)
//         {
//             var get_method = dep.MakeBoxGetMethodInfo();
//             ilg.Emit(OpCodes.Call, get_method);
//         }
//
//         #endregion
//
//         #region serializer.WriteSome<T, S>(value.Value, Dep.impl)
//
//         ilg.Emit(OpCodes.Constrained, TS);
//         ilg.Emit(OpCodes.Callvirt, write_some);
//
//         #endregion
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

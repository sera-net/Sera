using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Sera.Core;
using Sera.Core.Impls;
using Sera.Core.Ser;
using Sera.Runtime.Utils;

namespace Sera.Runtime.Emit;

internal partial class EmitSerializeProvider
{
    private void GenPublicStruct(TypeMeta target, StructMember[] members, CacheStub stub)
    {
        #region create type builder

        var guid = Guid.NewGuid();
        var module = ReflectionUtils.CreateAssembly($"Ser.{target.Type.Name}._{guid:N}_");
        var type_builder = module.DefineType(
            $"{module.Assembly.GetName().Name}.SerializeImpl_{target.Type.Name}",
            TypeAttributes.Public | TypeAttributes.Sealed
        );
        Type? reference_type_wrapper = null;

        if (target.Type.IsValueType)
        {
            stub.ProvideType(type_builder);
        }
        else
        {
            reference_type_wrapper = typeof(ReferenceTypeWrapperSerializeImpl<,>).MakeGenericType(target.Type, type_builder);
            stub.ProvideType(reference_type_wrapper);
        }

        #endregion

        #region ready

        var start_struct = ReflectionUtils.ISerializer_StartStruct_3generic
            .MakeGenericMethod(target.Type, target.Type, type_builder);

        #endregion

        #region ready members

        var field_count = members.Length;

        var ser_deps = GetSerDeps(members, type_builder, stub.CreateThread);

        #endregion

        #region accesses

        var accesses = new List<(Delegate del, string name)>();

        (FieldBuilder access, MethodInfo access_invoke) AddAccess(Delegate del, Type value_type)
        {
            var access_del_type = typeof(AccessGet<,>).MakeGenericType(target.Type, value_type);
            var access_invoke = access_del_type.GetMethod(nameof(Action.Invoke), new[] { target.Type.MakeByRefType() })!;
            var access_name = $"_access_{accesses.Count}";
            var access = type_builder.DefineField(
                access_name, access_del_type,
                FieldAttributes.Public | FieldAttributes.Static
            );
            accesses.Add((del, access_name));
            return (access, access_invoke);
        }

        #endregion

        #region public void Write<S>(S serializer, T value, ISeraOptions options) where S : ISerializer

        {
            var write_method = type_builder.DefineMethod(nameof(ISerialize<object>.Write),
                MethodAttributes.Public | MethodAttributes.Virtual);
            var generic_parameters = write_method.DefineGenericParameters("S");
            var TS = generic_parameters[0];
            TS.SetInterfaceConstraints(typeof(ISerializer));
            write_method.SetParameters(TS, target.Type, typeof(ISeraOptions));
            write_method.DefineParameter(1, ParameterAttributes.None, "serializer");
            write_method.DefineParameter(2, ParameterAttributes.None, "value");
            write_method.DefineParameter(3, ParameterAttributes.None, "options");

            var ilg = write_method.GetILGenerator();

            var not_null_label = ilg.DefineLabel();

            if (!target.Type.IsValueType)
            {
                #region if (value == null) throw new NullReferenceException();

                ilg.Emit(OpCodes.Ldarg_2);
                ilg.Emit(OpCodes.Brtrue_S, not_null_label);
                ilg.Emit(OpCodes.Newobj, ReflectionUtils.NullReferenceException_ctor);
                ilg.Emit(OpCodes.Throw);

                #endregion
            }

            #region serializer.StartStruct<T, T, Self>(target.Name, field_count, value, this);

            ilg.MarkLabel(not_null_label);
            ilg.Emit(OpCodes.Ldarga, 1);
            ilg.Emit(OpCodes.Ldstr, target.Type.Name);
            ilg.Emit(OpCodes.Ldc_I4, field_count);
            ilg.Emit(OpCodes.Conv_I);
            ilg.Emit(OpCodes.Ldarg_2);
            ilg.Emit(OpCodes.Ldarg_0);
            ilg.Emit(OpCodes.Constrained, TS);
            ilg.Emit(OpCodes.Callvirt, start_struct);

            #endregion

            #region return;

            ilg.Emit(OpCodes.Ret);

            #endregion

            var interface_type = typeof(ISerialize<>).MakeGenericType(target.Type);
            type_builder.AddInterfaceImplementation(interface_type);
            type_builder.DefineMethodOverride(write_method,
                interface_type.GetMethod(nameof(ISerialize<object>.Write))!);
        }

        #endregion

        #region public void Receive<S>(T value, S serializer) where S : IStructSerializer

        {
            var receive_method =
                type_builder.DefineMethod(nameof(IStructSerializerReceiver<object>.Receive),
                    MethodAttributes.Public | MethodAttributes.Virtual);
            var generic_parameters = receive_method.DefineGenericParameters("S");
            var TS = generic_parameters[0];
            TS.SetInterfaceConstraints(typeof(IStructSerializer));
            receive_method.SetParameters(target.Type, TS);
            receive_method.DefineParameter(1, ParameterAttributes.None, "value");
            receive_method.DefineParameter(2, ParameterAttributes.None, "serializer");

            var ilg = receive_method.GetILGenerator();

            #region def local_int_key_null

            LocalBuilder? local_int_key_null = null;
            if (members.Any(m => !m.IntKey.HasValue))
            {
                local_int_key_null = ilg.DeclareLocal(typeof(long?));
                ilg.Emit(OpCodes.Ldloca_S, local_int_key_null);
                ilg.Emit(OpCodes.Initobj, typeof(long?));
            }

            #endregion

            #region write members

            foreach (var member in members)
            {
                var field_type = member.Type;
                var dep = ser_deps[field_type];

                var write_field = ReflectionUtils.IStructSerializer_WriteField_2generic_3arg_string_t_s
                    .MakeGenericMethod(member.Type, dep.ImplType);

                #region load serializer

                ilg.Emit(OpCodes.Ldarga_S, 2);

                #endregion

                #region nameof member

                ilg.Emit(OpCodes.Ldstr, member.Name);

                #endregion

                #region load int_key

                if (member.IntKey.HasValue)
                {
                    ilg.Emit(OpCodes.Ldc_I8, member.IntKey.Value);
                    ilg.Emit(OpCodes.Newobj, ReflectionUtils.Nullable_UInt64_ctor);
                }
                else
                {
                    ilg.Emit(OpCodes.Ldloc, local_int_key_null!);
                }

                #endregion

                #region get member value

                if (member.Kind is PropertyOrField.Property)
                {
                    var property = member.Property!;
                    var prop_type = property.PropertyType;
                    var get_method = property.GetMethod!;
                    if (!get_method.IsPublic)
                    {
                        var (del, _) = EmitPrivateAccess.Instance.AccessGetProperty(target.Type, property);
                        var (access, access_invoke) = AddAccess(del, prop_type);

                        #region access Get(ref value)

                        ilg.Emit(OpCodes.Ldsfld, access);
                        ilg.Emit(OpCodes.Ldarga_S, 1);
                        ilg.Emit(OpCodes.Callvirt, access_invoke);

                        #endregion
                    }
                    else if (target.Type.IsValueType)
                    {
                        #region load value

                        ilg.Emit(OpCodes.Ldarga_S, 1);

                        #endregion

                        #region get value.mermber_property

                        ilg.Emit(OpCodes.Call, get_method);

                        #endregion
                    }
                    else
                    {
                        #region load value

                        ilg.Emit(OpCodes.Ldarg_1);

                        #endregion

                        #region get value.mermber_property

                        ilg.Emit(OpCodes.Callvirt, get_method);

                        #endregion
                    }
                }
                else if (member.Kind is PropertyOrField.Field)
                {
                    var field = member.Field!;
                    if (!field.IsPublic)
                    {
                        var (del, _) = EmitPrivateAccess.Instance.AccessGetField(target.Type, field);
                        var (access, access_invoke) = AddAccess(del, field.FieldType);

                        #region access Get(ref value)

                        ilg.Emit(OpCodes.Ldsfld, access);
                        ilg.Emit(OpCodes.Ldarga_S, 1);
                        ilg.Emit(OpCodes.Callvirt, access_invoke);

                        #endregion
                    }
                    else
                    {
                        #region load value

                        ilg.Emit(OpCodes.Ldarg_1);

                        #endregion

                        #region load get value.mermber_field

                        ilg.Emit(OpCodes.Ldfld, field);

                        #endregion
                    }
                }
                else throw new ArgumentOutOfRangeException();

                #endregion

                #region load dep

                ilg.Emit(OpCodes.Ldsfld, dep.Field!);
                if (dep.Boxed)
                {
                    var get_method = Box.GetMethodInfo.MakeGenericMethod(dep.ImplType);
                    ilg.Emit(OpCodes.Call, get_method);
                }

                #endregion

                #region serializer.WriteField<V, VImpl>(nameof member, member_value, Self._impl_n);

                ilg.Emit(OpCodes.Constrained, TS);
                ilg.Emit(OpCodes.Callvirt, write_field);

                #endregion
            }

            #endregion

            #region return;

            ilg.Emit(OpCodes.Ret);

            #endregion

            var interface_type = typeof(IStructSerializerReceiver<>).MakeGenericType(target.Type);
            type_builder.AddInterfaceImplementation(interface_type);
            type_builder.DefineMethodOverride(receive_method,
                interface_type.GetMethod(nameof(IStructSerializerReceiver<object>.Receive))!);
        }

        #endregion

        #region create type

        var type = type_builder.CreateType();
        if (reference_type_wrapper == null)
        {
            stub.ProvideType(type);
        }
        else
        {
            reference_type_wrapper = typeof(ReferenceTypeWrapperSerializeImpl<,>).MakeGenericType(target.Type, type);
            stub.ProvideType(reference_type_wrapper);
        }

        stub.ProvideDeps(type, ser_deps.Values);

        #endregion

        #region create inst

        var inst = Activator.CreateInstance(type)!;
        if (reference_type_wrapper == null)
        {
            stub.ProvideInst(inst);
        }
        else
        {
            var ctor = reference_type_wrapper.GetConstructor(new[] { type })!;
            stub.ProvideInst(ctor.Invoke(new[] { inst }));
        }

        #endregion

        #region init accesses

        foreach (var (del, name) in accesses)
        {
            var field = type.GetField(name)!;
            field.SetValue(null, del);
        }

        #endregion
    }
}

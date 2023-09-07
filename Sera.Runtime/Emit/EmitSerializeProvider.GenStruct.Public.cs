using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Sera.Core;
using Sera.Core.Ser;

namespace Sera.Runtime.Emit;

internal partial class EmitSerializeProvider
{
    private void GenPublicStruct(Type target, CacheCell cell)
    {
        var guid = Guid.NewGuid();
        var module = ReflectionUtils.CreateAssembly($"Ser.{target.Name}._{guid:N}_");
        var type_builder = module.DefineType($"{module.Assembly.GetName().Name}.SerializeImpl_{target.Name}");
        cell.ser_type = type_builder;
        cell.WaitType.Set();

        var start_struct = ReflectionUtils.ISerializer_StartStruct_3generic
            .MakeGenericMethod(target, target, type_builder);

        #region ready members

        var members = GetStructMembers(target, cell.CreateThread);

        var field_count = members.Length;

        var ser_types = members.AsParallel().AsOrdered()
            .Select(m => m.ImplType)
            .Distinct()
            .ToArray();
        var ser_impl_field_names = ser_types.AsParallel().AsOrdered()
            .Select((t, i) => (i, t))
            .ToDictionary(a => a.t, a => $"_ser_impl_{a.i}");
        var ser_impl_fields = new Dictionary<Type, FieldBuilder>();

        foreach (var (field_type, field_name) in ser_impl_field_names)
        {
            var field = type_builder.DefineField(field_name, field_type, FieldAttributes.Public | FieldAttributes.Static);
            ser_impl_fields.Add(field_type, field);
        }

        #endregion

        #region public void WriteS>(S serializer, T value, ISeraOptions options) where S : ISerializer

        {
            var write_method = type_builder.DefineMethod("Write", MethodAttributes.Public | MethodAttributes.Virtual);
            var generic_parameters = write_method.DefineGenericParameters("S");
            var TS = generic_parameters[0];
            TS.SetInterfaceConstraints(typeof(ISerializer));
            write_method.SetParameters(TS, target, typeof(ISeraOptions));
            write_method.DefineParameter(1, ParameterAttributes.None, "serializer");
            write_method.DefineParameter(2, ParameterAttributes.None, "value");
            write_method.DefineParameter(3, ParameterAttributes.None, "options");

            var ilg = write_method.GetILGenerator();

            #region serializer.StartStruct<T, T, Self>(target.Name, field_count, value, this);

            ilg.Emit(OpCodes.Ldarga, 1);
            ilg.Emit(OpCodes.Ldstr, target.Name);
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

            var interface_type = typeof(ISerialize<>).MakeGenericType(target);
            type_builder.AddInterfaceImplementation(interface_type);
            type_builder.DefineMethodOverride(write_method, interface_type.GetMethod("Write")!);
        }

        #endregion

        #region public void Receive<S>(T value, S serializer) where S : IStructSerializer

        {
            var receive_method = type_builder.DefineMethod("Receive", MethodAttributes.Public | MethodAttributes.Virtual);
            var generic_parameters = receive_method.DefineGenericParameters("S");
            var TS = generic_parameters[0];
            TS.SetInterfaceConstraints(typeof(IStructSerializer));
            receive_method.SetParameters(target, TS);
            receive_method.DefineParameter(1, ParameterAttributes.None, "value");
            receive_method.DefineParameter(2, ParameterAttributes.None, "serializer");

            var ilg = receive_method.GetILGenerator();

            #region write members

            foreach (var member in members)
            {
                var impl_type = member.ImplType;
                var field = ser_impl_fields[impl_type];

                var write_field = ReflectionUtils.IStructSerializer_WriteField_2generic_3arg_string_t_s
                    .MakeGenericMethod(member.Type, impl_type);

                #region load serializer

                ilg.Emit(OpCodes.Ldarga_S, 2);

                #endregion

                #region nameof member

                ilg.Emit(OpCodes.Ldstr, member.Name);

                #endregion

                #region get member value

                if (member.Kind is PropertyOrField.Property)
                {
                    #region load value

                    ilg.Emit(OpCodes.Ldarga_S, 1);

                    #endregion

                    #region get value.mermber_property

                    ilg.Emit(OpCodes.Constrained, target);
                    ilg.Emit(OpCodes.Callvirt, member.Property!.GetMethod!);

                    #endregion
                }
                else if (member.Kind is PropertyOrField.Field)
                {
                    #region load value

                    ilg.Emit(OpCodes.Ldarg_1);

                    #endregion

                    #region load get value.mermber_field

                    ilg.Emit(OpCodes.Ldfld, member.Field!);

                    #endregion
                }
                else throw new ArgumentOutOfRangeException();

                #endregion

                #region load Self._impl_n

                ilg.Emit(OpCodes.Ldsfld, field);

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

            var interface_type = typeof(IStructSerializerReceiver<>).MakeGenericType(target);
            type_builder.AddInterfaceImplementation(interface_type);
            type_builder.DefineMethodOverride(receive_method, interface_type.GetMethod("Receive")!);
        }

        #endregion

        #region create type

        var type = type_builder.CreateType();
        cell.ser_type = type;

        #endregion

        #region create inst

        var inst = Activator.CreateInstance(type)!;
        cell.ser_inst = inst;

        #endregion

        #region mark type created

        cell.state = CacheCell.CreateState.Created;
        cell.WaitCreate.Set();

        #endregion

        #region set impl insts

        foreach (var member in members)
        {
            var impl_type = member.ImplType;
            var field_name = ser_impl_field_names[impl_type];
            var field = type.GetField(field_name)!;

            if (member.Impl != null)
            {
                field.SetValue(null, member.Impl);
            }
            else
            {
                member.Cell!.WaitCreate.WaitOne();
                field.SetValue(null, member.Cell.ser_inst);
            }
        }

        #endregion
    }
}

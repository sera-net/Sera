using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Sera.Core;
using Sera.Core.Ser;
using Sera.Runtime.Emit.Deps;
using Sera.Runtime.Utils;

namespace Sera.Runtime.Emit.Ser.Jobs._Struct;

internal sealed class _Public(StructMember[] Members) : _Struct(Members)
{
    public TypeBuilder TypeBuilder { get; set; } = null!;
    public Type RuntimeType { get; set; } = null!;

    private readonly List<(Delegate del, string name)> Accesses = new();

    private MethodInfo StartStruct { get; set; } = null!;
    
    public override void Init(EmitStub stub, EmitMeta target)
    {
        TypeBuilder = CreateTypeBuilderStruct($"Ser_{target.Type.Name}");
        TypeBuilder.MarkReadonly();
        StartStruct = ReflectionUtils.ISerializer_StartStruct_3generic
            .MakeGenericMethod(target.Type, target.Type, TypeBuilder);
    }

    public override Type GetEmitPlaceholderType(EmitStub stub, EmitMeta target) => TypeBuilder;

    public override Type GetRuntimePlaceholderType(EmitStub stub, EmitMeta target) => RuntimeType;

    public override Type GetEmitType(EmitStub stub, EmitMeta target, EmitDeps deps) => TypeBuilder;

    public override Type GetRuntimeType(EmitStub stub, EmitMeta target, RuntimeDeps deps) => RuntimeType;

    public override void Emit(EmitStub stub, EmitMeta target, EmitDeps deps)
    {
        EmitWrite(target);
        EmitReceive(target, deps);
        RuntimeType = TypeBuilder.CreateType();
    }

    public override object CreateInst(EmitStub stub, EmitMeta target, RuntimeDeps deps)
    {
        InitAccesses(RuntimeType);
        return Activator.CreateInstance(RuntimeType)!;
    }

    private (FieldBuilder access, MethodInfo access_invoke) AddAccess(EmitMeta target, Delegate del, Type value_type)
    {
        var access_del_type = typeof(AccessGet<,>).MakeGenericType(target.Type, value_type);
        var access_invoke = access_del_type.GetMethod(nameof(Action.Invoke), new[] { target.Type.MakeByRefType() })!;
        var access_name = $"_access_{Accesses.Count}";
        var access = TypeBuilder.DefineField(
            access_name, access_del_type,
            FieldAttributes.Public | FieldAttributes.Static
        );
        Accesses.Add((del, access_name));
        return (access, access_invoke);
    }

    private void EmitWrite(EmitMeta target)
    {
        var write_method = TypeBuilder.DefineMethod(nameof(ISerialize<object>.Write),
            MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.NewSlot);
        var generic_parameters = write_method.DefineGenericParameters("S");
        var TS = generic_parameters[0];
        TS.SetInterfaceConstraints(typeof(ISerializer));
        write_method.SetParameters(TS, target.Type, typeof(ISeraOptions));
        write_method.DefineParameter(1, ParameterAttributes.None, "serializer");
        write_method.DefineParameter(2, ParameterAttributes.None, "value");
        write_method.DefineParameter(3, ParameterAttributes.None, "options");
        write_method.SetImplementationFlags(MethodImplAttributes.AggressiveInlining);

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
        ilg.Emit(OpCodes.Ldc_I4, Members.Length);
        ilg.Emit(OpCodes.Conv_I);
        ilg.Emit(OpCodes.Ldarg_2);
        ilg.Emit(OpCodes.Ldarg_0);
        ilg.Emit(OpCodes.Ldobj, TypeBuilder);
        ilg.Emit(OpCodes.Constrained, TS);
        ilg.Emit(OpCodes.Callvirt, StartStruct);

        #endregion

        #region return;

        ilg.Emit(OpCodes.Ret);

        #endregion

        var interface_type = typeof(ISerialize<>).MakeGenericType(target.Type);
        TypeBuilder.AddInterfaceImplementation(interface_type);
        TypeBuilder.DefineMethodOverride(write_method,
            interface_type.GetMethod(nameof(ISerialize<object>.Write))!);
    }

    private void EmitReceive(EmitMeta target, EmitDeps deps)
    {
        var receive_method =
            TypeBuilder.DefineMethod(nameof(IStructSerializerReceiver<object>.Receive),
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.NewSlot);
        var generic_parameters = receive_method.DefineGenericParameters("S");
        var TS = generic_parameters[0];
        TS.SetInterfaceConstraints(typeof(IStructSerializer));
        receive_method.SetParameters(target.Type, TS);
        receive_method.DefineParameter(1, ParameterAttributes.None, "value");
        receive_method.DefineParameter(2, ParameterAttributes.None, "serializer");
        receive_method.SetImplementationFlags(MethodImplAttributes.AggressiveInlining);

        var ilg = receive_method.GetILGenerator();

        #region def local_int_key_null

        LocalBuilder? local_int_key_null = null;
        if (Members.Any(m => !m.IntKey.HasValue))
        {
            local_int_key_null = ilg.DeclareLocal(typeof(long?));
            ilg.Emit(OpCodes.Ldloca_S, local_int_key_null);
            ilg.Emit(OpCodes.Initobj, typeof(long?));
        }

        #endregion

        #region write members

        foreach (var (member, index) in Members.Select((a, b) => (a, b)))
        {
            var dep = deps.Get(index);

            var write_field = ReflectionUtils.IStructSerializer_WriteField_2generic_3arg_string_t_s
                .MakeGenericMethod(member.Type, dep.TransformedType);

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
                    var (access, access_invoke) = AddAccess(target, del, prop_type);

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
                    var (access, access_invoke) = AddAccess(target, del, field.FieldType);

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

            ilg.Emit(OpCodes.Call, dep.GetDepMethodInfo);
            if (dep.Boxed)
            {
                var get_method = dep.MakeBoxGetMethodInfo();
                ilg.Emit(OpCodes.Call, get_method);
            }

            #endregion

            #region serializer.WriteField<V, VImpl>(nameof member, member_value, Dep.Impl);

            ilg.Emit(OpCodes.Constrained, TS);
            ilg.Emit(OpCodes.Callvirt, write_field);

            #endregion
        }

        #endregion

        #region return;

        ilg.Emit(OpCodes.Ret);

        #endregion

        var interface_type = typeof(IStructSerializerReceiver<>).MakeGenericType(target.Type);
        TypeBuilder.AddInterfaceImplementation(interface_type);
        TypeBuilder.DefineMethodOverride(receive_method,
            interface_type.GetMethod(nameof(IStructSerializerReceiver<object>.Receive))!);
    }

    private void InitAccesses(Type type)
    {
        foreach (var (del, name) in Accesses)
        {
            var field = type.GetField(name)!;
            field.SetValue(null, del);
        }
    }
}

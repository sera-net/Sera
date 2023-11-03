using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Sera.Core;
using Sera.Runtime.Emit.Deps;
using Sera.Runtime.Utils;

namespace Sera.Runtime.Emit.Ser.Jobs._Struct;

internal sealed class _Public(string StructName, StructMember[] Members) : _Struct(Members)
{
    public TypeBuilder TypeBuilder { get; set; } = null!;
    public Type RuntimeType { get; set; } = null!;

    private readonly List<(Delegate del, string name)> Accesses = new();

    public override void Init(EmitStub stub, EmitMeta target)
    {
        TypeBuilder = CreateTypeBuilderStruct($"Ser_{target.Type.Name}");
        TypeBuilder.MarkReadonly();
    }

    public override Type GetEmitPlaceholderType(EmitStub stub, EmitMeta target) => TypeBuilder;

    public override Type GetRuntimePlaceholderType(EmitStub stub, EmitMeta target) => RuntimeType;

    public override Type GetEmitType(EmitStub stub, EmitMeta target, EmitDeps deps) => TypeBuilder;

    public override Type GetRuntimeType(EmitStub stub, EmitMeta target, RuntimeDeps deps) => RuntimeType;

    public override void Emit(EmitStub stub, EmitMeta target, EmitDeps deps)
    {
        EmitAccept(target);
        EmitStruct(target, deps);
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

    private void EmitAccept(EmitMeta target)
    {
        var accept_method = TypeBuilder.DefineMethod(nameof(ISeraVision<object>.Accept),
            MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.NewSlot);
        var generic_parameters = accept_method.DefineGenericParameters("R", "V");
        var TR = generic_parameters[0];
        var TV = generic_parameters[1];
        var visitor = typeof(ASeraVisitor<>).MakeGenericType(TR);
        TV.SetBaseTypeConstraint(visitor);
        accept_method.SetReturnType(TR);
        accept_method.SetParameters(TV, target.Type);
        accept_method.DefineParameter(1, ParameterAttributes.None, "visitor");
        accept_method.DefineParameter(2, ParameterAttributes.None, "value");
        accept_method.SetImplementationFlags(MethodImplAttributes.AggressiveInlining);

        var v_struct_method = TypeBuilder.GetMethod(visitor, ReflectionUtils.ASeraVisitor_VStruct)
            .MakeGenericMethod(TypeBuilder, target.Type);

        var ilg = accept_method.GetILGenerator();

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

        #region return visitor.VStruct<Self, T>(this, value);

        ilg.MarkLabel(not_null_label);
        ilg.Emit(OpCodes.Ldarg_1);
        ilg.Emit(OpCodes.Box, TV);
        ilg.Emit(OpCodes.Ldarg_0);
        ilg.Emit(OpCodes.Ldobj, TypeBuilder);
        ilg.Emit(OpCodes.Ldarg_2);
        ilg.Emit(OpCodes.Callvirt, v_struct_method);
        ilg.Emit(OpCodes.Ret);

        #endregion

        var interface_type = typeof(ISeraVision<>).MakeGenericType(target.Type);
        TypeBuilder.AddInterfaceImplementation(interface_type);
        TypeBuilder.DefineMethodOverride(accept_method,
            interface_type.GetMethod(nameof(ISeraVision<object>.Accept))!);
    }

    private void EmitStruct(EmitMeta target, EmitDeps deps)
    {
        #region Name

        var name_property = TypeBuilder.DefineProperty(
            nameof(IStructSeraVision<object>.Name),
            PropertyAttributes.None,
            typeof(string),
            Array.Empty<Type>()
        );
        var get_name_method = TypeBuilder.DefineMethod(
            $"get_{nameof(IStructSeraVision<object>.Name)}",
            MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.Virtual | MethodAttributes.NewSlot,
            typeof(string), Array.Empty<Type>()
        );
        get_name_method.SetImplementationFlags(MethodImplAttributes.AggressiveInlining);
        name_property.SetGetMethod(get_name_method);

        {
            var ilg = get_name_method.GetILGenerator();
            ilg.Emit(OpCodes.Ldstr, StructName);
            ilg.Emit(OpCodes.Ret);
        }

        #endregion

        #region Count

        var count_property = TypeBuilder.DefineProperty(
            nameof(IStructSeraVision<object>.Count),
            PropertyAttributes.None,
            typeof(int),
            Array.Empty<Type>()
        );
        var get_count_method = TypeBuilder.DefineMethod(
            $"get_{nameof(IStructSeraVision<object>.Count)}",
            MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.Virtual | MethodAttributes.NewSlot,
            typeof(int), Array.Empty<Type>()
        );
        get_count_method.SetImplementationFlags(MethodImplAttributes.AggressiveInlining);

        {
            var ilg = get_count_method.GetILGenerator();
            ilg.Emit(OpCodes.Ldc_I4, Members.Length);
            ilg.Emit(OpCodes.Ret);
        }

        count_property.SetGetMethod(get_count_method);

        #endregion

        #region AcceptField

        var accept_field_method = TypeBuilder.DefineMethod(nameof(IStructSeraVision<object>.AcceptField),
            MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.NewSlot);
        var generic_parameters = accept_field_method.DefineGenericParameters("R", "V");
        var TR = generic_parameters[0];
        var TV = generic_parameters[1];
        var visitor = typeof(AStructSeraVisitor<>).MakeGenericType(TR);
        TV.SetBaseTypeConstraint(visitor);
        accept_field_method.SetReturnType(TR);
        accept_field_method.SetParameters(TV, target.Type, typeof(int));
        accept_field_method.DefineParameter(1, ParameterAttributes.None, "visitor");
        accept_field_method.DefineParameter(2, ParameterAttributes.None, "value");
        accept_field_method.DefineParameter(3, ParameterAttributes.None, "field");
        accept_field_method.SetImplementationFlags(MethodImplAttributes.AggressiveInlining);

        {
            var v_field_method_decl = TypeBuilder.GetMethod(visitor, ReflectionUtils.AStructSeraVisitor_VField);
            var v_none_method = TypeBuilder.GetMethod(visitor, ReflectionUtils.AStructSeraVisitor_VNone);

            var ilg = accept_field_method.GetILGenerator();

            var label_default = ilg.DefineLabel();

            #region switch field

            var labels = Members.Select(_ => ilg.DefineLabel()).ToArray();
            ilg.Emit(OpCodes.Ldarg_3);
            ilg.Emit(OpCodes.Switch, labels);
            ilg.Emit(OpCodes.Br, label_default);

            #endregion

            #region members

            foreach (var (member, i) in Members.Select((a, b) => (a, b)))
            {
                var label = labels[i];
                var dep = deps.Get(i);
                var member_type = member.Type;

                #region load visitor

                ilg.MarkLabel(label);
                ilg.Emit(OpCodes.Ldarg_1);
                ilg.Emit(OpCodes.Box, TV);

                #endregion

                #region load dep

                ilg.Emit(OpCodes.Call, dep.GetDepMethodInfo);
                if (dep.Boxed)
                {
                    var get_method = dep.MakeBoxGetMethodInfo();
                    ilg.Emit(OpCodes.Call, get_method);
                }

                #endregion

                #region load member

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
                        ilg.Emit(OpCodes.Ldarga, 2);
                        ilg.Emit(OpCodes.Callvirt, access_invoke);

                        #endregion
                    }
                    else if (target.Type.IsValueType)
                    {
                        #region load value

                        ilg.Emit(OpCodes.Ldarga, 2);

                        #endregion

                        #region get value.mermber_property

                        ilg.Emit(OpCodes.Call, get_method);

                        #endregion
                    }
                    else
                    {
                        #region load value

                        ilg.Emit(OpCodes.Ldarg_2);

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
                        ilg.Emit(OpCodes.Ldarga, 2);
                        ilg.Emit(OpCodes.Callvirt, access_invoke);

                        #endregion
                    }
                    else
                    {
                        #region load value

                        ilg.Emit(OpCodes.Ldarg_2);

                        #endregion

                        #region load get value.mermber_field

                        ilg.Emit(OpCodes.Ldfld, field);

                        #endregion
                    }
                }
                else throw new ArgumentOutOfRangeException();

                #endregion

                #region load name

                ilg.Emit(OpCodes.Ldstr, member.Name);

                #endregion

                #region load key

                if (member.IntKey.HasValue)
                {
                    ilg.Emit(OpCodes.Ldc_I8, member.IntKey.Value);
                }
                else
                {
                    ilg.Emit(OpCodes.Ldarg_3);
                    ilg.Emit(OpCodes.Conv_I8);
                }

                #endregion

                #region call VField

                var v_field_method = v_field_method_decl.MakeGenericMethod(dep.TransformedType, member_type);
                ilg.Emit(OpCodes.Callvirt, v_field_method);
                ilg.Emit(OpCodes.Ret);

                #endregion
            }

            #endregion

            #region default => visitor.VNone();

            ilg.MarkLabel(label_default);
            ilg.Emit(OpCodes.Ldarg_1);
            ilg.Emit(OpCodes.Box, TV);
            ilg.Emit(OpCodes.Callvirt, v_none_method);
            ilg.Emit(OpCodes.Ret);

            #endregion
        }

        #endregion

        #region interface

        var interface_type = typeof(IStructSeraVision<>).MakeGenericType(target.Type);
        TypeBuilder.AddInterfaceImplementation(interface_type);
        TypeBuilder.DefineMethodOverride(get_name_method,
            interface_type.GetProperty(nameof(IStructSeraVision<object>.Name))!.GetMethod!);
        TypeBuilder.DefineMethodOverride(get_count_method,
            interface_type.GetProperty(nameof(IStructSeraVision<object>.Count))!.GetMethod!);
        TypeBuilder.DefineMethodOverride(accept_field_method,
            interface_type.GetMethod(nameof(IStructSeraVision<object>.AcceptField))!);

        #endregion
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

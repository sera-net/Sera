using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Sera.Core;
using Sera.Core.Ser;
using Sera.Runtime.Emit.Deps;
using Sera.Runtime.Utils;

namespace Sera.Runtime.Emit.Ser.Jobs._IEnumerable._Generic;

internal class _Public(Type ItemType, InterfaceMapping? mapping, MethodInfo? DirectGetEnumerator) : _Generic(ItemType)
{
    public TypeBuilder TypeBuilder { get; set; } = null!;
    public Type RuntimeType { get; set; } = null!;

    protected MethodInfo StartSeq { get; set; } = null!;

    public override void Init(EmitStub stub, EmitMeta target)
    {
        TypeBuilder = CreateTypeBuilderStruct($"Ser_{target.Type.Name}");
        TypeBuilder.MarkReadonly();
        StartSeq = ReflectionUtils.ISerializer_StartSeq_3generic
            .MakeGenericMethod(ItemType, target.Type, TypeBuilder);
    }

    public override Type GetEmitPlaceholderType(EmitStub stub, EmitMeta target)
        => TypeBuilder;

    public override Type GetRuntimePlaceholderType(EmitStub stub, EmitMeta target)
        => RuntimeType;

    public override Type GetEmitType(EmitStub stub, EmitMeta target, DepItem[] deps)
        => TypeBuilder;

    public override Type GetRuntimeType(EmitStub stub, EmitMeta target, DepItem[] deps)
        => RuntimeType;

    public override void Emit(EmitStub stub, EmitMeta target, EmitDeps deps)
    {
        EmitWrite(target);
        EmitReceive(target, deps);
        RuntimeType = TypeBuilder.CreateType();
    }

    public override object CreateInst(EmitStub stub, EmitMeta target, RuntimeDeps deps)
    {
        return Activator.CreateInstance(RuntimeType)!;
    }

    protected virtual void EmitWrite(EmitMeta target)
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

        #region serializer.StartSeq<I, T, Self>(null, value, this);

        ilg.MarkLabel(not_null_label);

        var nullable_tmp = ilg.DeclareLocal(typeof(nuint?));
        ilg.Emit(OpCodes.Ldloca, nullable_tmp);
        ilg.Emit(OpCodes.Initobj, typeof(nuint?));

        ilg.Emit(OpCodes.Ldarga, 1);
        ilg.Emit(OpCodes.Ldloc, nullable_tmp);
        ilg.Emit(OpCodes.Ldarg_2);
        ilg.Emit(OpCodes.Ldarg_0);
        ilg.Emit(OpCodes.Ldobj, TypeBuilder);
        ilg.Emit(OpCodes.Constrained, TS);
        ilg.Emit(OpCodes.Callvirt, StartSeq);

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
            TypeBuilder.DefineMethod(nameof(ISeqSerializerReceiver<object>.Receive),
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.NewSlot);
        var generic_parameters = receive_method.DefineGenericParameters("S");
        var TS = generic_parameters[0];
        TS.SetInterfaceConstraints(typeof(ISeqSerializer));
        receive_method.SetParameters(target.Type, TS);
        receive_method.DefineParameter(1, ParameterAttributes.None, "value");
        receive_method.DefineParameter(2, ParameterAttributes.None, "serializer");
        receive_method.SetImplementationFlags(MethodImplAttributes.AggressiveInlining);

        var ilg = receive_method.GetILGenerator();

        #region ready

        var dep = deps.Get(0);

        var write_element = ReflectionUtils.ISeqSerializer_WriteElement_2generic
            .MakeGenericMethod(ItemType, dep.TransformedType);

        var get_current = typeof(IEnumerator<>)
            .MakeGenericType(ItemType)
            .GetProperty(nameof(IEnumerator<int>.Current))!
            .GetMethod;

        var move_next = typeof(IEnumerator)
            .GetMethod(nameof(IEnumerator.MoveNext), Array.Empty<Type>())!;

        #endregion

        #region enumerator_tmp = value.GetEnumerator()

        LocalBuilder enumerator_tmp;

        if (
            DirectGetEnumerator != null && DirectGetEnumerator.ReturnType.IsVisible &&
            !DirectGetEnumerator.ReturnType.IsOpenTypeEq(typeof(IEnumerator<>)) &&
            DirectGetEnumerator.ReturnType.IsAssignableTo(typeof(IEnumerator<>).MakeGenericType(ItemType))
        )
        {
            enumerator_tmp = ilg.DeclareLocal(DirectGetEnumerator.ReturnType);

            if (target.IsValueType)
            {
                ilg.Emit(OpCodes.Ldarga, 1);
                ilg.Emit(OpCodes.Call, DirectGetEnumerator);
            }
            else
            {
                ilg.Emit(OpCodes.Ldarg_1);
                ilg.Emit(OpCodes.Callvirt, DirectGetEnumerator);
            }

            ilg.Emit(OpCodes.Stloc, enumerator_tmp);
        }
        else
        {
            enumerator_tmp = ilg.DeclareLocal(typeof(IEnumerator<>).MakeGenericType(ItemType));

            switch (mapping)
            {
                case null:
                {
                    var get_enumerator = typeof(IEnumerable<>).MakeGenericType(ItemType)
                        .GetMethod(nameof(IEnumerable<int>.GetEnumerator), Array.Empty<Type>())!;

                    ilg.Emit(OpCodes.Ldarga, 1);
                    ilg.Emit(OpCodes.Constrained, target.Type);
                    ilg.Emit(OpCodes.Callvirt, get_enumerator);

                    ilg.Emit(OpCodes.Stloc, enumerator_tmp);

                    break;
                }
                case not null:
                {
                    var get_enumerator =
                        mapping.Value.TargetMethods.FirstOrDefault(
                            a => a.Name == nameof(IEnumerable<int>.GetEnumerator));
                    if (get_enumerator is null or { IsPublic: false }) goto case null;

                    if (target.IsValueType)
                    {
                        ilg.Emit(OpCodes.Ldarga, 1);
                        ilg.Emit(OpCodes.Call, get_enumerator);
                    }
                    else
                    {
                        ilg.Emit(OpCodes.Ldarg_1);
                        ilg.Emit(OpCodes.Callvirt, get_enumerator);
                    }

                    ilg.Emit(OpCodes.Stloc, enumerator_tmp);

                    break;
                }
            }
        }

        #endregion

        ilg.BeginExceptionBlock();

        #region foreach

        var for_start_label = ilg.DefineLabel();
        var for_next_label = ilg.DefineLabel();

        var value_tmp = ilg.DeclareLocal(ItemType);

        #region start foreach

        ilg.Emit(OpCodes.Br, for_start_label);

        #endregion

        #region v = enumerator_tmp.Current

        ilg.MarkLabel(for_next_label);
        ilg.Emit(OpCodes.Ldloca, enumerator_tmp);
        ilg.Emit(OpCodes.Constrained, enumerator_tmp.LocalType);
        ilg.Emit(OpCodes.Callvirt, get_current!);
        ilg.Emit(OpCodes.Stloc, value_tmp);

        #endregion

        #region serializer.WriteElement<I, SI>(v, Dep.impl)

        ilg.Emit(OpCodes.Ldarga, 2);
        ilg.Emit(OpCodes.Ldloc, value_tmp);
        ilg.Emit(OpCodes.Call, dep.GetDepMethodInfo);
        if (dep.Boxed)
        {
            var get_method = dep.MakeBoxGetMethodInfo();
            ilg.Emit(OpCodes.Call, get_method);
        }
        ilg.Emit(OpCodes.Constrained, TS);
        ilg.Emit(OpCodes.Callvirt, write_element);

        #endregion

        #region enumerator_tmp.MoveNext()

        ilg.MarkLabel(for_start_label);
        ilg.Emit(OpCodes.Ldloca, enumerator_tmp);
        ilg.Emit(OpCodes.Constrained, enumerator_tmp.LocalType);
        ilg.Emit(OpCodes.Callvirt, move_next);
        ilg.Emit(OpCodes.Brtrue, for_next_label);

        #endregion

        #endregion

        ilg.BeginFinallyBlock();
        var end_finally_label = ilg.DefineLabel();

        #region finally

        #region if enumerator_tmp != null

        if (!enumerator_tmp.LocalType.IsValueType)
        {
            ilg.Emit(OpCodes.Ldloc, enumerator_tmp);
            ilg.Emit(OpCodes.Brfalse, end_finally_label);
        }

        #endregion

        #region enumerator_tmp.Dispose()

        var dispose = typeof(IDisposable).GetMethod(nameof(IDisposable.Dispose), Array.Empty<Type>())!;

        ilg.Emit(OpCodes.Ldloca, enumerator_tmp);
        ilg.Emit(OpCodes.Constrained, enumerator_tmp.LocalType);
        ilg.Emit(OpCodes.Callvirt, dispose);

        #endregion

        #endregion

        ilg.MarkLabel(end_finally_label);
        ilg.EndExceptionBlock();

        #region return;

        ilg.Emit(OpCodes.Ret);

        #endregion

        var interface_type = typeof(ISeqSerializerReceiver<>).MakeGenericType(target.Type);
        TypeBuilder.AddInterfaceImplementation(interface_type);
        TypeBuilder.DefineMethodOverride(receive_method,
            interface_type.GetMethod(nameof(ISeqSerializerReceiver<object>.Receive))!);
    }
}

using System;
using System.Reflection;
using System.Reflection.Emit;
using Sera.Core;
using Sera.Core.Ser;
using Sera.Runtime.Emit.Deps;
using Sera.Runtime.Utils;

namespace Sera.Runtime.Emit.Ser.Jobs._Tuples;

internal class _Public(bool IsValueTuple, Type[] ItemTypes) : _Tuples(IsValueTuple, ItemTypes)
{
    public MethodInfo StartSeq { get; set; } = null!;
    public TypeBuilder TypeBuilder { get; set; } = null!;
    public Type RuntimeType { get; set; } = null!;

    public override void Init(EmitStub stub, EmitMeta target)
    {
        base.Init(stub, target);

        TypeBuilder = CreateTypeBuilderStruct($"Ser_{target.Type.Name}");
        TypeBuilder.MarkReadonly();

        StartSeq = ReflectionUtils.ISerializer_StartSeq_2generic
            .MakeGenericMethod(target.Type, TypeBuilder);
    }

    public override EmitTransform[] CollectTransforms(EmitStub stub, EmitMeta target)
    {
        if (target.IsValueType || Size == 8) return EmitTransform.EmptyTransforms;
        else return SerializeEmitProvider.ReferenceTypeTransforms;
    }
    
    public override Type GetEmitType(EmitStub stub, EmitMeta target, DepItem[] deps)
        => TypeBuilder;

    public override Type GetEmitPlaceholderType(EmitStub stub, EmitMeta target)
        => TypeBuilder;

    public override Type GetRuntimeType(EmitStub stub, EmitMeta target, DepItem[] deps)
        => RuntimeType;

    public override Type GetRuntimePlaceholderType(EmitStub stub, EmitMeta target)
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

        #region serializer.StartSeq<T, Self>(len, value, this);

        ilg.MarkLabel(not_null_label);
        ilg.Emit(OpCodes.Ldarga, 1);
        ilg.Emit(OpCodes.Ldc_I4, TupleSize);
        ilg.Emit(OpCodes.Conv_I);
        ilg.Emit(OpCodes.Newobj, ReflectionUtils.Nullable_UIntPtr_ctor);
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

        #region write items

        for (var i = 0; i < ItemTypes.Length; i++)
        {
            var item = ItemTypes[i];
            var dep = deps.Get(i);

            if (i > 7) throw new ArgumentOutOfRangeException();
            if (i < 7)
            {
                var write_field = ReflectionUtils.ISeqSerializer_WriteElement_2generic
                    .MakeGenericMethod(item, dep.TransformedType);

                #region load serializer

                ilg.Emit(OpCodes.Ldarga_S, 2);

                #endregion

                #region load item

                if (IsValueTuple)
                {
                    var field = target.Type.GetField($"Item{i + 1}", BindingFlags.Public | BindingFlags.Instance)!;

                    #region load value

                    ilg.Emit(OpCodes.Ldarg_1);

                    #endregion

                    #region load get value.ItemN

                    ilg.Emit(OpCodes.Ldfld, field);

                    #endregion
                }
                else
                {
                    var property =
                        target.Type.GetProperty($"Item{i + 1}", BindingFlags.Public | BindingFlags.Instance)!;
                    var get_method = property.GetMethod!;

                    #region load value

                    ilg.Emit(OpCodes.Ldarg_1);

                    #endregion

                    #region get value.ItemN

                    ilg.Emit(OpCodes.Callvirt, get_method);

                    #endregion
                }

                #endregion

                #region load dep

                ilg.Emit(OpCodes.Call, dep.GetDepMethodInfo);
                if (dep.Boxed)
                {
                    var get_method = dep.MakeBoxGetMethodInfo();
                    ilg.Emit(OpCodes.Call, get_method);
                }

                #endregion

                #region serializer.WriteElement<V, VImpl>(item_value, Dep.Impl);

                ilg.Emit(OpCodes.Constrained, TS);
                ilg.Emit(OpCodes.Callvirt, write_field);

                #endregion
            }
            else
            {
                #region load dep

                ilg.Emit(OpCodes.Call, dep.GetDepMethodInfo);
                if (dep.Boxed)
                {
                    var get_ref_method = dep.MakeBoxGetRefMethodInfo();
                    ilg.Emit(OpCodes.Call, get_ref_method);
                }
                else
                {
                    var tmp = ilg.DeclareLocal(dep.RawType);
                    ilg.Emit(OpCodes.Stloc, tmp);
                    ilg.Emit(OpCodes.Ldloca, tmp);
                }

                #endregion

                var sub_interface_type = typeof(ISeqSerializerReceiver<>).MakeGenericType(item);
                var sub_method = sub_interface_type.GetMethod(nameof(ISeqSerializerReceiver<object>.Receive))!
                    .MakeGenericMethod(TS);

                #region load item

                if (IsValueTuple)
                {
                    var field = target.Type.GetField("Rest", BindingFlags.Public | BindingFlags.Instance)!;

                    #region load value

                    ilg.Emit(OpCodes.Ldarg_1);

                    #endregion

                    #region load get value.Rest

                    ilg.Emit(OpCodes.Ldfld, field);

                    #endregion
                }
                else
                {
                    var property =
                        target.Type.GetProperty("Rest", BindingFlags.Public | BindingFlags.Instance)!;
                    var get_method = property.GetMethod!;

                    #region load value

                    ilg.Emit(OpCodes.Ldarg_1);

                    #endregion

                    #region get value.Rest

                    ilg.Emit(OpCodes.Callvirt, get_method);

                    #endregion
                }

                #endregion

                #region Dep.Impl.Receive<V, S>(item_value, serializer);

                ilg.Emit(OpCodes.Ldarg_2);
                ilg.Emit(OpCodes.Constrained, dep.RawType);
                ilg.Emit(OpCodes.Callvirt, sub_method);

                #endregion
            }
        }

        #endregion

        #region return;

        ilg.Emit(OpCodes.Ret);

        #endregion

        var interface_type = typeof(ISeqSerializerReceiver<>).MakeGenericType(target.Type);
        TypeBuilder.AddInterfaceImplementation(interface_type);
        TypeBuilder.DefineMethodOverride(receive_method,
            interface_type.GetMethod(nameof(ISeqSerializerReceiver<object>.Receive))!);
    }
}

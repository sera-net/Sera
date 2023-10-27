using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Sera.Core;
using Sera.Core.Ser;
using Sera.Runtime.Emit.Deps;
using Sera.Runtime.Utils;

namespace Sera.Runtime.Emit.Ser.Jobs._KeyValuePair;

internal class _Public(Type KeyType, Type ValueType) : _KeyValuePair(KeyType, ValueType)
{
    public TypeBuilder TypeBuilder { get; set; } = null!;
    public Type RuntimeType { get; set; } = null!;
    
    public MethodInfo StartSeq { get; set; } = null!;

    public override void Init(EmitStub stub, EmitMeta target)
    {
        TypeBuilder = CreateTypeBuilderStruct($"Ser_{target.Type.Name}");
        TypeBuilder.MarkReadonly();
        StartSeq = ReflectionUtils.ISerializer_StartSeq_2generic
            .MakeGenericMethod(target.Type, TypeBuilder);
    }

    public override Type GetEmitPlaceholderType(EmitStub stub, EmitMeta target)
        => TypeBuilder;

    public override Type GetRuntimePlaceholderType(EmitStub stub, EmitMeta target)
        => RuntimeType;

    public override Type GetEmitType(EmitStub stub, EmitMeta target, EmitDeps deps)
        => TypeBuilder;

    public override Type GetRuntimeType(EmitStub stub, EmitMeta target, RuntimeDeps deps)
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
        
        #region serializer.StartSeq<T, Self>(2, value, this);

        ilg.MarkLabel(not_null_label);
        ilg.Emit(OpCodes.Ldarga, 1);
        ilg.Emit(OpCodes.Ldc_I4_2);
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
        
        var dep_key = deps.Get(0);
        var dep_val = deps.Get(1);

        var get_key = target.Type.GetProperty(nameof(KeyValuePair<int, int>.Key))!.GetMethod!;
        var get_val = target.Type.GetProperty(nameof(KeyValuePair<int, int>.Value))!.GetMethod!;
        
        var write_key = ReflectionUtils.ISeqSerializer_WriteElement_2generic
            .MakeGenericMethod(KeyType, dep_key.TransformedType);
        var write_val = ReflectionUtils.ISeqSerializer_WriteElement_2generic
            .MakeGenericMethod(ValueType, dep_val.TransformedType);

        #region serializer.WriteElement(value.Key, Dep.impl)
        
        ilg.Emit(OpCodes.Ldarga, 2);
        ilg.Emit(OpCodes.Ldarga, 1);
        ilg.Emit(OpCodes.Call, get_key);
        ilg.Emit(OpCodes.Call, dep_key.GetDepMethodInfo);
        if (dep_key.Boxed)
        {
            var get_method = dep_key.MakeBoxGetMethodInfo();
            ilg.Emit(OpCodes.Call, get_method);
        }
        ilg.Emit(OpCodes.Constrained, TS);
        ilg.Emit(OpCodes.Callvirt, write_key);

        #endregion
        
        #region serializer.WriteElement(value.Value, Dep.impl)
        
        ilg.Emit(OpCodes.Ldarga, 2);
        ilg.Emit(OpCodes.Ldarga, 1);
        ilg.Emit(OpCodes.Call, get_val);
        ilg.Emit(OpCodes.Call, dep_val.GetDepMethodInfo);
        if (dep_val.Boxed)
        {
            var get_method = dep_val.MakeBoxGetMethodInfo();
            ilg.Emit(OpCodes.Call, get_method);
        }
        ilg.Emit(OpCodes.Constrained, TS);
        ilg.Emit(OpCodes.Callvirt, write_val);

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

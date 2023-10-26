using System;
using System.Reflection;
using System.Reflection.Emit;
using Sera.Core;
using Sera.Core.Ser;
using Sera.Runtime.Emit.Deps;
using Sera.Runtime.Utils;

namespace Sera.Runtime.Emit.Ser.Jobs._FSharpOption;

internal class _Class_Public(Type UnderlyingType) : _Public(UnderlyingType)
{
    public override void Emit(EmitStub stub, EmitMeta target, EmitDeps deps)
    {
        EmitWrite(target, deps);
        RuntimeType = TypeBuilder.CreateType();
    }
    
    private void EmitWrite(EmitMeta target, EmitDeps deps)
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

        #region ready

        var dep = deps.Get(0);

        var get_value = target.Type.GetProperty("Value", BindingFlags.Public | BindingFlags.Instance)!.GetMethod!;

        var write_none = ReflectionUtils.ISerializer_WriteNone_1generic
            .MakeGenericMethod(UnderlyingType);

        var write_some = ReflectionUtils.ISerializer_WriteSome_2generic
            .MakeGenericMethod(UnderlyingType, dep.TransformedType);

        #endregion

        var ilg = write_method.GetILGenerator();

        var not_null_label = ilg.DefineLabel();
        
        #region if (value == null)

        ilg.Emit(OpCodes.Ldarg_2);
        ilg.Emit(OpCodes.Brtrue_S, not_null_label);

        #endregion

        #region Write None

        ilg.Emit(OpCodes.Ldarga, 1);
        ilg.Emit(OpCodes.Constrained, TS);
        ilg.Emit(OpCodes.Callvirt, write_none);

        ilg.Emit(OpCodes.Ret);

        #endregion

        #region Write Some

        #region load serializer

        ilg.MarkLabel(not_null_label);
        ilg.Emit(OpCodes.Ldarga, 1);

        #endregion

        #region value.Value

        ilg.Emit(OpCodes.Ldarg, 2);
        ilg.Emit(OpCodes.Callvirt, get_value);

        #endregion

        #region get dep

        ilg.Emit(OpCodes.Call, dep.GetDepMethodInfo);
        if (dep.Boxed)
        {
            var get_method = dep.MakeBoxGetMethodInfo();
            ilg.Emit(OpCodes.Call, get_method);
        }

        #endregion

        #region serializer.WriteSome<T, S>(value.Value, Dep.impl)

        ilg.Emit(OpCodes.Constrained, TS);
        ilg.Emit(OpCodes.Callvirt, write_some);

        #endregion

        #endregion

        #region return;

        ilg.Emit(OpCodes.Ret);

        #endregion

        var interface_type = typeof(ISerialize<>).MakeGenericType(target.Type);
        TypeBuilder.AddInterfaceImplementation(interface_type);
        TypeBuilder.DefineMethodOverride(write_method,
            interface_type.GetMethod(nameof(ISerialize<object>.Write))!);
    }

    public override object CreateInst(EmitStub stub, EmitMeta target, RuntimeDeps deps)
    {
        return Activator.CreateInstance(RuntimeType)!;
    }
}

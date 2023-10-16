using System;
using System.Reflection;
using System.Reflection.Emit;
using Sera.Core;
using Sera.Core.Ser;
using Sera.Runtime.Emit.Deps;
using Sera.Runtime.Utils;

namespace Sera.Runtime.Emit.Ser.Jobs;

internal abstract class _Array_Public(Type ItemType) : _Array(ItemType)
{
    public TypeBuilder TypeBuilder { get; set; } = null!;
    public Type RuntimeType { get; set; } = null!;

    protected abstract MethodInfo WriteArrayMethod { get; }

    public override void Init(EmitStub stub, EmitMeta target)
    {
        TypeBuilder = CreateTypeBuilderStruct($"Ser_{target.Type.Name}");
        TypeBuilder.MarkReadonly();
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
        EmitWrite(target, deps);
        RuntimeType = TypeBuilder.CreateType();
    }

    public override object CreateInst(EmitStub stub, EmitMeta target, RuntimeDeps deps)
    {
        return Activator.CreateInstance(RuntimeType)!;
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

        #region serializer.WriteArray<T, S>(value, Serialize);

        var dep = deps.Deps[0];
        var write_array = WriteArrayMethod.MakeGenericMethod(ItemType, dep.TransformedType);

        ilg.MarkLabel(not_null_label);
        ilg.Emit(OpCodes.Ldarga, 1);
        ilg.Emit(OpCodes.Ldarg_2);
        ConvertValue(target, dep, ilg);
        ilg.Emit(OpCodes.Call, dep.GetDepMethodInfo);
        if (dep.Boxed)
        {
            var get_method = dep.MakeBoxGetMethodInfo();
            ilg.Emit(OpCodes.Call, get_method);
        }
        ilg.Emit(OpCodes.Constrained, TS);
        ilg.Emit(OpCodes.Callvirt, write_array);

        #endregion

        #region return;

        ilg.Emit(OpCodes.Ret);

        #endregion

        var interface_type = typeof(ISerialize<>).MakeGenericType(target.Type);
        TypeBuilder.AddInterfaceImplementation(interface_type);
        TypeBuilder.DefineMethodOverride(write_method,
            interface_type.GetMethod(nameof(ISerialize<object>.Write))!);
    }

    protected virtual void ConvertValue(EmitMeta target, DepPlace dep, ILGenerator ilg) { }
}

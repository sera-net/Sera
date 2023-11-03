using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Sera.Core.Impls.Deps;
using Sera.Runtime.Emit.Ser.Internal;
using Sera.Runtime.Utils;

namespace Sera.Runtime.Emit.Deps;

internal readonly record struct DepPlace(
    DepItem Dep, Type RawContainerType, Type ContainerType, MethodInfo GetDepMethodInfo,
    Type ActualType, Type TransformedType, Type RawType, bool Boxed
)
{
    public MethodInfo MakeBoxGetMethodInfo() => Box.GetMethodInfo.MakeGenericMethod(TransformedType);
    public MethodInfo MakeBoxGetRefMethodInfo() => Box.GetRefMethodInfo.MakeGenericMethod(TransformedType);

    public Type MakeSerWrapper(Type target) =>
        (Boxed ? typeof(BoxedDepsSerWrapper<,,>) : typeof(DepsSerWrapper<,,>))
        .MakeGenericType(target, TransformedType, ContainerType);

    public Type MakeSerTupleWrapper(Type target) =>
        (Boxed ? typeof(BoxedDepsSerTupleWrapper<,,>) : typeof(DepsSerTupleWrapper<,,>))
        .MakeGenericType(target, TransformedType, ContainerType);
}

internal abstract record BaseDeps(EmitStub Stub, DepPlace[] Deps)
{
    public DepPlace Get(int rawIndex) => Deps[Stub.DepsIndexMap[rawIndex]];
}

internal sealed record EmitDeps(EmitStub Stub, DepPlace[] Deps) : BaseDeps(Stub, Deps);

internal sealed record RuntimeDeps(EmitStub Stub, DepPlace[] Deps) : BaseDeps(Stub, Deps);

public static class EmitDepContainer
{
    private const string ImplFieldName = "impl";
    private const string ImplPropName = "Impl";
    private const string GetImplName = "GetImpl";
    private const string SetImplName = "SetImpl";

    internal static EmitDeps BuildEmitDeps(EmitStub Stub, DepItem[] deps)
    {
        var places = deps.AsParallel().AsOrdered()
            .Select(dep =>
            {
                var container = CreateDepContainer();
                var emit_type = dep.ApplyTypeTransformOnEmitType();
                var boxed = emit_type.IsValueType;
                var t_type = boxed ? typeof(Box<>).MakeGenericType(emit_type) : emit_type;
                var applied_container = container.MakeGenericType(t_type);
                MethodInfo applied_get;
                if (applied_container.IsTypeBuilder())
                {
                    var get = container.GetMethod(GetImplName, BindingFlags.Public | BindingFlags.Static)!;
                    applied_get = TypeBuilder.GetMethod(applied_container, get);
                }
                else
                {
                    applied_get = applied_container.GetMethod(GetImplName, BindingFlags.Public | BindingFlags.Static)!;
                }
                return new DepPlace(
                    dep, container, applied_container, applied_get,
                    t_type, emit_type, dep.RawEmitType!, boxed
                );
            })
            .ToArray();
        return new EmitDeps(Stub, places);
    }

    internal static RuntimeDeps BuildRuntimeDeps(EmitDeps deps)
    {
        var places = deps.Deps.AsParallel().AsOrdered()
            .Select(dep =>
            {
                var container = dep.RawContainerType;
                var runtime_type = dep.Dep.ApplyTypeTransformOnRuntimeType();
                var boxed = dep.Boxed;
                var t_type = boxed ? typeof(Box<>).MakeGenericType(runtime_type) : runtime_type;
                var applied_container = container.MakeGenericType(t_type);
                var applied_get =
                    applied_container.GetMethod(GetImplName, BindingFlags.Public | BindingFlags.Static)!;
                return new DepPlace(
                    dep.Dep, container, applied_container, applied_get,
                    t_type, runtime_type, dep.Dep.RawRuntimeType!, boxed
                );
            })
            .ToArray();
        return new RuntimeDeps(deps.Stub, places);
    }

    internal static void SetDepsInst(RuntimeDeps deps)
    {
        deps.Deps.AsParallel().ForAll(dep =>
        {
            var field = dep.ContainerType.GetField(ImplFieldName, BindingFlags.Public | BindingFlags.Static)!;
            var inst = dep.Dep.ApplyTypeTransformOnInst();
            var boxed = dep.Boxed;
            var a_inst = inst;
            if (boxed)
            {
                var ctor = dep.ActualType.GetConstructor(BindingFlags.Public | BindingFlags.Instance,
                    new[] { dep.TransformedType })!;
                a_inst = ctor.Invoke(new[] { inst });
            }
            field.SetValue(null, a_inst);
        });
    }

    public static Type CreateDepContainer()
    {
        var guid = Guid.NewGuid();
        var module = ReflectionUtils.CreateAssembly($"_{guid:N}_");
        var type_builder = module.DefineType(
            $"{module.Assembly.GetName().Name}.DepContainer_{guid:N}",
            TypeAttributes.Public | TypeAttributes.Sealed
        );

        var T = type_builder.DefineGenericParameters("T")[0];

        var impl = type_builder.DefineField(ImplFieldName, T, FieldAttributes.Public | FieldAttributes.Static);

        var Impl = type_builder.DefineProperty(ImplPropName, PropertyAttributes.None, T, null);

        var GetImpl = type_builder.DefineMethod(
            GetImplName, MethodAttributes.Public | MethodAttributes.Static,
            T, Array.Empty<Type>()
        );
        var SetImpl = type_builder.DefineMethod(
            SetImplName, MethodAttributes.Public | MethodAttributes.Static,
            typeof(void), new Type[] { T }
        );

        Impl.SetGetMethod(GetImpl);
        Impl.SetSetMethod(SetImpl);

        {
            GetImpl.SetImplementationFlags(MethodImplAttributes.AggressiveInlining);
            var ilg = GetImpl.GetILGenerator();
            ilg.Emit(OpCodes.Ldsfld, impl);
            ilg.Emit(OpCodes.Ret);
        }

        {
            SetImpl.SetImplementationFlags(MethodImplAttributes.AggressiveInlining);
            var ilg = SetImpl.GetILGenerator();
            ilg.Emit(OpCodes.Ldarg_0);
            ilg.Emit(OpCodes.Stsfld, impl);
            ilg.Emit(OpCodes.Ret);
        }

        var dec1 = typeof(IDepsContainer1<>);
        var dec1_T = typeof(IDepsContainer1<>).MakeGenericType(T);

        type_builder.AddInterfaceImplementation(typeof(IDepsContainer));
        type_builder.AddInterfaceImplementation(dec1_T);
        type_builder.AddInterfaceImplementation(typeof(IDepsContainer<>).MakeGenericType(T));

        var dec1_prop =
            dec1.GetProperty(nameof(IDepsContainer1<int>.Impl1), BindingFlags.Public | BindingFlags.Static)!;
        var dec1_get = TypeBuilder.GetMethod(dec1_T, dec1_prop.GetMethod!);

        type_builder.DefineMethodOverride(GetImpl, dec1_get);

        var type = type_builder.CreateType();
        return type;
    }
}

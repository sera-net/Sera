using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Sera.Runtime.Utils;

namespace Sera.Runtime.Emit;

public delegate V AccessGet<T, out V>(ref T target);

public delegate void AccessSet<T, in V>(ref T target, V value);

public class EmitPrivateAccess
{
    private readonly ConditionalWeakTable<Type, CacheGroup> cache = new();

    private class CacheGroup
    {
        public readonly ConditionalWeakTable<FieldInfo, CacheStub> fields = new();
        public readonly ConditionalWeakTable<PropertyInfo, CacheStub> properties = new();
    }

    private class CacheStub
    {
        public volatile bool created;
        public readonly object create_lock = new();
        public Delegate? Delegate { get; set; }
        public DynamicMethod? Method { get; set; }
    }

    public static EmitPrivateAccess Instance { get; } = new();

    public (Delegate, DynamicMethod) AccessGetField(Type target, FieldInfo field)
    {
        var group = cache.GetOrCreateValue(target);
        var stub = group.fields.GetOrCreateValue(field);
        if (stub.created) goto ret;
        lock (stub.create_lock)
        {
            if (stub.created) goto ret;
            GenGetField(target, field, stub);
            stub.created = true;
        }
        ret:
        return (stub.Delegate!, stub.Method!);
    }

    public (Delegate, MethodInfo) AccessGetProperty(Type target, PropertyInfo property)
    {
        if (property.GetMethod == null) throw new ArgumentException("This property cannot be read", nameof(property));
        var group = cache.GetOrCreateValue(target);
        var stub = group.properties.GetOrCreateValue(property);
        if (stub.created) goto ret;
        lock (stub.create_lock)
        {
            if (stub.created) goto ret;
            GenGetProperty(target, property, stub);
            stub.created = true;
        }
        ret:
        return (stub.Delegate!, stub.Method!);
    }

    private void GenGetField(Type target, FieldInfo field, CacheStub stub)
    {
        #region create dyn method : public static V Get(ref T target);

        var guid = Guid.NewGuid();
        var asm_name = ReflectionUtils.GetAsmName($"Access.Field.{target.Name}.{field.Name}._{guid:N}_");
        var method_name = $"{asm_name}.FieldAccess_Get_{target.Name}_{field.Name}";

        var dyn_method = new DynamicMethod(
            method_name,
            MethodAttributes.Public | MethodAttributes.Static,
            CallingConventions.Standard,
            field.FieldType, new[] { target.MakeByRefType() },
            target.Module, true
        );
        dyn_method.DefineParameter(1, ParameterAttributes.None, "target");

        {
            var ilg = dyn_method.GetILGenerator();
            ilg.Emit(OpCodes.Ldarg_0);
            if (!target.IsValueType) ilg.Emit(OpCodes.Ldind_Ref);
            ilg.Emit(OpCodes.Ldfld, field);
            ilg.Emit(OpCodes.Ret);
        }

        #endregion

        #region finish

        var del = dyn_method.CreateDelegate(typeof(AccessGet<,>).MakeGenericType(target, field.FieldType));
        stub.Delegate = del;
        stub.Method = dyn_method;
        

        #endregion
    }

    private void GenGetProperty(Type target, PropertyInfo property, CacheStub stub)
    {
        #region create dyn method : public static V Get(ref T target);

        var guid = Guid.NewGuid();
        var asm_name = ReflectionUtils.GetAsmName($"Access.Property.{target.Name}.{property.Name}._{guid:N}_");
        var method_name = $"{asm_name}.PropertyAccess_Get_{target.Name}_{property.Name}";

        var dyn_method = new DynamicMethod(
            method_name,
            MethodAttributes.Public | MethodAttributes.Static,
            CallingConventions.Standard,
            property.PropertyType, new[] { target.MakeByRefType() },
            target.Module, true
        );
        dyn_method.DefineParameter(1, ParameterAttributes.None, "target");

        {
            var ilg = dyn_method.GetILGenerator();
            ilg.Emit(OpCodes.Ldarg_0);
            if (target.IsValueType)
            {
                ilg.Emit(OpCodes.Call, property.GetMethod!);
            }
            else
            {
                ilg.Emit(OpCodes.Ldind_Ref);
                ilg.Emit(OpCodes.Callvirt, property.GetMethod!);
            }
            ilg.Emit(OpCodes.Ret);
        }

        #endregion

        #region finish

        var del = dyn_method.CreateDelegate(typeof(AccessGet<,>).MakeGenericType(target, property.PropertyType));
        stub.Delegate = del;
        stub.Method = dyn_method;

        #endregion
    }
}

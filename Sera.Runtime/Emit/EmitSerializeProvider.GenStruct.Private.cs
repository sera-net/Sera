using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Sera.Core;
using Sera.Core.Impls;
using Sera.Core.Ser;
using Sera.Runtime.Utils;

namespace Sera.Runtime.Emit;

internal partial class EmitSerializeProvider
{
    private void GenPrivateStruct(Type target, StructMember[] members, CacheStub stub)
    {
        #region ready type

        var type = typeof(PrivateStructSerializeImpl<>).MakeGenericType(target);

        Type? reference_type_wrapper = null;
        if (target.IsValueType)
        {
            stub.ProvideType(type);
        }
        else
        {
            reference_type_wrapper = typeof(ReferenceTypeWrapperSerializeImpl<,>).MakeGenericType(target, type);
            stub.ProvideType(reference_type_wrapper);
        }

        #endregion

        #region create dep_container_type type builder

        var guid = Guid.NewGuid();
        var module = ReflectionUtils.CreateAssembly($"Ser.{target.Name}._{guid:N}_");
        var deps_type_builder = module.DefineType(
            $"{module.Assembly.GetName().Name}.SerializeImpl_{target.Name}_Deps",
            TypeAttributes.Public | TypeAttributes.Sealed
        );

        #endregion

        #region ready deps

        var field_count = members.Length;

        var ser_deps = GetSerDeps(members, deps_type_builder, stub.CreateThread);

        var dep_container_type = deps_type_builder.CreateType();

        stub.ProvideDeps(dep_container_type, ser_deps.Values);

        #endregion

        #region set statics

        var dep_container_type_field =
            type.GetField(nameof(PrivateStructSerializeImpl<object>.dep_container_type),
                BindingFlags.Static | BindingFlags.NonPublic)!;
        dep_container_type_field.SetValue(null, dep_container_type);

        var members_field = type.GetField(nameof(PrivateStructSerializeImpl<object>.members),
            BindingFlags.Static | BindingFlags.NonPublic)!;
        members_field.SetValue(null, members);

        var ser_deps_field = type.GetField(nameof(PrivateStructSerializeImpl<object>.ser_deps),
            BindingFlags.Static | BindingFlags.NonPublic)!;
        ser_deps_field.SetValue(null, ser_deps);

        #endregion

        #region create inst

        var ctor = type.GetConstructor(BindingFlags.Public | BindingFlags.Instance,
            new[] { typeof(string), typeof(nuint) })!;
        var inst = ctor.Invoke(new object[] { target.Name, (nuint)field_count });
        if (reference_type_wrapper == null)
        {
            stub.ProvideInst(inst);
        }
        else
        {
            var ctor2 = reference_type_wrapper.GetConstructor(new[] { type })!;
            stub.ProvideInst(ctor2.Invoke(new[] { inst }));
        }

        #endregion
    }

    internal record PrivateStructSerializeImpl<T>(string name, nuint field_count) : ISerialize<T>,
        IStructSerializerReceiver<T>
    {
#pragma warning disable CS0414
        // ReSharper disable once StaticMemberInGenericType
        internal static Type dep_container_type = null!;
        // ReSharper disable once StaticMemberInGenericType
        internal static StructMember[] members = null!;
        // ReSharper disable once StaticMemberInGenericType
        internal static Dictionary<Type, CacheStubDeps> ser_deps = null!;
#pragma warning restore CS0414

        public void Write<S>(S serializer, T value, ISeraOptions options) where S : ISerializer
        {
            serializer.StartStruct(name, field_count, value, this);
        }

        public void Receive<S>(T value, S serializer) where S : IStructSerializer
        {
            ReceiveImpl<S>.Delegate.Value.Invoke(value, serializer);
        }

        internal static class ReceiveImpl<S> where S : IStructSerializer
        {
            public static readonly Lazy<Action<T, S>> Delegate = new(Create);

            private static Action<T, S> Create()
            {
                var target = typeof(T);

                var guid = Guid.NewGuid();
                var dyn_method_name = ReflectionUtils.GetAsmName($"ReceiveImpl_{typeof(T).Name}_{guid}");
                var dyn_method = new DynamicMethod(
                    dyn_method_name,
                    MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard,
                    typeof(void), new[] { typeof(T), typeof(S) },
                    typeof(T).Module, true
                );

                var ilg = dyn_method.GetILGenerator();

                #region def local_int_key_null

                LocalBuilder? local_int_key_null = null;
                if (members.Any(m => !m.IntKey.HasValue))
                {
                    local_int_key_null = ilg.DeclareLocal(typeof(long?));
                    ilg.Emit(OpCodes.Ldloca_S, local_int_key_null);
                    ilg.Emit(OpCodes.Initobj, typeof(long?));
                }

                #endregion

                #region write members

                foreach (var member in members)
                {
                    var field_type = member.Type;
                    var dep = ser_deps[field_type];

                    var write_field = ReflectionUtils.IStructSerializer_WriteField_2generic_3arg_string_t_s
                        .MakeGenericMethod(member.Type, dep.ImplType);

                    #region load serializer

                    ilg.Emit(OpCodes.Ldarga_S, 1);

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
                        var get_method = property.GetMethod!;
                        if (target.IsValueType)
                        {
                            #region load value

                            ilg.Emit(OpCodes.Ldarga_S, 0);

                            #endregion

                            #region get value.mermber_property

                            ilg.Emit(OpCodes.Call, get_method);

                            #endregion
                        }
                        else
                        {
                            #region load value

                            ilg.Emit(OpCodes.Ldarg_0);

                            #endregion

                            #region get value.mermber_property

                            ilg.Emit(OpCodes.Callvirt, get_method);

                            #endregion
                        }
                    }
                    else if (member.Kind is PropertyOrField.Field)
                    {
                        var field = member.Field!;

                        #region load value

                        ilg.Emit(OpCodes.Ldarg_0);

                        #endregion

                        #region load get value.mermber_field

                        ilg.Emit(OpCodes.Ldfld, field);

                        #endregion
                    }
                    else throw new ArgumentOutOfRangeException();

                    #endregion

                    #region load Deps._impl_n

                    var dep_field = dep_container_type.GetField(
                        dep.Field.Name,
                        BindingFlags.Static | BindingFlags.Public
                    )!;
                    ilg.Emit(OpCodes.Ldsfld, dep_field);

                    #endregion

                    #region serializer.WriteField<V, VImpl>(nameof member, member_value, Self._impl_n);

                    ilg.Emit(OpCodes.Constrained, typeof(S));
                    ilg.Emit(OpCodes.Callvirt, write_field);

                    #endregion
                }

                #endregion

                #region return;

                ilg.Emit(OpCodes.Ret);

                #endregion

                var del = dyn_method.CreateDelegate<Action<T, S>>();
                return del;
            }
        }
    }
}

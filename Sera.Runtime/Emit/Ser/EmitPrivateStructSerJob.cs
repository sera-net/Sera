using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Sera.Core;
using Sera.Core.Ser;
using Sera.Runtime.Emit.Deps;
using Sera.Runtime.Utils;

namespace Sera.Runtime.Emit.Ser;

internal record EmitPrivateStructSerJob(StructMember[] Members) : EmitStructSerJob(Members)
{
    public override bool EmitTypeIsTypeBuilder => false;
    private Type type = null!;

    public override void Init(EmitStub stub, EmitMeta target)
    {
        type = typeof(PrivateStructSerializeImpl<>).MakeGenericType(target.Type);
    }

    public override Type GetEmitType(EmitStub stub, EmitMeta target, DepItem[] deps)
        => type;

    public override Type GetRuntimeType(EmitStub stub, EmitMeta target, DepItem[] deps)
        => type;

    public override Type GetEmitPlaceholderType(EmitStub stub, EmitMeta target)
        => type;

    public override Type GetRuntimePlaceholderType(EmitStub stub, EmitMeta target)
        => type;

    public override void Emit(EmitStub stub, EmitMeta target, EmitDeps deps)
    {
        var members_field = type.GetField(nameof(PrivateStructSerializeImpl<object>.members),
            BindingFlags.Static | BindingFlags.NonPublic)!;
        members_field.SetValue(null, Members);
    }

    public override object CreateInst(EmitStub stub, EmitMeta target, RuntimeDeps deps)
    {
        var deps_field = type.GetField(nameof(PrivateStructSerializeImpl<object>.deps),
            BindingFlags.Static | BindingFlags.NonPublic)!;
        deps_field.SetValue(null, deps);

        var ctor = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance,
            new[] { typeof(string), typeof(nuint) })!;
        return ctor.Invoke(new object[] { target.Type.Name, (nuint)Members.Length });
    }
}

public class PrivateStructSerializeImpl<T> : ISerialize<T>, IStructSerializerReceiver<T>
{
#pragma warning disable CS0414
    // ReSharper disable once StaticMemberInGenericType
    internal static StructMember[] members = null!;
    // ReSharper disable once StaticMemberInGenericType
    internal static RuntimeDeps deps = null!;
#pragma warning restore CS0414

    internal readonly string name;
    internal readonly nuint field_count;

    internal PrivateStructSerializeImpl(string name, nuint field_count)
    {
        this.name = name;
        this.field_count = field_count;
    }

    public void Write<S>(S serializer, T value, ISeraOptions options) where S : ISerializer
    {
        if (!typeof(T).IsValueType && value == null) throw new NullReferenceException();
        serializer.StartStruct(name, field_count, value, this);
    }

    public void Receive<S>(T value, S serializer) where S : IStructSerializer
    {
        ReceiveImpl<S>.Delegate.Value.Invoke(value, serializer);
    }

    private static class ReceiveImpl<S> where S : IStructSerializer
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

            foreach (var (member, index) in members.Select((a, b) => (a, b)))
            {
                var dep = deps.Get(index);

                var write_field = ReflectionUtils.IStructSerializer_WriteField_2generic_3arg_string_t_s
                    .MakeGenericMethod(member.Type, dep.TransformedType);

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

                #region load dep

                ilg.Emit(OpCodes.Call, dep.GetDepMethodInfo);
                if (dep.Boxed)
                {
                    var get_method = dep.MakeBoxGetMethodInfo();
                    ilg.Emit(OpCodes.Call, get_method);
                }

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

using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Sera.Core.Ser;
using Sera.Runtime.Emit.Deps;
using Sera.Runtime.Emit.Ser.Internal;
using Sera.Runtime.Utils;

namespace Sera.Runtime.Emit.Ser.Jobs._Struct;

internal class _Private(StructMember[] Members) : _Struct(Members)
{
    private Type ImplType = null!;
    
    public override void Init(EmitStub stub, EmitMeta target)
    {
        ImplType = typeof(PrivateStructSerializeImpl<>).MakeGenericType(target.Type);
    }

    public override Type GetEmitType(EmitStub stub, EmitMeta target, DepItem[] deps)
        => ImplType;

    public override Type GetRuntimeType(EmitStub stub, EmitMeta target, DepItem[] deps)
        => ImplType;

    public override Type GetEmitPlaceholderType(EmitStub stub, EmitMeta target)
        => ImplType;

    public override Type GetRuntimePlaceholderType(EmitStub stub, EmitMeta target)
        => ImplType;

    public override void Emit(EmitStub stub, EmitMeta target, EmitDeps deps) { }

    public override object CreateInst(EmitStub stub, EmitMeta target, RuntimeDeps deps)
    {
        var key = new object();
        var meta = new MetaData(Members, deps);
        Metas.Add(key, meta);

        var ctor = ImplType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance,
            new[] { typeof(object), typeof(string), typeof(nuint) })!;
        return ctor.Invoke(new[] { key, target.Type.Name, (nuint)Members.Length });
    }

    private record MetaData(StructMember[] Members, RuntimeDeps Deps);

    private static readonly ConditionalWeakTable<object, MetaData> Metas = new();

    internal static class ReceiveImpl<T, S> where S : IStructSerializer
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Receive(object meta_key, T value, S serializer)
        {
            GetDelegate(meta_key).Invoke(value, serializer);
        }

        private static readonly ConditionalWeakTable<object, Action<T, S>> Delegates = new();

        private static Action<T, S> GetDelegate(object meta_key) => Delegates.GetValue(meta_key, static key =>
        {
            if (Metas.TryGetValue(key, out var meta)) return Create(meta);
            throw new NullReferenceException();
        });
        
        private static Action<T, S> Create(MetaData meta)
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
            if (meta.Members.Any(m => !m.IntKey.HasValue))
            {
                local_int_key_null = ilg.DeclareLocal(typeof(long?));
                ilg.Emit(OpCodes.Ldloca_S, local_int_key_null);
                ilg.Emit(OpCodes.Initobj, typeof(long?));
            }

            #endregion

            #region write members

            foreach (var (member, index) in meta.Members.Select((a, b) => (a, b)))
            {
                var dep = meta.Deps.Get(index);

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

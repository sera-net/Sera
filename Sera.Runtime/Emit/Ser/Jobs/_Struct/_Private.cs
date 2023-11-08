using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Sera.Core;
using Sera.Runtime.Emit.Deps;
using Sera.Runtime.Emit.Ser.Internal;
using Sera.Runtime.Utils;
using Sera.Runtime.Utils.Internal;

namespace Sera.Runtime.Emit.Ser.Jobs._Struct;

internal class _Private(string StructName, StructMember[] Members) : _Struct(Members)
{
    private Type ImplType = null!;

    public override void Init(EmitStub stub, EmitMeta target)
    {
        ImplType = typeof(PrivateStructImpl<>).MakeGenericType(target.Type);
    }

    public override Type GetEmitType(EmitStub stub, EmitMeta target, EmitDeps deps)
        => ImplType;

    public override Type GetRuntimeType(EmitStub stub, EmitMeta target, RuntimeDeps deps)
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
            new[] { typeof(object), typeof(string), typeof(int) })!;
        return ctor.Invoke(new[] { key, StructName, Members.Length });
    }

    private record MetaData(StructMember[] Members, RuntimeDeps Deps);

    private static readonly ConditionalWeakTable<object, MetaData> Metas = new();

    internal static class Impl<T, R, V> where V : AStructSeraVisitor<R>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static R AcceptField(V visitor, ref T value, int field, object meta_key)
            => GetDelegate(meta_key).Invoke(visitor, ref value, field);

        private static readonly ConditionalWeakTable<object, FnAcceptField> Delegates = new();

        public delegate R FnAcceptField(V visitor, ref T value, int field);

        private static FnAcceptField GetDelegate(object meta_key) => Delegates.GetValue(meta_key, static key =>
        {
            if (Metas.TryGetValue(key, out var meta)) return Create(meta);
            throw new NullReferenceException();
        });

        private static FnAcceptField Create(MetaData meta)
        {
            var target = typeof(T);

            var guid = Guid.NewGuid();
            var dyn_method_name = ReflectionUtils.GetAsmName($"Ser_{typeof(T).Name}_{guid:N}");
            var dyn_method = new DynamicMethod(
                dyn_method_name,
                MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard,
                typeof(R), new[] { typeof(V), typeof(T).MakeByRefType(), typeof(int) },
                typeof(T).Module, true
            );

            var ilg = dyn_method.GetILGenerator();

            var label_default = ilg.DefineLabel();

            var visitor = typeof(AStructSeraVisitor<>).MakeGenericType(typeof(R));
            var v_field_method_decl = visitor.GetMethod(nameof(AStructSeraVisitor<object>.VField))!;
            var v_none_method = visitor.GetMethod(nameof(AStructSeraVisitor<object>.VNone))!;

            #region switch field

            var labels = meta.Members.Select(_ => ilg.DefineLabel()).ToArray();
            ilg.Emit(OpCodes.Ldarg_2);
            ilg.Emit(OpCodes.Switch, labels);
            ilg.Emit(OpCodes.Br, label_default);

            #endregion

            #region members

            foreach (var (member, i) in meta.Members.Select((a, b) => (a, b)))
            {
                var label = labels[i];
                var dep = meta.Deps.Get(i);
                var member_type = member.Type;

                #region load visitor

                ilg.MarkLabel(label);
                ilg.Emit(OpCodes.Ldarg_0);
                ilg.Emit(OpCodes.Box, typeof(V));

                #endregion

                #region load dep

                ilg.Emit(OpCodes.Call, dep.GetDepMethodInfo);
                if (dep.Boxed)
                {
                    var get_method = dep.MakeBoxGetMethodInfo();
                    ilg.Emit(OpCodes.Call, get_method);
                }

                #endregion

                #region load member

                if (member.Kind is PropertyOrField.Property)
                {
                    var property = member.Property!;
                    var get_method = property.GetMethod!;
                    if (target.IsValueType)
                    {
                        #region load value

                        ilg.Emit(OpCodes.Ldarg_1);

                        #endregion

                        #region get value.mermber_property

                        ilg.Emit(OpCodes.Call, get_method);

                        #endregion
                    }
                    else
                    {
                        #region load value

                        ilg.Emit(OpCodes.Ldarg_1);
                        ilg.Emit(OpCodes.Ldind_Ref);

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

                    ilg.Emit(OpCodes.Ldarg_1);
                    if (!target.IsValueType)
                        ilg.Emit(OpCodes.Ldind_Ref);

                    #endregion

                    #region load get value.mermber_field

                    ilg.Emit(OpCodes.Ldfld, field);

                    #endregion
                }
                else throw new ArgumentOutOfRangeException();

                #endregion

                #region load name

                ilg.Emit(OpCodes.Ldstr, member.Name);

                #endregion

                #region load key

                if (member.IntKey.HasValue)
                {
                    ilg.Emit(OpCodes.Ldc_I8, member.IntKey.Value);
                }
                else
                {
                    ilg.Emit(OpCodes.Ldarg_2);
                    ilg.Emit(OpCodes.Conv_I8);
                }

                #endregion

                #region call VField

                var v_field_method = v_field_method_decl.MakeGenericMethod(dep.TransformedType, member_type);
                ilg.Emit(OpCodes.Callvirt, v_field_method);
                ilg.Emit(OpCodes.Ret);

                #endregion
            }

            #endregion

            #region default => visitor.VNone();

            ilg.MarkLabel(label_default);
            ilg.Emit(OpCodes.Ldarg_0);
            ilg.Emit(OpCodes.Box, typeof(V));
            ilg.Emit(OpCodes.Callvirt, v_none_method);
            ilg.Emit(OpCodes.Ret);

            #endregion

            var del = dyn_method.CreateDelegate<FnAcceptField>();
            return del;
        }
    }
}

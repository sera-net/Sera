using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Sera.Core;
using Sera.Core.Ser;

namespace Sera.Runtime.Emit;

internal partial class EmitSerializeProvider
{
    private void GenEnumVariantPrivate(
        Type target, Type underlying_type, EnumInfo[] items, EnumJumpTables? jump_table, CacheStub stub
    )
    {
        #region ready type

        var type = typeof(PrivateEnumSerializeImpl<>).MakeGenericType(target);
        stub.ProvideType(type);

        #endregion

        #region ready

        var create_variant_tag = typeof(VariantTag)
            .GetMethod(
                nameof(VariantTag.Create), BindingFlags.Static | BindingFlags.Public,
                new[] { underlying_type }
            )!;

        #endregion

        #region ready to tag

        var fn_arg_1 = Expression.Parameter(target);
        var e_to_u = Expression.Convert(fn_arg_1, underlying_type);
        var to_tag_call = Expression.Call(null, create_variant_tag, e_to_u);
        var to_tag = Expression.Lambda(to_tag_call, fn_arg_1).Compile();

        #endregion

        #region metas

        var metas_field = type.GetField(nameof(PrivateEnumSerializeImpl<Enum>.metas),
            BindingFlags.Public | BindingFlags.Static)!;
        var metas_type = typeof(Dictionary<,>).MakeGenericType(target, variant_meta_type);
        var metas_inst = Activator.CreateInstance(metas_type!);
        var add = metas_type!.GetMethod(
            nameof(Dictionary<int, string>.Add),
            BindingFlags.Public | BindingFlags.Instance,
            new[] { target, variant_meta_type }
        )!;
        foreach (var item in items)
        {
            (string name, SerializerVariantHint? hint) meta = (item.Name, null);
            add.Invoke(metas_inst, new[] { item.Tag.ToObject(), meta });
        }
        metas_field.SetValue(null, metas_inst);

        #endregion

        #region create inst

        var del_type = typeof(Func<,>).MakeGenericType(target, typeof(VariantTag));

        var ctor = type.GetConstructor(
            BindingFlags.Public | BindingFlags.Instance,
            new[] { typeof(string), del_type }
        )!;
        var inst = ctor.Invoke(new object[] { target.Name, to_tag });
        stub.ProvideInst(inst);

        #endregion
    }

    internal record PrivateEnumSerializeImpl<T>(string union_name, Func<T, VariantTag> ToTag) : ISerialize<T>
        where T : Enum
    {
        public static Dictionary<T, (string name, SerializerVariantHint? hint)> metas = null!;

        public void Write<S>(S serializer, T value, ISeraOptions options) where S : ISerializer
        {
            serializer.WriteVariantUnit<T>(union_name,
                metas.TryGetValue(value, out var meta)
                    ? new Variant(meta.name, ToTag(value))
                    : new Variant(ToTag(value)), meta.hint);
        }
    }
}

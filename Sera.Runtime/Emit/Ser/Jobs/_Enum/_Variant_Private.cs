using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Sera.Core;
using Sera.Core.Ser;
using Sera.Runtime.Emit.Deps;
using Sera.Runtime.Emit.Ser.Internal;
using Sera.Runtime.Utils;

namespace Sera.Runtime.Emit.Ser.Jobs._Enum;

internal class _Variant_Private
    (Type UnderlyingType, EnumInfo[] Items, SeraEnumAttribute? EnumAttr)
    : _Variant(UnderlyingType, Items, EnumAttr)
{
    private Type ImplType = null!;
    private Type ToTagType = null!;
    private Type MetasDictType = null!;
    private Type MetasType = null!;

    private MethodInfo MetasDictAdd = null!;
    private MethodInfo ToFrozen = null!;
    private MethodInfo CreateVariantTag = null!;

    private Delegate ToTag = null!;
    private object Metas = null!;
    
    public override void Init(EmitStub stub, EmitMeta target)
    {
        ImplType = typeof(PrivateEnumSerializeImpl<>).MakeGenericType(target.Type);
        ToTagType = typeof(Func<,>).MakeGenericType(target.Type, typeof(VariantTag));

        MetasDictType = typeof(Dictionary<,>).MakeGenericType(target.Type, VariantMetaType);
        MetasType = typeof(FrozenDictionary<,>).MakeGenericType(target.Type, VariantMetaType);

        ToFrozen = ReflectionUtils.ToFrozenDictionary_2generic_2arg__IEnumerable_KeyValuePair__IEqualityComparer
            .MakeGenericMethod(target.Type, VariantMetaType);

        MetasDictAdd = MetasDictType.GetMethod(
            nameof(Dictionary<int, string>.Add),
            BindingFlags.Public | BindingFlags.Instance,
            new[] { target.Type, VariantMetaType }
        )!;

        CreateVariantTag = typeof(VariantTag)
            .GetMethod(
                nameof(VariantTag.Create), BindingFlags.Static | BindingFlags.Public,
                new[] { UnderlyingType }
            )!;
    }

    public override EmitTransform[] CollectTransforms(EmitStub stub, EmitMeta target)
        => EmitTransform.EmptyTransforms;

    public override DepMeta[] CollectDeps(EmitStub stub, EmitMeta target)
        => Array.Empty<DepMeta>();

    public override Type GetEmitType(EmitStub stub, EmitMeta target, DepItem[] deps)
        => ImplType;

    public override Type GetRuntimeType(EmitStub stub, EmitMeta target, DepItem[] deps)
        => ImplType;

    public override Type GetEmitPlaceholderType(EmitStub stub, EmitMeta target)
        => ImplType;

    public override Type GetRuntimePlaceholderType(EmitStub stub, EmitMeta target)
        => ImplType;

    public override void Emit(EmitStub stub, EmitMeta target, EmitDeps deps)
    {
        #region ready to tag

        var fn_arg_1 = Expression.Parameter(target.Type);
        var e_to_u = Expression.Convert(fn_arg_1, UnderlyingType);
        var to_tag_call = Expression.Call(null, CreateVariantTag, e_to_u);
        ToTag = Expression.Lambda(to_tag_call, fn_arg_1).Compile();

        #endregion

        #region ready metas

        var metas_dict = Activator.CreateInstance(MetasDictType);
        foreach (var item in Items)
        {
            var item_hint = item.EnumAttr?.SerHint ?? RootHint;
            (string name, SerializerVariantHint? hint) meta = (item.Name, item_hint);
            MetasDictAdd.Invoke(metas_dict, new[] { item.Tag.ToObject(), meta });
        }
        Metas = ToFrozen.Invoke(null, new[] { metas_dict, null })!;

        #endregion
    }

    public override object CreateInst(EmitStub stub, EmitMeta target, RuntimeDeps deps)
    {
        var ctor = ImplType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance,
            new[] { typeof(string), ToTagType, MetasType, typeof(SerializerVariantHint?) })!;
        return ctor.Invoke(new[] { target.Type.Name, ToTag, Metas, RootHint });
    }
}

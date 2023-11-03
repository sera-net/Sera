using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Sera.Core;
using Sera.Runtime.Emit.Deps;
using Sera.Runtime.Emit.Ser.Internal;
using Sera.Runtime.Utils;

namespace Sera.Runtime.Emit.Ser.Jobs._Enum;

internal class _Variant_Public
    (string UnionName, Type UnderlyingType, EnumInfo[] Items, EnumJumpTables? JumpTable,
        UnionStyle? UnionStyle, SeraUnionMode Mode)
    : _Variant
{
    public const int MaxIfNums = 16;

    public TypeBuilder TypeBuilder { get; private set; } = null!;
    public Type RuntimeType { get; set; } = null!;
    public MethodInfo CreateVariantTag { get; private set; } = null!;

    public static ConstructorInfo NewVariantByTag { get; } = typeof(Variant)
        .GetConstructor(
            BindingFlags.Public | BindingFlags.Instance,
            new[] { typeof(VariantTag) }
        )!;
    public static ConstructorInfo NewVariantByNameTag { get; } = typeof(Variant)
        .GetConstructor(
            BindingFlags.Public | BindingFlags.Instance,
            new[] { typeof(string), typeof(VariantTag) }
        )!;

    public readonly List<(FieldBuilder, VariantStyle)> VariantStyles = new();
    private FieldBuilder? union_style_field;

    private Type? MetasType;
    private Type? MetasDictType;
    private FieldBuilder? MetasField;

    private const string UnionStyleFieldName = "_union_style";

    public override void Init(EmitStub stub, EmitMeta target)
    {
        TypeBuilder = CreateTypeBuilderStruct($"Ser_{target.Type.Name}");
        TypeBuilder.MarkReadonly();

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

    public override Type GetEmitPlaceholderType(EmitStub stub, EmitMeta target) => TypeBuilder;

    public override Type GetRuntimePlaceholderType(EmitStub stub, EmitMeta target) => RuntimeType;

    public override Type GetEmitType(EmitStub stub, EmitMeta target, EmitDeps deps) => TypeBuilder;

    public override Type GetRuntimeType(EmitStub stub, EmitMeta target, RuntimeDeps deps) => RuntimeType;

    public override void Emit(EmitStub stub, EmitMeta target, EmitDeps deps)
    {
        VariantStyles.Clear();
        EmitAccept(target);
        EmitUnion(target);
        RuntimeType = TypeBuilder.CreateType();

        if (union_style_field != null)
        {
            var rt_union_style_field =
                RuntimeType.GetField(UnionStyleFieldName, BindingFlags.NonPublic | BindingFlags.Static)!;
            rt_union_style_field.SetValue(null, UnionStyle);
        }

        foreach (var (field, style) in VariantStyles)
        {
            var rt_field = RuntimeType.GetField(field.Name, BindingFlags.NonPublic | BindingFlags.Static)!;
            rt_field.SetValue(null, style);
        }

        if (MetasField != null)
        {
            var metas_field2 = RuntimeType.GetField(MetasField.Name, BindingFlags.NonPublic | BindingFlags.Static)!;
            var metas_inst = Activator.CreateInstance(MetasDictType!);
            var add = MetasDictType!.GetMethod(
                nameof(Dictionary<int, string>.Add),
                BindingFlags.Public | BindingFlags.Instance,
                new[] { UnderlyingType, VariantMetaType }
            )!;
            foreach (var item in Items)
            {
                var item_style = item.Style;
                var meta = new VariantMeta(item.Name, item_style);
                add.Invoke(metas_inst, new[] { item.Tag.ToObject(), meta });
            }
            var to_frozen = ReflectionUtils
                .ToFrozenDictionary_2generic_2arg__IEnumerable_KeyValuePair__IEqualityComparer
                .MakeGenericMethod(UnderlyingType, VariantMetaType);
            metas_inst = to_frozen.Invoke(null, new[] { metas_inst, null });
            metas_field2.SetValue(null, metas_inst);
        }
    }

    public override object CreateInst(EmitStub stub, EmitMeta target, RuntimeDeps deps)
    {
        return Activator.CreateInstance(RuntimeType)!;
    }

    private void EmitAccept(EmitMeta target)
    {
        var accept_method = TypeBuilder.DefineMethod(nameof(ISeraVision<object>.Accept),
            MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.NewSlot);
        var generic_parameters = accept_method.DefineGenericParameters("R", "V");
        var TR = generic_parameters[0];
        var TV = generic_parameters[1];
        var visitor = typeof(ASeraVisitor<>).MakeGenericType(TR);
        TV.SetBaseTypeConstraint(visitor);
        accept_method.SetReturnType(TR);
        accept_method.SetParameters(TV, target.Type);
        accept_method.DefineParameter(1, ParameterAttributes.None, "visitor");
        accept_method.DefineParameter(2, ParameterAttributes.None, "value");
        accept_method.SetImplementationFlags(MethodImplAttributes.AggressiveInlining);

        var v_union_method = TypeBuilder.GetMethod(visitor, ReflectionUtils.ASeraVisitor_VUnion)
            .MakeGenericMethod(TypeBuilder, target.Type);

        var ilg = accept_method.GetILGenerator();

        #region return visitor.VUnion<Self, T>(this, value);

        ilg.Emit(OpCodes.Ldarg_1);
        ilg.Emit(OpCodes.Box, TV);
        ilg.Emit(OpCodes.Ldarg_0);
        ilg.Emit(OpCodes.Ldobj, TypeBuilder);
        ilg.Emit(OpCodes.Ldarg_2);
        ilg.Emit(OpCodes.Callvirt, v_union_method);
        ilg.Emit(OpCodes.Ret);

        #endregion

        var interface_type = typeof(ISeraVision<>).MakeGenericType(target.Type);
        TypeBuilder.AddInterfaceImplementation(interface_type);
        TypeBuilder.DefineMethodOverride(accept_method,
            interface_type.GetMethod(nameof(ISeraVision<object>.Accept))!);
    }

    private void EmitUnion(EmitMeta target)
    {
        #region Name

        var name_property = TypeBuilder.DefineProperty(
            nameof(IUnionSeraVision<object>.Name),
            PropertyAttributes.None,
            typeof(string),
            Array.Empty<Type>()
        );
        var get_name_method = TypeBuilder.DefineMethod(
            $"get_{nameof(IUnionSeraVision<object>.Name)}",
            MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.Virtual | MethodAttributes.NewSlot,
            typeof(string), Array.Empty<Type>()
        );
        get_name_method.SetImplementationFlags(MethodImplAttributes.AggressiveInlining);
        name_property.SetGetMethod(get_name_method);

        {
            var ilg = get_name_method.GetILGenerator();
            ilg.Emit(OpCodes.Ldstr, UnionName);
            ilg.Emit(OpCodes.Ret);
        }

        #endregion

        #region AcceptUnion

        if (UnionStyle != null)
        {
            union_style_field = TypeBuilder.DefineField(UnionStyleFieldName, typeof(UnionStyle),
                FieldAttributes.Private | FieldAttributes.Static);
        }

        var accept_union_method = TypeBuilder.DefineMethod(nameof(IUnionSeraVision<object>.AcceptUnion),
            MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.NewSlot);
        var generic_parameters = accept_union_method.DefineGenericParameters("R", "V");
        var TR = generic_parameters[0];
        var TV = generic_parameters[1];
        var visitor = typeof(AUnionSeraVisitor<>).MakeGenericType(TR);
        TV.SetBaseTypeConstraint(visitor);
        accept_union_method.SetReturnType(TR);
        accept_union_method.SetParameters(TV, target.Type);
        accept_union_method.DefineParameter(1, ParameterAttributes.None, "visitor");
        accept_union_method.DefineParameter(2, ParameterAttributes.None, "value");
        accept_union_method.SetImplementationFlags(MethodImplAttributes.AggressiveInlining);

        {
            var v_variant_method = TypeBuilder.GetMethod(visitor, ReflectionUtils.AUnionSeraVisitor_VVariant);
            var v_none_method = TypeBuilder.GetMethod(visitor, ReflectionUtils.AUnionSeraVisitor_VNone);
            var v_empty_method = TypeBuilder.GetMethod(visitor, ReflectionUtils.AUnionSeraVisitor_VEmpty);
            var ilg = accept_union_method.GetILGenerator();

            var label_default = ilg.DefineLabel();

            #region Variants

            if (Items.Length > 0)
            {
                void GenCases(Label[] labels)
                {
                    for (int i = 0; i < Items.Length; i++)
                    {
                        var item = Items[i];
                        var label = labels[i];
                        var style = item.Style;

                        #region return visitor.VVariant(new Variant(Name, VariantTag.Create(value)), union_style, variant_style);

                        ilg.MarkLabel(label);
                        ilg.Emit(OpCodes.Ldarg_1);
                        ilg.Emit(OpCodes.Box, TV);
                        ilg.Emit(OpCodes.Ldstr, item.Name);
                        ilg.Emit(OpCodes.Ldarg_2);
                        ilg.Emit(OpCodes.Call, CreateVariantTag);
                        ilg.Emit(OpCodes.Newobj, NewVariantByNameTag);

                        #region load union_style

                        if (union_style_field == null) ilg.Emit(OpCodes.Ldnull);
                        else ilg.Emit(OpCodes.Ldsfld, union_style_field);

                        #endregion

                        #region load variant style

                        if (style == null) ilg.Emit(OpCodes.Ldnull);
                        else
                        {
                            var variant_style_field = TypeBuilder.DefineField($"_variant_style_{i}",
                                typeof(VariantStyle),
                                FieldAttributes.Private | FieldAttributes.Static);
                            VariantStyles.Add((variant_style_field, style));
                            ilg.Emit(OpCodes.Ldsfld, variant_style_field);
                        }

                        #endregion

                        ilg.Emit(OpCodes.Callvirt, v_variant_method);
                        ilg.Emit(OpCodes.Ret);

                        #endregion
                    }
                }

                if (JumpTable != null)
                {
                    var labels = Items
                        .Select(_ => ilg.DefineLabel())
                        .ToArray();

                    #region load value

                    ilg.Emit(OpCodes.Ldarg_2);

                    #endregion

                    #region apply offset

                    if (JumpTable.Offset == 0)
                    {
                        if (UnderlyingType != typeof(int) && UnderlyingType != typeof(uint))
                        {
                            ilg.Emit(OpCodes.Conv_I4);
                        }
                    }
                    else
                    {
                        if (UnderlyingType == typeof(long) || UnderlyingType == typeof(ulong))
                        {
                            ilg.Emit(OpCodes.Ldc_I8, (long)JumpTable.Offset);
                            ilg.Emit(OpCodes.Add);
                            ilg.Emit(OpCodes.Conv_I4);
                        }
                        else
                        {
                            ilg.Emit(OpCodes.Conv_I4);
                            ilg.Emit(OpCodes.Ldc_I4, JumpTable.Offset);
                            ilg.Emit(OpCodes.Add);
                        }
                    }

                    #endregion

                    #region switch

                    var table = JumpTable.Table
                        .Select(a => labels[a.Index])
                        .ToArray();
                    ilg.Emit(OpCodes.Switch, table);

                    #endregion

                    #region goto default

                    ilg.Emit(OpCodes.Br, label_default);

                    #endregion

                    GenCases(labels);
                }
                else if (Items.Length <= MaxIfNums)
                {
                    var labels = Items
                        .Select(_ => ilg.DefineLabel())
                        .ToArray();

                    for (var i = 0; i < Items.Length; i++)
                    {
                        var item = Items[i];
                        var label = labels[i];

                        #region if value == Enum.X then goto label

                        ilg.Emit(OpCodes.Ldarg_2);
                        if (UnderlyingType == typeof(long) || UnderlyingType == typeof(ulong))
                            ilg.Emit(OpCodes.Ldc_I8, item.Tag.ToLong());
                        else
                            ilg.Emit(OpCodes.Ldc_I4, item.Tag.ToInt());
                        ilg.Emit(OpCodes.Beq, label);

                        #endregion
                    }

                    #region goto default

                    ilg.Emit(OpCodes.Br, label_default);

                    #endregion

                    GenCases(labels);
                }
                else
                {
                    var meta_loc = ilg.DeclareLocal(VariantMetaType);
                    MetasType = typeof(FrozenDictionary<,>).MakeGenericType(UnderlyingType, VariantMetaType);
                    MetasDictType = typeof(Dictionary<,>).MakeGenericType(UnderlyingType, VariantMetaType);
                    MetasField = TypeBuilder.DefineField(
                        "_metas", MetasType,
                        FieldAttributes.Private | FieldAttributes.Static
                    );

                    var try_gey_meta = MetasType.GetMethod(
                        nameof(FrozenDictionary<int, int>.TryGetValue), BindingFlags.Public | BindingFlags.Instance,
                        new[] { UnderlyingType, VariantMetaType.MakeByRefType() }
                    )!;

                    #region if (!_metas.TryGetValue(value, out meta)) goto default

                    ilg.Emit(OpCodes.Ldsfld, MetasField);
                    ilg.Emit(OpCodes.Ldarg_2);
                    ilg.Emit(OpCodes.Ldloca_S, (byte)meta_loc.LocalIndex);
                    ilg.Emit(OpCodes.Callvirt, try_gey_meta);

                    ilg.Emit(OpCodes.Brfalse, label_default);

                    #endregion

                    #region return visitor.VVariant(new Variant(Name, VariantTag.Create(value)), union_style, variant_style);

                    ilg.Emit(OpCodes.Ldarg_1);
                    ilg.Emit(OpCodes.Box, TV);

                    #region load name

                    ilg.Emit(OpCodes.Ldloc, meta_loc);
                    ilg.Emit(OpCodes.Ldfld, VariantMetaFieldName);

                    #endregion

                    ilg.Emit(OpCodes.Ldarg_2);
                    ilg.Emit(OpCodes.Call, CreateVariantTag);
                    ilg.Emit(OpCodes.Newobj, NewVariantByNameTag);

                    #region load union_style

                    if (union_style_field == null) ilg.Emit(OpCodes.Ldnull);
                    else ilg.Emit(OpCodes.Ldsfld, union_style_field);

                    #endregion

                    #region load variant_style

                    ilg.Emit(OpCodes.Ldloc, meta_loc);
                    ilg.Emit(OpCodes.Ldfld, VariantMetaFieldStyle);

                    #endregion


                    ilg.Emit(OpCodes.Callvirt, v_variant_method);
                    ilg.Emit(OpCodes.Ret);

                    #endregion
                }
            }
            else if (Mode is SeraUnionMode.Exhaustive)
            {
                #region return visitor.VEmpty();

                ilg.Emit(OpCodes.Ldarg_1);
                ilg.Emit(OpCodes.Box, TV);
                ilg.Emit(OpCodes.Callvirt, v_empty_method);
                ilg.Emit(OpCodes.Ret);

                #endregion
            }

            #endregion

            #region default

            if (Mode is SeraUnionMode.Exhaustive)
            {
                if (Items.Length > 0)
                {
                    #region return visitor.VNone();

                    ilg.MarkLabel(label_default);
                    ilg.Emit(OpCodes.Ldarg_1);
                    ilg.Emit(OpCodes.Box, TV);
                    ilg.Emit(OpCodes.Callvirt, v_none_method);
                    ilg.Emit(OpCodes.Ret);

                    #endregion
                }
            }
            else
            {
                #region return visitor.VVariant(new Variant(VariantTag.Create(value), union_style, null);

                ilg.MarkLabel(label_default);
                ilg.Emit(OpCodes.Ldarg_1);
                ilg.Emit(OpCodes.Box, TV);
                ilg.Emit(OpCodes.Ldarg_2);
                ilg.Emit(OpCodes.Call, CreateVariantTag);
                ilg.Emit(OpCodes.Newobj, NewVariantByTag);

                #region load union_style

                if (union_style_field == null) ilg.Emit(OpCodes.Ldnull);
                else ilg.Emit(OpCodes.Ldsfld, union_style_field);

                #endregion

                ilg.Emit(OpCodes.Ldnull);
                ilg.Emit(OpCodes.Callvirt, v_variant_method);
                ilg.Emit(OpCodes.Ret);

                #endregion
            }

            #endregion
        }

        #endregion

        #region interface

        var interface_type = typeof(IUnionSeraVision<>).MakeGenericType(target.Type);
        TypeBuilder.AddInterfaceImplementation(interface_type);
        TypeBuilder.DefineMethodOverride(get_name_method,
            interface_type.GetProperty(nameof(IUnionSeraVision<object>.Name))!.GetMethod!);
        TypeBuilder.DefineMethodOverride(accept_union_method,
            interface_type.GetMethod(nameof(IUnionSeraVision<object>.AcceptUnion))!);

        #endregion
    }
}

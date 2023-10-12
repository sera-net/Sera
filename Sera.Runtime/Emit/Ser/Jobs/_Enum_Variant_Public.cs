using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Sera.Core;
using Sera.Core.Ser;
using Sera.Runtime.Emit.Deps;
using Sera.Runtime.Emit.Transform;
using Sera.Runtime.Utils;

namespace Sera.Runtime.Emit.Ser.Jobs;

internal class _Enum_Variant_Public
    (Type UnderlyingType, EnumInfo[] Items, EnumJumpTables? JumpTable, SeraEnumAttribute? EnumAttr)
    : _Enum_Variant(UnderlyingType, Items, EnumAttr)
{
    public const int MaxIfNums = 16;

    public TypeBuilder TypeBuilder { get; private set; } = null!;
    public Type RuntimeType { get; set; } = null!;
    public MethodInfo WriteVariantUnit { get; private set; } = null!;
    public MethodInfo CreateVariantTag { get; private set; } = null!;

    public static ConstructorInfo HintNullableCtor { get; } = typeof(SerializerVariantHint?)
        .GetConstructor(
            BindingFlags.Public | BindingFlags.Instance,
            new[] { typeof(SerializerVariantHint) }
        )!;
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

    private Type? MetasType;
    private Type? MetasDictType;
    private FieldBuilder? MetasField;
    
    public override void Init(EmitStub stub, EmitMeta target)
    {
        TypeBuilder = CreateTypeBuilderStruct($"Ser_{target.Type.Name}");
        TypeBuilder.MarkReadonly();

        WriteVariantUnit = ReflectionUtils.ISerializer_WriteVariantUnit_1generic
            .MakeGenericMethod(target.Type);

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

    public override Type GetEmitType(EmitStub stub, EmitMeta target, DepItem[] deps) => TypeBuilder;

    public override Type GetRuntimeType(EmitStub stub, EmitMeta target, DepItem[] deps) => RuntimeType;

    public override void Emit(EmitStub stub, EmitMeta target, EmitDeps deps)
    {
        EmitWrite(target);
        RuntimeType = TypeBuilder.CreateType();
        if (MetasField != null)
        {
            var metas_field2 = RuntimeType.GetField(MetasField.Name, BindingFlags.Public | BindingFlags.Static)!;
            var metas_inst = Activator.CreateInstance(MetasDictType!);
            var add = MetasDictType!.GetMethod(
                nameof(Dictionary<int, string>.Add),
                BindingFlags.Public | BindingFlags.Instance,
                new[] { UnderlyingType, VariantMetaType }
            )!;
            foreach (var item in Items)
            {
                var item_hint = item.EnumAttr?.SerHint ?? RootHint;
                (string name, SerializerVariantHint? hint) meta = (item.Name, item_hint);
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

    private void EmitWrite(EmitMeta target)
    {
        var write_method = TypeBuilder.DefineMethod(nameof(ISerialize<object>.Write),
            MethodAttributes.Public | MethodAttributes.Virtual);

        var generic_parameters = write_method.DefineGenericParameters("S");
        var TS = generic_parameters[0];
        TS.SetInterfaceConstraints(typeof(ISerializer));
        write_method.SetParameters(TS, target.Type, typeof(ISeraOptions));
        write_method.DefineParameter(1, ParameterAttributes.None, "serializer");
        write_method.DefineParameter(2, ParameterAttributes.None, "value");
        write_method.DefineParameter(3, ParameterAttributes.None, "options");
        write_method.SetImplementationFlags(MethodImplAttributes.AggressiveInlining);

        var ilg = write_method.GetILGenerator();

        var default_label = ilg.DefineLabel();

        #region def local_int_key_null

        var local_hint_null = ilg.DeclareLocal(typeof(SerializerVariantHint?));
        ilg.Emit(OpCodes.Ldloca_S, local_hint_null);
        ilg.Emit(OpCodes.Initobj, typeof(SerializerVariantHint?));

        #endregion

        if (Items.Length > 0)
        {
            void GenCases(Label[] labels)
            {
                #region cases

                for (var i = 0; i < Items.Length; i++)
                {
                    var item = Items[i];
                    var label = labels[i];

                    var item_hint = item.EnumAttr?.SerHint ?? RootHint;

                    ilg.MarkLabel(label);

                    #region load serializer

                    ilg.Emit(OpCodes.Ldarga, 1);

                    #endregion

                    #region load target.Name

                    ilg.Emit(OpCodes.Ldstr, target.Type.Name);

                    #endregion

                    #region new Variant

                    ilg.Emit(OpCodes.Ldstr, item.Name);
                    ilg.Emit(OpCodes.Ldarg_2);
                    ilg.Emit(OpCodes.Call, CreateVariantTag);
                    ilg.Emit(OpCodes.Newobj, NewVariantByNameTag);

                    #endregion

                    #region load hint

                    if (item_hint == null)
                    {
                        ilg.Emit(OpCodes.Ldloc, local_hint_null);
                    }
                    else
                    {
                        ilg.Emit(OpCodes.Ldc_I4, (int)item_hint.Value);
                        ilg.Emit(OpCodes.Newobj, HintNullableCtor);
                    }

                    #endregion

                    #region serializer.WriteVariantUnit<T>(target.Name, variant, null);

                    ilg.Emit(OpCodes.Constrained, TS);
                    ilg.Emit(OpCodes.Callvirt, WriteVariantUnit);

                    #endregion

                    #region return;

                    ilg.Emit(OpCodes.Ret);

                    #endregion
                }

                #endregion
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

                ilg.Emit(OpCodes.Br, default_label);

                #endregion

                GenCases(labels);
            }
            else if (Items.Length <= MaxIfNums)
            {
                var labels = Items
                    .Select(_ => ilg.DefineLabel())
                    .ToArray();

                #region if ... else if ...

                if (UnderlyingType == typeof(long) || UnderlyingType == typeof(ulong))
                {
                    for (var i = 0; i < Items.Length; i++)
                    {
                        var item = Items[i];
                        var label = labels[i];

                        #region if value == Enum.X then goto label

                        ilg.Emit(OpCodes.Ldarg_2);
                        ilg.Emit(OpCodes.Ldc_I8, item.Tag.ToLong());
                        ilg.Emit(OpCodes.Beq, label);

                        #endregion
                    }
                }
                else if (UnderlyingType == typeof(int) || UnderlyingType == typeof(uint))
                {
                    for (var i = 0; i < Items.Length; i++)
                    {
                        var item = Items[i];
                        var label = labels[i];

                        #region if value == Enum.X then goto label

                        ilg.Emit(OpCodes.Ldarg_2);
                        ilg.Emit(OpCodes.Ldc_I4, item.Tag.ToInt());
                        ilg.Emit(OpCodes.Beq, label);

                        #endregion
                    }
                }
                else
                {
                    var tmp = ilg.DeclareLocal(typeof(int));

                    #region tmp = (int)value;

                    ilg.Emit(OpCodes.Ldarg_2);
                    ilg.Emit(OpCodes.Conv_I4);
                    ilg.Emit(OpCodes.Stloc, tmp);

                    #endregion

                    for (var i = 0; i < Items.Length; i++)
                    {
                        var item = Items[i];
                        var label = labels[i];

                        #region if tmp == Enum.X then goto label

                        ilg.Emit(OpCodes.Ldloc, tmp);
                        ilg.Emit(OpCodes.Ldc_I4, item.Tag.ToInt());
                        ilg.Emit(OpCodes.Beq, label);

                        #endregion
                    }
                }

                #endregion

                #region goto default

                ilg.Emit(OpCodes.Br, default_label);

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
                    FieldAttributes.Public | FieldAttributes.Static
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

                ilg.Emit(OpCodes.Brfalse, default_label);

                #endregion

                #region write by meta

                #region load serializer

                ilg.Emit(OpCodes.Ldarga, 1);

                #endregion

                #region load target.Name

                ilg.Emit(OpCodes.Ldstr, target.Type.Name);

                #endregion

                #region new Variant

                #region load meta.name

                ilg.Emit(OpCodes.Ldloc, meta_loc);
                ilg.Emit(OpCodes.Ldfld, VariantMetaFieldName);

                #endregion

                #region load value

                ilg.Emit(OpCodes.Ldarg_2);

                #endregion

                ilg.Emit(OpCodes.Call, CreateVariantTag);
                ilg.Emit(OpCodes.Newobj, NewVariantByNameTag);

                #endregion

                #region load meta.hint

                ilg.Emit(OpCodes.Ldloc, meta_loc);
                ilg.Emit(OpCodes.Ldfld, VariantMetaFieldHint);

                #endregion

                #region serializer.WriteVariantUnit<T>(target.Name, variant, null);

                ilg.Emit(OpCodes.Constrained, TS);
                ilg.Emit(OpCodes.Callvirt, WriteVariantUnit);

                #endregion

                #region return;

                ilg.Emit(OpCodes.Ret);

                #endregion

                #endregion
            }
        }

        #region default

        ilg.MarkLabel(default_label);

        #region load serializer

        ilg.Emit(OpCodes.Ldarga, 1);

        #endregion

        #region load target.Name

        ilg.Emit(OpCodes.Ldstr, target.Type.Name);

        #endregion

        #region new Variant

        ilg.Emit(OpCodes.Ldarg_2);
        ilg.Emit(OpCodes.Call, CreateVariantTag);
        ilg.Emit(OpCodes.Newobj, NewVariantByTag);

        #endregion

        #region load hint

        if (RootHint == null)
        {
            ilg.Emit(OpCodes.Ldloc, local_hint_null);
        }
        else
        {
            ilg.Emit(OpCodes.Ldc_I4, (int)RootHint.Value);
            ilg.Emit(OpCodes.Newobj, HintNullableCtor);
        }

        #endregion

        #region serializer.WriteVariantUnit<T>(target.Name, variant, null);

        ilg.Emit(OpCodes.Constrained, TS);
        ilg.Emit(OpCodes.Callvirt, WriteVariantUnit);

        #endregion

        #region return;

        ilg.Emit(OpCodes.Ret);

        #endregion

        #endregion

        var interface_type = typeof(ISerialize<>).MakeGenericType(target.Type);
        TypeBuilder.AddInterfaceImplementation(interface_type);
        TypeBuilder.DefineMethodOverride(write_method,
            interface_type.GetMethod(nameof(ISerialize<object>.Write))!);
    }
}

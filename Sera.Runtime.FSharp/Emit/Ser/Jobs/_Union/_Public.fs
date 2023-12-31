﻿namespace Sera.Runtime.FSharp.Emit.Ser.Jobs._Union

open System
open System.Reflection
open System.Reflection.Emit
open Sera
open Sera.Core
open Sera.Runtime.Emit.Deps
open Sera.Runtime.FSharp.Exceptions
open Sera.Runtime.Utils.Internal
open Sera.Runtime.FSharp.Utils
open _Union_Mod

type private SList<'T> = System.Collections.Generic.List<'T>

type internal _Public(union_name: string, union_info: UnionInfo, union_style: UnionStyle) =
    inherit _Union(union_info)

    let mutable TypeBuilder: TypeBuilder = null
    let mutable RuntimeType: Type = null

    let mutable union_style_field: FieldBuilder = null

    let mutable VariantStyles: SList<struct (FieldBuilder * VariantStyle)> = SList()

    override this.Init(stub, target) =
        TypeBuilder <- this.CreateTypeBuilderStruct($"Ser_{target.Type.Name}")
        TypeBuilder.MarkReadonly()

    override this.GetEmitPlaceholderType(stub, target) = TypeBuilder
    override this.GetEmitType(stub, target, deps) = TypeBuilder
    override this.GetRuntimePlaceholderType(stub, target) = RuntimeType
    override this.GetRuntimeType(stub, target, deps) = RuntimeType

    override this.Emit(stub, target, deps) =
        VariantStyles.Clear()

        this.EmitAccept(target)
        this.EmitUnion(target, deps)

        RuntimeType <- TypeBuilder.CreateType()

        if union_style_field <> null then
            let rt_union_style_field =
                RuntimeType.GetField(UnionStyleFieldName, BindingFlags.NonPublic ||| BindingFlags.Static)

            rt_union_style_field.SetValue(null, union_style)

        for struct (field, style) in VariantStyles do
            let rt_field =
                RuntimeType.GetField(field.Name, BindingFlags.NonPublic ||| BindingFlags.Static)

            rt_field.SetValue(null, style)

    override this.CreateInst(stub, target, deps) = Activator.CreateInstance(RuntimeType)

    member private this.EmitAccept(target) =
        let accept_method =
            TypeBuilder.DefineMethod(
                ReflectionUtils.Name__ISeraVision_Accept,
                MethodAttributes.Public
                ||| MethodAttributes.Virtual
                ||| MethodAttributes.Final
                ||| MethodAttributes.NewSlot
            )

        let generic_parameters = accept_method.DefineGenericParameters("R", "V")
        let TR = generic_parameters[0]
        let TV = generic_parameters[1]
        let visitor = ReflectionUtils.TypeDel__ASeraVisitor.MakeGenericType(TR)
        TV.SetBaseTypeConstraint(visitor)
        accept_method.SetReturnType(TR)
        accept_method.SetParameters(TV, target.Type)
        let _ = accept_method.DefineParameter(1, ParameterAttributes.None, "visitor")
        let _ = accept_method.DefineParameter(2, ParameterAttributes.None, "value")
        accept_method.SetImplementationFlags(MethodImplAttributes.AggressiveInlining)

        let v_union_method =
            Emit.TypeBuilder
                .GetMethod(visitor, ReflectionUtils.ASeraVisitor_VUnion)
                .MakeGenericMethod(TypeBuilder, target.Type)

        let ilg = accept_method.GetILGenerator()

        //#region return visitor.VUnion<Self, T>(this, value)

        ilg.Emit(OpCodes.Ldarg_1)
        ilg.Emit(OpCodes.Ldarg_0)
        ilg.Emit(OpCodes.Ldobj, TypeBuilder)
        ilg.Emit(OpCodes.Ldarg_2)
        ilg.Emit(OpCodes.Callvirt, v_union_method)
        ilg.Emit(OpCodes.Ret)

        //#endregion

        let interface_type =
            ReflectionUtils.TypeDel__ISeraVision.MakeGenericType(target.Type)

        TypeBuilder.AddInterfaceImplementation(interface_type)

        TypeBuilder.DefineMethodOverride(
            accept_method,
            interface_type.GetMethod(ReflectionUtils.Name__ISeraVision_Accept)
        )

        ()

    member private this.EmitUnion(target, deps) =
        let get_name_method = this.EmitUnion_Name()
        let accept_union_method = this.EmitUnion_AcceptUnion(target, deps)

        let interface_type =
            ReflectionUtils.TypeDel__IUnionSeraVision.MakeGenericType(target.Type)

        TypeBuilder.AddInterfaceImplementation(interface_type)

        TypeBuilder.DefineMethodOverride(
            get_name_method,
            interface_type
                .GetProperty(ReflectionUtils.Name__IUnionSeraVision_Name)
                .GetMethod
        )

        TypeBuilder.DefineMethodOverride(
            accept_union_method,
            interface_type.GetMethod(ReflectionUtils.Name__IUnionSeraVision_AcceptUnion)
        )

        ()

    member private this.EmitUnion_Name() =
        let name_property =
            TypeBuilder.DefineProperty(
                ReflectionUtils.Name__IUnionSeraVision_Name,
                PropertyAttributes.None,
                typeof<string>,
                Array.Empty<Type>()
            )

        let get_name_method =
            TypeBuilder.DefineMethod(
                $"get_{ReflectionUtils.Name__IUnionSeraVision_Name}",
                MethodAttributes.Public
                ||| MethodAttributes.Final
                ||| MethodAttributes.Virtual
                ||| MethodAttributes.NewSlot,
                typeof<string>,
                Array.Empty<Type>()
            )

        get_name_method.SetImplementationFlags(MethodImplAttributes.AggressiveInlining)
        name_property.SetGetMethod(get_name_method)

        let ilg = get_name_method.GetILGenerator()
        ilg.Emit(OpCodes.Ldstr, union_name)
        ilg.Emit(OpCodes.Ret)

        get_name_method

    member private this.EmitUnion_AcceptUnion(target, deps) =
        if union_style <> null then
            union_style_field <-
                TypeBuilder.DefineField(
                    UnionStyleFieldName,
                    typeof<UnionStyle>,
                    FieldAttributes.Private ||| FieldAttributes.Static
                )

        let accept_union_method =
            TypeBuilder.DefineMethod(
                ReflectionUtils.Name__IUnionSeraVision_AcceptUnion,
                MethodAttributes.Public
                ||| MethodAttributes.Virtual
                ||| MethodAttributes.Final
                ||| MethodAttributes.NewSlot
            )

        let generic_parameters = accept_union_method.DefineGenericParameters("R", "V")
        let TR = generic_parameters[0]
        let TV = generic_parameters[1]
        let visitor = ReflectionUtils.TypeDel__AUnionSeraVisitor.MakeGenericType(TR)
        TV.SetBaseTypeConstraint(visitor)
        accept_union_method.SetReturnType(TR)
        accept_union_method.SetParameters(TV, target.Type.MakeByRefType())
        let _ = accept_union_method.DefineParameter(1, ParameterAttributes.None, "visitor")
        let _ = accept_union_method.DefineParameter(2, ParameterAttributes.None, "value")
        accept_union_method.SetImplementationFlags(MethodImplAttributes.AggressiveInlining)

        //#region ready

        let get_tag =
            target.Type.GetProperty("Tag", BindingFlags.Public ||| BindingFlags.Instance)

        if get_tag = null then
            raise (IllegalUnionException(target.Type, $"Target union {target.Type} does not contain Tag property"))

        let get_tag = get_tag.GetMethod

        if get_tag = null then
            raise (IllegalUnionException(target.Type, $"Target union {target.Type} does not contain Tag property"))

        let v_variant_method =
            Emit.TypeBuilder.GetMethod(visitor, ReflectionUtils.AUnionSeraVisitor_VVariant)

        let v_none_method =
            Emit.TypeBuilder.GetMethod(visitor, ReflectionUtils.AUnionSeraVisitor_VNone)

        let v_empty_method =
            Emit.TypeBuilder.GetMethod(visitor, ReflectionUtils.AUnionSeraVisitor_VEmpty)

        //#endregion

        let ilg = accept_union_method.GetILGenerator()

        let label_default = ilg.DefineLabel()

        //#region variants

        if union_info.cases.Length = 0 then
            // #region return visitor.VEmpty();

            ilg.Emit(OpCodes.Ldarg_1)
            ilg.Emit(OpCodes.Callvirt, v_empty_method)
            ilg.Emit(OpCodes.Ret)

            //#endregion
            ()
        else
            let table = union_info.table
            let labels = union_info.cases |> Seq.map (fun _ -> ilg.DefineLabel()) |> Seq.toArray

            //load tag
            if target.IsValueType then
                ilg.Emit(OpCodes.Ldarg_2)
                ilg.Emit(OpCodes.Call, get_tag)
            else
                ilg.Emit(OpCodes.Ldarg_2)
                ilg.Emit(OpCodes.Ldind_Ref)
                ilg.Emit(OpCodes.Call, get_tag)

            //#region apply offset

            if table.offset > 0 then
                raise (
                    IllegalUnionException(
                        target.Type,
                        $"The tag of the target union {target.Type} does not start from 0"
                    )
                )
                // ilg.Emit(OpCodes.Ldc_I4, table.offset)
                // ilg.Emit(OpCodes.Add)
                ()

            //endregion

            //#region switch

            let switch_table = table.table |> Seq.map (fun a -> labels[a.index]) |> Seq.toArray
            ilg.Emit(OpCodes.Switch, switch_table)

            // goto default
            ilg.Emit(OpCodes.Br, label_default)

            //endregion

            this.EmitUnion_Cases(target, deps, ilg, labels, visitor, v_variant_method)

            //#region default

            //#region return visitor.VNone();

            ilg.MarkLabel(label_default)
            ilg.Emit(OpCodes.Ldarg_1)
            ilg.Emit(OpCodes.Callvirt, v_none_method)
            ilg.Emit(OpCodes.Ret)

            //#endregion

            //#endregion
            ()

        //#endregion

        accept_union_method

    member private this.EmitUnion_Cases(target, deps, ilg, labels, visitor, v_variant_method) =
        for i = 0 to union_info.cases.Length - 1 do
            let case = union_info.cases[i]
            let label = labels[i]
            let style = case.style
            let format = case.format

            // new Variant(Name, VariantTag.Create(Tag))
            let inline create_variant (ilg: ILGenerator) =
                ilg.Emit(OpCodes.Ldstr, case.name)
                ilg.Emit(OpCodes.Ldc_I4, case.tag)
                ilg.Emit(OpCodes.Call, CreateVariantTag)
                ilg.Emit(OpCodes.Newobj, NewVariantByNameTag)

            let inline load__union_style (ilg: ILGenerator) =
                if union_style_field = null then
                    ilg.Emit(OpCodes.Ldnull)
                else
                    ilg.Emit(OpCodes.Ldsfld, union_style_field)

            let inline load__variant_style (ilg: ILGenerator) =
                if style = null then
                    ilg.Emit(OpCodes.Ldnull)
                else
                    let variant_style_field =
                        TypeBuilder.DefineField(
                            $"_variant_style_{i}",
                            typeof<VariantStyle>,
                            FieldAttributes.Private ||| FieldAttributes.Static
                        )

                    VariantStyles.Add(struct (variant_style_field, style))
                    ilg.Emit(OpCodes.Ldsfld, variant_style_field)

            let inline load_dep (dep: DepPlace) (ilg: ILGenerator) =
                ilg.Emit(OpCodes.Call, dep.GetDepMethodInfo)

                if dep.Boxed then
                    let get_method = dep.MakeBoxGetMethodInfo()
                    ilg.Emit(OpCodes.Call, get_method)

            let inline load_case (ilg: ILGenerator) =
                let field = case.fields[0]

                if target.Type.IsValueType then
                    ilg.Emit(OpCodes.Ldarg_2)
                else
                    let case_type = field.DeclaringType
                    ilg.Emit(OpCodes.Ldarg_2)
                    ilg.Emit(OpCodes.Ldind_Ref)
                    ilg.Emit(OpCodes.Castclass, case_type)

            //#region common

            ilg.MarkLabel(label)
            ilg.Emit(OpCodes.Ldarg_1)

            //#endregion

            if case.fields.Length = 0 then
                //#region return visitor.VVariant(new Variant(Name, VariantTag.Create(value)), union_style, variant_style);

                create_variant ilg
                load__union_style ilg
                load__variant_style ilg

                ilg.Emit(OpCodes.Callvirt, v_variant_method)
                ilg.Emit(OpCodes.Ret)

                //#endregion
                ()
            elif format = VariantFormat.Value && case.fields.Length = 1 then // Only supports single field as value
                //#region return visitor.VVariantValue(dep, v, new Variant(Name, VariantTag.Create(value)), union_style, variant_style);

                let dep = deps.Get(this.tag_deps[case.tag][0])
                let field = case.fields[0]

                let v_variant_value_method =
                    Emit.TypeBuilder
                        .GetMethod(visitor, ReflectionUtils.AUnionSeraVisitor_VVariantValue)
                        .MakeGenericMethod(dep.TransformedType, field.PropertyType)

                load_dep dep ilg

                load_case ilg

                ilg.Emit(OpCodes.Call, field.GetMethod)

                create_variant ilg
                load__union_style ilg
                load__variant_style ilg

                ilg.Emit(OpCodes.Callvirt, v_variant_value_method)
                ilg.Emit(OpCodes.Ret)

                //#endregion
                ()
            elif format = VariantFormat.Tuple then
                //#region return visitor.VVariantTuple(impl, value, new Variant(Name, VariantTag.Create(value)), union_style, variant_style);

                //#region tuple impl

                let inline emit_tuple_impl () =

                    let tuple_impl =
                        TypeBuilder.DefineNestedType(
                            $"_{case.name}_TupleImpl",
                            TypeAttributes.NestedPublic,
                            typeof<ValueType>
                        )

                    tuple_impl.MarkReadonly()

                    let get_size_method =
                        this.EmitUnion_TupleOrStruct_Size(
                            tuple_impl,
                            ReflectionUtils.Name__ITupleSeraVision_Size,
                            case.fields.Length
                        )

                    //#region AcceptItem

                    let inline emit_accept_item_method () =
                        let accept_item_method =
                            tuple_impl.DefineMethod(
                                ReflectionUtils.Name__ITupleSeraVision_AcceptItem,
                                MethodAttributes.Public
                                ||| MethodAttributes.Final
                                ||| MethodAttributes.Virtual
                                ||| MethodAttributes.NewSlot
                            )

                        let generic_parameters = accept_item_method.DefineGenericParameters("R", "V")
                        let TR = generic_parameters[0]
                        let TV = generic_parameters[1]
                        let visitor = ReflectionUtils.TypeDel__ATupleSeraVisitor.MakeGenericType(TR)
                        TV.SetBaseTypeConstraint(visitor)
                        accept_item_method.SetReturnType(TR)
                        accept_item_method.SetParameters(TV, target.Type.MakeByRefType(), typeof<int>)
                        let _ = accept_item_method.DefineParameter(1, ParameterAttributes.None, "visitor")
                        let _ = accept_item_method.DefineParameter(2, ParameterAttributes.None, "value")
                        let _ = accept_item_method.DefineParameter(3, ParameterAttributes.None, "index")
                        accept_item_method.SetImplementationFlags(MethodImplAttributes.AggressiveInlining)

                        let v_none_method =
                            Emit.TypeBuilder.GetMethod(visitor, ReflectionUtils.ATupleSeraVisitor_VNone)

                        let v_item_method_decl =
                            Emit.TypeBuilder.GetMethod(visitor, ReflectionUtils.ATupleSeraVisitor_VItem)

                        let ilg = accept_item_method.GetILGenerator()

                        let label_default = ilg.DefineLabel()

                        //#region switch index

                        let labels = case.fields |> Seq.map (fun _ -> ilg.DefineLabel()) |> Seq.toArray
                        ilg.Emit(OpCodes.Ldarg_3)
                        ilg.Emit(OpCodes.Switch, labels)
                        ilg.Emit(OpCodes.Br, label_default)

                        //#endregion

                        //#region items

                        for i = 0 to case.fields.Length - 1 do
                            //#region return visitor.VItem(dep, value.field);

                            let field = case.fields[i]
                            let label = labels[i]
                            let dep = deps.Get(this.tag_deps[case.tag][i])

                            let v_item_method =
                                v_item_method_decl.MakeGenericMethod(dep.TransformedType, field.PropertyType)

                            ilg.MarkLabel(label)
                            ilg.Emit(OpCodes.Ldarg_1)

                            load_dep dep ilg

                            load_case ilg

                            ilg.Emit(OpCodes.Call, field.GetMethod)

                            ilg.Emit(OpCodes.Callvirt, v_item_method)
                            ilg.Emit(OpCodes.Ret)

                            //#endregion
                            ()

                        //#endregion

                        //#region default

                        ilg.MarkLabel(label_default)
                        ilg.Emit(OpCodes.Ldarg_1)
                        ilg.Emit(OpCodes.Callvirt, v_none_method)
                        ilg.Emit(OpCodes.Ret)

                        //#endregion

                        accept_item_method

                    let accept_item_method = emit_accept_item_method ()

                    //#endregion

                    let interface_type =
                        ReflectionUtils.TypeDel__ITupleSeraVision.MakeGenericType(target.Type)

                    tuple_impl.AddInterfaceImplementation(interface_type)

                    tuple_impl.DefineMethodOverride(
                        get_size_method,
                        interface_type
                            .GetProperty(ReflectionUtils.Name__ITupleSeraVision_Size)
                            .GetMethod
                    )

                    tuple_impl.DefineMethodOverride(
                        accept_item_method,
                        interface_type.GetMethod(ReflectionUtils.Name__ITupleSeraVision_AcceptItem)
                    )

                    tuple_impl.CreateType() |> ignore
                    tuple_impl

                let tuple_impl = emit_tuple_impl ()

                //#endregion

                let v_variant_tuple_method =
                    Emit.TypeBuilder
                        .GetMethod(visitor, ReflectionUtils.AUnionSeraVisitor_VVariantTuple)
                        .MakeGenericMethod(tuple_impl, target.Type)

                let local_tuple_impl = ilg.DeclareLocal(tuple_impl)
                ilg.Emit(OpCodes.Ldloc, local_tuple_impl)
                ilg.Emit(OpCodes.Ldarg_2)
                ilg.Emit(OpCodes.Ldobj, target.Type)

                create_variant ilg
                load__union_style ilg
                load__variant_style ilg

                ilg.Emit(OpCodes.Callvirt, v_variant_tuple_method)
                ilg.Emit(OpCodes.Ret)

                //#endregion
                ()
            else // VariantFormat.Struct
                //#region return visitor.VVariantStruct(impl, value, new Variant(Name, VariantTag.Create(value)), union_style, variant_style);

                //#region struct impl

                let inline emit_struct_impl () =

                    let struct_impl =
                        TypeBuilder.DefineNestedType(
                            $"_{case.name}_StructImpl",
                            TypeAttributes.NestedPublic,
                            typeof<ValueType>
                        )

                    struct_impl.MarkReadonly()

                    let get_count_method =
                        this.EmitUnion_TupleOrStruct_Size(
                            struct_impl,
                            ReflectionUtils.Name__IStructSeraVision_Count,
                            case.fields.Length
                        )

                    let get_name_method =
                        this.EmitUnion_Struct_Name(
                            struct_impl,
                            ReflectionUtils.Name__IStructSeraVision_Name,
                            $"{union_name}.{case.name}"
                        )

                    //#region accept field method

                    let inline emit_accept_field_method () =
                        let accept_field_method =
                            struct_impl.DefineMethod(
                                ReflectionUtils.Name__IStructSeraVision_AcceptField,
                                MethodAttributes.Public
                                ||| MethodAttributes.Final
                                ||| MethodAttributes.Virtual
                                ||| MethodAttributes.NewSlot
                            )

                        let generic_parameters = accept_field_method.DefineGenericParameters("R", "V")
                        let TR = generic_parameters[0]
                        let TV = generic_parameters[1]
                        let visitor = ReflectionUtils.TypeDel__AStructSeraVisitor.MakeGenericType(TR)
                        TV.SetBaseTypeConstraint(visitor)
                        accept_field_method.SetReturnType(TR)
                        accept_field_method.SetParameters(TV, target.Type.MakeByRefType(), typeof<int>)
                        let _ = accept_field_method.DefineParameter(1, ParameterAttributes.None, "visitor")
                        let _ = accept_field_method.DefineParameter(2, ParameterAttributes.None, "value")
                        let _ = accept_field_method.DefineParameter(3, ParameterAttributes.None, "field")
                        accept_field_method.SetImplementationFlags(MethodImplAttributes.AggressiveInlining)

                        let v_none_method =
                            Emit.TypeBuilder.GetMethod(visitor, ReflectionUtils.AStructSeraVisitor_VNone)

                        let v_field_method_decl =
                            Emit.TypeBuilder.GetMethod(visitor, ReflectionUtils.AStructSeraVisitor_VField)

                        let ilg = accept_field_method.GetILGenerator()

                        let label_default = ilg.DefineLabel()

                        //#region switch index

                        let labels = case.fields |> Seq.map (fun _ -> ilg.DefineLabel()) |> Seq.toArray
                        ilg.Emit(OpCodes.Ldarg_3)
                        ilg.Emit(OpCodes.Switch, labels)
                        ilg.Emit(OpCodes.Br, label_default)

                        //#endregion

                        //#region items

                        for i = 0 to case.fields.Length - 1 do
                            //#region return visitor.VField(dep, name, key, value.field);

                            let field = case.fields[i]
                            let label = labels[i]
                            let dep = deps.Get(this.tag_deps[case.tag][i])

                            let v_field_method =
                                v_field_method_decl.MakeGenericMethod(dep.TransformedType, field.PropertyType)

                            ilg.MarkLabel(label)
                            ilg.Emit(OpCodes.Ldarg_1)

                            load_dep dep ilg

                            load_case ilg

                            ilg.Emit(OpCodes.Call, field.GetMethod)

                            ilg.Emit(OpCodes.Ldstr, field.Name)
                            ilg.Emit(OpCodes.Ldc_I4, i)

                            ilg.Emit(OpCodes.Callvirt, v_field_method)
                            ilg.Emit(OpCodes.Ret)

                            //#endregion
                            ()

                        //#endregion

                        //#region default

                        ilg.MarkLabel(label_default)
                        ilg.Emit(OpCodes.Ldarg_1)
                        ilg.Emit(OpCodes.Callvirt, v_none_method)
                        ilg.Emit(OpCodes.Ret)

                        //#endregion

                        accept_field_method

                    let accept_field_method = emit_accept_field_method ()

                    //#endregion

                    let interface_type =
                        ReflectionUtils.TypeDel__IStructSeraVision.MakeGenericType(target.Type)

                    struct_impl.AddInterfaceImplementation(interface_type)

                    struct_impl.DefineMethodOverride(
                        get_count_method,
                        interface_type
                            .GetProperty(ReflectionUtils.Name__IStructSeraVision_Count)
                            .GetMethod
                    )

                    struct_impl.DefineMethodOverride(
                        get_name_method,
                        interface_type
                            .GetProperty(ReflectionUtils.Name__IStructSeraVision_Name)
                            .GetMethod
                    )

                    struct_impl.DefineMethodOverride(
                        accept_field_method,
                        interface_type.GetMethod(ReflectionUtils.Name__IStructSeraVision_AcceptField)
                    )

                    struct_impl.CreateType() |> ignore
                    struct_impl

                let struct_impl = emit_struct_impl ()

                //#endregion

                let v_variant_struct_method =
                    Emit.TypeBuilder
                        .GetMethod(visitor, ReflectionUtils.AUnionSeraVisitor_VVariantStruct)
                        .MakeGenericMethod(struct_impl, target.Type)

                let local_struct_impl = ilg.DeclareLocal(struct_impl)
                ilg.Emit(OpCodes.Ldloc, local_struct_impl)
                ilg.Emit(OpCodes.Ldarg_2)
                ilg.Emit(OpCodes.Ldobj, target.Type)

                create_variant ilg
                load__union_style ilg
                load__variant_style ilg

                ilg.Emit(OpCodes.Callvirt, v_variant_struct_method)
                ilg.Emit(OpCodes.Ret)

                //#endregion
                ()

        () // for

    member private this.EmitUnion_TupleOrStruct_Size(tb, name, size: int) =
        let size_property =
            tb.DefineProperty(name, PropertyAttributes.None, typeof<int>, Array.Empty<Type>())

        let get_size_method =
            tb.DefineMethod(
                $"get_{name}",
                MethodAttributes.Public
                ||| MethodAttributes.Final
                ||| MethodAttributes.Virtual
                ||| MethodAttributes.NewSlot,
                typeof<int>,
                Array.Empty<Type>()
            )

        get_size_method.SetImplementationFlags(MethodImplAttributes.AggressiveInlining)
        size_property.SetGetMethod(get_size_method)

        let ilg = get_size_method.GetILGenerator()
        ilg.Emit(OpCodes.Ldc_I4, size)
        ilg.Emit(OpCodes.Ret)

        get_size_method

    member private this.EmitUnion_Struct_Name(tb, name, v_name: string) =
        let name_property =
            tb.DefineProperty(name, PropertyAttributes.None, typeof<string>, Array.Empty<Type>())

        let get_name_method =
            tb.DefineMethod(
                $"get_{name}",
                MethodAttributes.Public
                ||| MethodAttributes.Final
                ||| MethodAttributes.Virtual
                ||| MethodAttributes.NewSlot,
                typeof<string>,
                Array.Empty<Type>()
            )

        get_name_method.SetImplementationFlags(MethodImplAttributes.AggressiveInlining)
        name_property.SetGetMethod(get_name_method)

        let ilg = get_name_method.GetILGenerator()
        ilg.Emit(OpCodes.Ldstr, v_name)
        ilg.Emit(OpCodes.Ret)

        get_name_method

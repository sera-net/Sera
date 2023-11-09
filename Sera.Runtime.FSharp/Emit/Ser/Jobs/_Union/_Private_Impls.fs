module rec Sera.Runtime.FSharp.Emit.Ser.Jobs._Union._Private_Impls

open System
open System.Collections.Generic
open System.Reflection
open System.Reflection.Emit
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core
open Sera
open Sera.Core
open Sera.Runtime.Emit.Deps
open Sera.Runtime.FSharp.Exceptions
open Sera.Runtime.FSharp.Utils
open Sera.Runtime.Utils.Delegates
open Sera.Runtime.Utils.Internal
open _Union_Mod

[<NoEquality; NoComparison>]
type internal ImplData = { meta_key: obj; union_name: string }

[<NoEquality; NoComparison>]
type internal ImplMeta =
    { union_info: UnionInfo
      union_style: UnionStyle
      deps: RuntimeDeps
      tag_deps: Dictionary<int, List<int>> }

[<NoEquality; NoComparison>]
type internal UnionCaseMeta =
    { case: UnionCase
      deps: RuntimeDeps
      tag_deps: Dictionary<int, List<int>> }

let internal metas = ConditionalWeakTable<obj, ImplMeta>()

let inline load_dep (dep: DepPlace) (ilg: ILGenerator) =
    ilg.Emit(OpCodes.Call, dep.GetDepMethodInfo)

    if dep.Boxed then
        let get_method = dep.MakeBoxGetMethodInfo()
        ilg.Emit(OpCodes.Call, get_method)

[<Struct; IsReadOnly; NoEquality; NoComparison>]
type PrivateUnionImpl<'T> internal (data: ImplData) =
    interface ISeraVision<'T> with
        [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
        member this.Accept(visitor, value) = visitor.VUnion(this, value)

    interface IUnionSeraVision<'T> with
        member this.Name = data.union_name

        [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
        member this.AcceptUnion(visitor, value) =
            UnionImpl.AcceptUnion(visitor, &value, data.meta_key)

[<AbstractClass; Sealed>]
type internal UnionImpl<'R, 'V, 'T when 'V :> AUnionSeraVisitor<'R>> private () =

    static let Delegates = ConditionalWeakTable<obj, FnSerAcceptUnion<'R, 'V, 'T>>()

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    static member AcceptUnion(visitor: 'V, value: byref<'T>, key: obj) : 'R =
        UnionImpl.GetDelegateCache(key).Invoke(visitor, &value)

    static member GetDelegateCache(meta_key: obj) : FnSerAcceptUnion<'R, 'V, 'T> =
        Delegates.GetValue(meta_key, ConditionalWeakTable.CreateValueCallback(UnionImpl.GetDelegate))

    static member GetDelegate(meta_key: obj) : FnSerAcceptUnion<'R, 'V, 'T> =
        let mutable meta = Unchecked.defaultof<_>

        if not <| metas.TryGetValue(meta_key, &meta) then
            raise (NullReferenceException())

        UnionImpl.Create(meta)

    static member Create(meta: ImplMeta) : FnSerAcceptUnion<'R, 'V, 'T> =
        let target = typeof<'T>
        let TR = typeof<'R>
        let TV = typeof<'V>

        let guid = Guid.NewGuid()
        let dyn_method_name = ReflectionUtils.GetAsmName($"Ser_{target.Name}_{guid:N}")

        let asm = ReflectionUtils.CreateAssembly($"_{guid:N}_")

        //#region container type

        let container_type_builder =
            asm.DefineType(
                $"{asm.Assembly.GetName().Name}.SerContainer_{target.Name}_{guid:N}",
                TypeAttributes.Public ||| TypeAttributes.Sealed
            )

        let mutable union_style_field = null
        let variant_styles = List<struct (FieldBuilder * VariantStyle)>()

        if meta.union_style <> null then
            union_style_field <-
                container_type_builder.DefineField(
                    UnionStyleFieldName,
                    typeof<UnionStyle>,
                    FieldAttributes.Public ||| FieldAttributes.Static
                )

        UnionImpl<'R, 'V, 'T>
            .EmitUnion_Cases_Define_Styles(meta, container_type_builder, variant_styles)

        let container_type = container_type_builder.CreateType()

        if union_style_field <> null then
            let rt_union_style_field =
                container_type.GetField(UnionStyleFieldName, BindingFlags.Public ||| BindingFlags.Static)

            rt_union_style_field.SetValue(null, meta.union_style)
            ()

        for struct (field, style) in variant_styles do
            let rt_field =
                container_type.GetField(field.Name, BindingFlags.Public ||| BindingFlags.Static)

            rt_field.SetValue(null, style)

        //#endregion

        let dyn_method =
            DynamicMethod(
                dyn_method_name,
                MethodAttributes.Public ||| MethodAttributes.Static,
                CallingConventions.Standard,
                TR,
                [| TV; target.MakeByRefType() |],
                target.Module,
                true
            )

        //#region ready

        let get_tag =
            target.GetProperty("Tag", BindingFlags.NonPublic ||| BindingFlags.Public ||| BindingFlags.Instance)

        if get_tag = null then
            raise (IllegalUnionException(target, $"Target union {target} does not contain Tag property"))

        let get_tag = get_tag.GetMethod

        if get_tag = null then
            raise (IllegalUnionException(target, $"Target union {target} does not contain Tag property"))

        let v_variant_method =
            TV.GetMethod(
                ReflectionUtils.Name__AUnionSeraVisitor_VVariant,
                BindingFlags.Public ||| BindingFlags.Instance
            )

        let v_none_method =
            TV.GetMethod(ReflectionUtils.Name__AUnionSeraVisitor_VNone, BindingFlags.Public ||| BindingFlags.Instance)

        let v_empty_method =
            TV.GetMethod(ReflectionUtils.Name__AUnionSeraVisitor_VEmpty, BindingFlags.Public ||| BindingFlags.Instance)

        //#endregion

        let ilg = dyn_method.GetILGenerator()

        let label_default = ilg.DefineLabel()

        // #region variants

        if meta.union_info.cases.Length = 0 then
            // #region return visitor.VEmpty();

            ilg.Emit(OpCodes.Ldarg_0)
            ilg.Emit(OpCodes.Box, TV)
            ilg.Emit(OpCodes.Callvirt, v_empty_method)
            ilg.Emit(OpCodes.Ret)

            //#endregion
            ()
        else
            let table = meta.union_info.table

            let labels =
                meta.union_info.cases |> Seq.map (fun _ -> ilg.DefineLabel()) |> Seq.toArray

            //load tag
            if target.IsValueType then
                ilg.Emit(OpCodes.Ldarg_1)
                ilg.Emit(OpCodes.Call, get_tag)
            else
                ilg.Emit(OpCodes.Ldarg_1)
                ilg.Emit(OpCodes.Ldind_Ref)
                ilg.Emit(OpCodes.Call, get_tag)

            //#region apply offset

            if table.offset > 0 then
                raise (IllegalUnionException(target, $"The tag of the target union {target} does not start from 0"))
                ()

            //endregion

            //#region switch

            let switch_table = table.table |> Seq.map (fun a -> labels[a.index]) |> Seq.toArray
            ilg.Emit(OpCodes.Switch, switch_table)

            // goto default
            ilg.Emit(OpCodes.Br, label_default)

            //endregion

            UnionImpl<'R, 'V, 'T>
                .EmitUnion_Cases(meta, target, ilg, TV, labels, container_type, v_variant_method)

            //#region default

            //#region return visitor.VNone();

            ilg.MarkLabel(label_default)
            ilg.Emit(OpCodes.Ldarg_0)
            ilg.Emit(OpCodes.Box, TV)
            ilg.Emit(OpCodes.Callvirt, v_none_method)
            ilg.Emit(OpCodes.Ret)

            //#endregion

            //#endregion
            ()

        //#endregion

        let del = dyn_method.CreateDelegate<FnSerAcceptUnion<'R, 'V, 'T>>()
        del

    static member EmitUnion_Cases_Define_Styles(meta: ImplMeta, container_type_builder, variant_styles) =
        for i = 0 to meta.union_info.cases.Length - 1 do
            let case = meta.union_info.cases[i]
            let style = case.style

            if style <> null then
                let variant_style_field =
                    container_type_builder.DefineField(
                        $"{VariantStyleFieldName}_{i}",
                        typeof<VariantStyle>,
                        FieldAttributes.Public ||| FieldAttributes.Static
                    )

                variant_styles.Add(struct (variant_style_field, style))

    static member EmitUnion_Cases(meta: ImplMeta, target, ilg, TV, labels, container_type, v_variant_method) =
        let union_style_field =
            container_type.GetField(UnionStyleFieldName, BindingFlags.Public ||| BindingFlags.Static)

        for i = 0 to meta.union_info.cases.Length - 1 do
            let case = meta.union_info.cases[i]
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
                        container_type.GetField(
                            $"{VariantStyleFieldName}_{i}",
                            BindingFlags.Public ||| BindingFlags.Static
                        )

                    ilg.Emit(OpCodes.Ldsfld, variant_style_field)

            let inline load_case (ilg: ILGenerator) =
                let field = case.fields[0]

                if target.IsValueType then
                    ilg.Emit(OpCodes.Ldarg_1)
                else
                    let case_type = field.DeclaringType
                    ilg.Emit(OpCodes.Ldarg_1)
                    ilg.Emit(OpCodes.Ldind_Ref)
                    ilg.Emit(OpCodes.Castclass, case_type)

            //#region common

            ilg.MarkLabel(label)
            ilg.Emit(OpCodes.Ldarg_0)

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
            elif format = VariantFormat.Value && case.fields.Length = 1 then
                //#region return visitor.VVariantValue(dep, v, new Variant(Name, VariantTag.Create(value)), union_style, variant_style);

                let dep = meta.deps.Get(meta.tag_deps[case.tag][0])
                let field = case.fields[0]

                let v_variant_value_method =
                    TV
                        .GetMethod(
                            ReflectionUtils.Name__AUnionSeraVisitor_VVariantValue,
                            BindingFlags.Public ||| BindingFlags.Instance
                        )
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

                let impl_type = typedefof<PrivateUnionCaseTupleImpl<_>>.MakeGenericType(target)

                let impl_container =
                    EmitDepContainer.CreateDepContainer().MakeGenericType(typeof<obj>)

                let impl_ctor =
                    impl_type.GetConstructor(
                        BindingFlags.Public ||| BindingFlags.NonPublic ||| BindingFlags.Instance,
                        [| typeof<int>; typeof<UnionCaseMeta> |]
                    )

                let case_meta: UnionCaseMeta =
                    { case = case
                      deps = meta.deps
                      tag_deps = meta.tag_deps }

                let impl_inst = impl_ctor.Invoke([| case.fields.Length; case_meta |])
                EmitDepContainer.SetData(impl_container, impl_inst)

                let container_field = EmitDepContainer.GetField(impl_container)

                let v_variant_tuple_method =
                    TV
                        .GetMethod(ReflectionUtils.Name__AUnionSeraVisitor_VVariantTuple)
                        .MakeGenericMethod(impl_type, target)

                ilg.Emit(OpCodes.Ldsfld, container_field)
                ilg.Emit(OpCodes.Castclass, impl_type)
                ilg.Emit(OpCodes.Ldarg_1)
                ilg.Emit(OpCodes.Ldobj, target)

                create_variant ilg
                load__union_style ilg
                load__variant_style ilg

                ilg.Emit(OpCodes.Callvirt, v_variant_tuple_method)
                ilg.Emit(OpCodes.Ret)

                //#endregion
                ()
            else
                //#region return visitor.VVariantStruct(impl, value, new Variant(Name, VariantTag.Create(value)), union_style, variant_style);

                let impl_type = typedefof<PrivateUnionCaseStructImpl<_>>.MakeGenericType(target)

                let impl_container =
                    EmitDepContainer.CreateDepContainer().MakeGenericType(typeof<obj>)

                let impl_ctor =
                    impl_type.GetConstructor(
                        BindingFlags.Public ||| BindingFlags.NonPublic ||| BindingFlags.Instance,
                        [| typeof<int>; typeof<string>; typeof<UnionCaseMeta> |]
                    )

                let case_meta: UnionCaseMeta =
                    { case = case
                      deps = meta.deps
                      tag_deps = meta.tag_deps }

                let impl_inst = impl_ctor.Invoke([| case.fields.Length; case.name; case_meta |])
                EmitDepContainer.SetData(impl_container, impl_inst)

                let container_field = EmitDepContainer.GetField(impl_container)

                let v_variant_struct_method =
                    TV
                        .GetMethod(ReflectionUtils.Name__AUnionSeraVisitor_VVariantStruct)
                        .MakeGenericMethod(impl_type, target)

                ilg.Emit(OpCodes.Ldsfld, container_field)
                ilg.Emit(OpCodes.Castclass, impl_type)
                ilg.Emit(OpCodes.Ldarg_1)
                ilg.Emit(OpCodes.Ldobj, target)

                create_variant ilg
                load__union_style ilg
                load__variant_style ilg

                ilg.Emit(OpCodes.Callvirt, v_variant_struct_method)
                ilg.Emit(OpCodes.Ret)

                //#endregion
                ()

        () // for

[<NoEquality; NoComparison>]
type PrivateUnionCaseTupleImpl<'T> internal (size: int, meta: UnionCaseMeta) =
    interface ITupleSeraVision<'T> with
        member this.Size = size

        member this.AcceptItem<'R, 'V when 'V :> ATupleSeraVisitor<'R>>(visitor: 'V, value, index) =
            UnionCaseTupleImpl<'R, 'V, 'T>.AcceptItem(meta, visitor, &value, index)

[<AbstractClass; Sealed>]
type internal UnionCaseTupleImpl<'R, 'V, 'T when 'V :> ATupleSeraVisitor<'R>> private () =
    static let cache = ConditionalWeakTable<UnionCaseMeta, FnSerAcceptItem<'R, 'V, 'T>>()

    static member AcceptItem(meta: UnionCaseMeta, visitor: 'V, value: byref<'T>, index: int) =
        cache
            .GetValue(meta, ConditionalWeakTable.CreateValueCallback(UnionCaseTupleImpl.Create))
            .Invoke(visitor, &value, index)

    static member Create(meta: UnionCaseMeta) : FnSerAcceptItem<'R, 'V, 'T> =
        let target = typeof<'T>
        let TR = typeof<'R>
        let TV = typeof<'V>

        let guid = Guid.NewGuid()

        let dyn_method_name =
            ReflectionUtils.GetAsmName($"Ser_{target.Name}_{guid:N}_{meta.case.name}")

        let dyn_method =
            DynamicMethod(
                dyn_method_name,
                MethodAttributes.Public ||| MethodAttributes.Static,
                CallingConventions.Standard,
                TR,
                [| TV; target.MakeByRefType(); typeof<int> |],
                target.Module,
                true
            )

        //#region ready

        let v_none_method = TV.GetMethod(ReflectionUtils.Name__ATupleSeraVisitor_VNone)

        let v_item_method_decl = TV.GetMethod(ReflectionUtils.Name__ATupleSeraVisitor_VItem)

        let inline load_case (ilg: ILGenerator) =
            let field = meta.case.fields[0]

            if target.IsValueType then
                ilg.Emit(OpCodes.Ldarg_1)
            else
                let case_type = field.DeclaringType
                ilg.Emit(OpCodes.Ldarg_1)
                ilg.Emit(OpCodes.Ldind_Ref)
                ilg.Emit(OpCodes.Castclass, case_type)

        //#endregion

        let ilg = dyn_method.GetILGenerator()

        let label_default = ilg.DefineLabel()

        //#region switch index

        let labels = meta.case.fields |> Seq.map (fun _ -> ilg.DefineLabel()) |> Seq.toArray
        ilg.Emit(OpCodes.Ldarg_2)
        ilg.Emit(OpCodes.Switch, labels)
        ilg.Emit(OpCodes.Br, label_default)

        //#endregion

        //#region items

        for i = 0 to meta.case.fields.Length - 1 do
            //#region return visitor.VItem(dep, value.field);

            let field = meta.case.fields[i]
            let label = labels[i]
            let dep = meta.deps.Get(meta.tag_deps[meta.case.tag][i])

            let v_item_method =
                v_item_method_decl.MakeGenericMethod(dep.TransformedType, field.PropertyType)

            ilg.MarkLabel(label)
            ilg.Emit(OpCodes.Ldarg_0)

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
        ilg.Emit(OpCodes.Ldarg_0)
        ilg.Emit(OpCodes.Callvirt, v_none_method)
        ilg.Emit(OpCodes.Ret)

        //#endregion

        let del = dyn_method.CreateDelegate<FnSerAcceptItem<'R, 'V, 'T>>()
        del

[<NoEquality; NoComparison>]
type PrivateUnionCaseStructImpl<'T> internal (count: int, name: string, meta: UnionCaseMeta) =
    interface IStructSeraVision<'T> with
        member this.Count = count
        member this.Name = name

        member this.AcceptField<'R, 'V when 'V :> AStructSeraVisitor<'R>>(visitor: 'V, value, index) =
            UnionCaseStructImpl<'R, 'V, 'T>.AcceptItem(meta, visitor, &value, index)

[<AbstractClass; Sealed>]
type internal UnionCaseStructImpl<'R, 'V, 'T when 'V :> AStructSeraVisitor<'R>> private () =
    static let cache = ConditionalWeakTable<UnionCaseMeta, FnSerAcceptField<'R, 'V, 'T>>()

    static member AcceptItem(meta: UnionCaseMeta, visitor: 'V, value: byref<'T>, index: int) =
        cache
            .GetValue(meta, ConditionalWeakTable.CreateValueCallback(UnionCaseStructImpl.Create))
            .Invoke(visitor, &value, index)

    static member Create(meta: UnionCaseMeta) : FnSerAcceptField<'R, 'V, 'T> =
        let target = typeof<'T>
        let TR = typeof<'R>
        let TV = typeof<'V>

        let guid = Guid.NewGuid()

        let dyn_method_name =
            ReflectionUtils.GetAsmName($"Ser_{target.Name}_{guid:N}_{meta.case.name}")

        let dyn_method =
            DynamicMethod(
                dyn_method_name,
                MethodAttributes.Public ||| MethodAttributes.Static,
                CallingConventions.Standard,
                TR,
                [| TV; target.MakeByRefType(); typeof<int> |],
                target.Module,
                true
            )

        //#region ready

        let v_none_method = TV.GetMethod(ReflectionUtils.Name__AStructSeraVisitor_VNone)

        let v_field_method_decl =
            TV.GetMethod(ReflectionUtils.Name__AStructSeraVisitor_VField)

        let inline load_case (ilg: ILGenerator) =
            let field = meta.case.fields[0]

            if target.IsValueType then
                ilg.Emit(OpCodes.Ldarg_1)
            else
                let case_type = field.DeclaringType
                ilg.Emit(OpCodes.Ldarg_1)
                ilg.Emit(OpCodes.Ldind_Ref)
                ilg.Emit(OpCodes.Castclass, case_type)

        //#endregion

        let ilg = dyn_method.GetILGenerator()

        let label_default = ilg.DefineLabel()

        //#region switch index

        let labels = meta.case.fields |> Seq.map (fun _ -> ilg.DefineLabel()) |> Seq.toArray
        ilg.Emit(OpCodes.Ldarg_2)
        ilg.Emit(OpCodes.Switch, labels)
        ilg.Emit(OpCodes.Br, label_default)

        //#endregion

        //#region items

        for i = 0 to meta.case.fields.Length - 1 do
            //#region return visitor.VField(dep, name, key, value.field);

            let field = meta.case.fields[i]
            let label = labels[i]
            let dep = meta.deps.Get(meta.tag_deps[meta.case.tag][i])

            let v_field_method =
                v_field_method_decl.MakeGenericMethod(dep.TransformedType, field.PropertyType)

            ilg.MarkLabel(label)
            ilg.Emit(OpCodes.Ldarg_0)

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
        ilg.Emit(OpCodes.Ldarg_0)
        ilg.Emit(OpCodes.Callvirt, v_none_method)
        ilg.Emit(OpCodes.Ret)

        //#endregion

        let del = dyn_method.CreateDelegate<FnSerAcceptField<'R, 'V, 'T>>()
        del

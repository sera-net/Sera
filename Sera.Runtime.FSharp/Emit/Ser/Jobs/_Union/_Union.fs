namespace Sera.Runtime.FSharp.Emit.Ser.Jobs._Union

open System.Collections.Generic
open System.Reflection
open Sera
open Sera.Core
open Sera.Runtime
open Sera.Runtime.Emit
open Sera.Runtime.Emit.Deps
open Sera.Runtime.Emit.Ser
open Sera.Runtime.FSharp.Emit.Ser.Jobs
open Sera.Runtime.FSharp.Utils
open Sera.Runtime.Utils

module _Union_Mod =
    [<Literal>]
    let MaxIfNums = 16

    [<Literal>]
    let UnionStyleFieldName = "_union_style"

    let NewVariantByTag =
        typeof<Variant>
            .GetConstructor(BindingFlags.Public ||| BindingFlags.Instance, [| typeof<VariantTag> |])

    let NewVariantByNameTag =
        typeof<Variant>
            .GetConstructor(BindingFlags.Public ||| BindingFlags.Instance, [| typeof<string>; typeof<VariantTag> |])

[<AbstractClass>]
type internal _Union(info: UnionInfo) =
    inherit _Base()

    [<DefaultValue>]
    val mutable tag_deps: Dictionary<int, List<int>>

    override this.CollectTransforms(stub, target) =
        if target.IsValueType then
            System.Array.Empty<EmitTransform>()
        else
            SerEmitJob.ReferenceTypeTransforms

    override this.CollectDeps(stub, target) =
        let tag_deps = Dictionary<int, List<int>>()
        let deps = List<DepMeta>()

        for case in info.cases do
            if case.fields.Length = 0 then
                tag_deps.Add(case.tag, List())
            else
                let current_deps = List<int>()

                for field in case.fields do
                    let i = deps.Count
                    current_deps.Add(i)
                    let typ = field.PropertyType

                    let transforms =
                        if typ.IsValueType then
                            EmitTransform.EmptyTransforms
                        else
                            SerEmitJob.NullableClassImplTransforms

                    let sera_attr = field.GetCustomAttribute<SeraAttribute>()
                    let sera_format_attr = field.GetCustomAttribute<SeraFormatsAttribute>()
                    let data = SeraStyles.FromAttr(sera_attr, sera_format_attr)

                    let meta = DepMeta(EmitMeta(TypeMetas.GetTypeMeta(typ, null), data), transforms)
                    deps.Add(meta)

                tag_deps.Add(case.tag, current_deps)

        this.tag_deps <- tag_deps

        deps.ToArray()

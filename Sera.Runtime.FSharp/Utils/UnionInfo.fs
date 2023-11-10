namespace Sera.Runtime.FSharp.Utils

open System
open System.Collections.Generic
open System.Reflection
open System.Text.RegularExpressions
open Microsoft.FSharp.Reflection
open Sera.Core
open System.Linq
open Sera
open Sera.Utils

type PSeq = ParallelEnumerable

type internal UnionCase =
    { info: UnionCaseInfo
      name: string
      tag: int
      variant_attr: SeraVariantAttribute option
      formats_attr: SeraFormatsAttribute option
      format: VariantFormat
      style: VariantStyle
      fields: PropertyInfo[] }

[<Struct>]
type internal UnionJumpTable = { index: int; case: UnionCase }

type internal UnionJumpTables =
    { offset: int; table: UnionJumpTable[] }

type internal UnionInfo =
    { cases: UnionCase[]
      table: UnionJumpTables }

module internal UnionReflectionUtils =

    let item_n = Regex(@"Item\d+", RegexOptions.Compiled)

    let IsUnion (target: Type) =
        FSharpType.IsUnion(target, BindingFlags.Public ||| BindingFlags.NonPublic)

    let buildJumpTable
        (cases: UnionCase[])
        (items: struct (UnionCase * int)[])
        (value_index_map: Dictionary<int, int>)
        min
        max
        offset
        : UnionJumpTables =
        let mutable last = cases.Length - 1
        let tables = List<UnionJumpTable>()

        let mutable v = max

        while (v > min) do
            let mutable last1: int = 0

            if value_index_map.TryGetValue(v, &last1) then
                last <- last1

            let struct (case, _) = items[last]
            tables.Add({ index = last; case = case })
            v <- v - 1

        let struct (case, _) = items[0]
        tables.Add({ index = 0; case = case })
        tables.Reverse()

        { offset = offset
          table = tables.ToArray() }

    let MakeJumpTable (cases: UnionCase[]) =
        let items =
            cases
            |> Seq.mapi (fun i a -> struct (a, i))
            |> Seq.sortBy (fun struct (a, _) -> a.tag)
            |> Seq.toArray

        let value_index_map =
            items.ToDictionary((fun struct (a, _) -> a.tag), (fun struct (_, i) -> i))

        let struct (min, _) = items.First()
        let min = min.tag
        let struct (max, _) = items.Last()
        let max = max.tag

        let offset: int = Int128.op_Explicit (Int128.Zero - (Int128.op_Implicit min))

        buildJumpTable cases items value_index_map min max offset

    let GetUnionInfo (target: Type) =
        let union_sera_attr = target.GetCustomAttribute<SeraAttribute>()

        let union_rename =
            if union_sera_attr <> null then
                Nullable(union_sera_attr.Rename)
            else
                Nullable()

        let cases =
            FSharpType.GetUnionCases(target, BindingFlags.Public ||| BindingFlags.NonPublic)

        let cases =
            query {
                for case in cases do
                    let variant_sera_attr =
                        case.GetCustomAttributes(typeof<SeraAttribute>)
                        |> Seq.cast<SeraAttribute>
                        |> Seq.tryHead

                    let variant_rename =
                        match variant_sera_attr with
                        | None -> Nullable()
                        | Some value -> Nullable(value.Rename)

                    let variant_name =
                        match variant_sera_attr with
                        | None -> case.Name
                        | Some value -> value.Name

                    let variant_name = if variant_name = null then case.Name else variant_name

                    let rename = SeraRename.Or(variant_rename, union_rename)
                    let name = SeraRename.Rename(variant_name, rename)

                    let variant_attr =
                        case.GetCustomAttributes(typeof<SeraVariantAttribute>)
                        |> Seq.cast<SeraVariantAttribute>
                        |> Seq.tryHead

                    let formats_attr =
                        case.GetCustomAttributes(typeof<SeraFormatsAttribute>)
                        |> Seq.cast<SeraFormatsAttribute>
                        |> Seq.tryHead

                    let fields = case.GetFields()

                    let format =
                        match variant_attr with
                        | Some v -> v.Format
                        | _ -> VariantFormat.None

                    let format =
                        if format <> VariantFormat.None then
                            format
                        else if fields.Length = 0 then
                            VariantFormat.None
                        else if fields.Length = 1 && fields[0].Name = "Item" then
                            VariantFormat.Value
                        else if (fields |> Seq.forall (fun field -> item_n.IsMatch(field.Name))) then
                            VariantFormat.Tuple
                        else
                            VariantFormat.Struct

                    let style =
                        VariantStyle.FromAttr(
                            Option.defaultValue null variant_attr,
                            Option.defaultValue null formats_attr
                        )

                    select
                        { info = case
                          name = name
                          tag = case.Tag
                          variant_attr = variant_attr
                          formats_attr = formats_attr
                          format = format
                          style = style
                          fields = fields }
            }
            |> Seq.toArray

        { cases = cases
          table = MakeJumpTable cases }

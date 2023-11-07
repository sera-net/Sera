namespace rec Sera.Emit.FSharp.Template

open Sera
open Sera.Core
open Sera.Core.Impls.Ser

[<SeraUnion(Format = UnionFormat.External)>]
type TypeTestUnion1 =
    | TypeTestUnion1_A
    | TypeTestUnion1_B of int
    | TypeTestUnion1_C of int * int
    | TypeTestUnion1_D of a: int * b: int

[<Struct; SeraUnion(Format = UnionFormat.External)>]
type TypeTestUnion2 =
    | TypeTestUnion2_A
    | TypeTestUnion2_B of int
    | TypeTestUnion2_C of int * int
    | TypeTestUnion2_D of a: int * b: int

[<Struct; NoEquality; NoComparison>]
type Impl1 =

    interface ISeraVision<TypeTestUnion1> with
        member this.Accept(visitor, value) = visitor.VUnion(this, value)

    interface IUnionSeraVision<TypeTestUnion1> with
        member this.Name = nameof TypeTestUnion1

        member this.AcceptUnion(visitor, value) =
            match value with
            | TypeTestUnion1_A -> visitor.VVariant(Variant(nameof TypeTestUnion1_A, VariantTag.Create(0)))
            | TypeTestUnion1_B i ->
                visitor.VVariantValue<_, int>(
                    PrimitiveImpl(),
                    i,
                    Variant(nameof TypeTestUnion1_B, VariantTag.Create(0))
                )
            | TypeTestUnion1_C _ ->
                visitor.VVariantTuple(
                    TypeTestUnion1_C_Impl(),
                    value,
                    Variant(nameof TypeTestUnion1_C, VariantTag.Create(0))
                )
            | TypeTestUnion1_D _ ->
                visitor.VVariantStruct(
                    TypeTestUnion1_D_Impl(),
                    value,
                    Variant(nameof TypeTestUnion1_D, VariantTag.Create(0))
                )

[<Struct; NoEquality; NoComparison>]
type TypeTestUnion1_C_Impl =
    interface ITupleSeraVision<TypeTestUnion1> with
        member this.Size = 2

        member this.AcceptItem(visitor, value, index) =
            match index with
            | 0 ->
                match value with
                | TypeTestUnion1_C(a, _) -> visitor.VItem<_, int>(PrimitiveImpl(), a)
                | _ -> visitor.VNone()
            | 1 ->
                match value with
                | TypeTestUnion1_C(_, b) -> visitor.VItem<_, int>(PrimitiveImpl(), b)
                | _ -> visitor.VNone()
            | _ -> visitor.VNone()

[<Struct; NoEquality; NoComparison>]
type TypeTestUnion1_D_Impl =
    interface IStructSeraVision<TypeTestUnion1> with
        member this.Name = "TypeTestUnion1.TypeTestUnion1_D"
        member this.Count = 2

        member this.AcceptField(visitor, value, field) =
            match field with
            | 0 ->
                match value with
                | TypeTestUnion1_D(a, _) -> visitor.VField<_, int>(PrimitiveImpl(), a, "a", 0)
                | _ -> visitor.VNone()
            | 1 ->
                match value with
                | TypeTestUnion1_D(_, b) -> visitor.VField<_, int>(PrimitiveImpl(), b, "b", 0)
                | _ -> visitor.VNone()
            | _ -> visitor.VNone()

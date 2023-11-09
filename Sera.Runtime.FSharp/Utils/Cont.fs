module internal Sera.Runtime.FSharp.Utils.Cont

type Delay<'a> = unit -> 'a
type Branch<'a> = Delay<'a> -> 'a
type DelayBranch<'a> = Delay<Branch<'a>>

[<Struct; NoEquality; NoComparison>]
type ContBuilder<'r> =
    member inline _.Return(v: 'r) = v
    member inline _.ReturnFrom(v: 'r) : Branch<'r> = fun _ -> v
    member inline _.Zero() : Branch<'a> = fun e -> e ()

    member inline _.Combine([<InlineIfLambda>] a: Branch<'r>, [<InlineIfLambda>] b: Delay<'r>) = a b

    member inline _.Combine([<InlineIfLambda>] a: Branch<'r>, [<InlineIfLambda>] b: DelayBranch<'r>) : Branch<'r> =
        fun e -> a (fun _ -> (b ()) e)

    member inline _.Delay([<InlineIfLambda>] f: Delay<'r>) = f
    member inline _.Delay([<InlineIfLambda>] f: DelayBranch<'r>) = f
    member inline _.Run([<InlineIfLambda>] f: Delay<'r>) = f ()

    // member inline _.For(s: 'a seq, [<InlineIfLambda>] f: 'a -> Branch<'r>) : Branch<'r> =
    //     let iter = s.GetEnumerator()
    //
    //     fun e ->
    //         let rec for_body () =
    //             if iter.MoveNext() then
    //                 let b = f iter.Current
    //                 b for_body
    //             else
    //                 e ()
    //
    //         for_body ()

let cont<'r> = ContBuilder<'r>()

namespace Sera.Runtime.FSharp.Emit.Ser.Jobs._Union

open System
open System.Reflection
open Sera.Core
open Sera.Runtime.FSharp.Emit.Ser.Jobs._Union._Private_Impls
open Sera.Runtime.FSharp.Utils

type List<'T> = System.Collections.Generic.List<'T>

type internal _Private(union_name: string, union_info: UnionInfo, union_style: UnionStyle) =
    inherit _Union(union_info)

    let mutable ImplType: Type = null

    override this.Init(stub, target) =
        ImplType <- typedefof<PrivateUnionImpl<_>>.MakeGenericType(target.Type)

    override this.GetEmitPlaceholderType(stub, target) = ImplType
    override this.GetEmitType(stub, target, deps) = ImplType
    override this.GetRuntimePlaceholderType(stub, target) = ImplType
    override this.GetRuntimeType(stub, target, deps) = ImplType

    override this.Emit(stub, target, deps) = ()

    override this.CreateInst(stub, target, deps) =
        let key = obj ()

        let meta: ImplMeta =
            { union_info = union_info
              union_style = union_style
              deps = deps
              tag_deps = this.tag_deps }

        let data: ImplData =
            { meta_key = key
              union_name = union_name }

        metas.Add(key, meta)

        let ctor =
            ImplType.GetConstructor(BindingFlags.NonPublic ||| BindingFlags.Instance, [| typeof<ImplData> |])

        let inst = ctor.Invoke([| data |])

        inst

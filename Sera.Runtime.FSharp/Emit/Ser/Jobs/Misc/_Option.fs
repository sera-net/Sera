namespace Sera.Runtime.FSharp.Emit.Ser.Jobs

open System
open System.Reflection
open Sera.FSharp.Impls.Ser
open Sera.Runtime.Emit
open Sera.Runtime.Emit.Deps
open Sera.Runtime.FSharp.Emit.Ser.Jobs
open Sera.Runtime.Utils

type _Option(UnderlyingType: Type) =
    inherit _Base()

    let mutable RuntimeType: Type = null

    override this.Init(stub, target) = ()
    override this.CollectTransforms(stub, target) = EmitTransform.EmptyTransforms

    override this.CollectDeps(stub, target) =
        let transforms = EmitTransform.EmptyTransforms

        let meta =
            DepMeta(
                EmitMeta(TypeMetas.GetTypeMeta(UnderlyingType, NullabilityMeta(null)), target.Styles.TakeFormats()),
                transforms
            )

        [| meta |]

    override this.GetEmitType(stub, target, deps) =
        let dep = deps.Get(0)
        let wrapper = dep.MakeSerWrapper(UnderlyingType)
        typedefof<OptionImpl<_, _>>.MakeGenericType(UnderlyingType, wrapper)

    override this.GetRuntimeType(stub, target, deps) =
        let dep = deps.Get(0)
        let wrapper = dep.MakeSerWrapper(UnderlyingType)
        RuntimeType <- typedefof<OptionImpl<_, _>>.MakeGenericType(UnderlyingType, wrapper)
        RuntimeType

    override this.Emit(stub, target, deps) = ()

    override this.CreateInst(stub, target, deps) =
        let dep = deps.Get(0)
        let wrapper = dep.MakeSerWrapper(UnderlyingType)

        let ctor =
            RuntimeType.GetConstructor(BindingFlags.Public ||| BindingFlags.Instance, [| wrapper |])

        let inst = ctor.Invoke([| null |])
        inst

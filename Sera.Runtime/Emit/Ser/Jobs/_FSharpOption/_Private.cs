// using System;
// using Sera.Runtime.Emit.Deps;
//
// namespace Sera.Runtime.Emit.Ser.Jobs._FSharpOption;
//
// internal abstract class _Private(Type UnderlyingType) : _FSharpOption(UnderlyingType)
// {
//     protected Type Type { get; set; } = null!;
//
//     public override Type GetEmitPlaceholderType(EmitStub stub, EmitMeta target)
//         => Type;
//
//     public override Type GetRuntimePlaceholderType(EmitStub stub, EmitMeta target)
//         => Type;
//
//     public override Type GetEmitType(EmitStub stub, EmitMeta target, EmitDeps deps)
//         => Type;
//
//     public override Type GetRuntimeType(EmitStub stub, EmitMeta target, RuntimeDeps deps)
//         => Type;
//
//     public override void Emit(EmitStub stub, EmitMeta target, EmitDeps deps) { }
// }

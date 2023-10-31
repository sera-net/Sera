// using System;
// using Sera.Core.Impls;
// using Sera.Runtime.Emit.Deps;
// using BindingFlags = System.Reflection.BindingFlags;
//
// namespace Sera.Runtime.Emit.Ser.Jobs._Array;
//
// internal class _SZ_Private(Type ItemType) : _Private(ItemType)
// {
//     public override void Init(EmitStub stub, EmitMeta target) { }
//
//     public override Type GetEmitPlaceholderType(EmitStub stub, EmitMeta target)
//         => typeof(ISerialize<>).MakeGenericType(target.Type);
//
//     public override Type GetEmitType(EmitStub stub, EmitMeta target, EmitDeps deps)
//     {
//         var dep = deps.Get(0);
//         var wrapper = dep.MakeSerializeWrapper(ItemType);
//         return typeof(ArraySerializeImpl<,>).MakeGenericType(ItemType, wrapper);
//     }
//
//     public override Type GetRuntimePlaceholderType(EmitStub stub, EmitMeta target)
//         => typeof(ISerialize<>).MakeGenericType(target.Type);
//
//     public override Type GetRuntimeType(EmitStub stub, EmitMeta target, RuntimeDeps deps)
//     {
//         var dep = deps.Get(0);
//         var wrapper = dep.MakeSerializeWrapper(ItemType);
//         return typeof(ArraySerializeImpl<,>).MakeGenericType(ItemType, wrapper);
//     }
//
//     public override object CreateInst(EmitStub stub, EmitMeta target, RuntimeDeps deps)
//     {
//         var dep = deps.Get(0);
//         var wrapper = dep.MakeSerializeWrapper(ItemType);
//         var inst_type = typeof(ArraySerializeImpl<,>).MakeGenericType(ItemType, wrapper);
//         var ctor = inst_type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, new[] { wrapper })!;
//         var inst = ctor.Invoke(new object?[] { null });
//         return inst;
//     }
// }

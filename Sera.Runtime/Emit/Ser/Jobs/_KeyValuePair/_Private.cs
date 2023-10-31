// using System;
// using System.Reflection;
// using Sera.Core.Impls;
// using Sera.Runtime.Emit.Deps;
//
// namespace Sera.Runtime.Emit.Ser.Jobs._KeyValuePair;
//
// internal class _Private(Type KeyType, Type ValueType) : _KeyValuePair(KeyType, ValueType)
// { 
//     private static readonly EmitTransform[] Transforms =
//     {
//         new Transforms._KeyValuePairSerializeImplWrapper()
//     };
//
//     public override EmitTransform[] CollectTransforms(EmitStub stub, EmitMeta target)
//         => Transforms;
//
//     public Type ImplType { get; set; } = null!;
//
//     public override void Init(EmitStub stub, EmitMeta target)
//     {
//         ImplType = typeof(KeyValuePairSerializeImplBase<,>).MakeGenericType(KeyType, ValueType);
//     }
//
//     public override Type GetEmitType(EmitStub stub, EmitMeta target, EmitDeps deps)
//         => ImplType;
//
//     public override Type GetRuntimeType(EmitStub stub, EmitMeta target, RuntimeDeps deps)
//         => ImplType;
//
//     public override Type GetEmitPlaceholderType(EmitStub stub, EmitMeta target)
//         => ImplType;
//
//     public override Type GetRuntimePlaceholderType(EmitStub stub, EmitMeta target)
//         => ImplType;
//
//     public override void Emit(EmitStub stub, EmitMeta target, EmitDeps deps) { }
//
//     public override object CreateInst(EmitStub stub, EmitMeta target, RuntimeDeps deps)
//     {
//         var dep_key = deps.Get(0);
//         var dep_val = deps.Get(1);
//         var wrapper_key = dep_key.MakeSerializeWrapper(KeyType);
//         var wrapper_val = dep_val.MakeSerializeWrapper(ValueType);
//         var inst_type =
//             typeof(KeyValuePairSerializeImpl<,,,>).MakeGenericType(KeyType, ValueType, wrapper_key, wrapper_val);
//         var ctor = inst_type.GetConstructor(BindingFlags.Public | BindingFlags.Instance,
//             new[] { wrapper_key, wrapper_val })!;
//         var inst = ctor.Invoke(new object?[] { null, null });
//         return inst;
//     }
// }

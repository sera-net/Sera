// using System;
// using System.Reflection;
// using Sera.Core.Impls;
// using Sera.Runtime.Emit.Deps;
//
// namespace Sera.Runtime.Emit.Ser.Jobs._Array;
//
// internal class _ReadOnlySequence_Private(Type ItemType) : _Private(ItemType)
// {
//     protected override NullabilityInfo? GetElementNullabilityInfo(EmitMeta target)
//         => target.TypeMeta.Nullability?.NullabilityInfo?.GenericTypeArguments[0];
//     
//     public static readonly EmitTransform[] Transforms =
//     {
//          new Transforms._ReadOnlySequenceSerializeImplWrapper(),
//     };
//
//     public override void Init(EmitStub stub, EmitMeta target)
//     {
//         BaseType = typeof(ReadOnlySequenceSerializeImplBase<>).MakeGenericType(ItemType);
//     }
//     
//     public override EmitTransform[] CollectTransforms(EmitStub stub, EmitMeta target)
//         => Transforms;
//
//     public override object CreateInst(EmitStub stub, EmitMeta target, RuntimeDeps deps)
//     {
//         var dep = deps.Get(0);
//         var wrapper = dep.MakeSerializeWrapper(ItemType);
//         var inst_type = typeof(ReadOnlySequenceSerializeImpl<,>).MakeGenericType(ItemType, wrapper);
//         var ctor = inst_type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, new[] { wrapper })!;
//         var inst = ctor.Invoke(new object?[] { null });
//         return inst;
//     }
// }

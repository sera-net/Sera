// using System;
// using System.Collections.Concurrent;
// using System.Collections.Frozen;
// using System.Collections.Generic;
// using System.Collections.Immutable;
// using System.Reflection;
// using Sera.Runtime.Emit.Deps;
// using Sera.Runtime.Utils;
//
// namespace Sera.Runtime.Emit.Ser.Jobs._IEnumerable._Generic;
//
// internal abstract class _Generic(Type ItemType) : _IEnumerable
// {
//     public override EmitTransform[] CollectTransforms(EmitStub stub, EmitMeta target)
//         => target.IsValueType ? EmitTransform.EmptyTransforms : SerializeEmitProvider.ReferenceTypeTransforms;
//
//     public override DepMeta[] CollectDeps(EmitStub stub, EmitMeta target)
//     {
//         NullabilityInfo[]? nullables = null;
//         if (target.Type.IsOpenTypeEqAny(
//                 typeof(LinkedList<>),
//                 typeof(Queue<>),
//                 typeof(Stack<>),
//                 typeof(HashSet<>),
//                 typeof(SortedSet<>),
//                 typeof(FrozenSet<>),
//                 typeof(ConcurrentBag<>),
//                 typeof(ConcurrentQueue<>),
//                 typeof(ConcurrentStack<>),
//                 typeof(BlockingCollection<>),
//                 typeof(ImmutableList<>),
//                 typeof(ImmutableQueue<>),
//                 typeof(ImmutableStack<>),
//                 typeof(ImmutableHashSet<>),
//                 typeof(ImmutableHashSet<>)
//             ))
//         {
//             nullables = target.TypeMeta.Nullability?.NullabilityInfo?.GenericTypeArguments;
//         }
//         var nullable = nullables?[0];
//         var transforms = !ItemType.IsValueType && nullable is not
//             { ReadState: NullabilityState.NotNull }
//             ? SerializeEmitProvider.NullableClassImplTransforms
//             : EmitTransform.EmptyTransforms;
//         var meta = new DepMeta(
//             new(TypeMetas.GetTypeMeta(ItemType, new NullabilityMeta(nullable)), target.Data),
//             transforms);
//         return new[] { meta };
//     }
// }

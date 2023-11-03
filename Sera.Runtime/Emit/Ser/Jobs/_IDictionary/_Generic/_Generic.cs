// using System;
// using System.Collections.Concurrent;
// using System.Collections.Frozen;
// using System.Collections.Generic;
// using System.Collections.Immutable;
// using System.Collections.ObjectModel;
// using System.Linq;
// using System.Reflection;
// using System.Runtime.CompilerServices;
// using Sera.Runtime.Emit.Deps;
// using Sera.Runtime.Utils;
//
// namespace Sera.Runtime.Emit.Ser.Jobs._IDictionary._Generic;
//
// internal abstract class _Generic(Type KeyType, Type ValueType) : _IDictionary
// {
//     public override EmitTransform[] CollectTransforms(EmitStub stub, EmitMeta target)
//         => target.IsValueType ? EmitTransform.EmptyTransforms : SerializeEmitProvider.ReferenceTypeTransforms;
//
//     public override DepMeta[] CollectDeps(EmitStub stub, EmitMeta target)
//     {
//         NullabilityInfo[]? nullables = null;
//         if (target.Type.IsOpenTypeEqAny(
//                 typeof(Dictionary<,>),
//                 typeof(ReadOnlyDictionary<,>),
//                 typeof(ConcurrentDictionary<,>),
//                 typeof(FrozenDictionary<,>),
//                 typeof(ConditionalWeakTable<,>),
//                 typeof(SortedDictionary<,>),
//                 typeof(SortedList<,>),
//                 typeof(ImmutableDictionary<,>),
//                 typeof(ImmutableSortedDictionary<,>)
//             ))
//         {
//             nullables = target.TypeMeta.Nullability?.NullabilityInfo?.GenericTypeArguments;
//         }
//         var types = new[] { KeyType, ValueType };
//         return types.Select((t, i) =>
//         {
//             var nullable = nullables?[i];
//             var transforms = !t.IsValueType && nullable is not
//                 { ReadState: NullabilityState.NotNull }
//                 ? SerializeEmitProvider.NullableClassImplTransforms
//                 : EmitTransform.EmptyTransforms;
//             var meta = new DepMeta(
//                 new(TypeMetas.GetTypeMeta(t, new NullabilityMeta(nullable)), target.Data),
//                 transforms);
//             return meta;
//         }).ToArray();
//     }
// }

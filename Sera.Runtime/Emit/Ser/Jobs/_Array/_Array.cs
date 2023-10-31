// using System;
// using System.Reflection;
// using Sera.Runtime.Emit.Deps;
// using Sera.Runtime.Utils;
//
// namespace Sera.Runtime.Emit.Ser.Jobs._Array;
//
// // todo ArraySegment, ImmutableArray
//
// internal abstract class _Array(Type ItemType) : _Base
// {
//     public override EmitTransform[] CollectTransforms(EmitStub stub, EmitMeta target)
//         => target.IsValueType ? EmitTransform.EmptyTransforms : SerializeEmitProvider.ReferenceTypeTransforms;
//
//     protected virtual NullabilityInfo? GetElementNullabilityInfo(EmitMeta target)
//         => target.TypeMeta.Nullability?.NullabilityInfo?.ElementType;
//
//     public override DepMeta[] CollectDeps(EmitStub stub, EmitMeta target)
//     {
//         var item_nullable = GetElementNullabilityInfo(target);
//         var transforms = !ItemType.IsValueType && item_nullable is not
//             { ReadState: NullabilityState.NotNull }
//             ? SerializeEmitProvider.NullableClassImplTransforms
//             : EmitTransform.EmptyTransforms;
//         var meta = new DepMeta(
//             new(TypeMetas.GetTypeMeta(ItemType, new NullabilityMeta(item_nullable)), target.Data),
//             transforms);
//         return new[] { meta };
//     }
// }

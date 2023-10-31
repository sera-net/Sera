// using System;
// using System.Linq;
// using System.Reflection;
// using Sera.Runtime.Emit.Deps;
// using Sera.Runtime.Utils;
//
// namespace Sera.Runtime.Emit.Ser.Jobs._KeyValuePair;
//
// internal abstract class _KeyValuePair(Type KeyType, Type ValueType) : _Base
// {
//     public override EmitTransform[] CollectTransforms(EmitStub stub, EmitMeta target)
//         => EmitTransform.EmptyTransforms;
//     
//     public override DepMeta[] CollectDeps(EmitStub stub, EmitMeta target)
//     {
//         var nullables = target.TypeMeta.Nullability?.NullabilityInfo?.GenericTypeArguments;
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

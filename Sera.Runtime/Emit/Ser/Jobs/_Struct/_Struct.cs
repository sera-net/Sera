// using System;
// using System.Linq;
// using System.Reflection;
// using Sera.Runtime.Emit.Deps;
// using Sera.Runtime.Utils;
//
// namespace Sera.Runtime.Emit.Ser.Jobs._Struct;
//
// internal abstract class _Struct(StructMember[] Members) : _Base
// {
//     public override EmitTransform[] CollectTransforms(EmitStub stub, EmitMeta target)
//     {
//         if (target.IsValueType) return Array.Empty<EmitTransform>();
//         else return SerializeEmitProvider.ReferenceTypeTransforms;
//     }
//
//     public override DepMeta[] CollectDeps(EmitStub stub, EmitMeta target)
//     {
//         return Members.AsParallel()
//             .Select(m =>
//             {
//                 var null_meta = m.Kind switch
//                 {
//                     PropertyOrField.Property => TypeMetas.GetNullabilityMeta(m.Property!),
//                     PropertyOrField.Field => TypeMetas.GetNullabilityMeta(m.Field!),
//                     _ => throw new ArgumentOutOfRangeException()
//                 };
//                 var transforms = !m.Type.IsValueType && null_meta is not
//                     { NullabilityInfo.ReadState: NullabilityState.NotNull }
//                     ? SerializeEmitProvider.NullableClassImplTransforms
//                     : EmitTransform.EmptyTransforms;
//
//                 var sera_attr = m.Kind switch
//                 {
//                     PropertyOrField.Property => m.Property!.GetCustomAttribute<SeraAttribute>(),
//                     PropertyOrField.Field => m.Field!.GetCustomAttribute<SeraAttribute>(),
//                     _ => throw new ArgumentOutOfRangeException()
//                 };
//                 var data = new SeraHints(
//                     Primitive: sera_attr?.SerPrimitive,
//                     As: sera_attr?.As ?? SeraAs.None
//                 );
//
//                 return new DepMeta(new(TypeMetas.GetTypeMeta(m.Type, null_meta), data), transforms);
//             }).ToArray();
//     }
// }

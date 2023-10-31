// using System;
// using System.Collections.Generic;
// using System.Reflection;
// using Sera.Runtime.Utils;
//
// namespace Sera.Runtime.Emit.Ser.Jobs._Array;
//
// internal class _List_Public(Type ItemType) : _Public(ItemType)
// {
//     protected override NullabilityInfo? GetElementNullabilityInfo(EmitMeta target)
//     {
//         if (target.Type.IsOpenTypeEq(typeof(List<>)))
//         {
//             return target.TypeMeta.Nullability?.NullabilityInfo?.GenericTypeArguments[0];
//         }
//         return null;
//     }
//
//     protected override MethodInfo WriteArrayMethod => ReflectionUtils.ISerializer_WriteArray_2generic_list;
// }

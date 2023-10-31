// using System;
// using System.Reflection;
// using Sera.Runtime.Utils;
//
// namespace Sera.Runtime.Emit.Ser.Jobs._Array;
//
// internal class _ReadOnlyMemory_Public(Type ItemType) : _Public(ItemType)
// {
//     protected override NullabilityInfo? GetElementNullabilityInfo(EmitMeta target)
//         => target.TypeMeta.Nullability?.NullabilityInfo?.GenericTypeArguments[0];
//     
//     protected override MethodInfo WriteArrayMethod => ReflectionUtils.ISerializer_WriteArray_2generic_ReadOnlyMemory;
// }

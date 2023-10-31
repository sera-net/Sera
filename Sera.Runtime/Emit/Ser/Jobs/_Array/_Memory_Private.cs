// using System;
// using System.Reflection;
//
// namespace Sera.Runtime.Emit.Ser.Jobs._Array;
//
// internal class _Memory_Private(Type ItemType) : _ReadOnlyMemory_Private(ItemType)
// {
//     protected override NullabilityInfo? GetElementNullabilityInfo(EmitMeta target)
//         => target.TypeMeta.Nullability?.NullabilityInfo?.GenericTypeArguments[0];
// }

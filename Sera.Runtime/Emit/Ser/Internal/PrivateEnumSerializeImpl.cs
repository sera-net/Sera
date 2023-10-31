using System;
using System.Collections.Frozen;
using System.Runtime.CompilerServices;
using Sera.Core;

namespace Sera.Runtime.Emit.Ser.Internal;

// public struct PrivateEnumSerializeImpl<T> : ISerialize<T> where T : Enum
// {
//     private readonly Inner inner;
//
//     internal PrivateEnumSerializeImpl(
//         string unionName, Func<T, VariantTag> toTag,
//         FrozenDictionary<T, (string name, SerializerVariantHint? hint)> metas,
//         SerializerVariantHint? rootHint
//     ) => inner = new(unionName, toTag, metas, rootHint);
//
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     public void Write<S>(S serializer, T value, ISeraOptions options) where S : ISerializer
//         => inner.Write(serializer, value);
//
//     private sealed class Inner(
//         string unionName, Func<T, VariantTag> toTag,
//         FrozenDictionary<T, (string name, SerializerVariantHint? hint)> metas,
//         SerializerVariantHint? rootHint
//     )
//     {
//         [MethodImpl(MethodImplOptions.AggressiveInlining)]
//         public void Write<S>(S serializer, T value) where S : ISerializer
//         {
//             if (metas.TryGetValue(value, out var meta))
//             {
//                 serializer.WriteVariantUnit<T>(unionName, new Variant(meta.name, toTag(value)), meta.hint);
//             }
//             else
//             {
//                 serializer.WriteVariantUnit<T>(unionName, new Variant(toTag(value)), rootHint);
//             }
//         }
//     }
// }

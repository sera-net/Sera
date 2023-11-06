using System;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using Sera.Runtime.Emit.Deps;
using Sera.Runtime.Utils;
using Sera.Runtime.Utils.Internal;

namespace Sera.Runtime.Emit.Ser.Jobs._Map._Generic;

internal abstract class _Generic(Type KeyType, Type ValType) : _Map
{
    protected virtual (NullabilityInfo?, NullabilityInfo?) GetElementNullabilityInfo(EmitMeta target)
    {
        var type = target.Type;
        if (type.IsGenericType)
        {
            var del = type.GetGenericTypeDefinition();
            if (ReflectionUtils.SingleGenericTypes.Contains(del))
            {
                var args = target.TypeMeta.Nullability?.NullabilityInfo?.GenericTypeArguments[0].GenericTypeArguments;
                return args is { Length: 2 } ? (args?[0], args?[1]) : (null, null);
            }
            if (ReflectionUtils.DoubleGenericTypes.Contains(del))
            {
                var args = target.TypeMeta.Nullability?.NullabilityInfo?.GenericTypeArguments;
                return args is { Length: 2 } ? (args?[0], args?[1]) : (null, null);
            }
        }
        return (null, null);
    }

    public override DepMeta[] CollectDeps(EmitStub stub, EmitMeta target)
    {
        var (key_nullable, val_nullable) = GetElementNullabilityInfo(target);
        var key_transforms = !KeyType.IsValueType && key_nullable is not
            { ReadState: NullabilityState.NotNull }
            ? NullableClassImplTransforms
            : EmitTransform.EmptyTransforms;
        var val_transforms = !ValType.IsValueType && val_nullable is not
            { ReadState: NullabilityState.NotNull }
            ? NullableClassImplTransforms
            : EmitTransform.EmptyTransforms;
        var key_meta = new DepMeta(
            new(TypeMetas.GetTypeMeta(KeyType, new NullabilityMeta(key_nullable)), target.Styles.TakeFormats()),
            key_transforms);
        var val_meta = new DepMeta(
            new(TypeMetas.GetTypeMeta(ValType, new NullabilityMeta(val_nullable)), target.Styles.TakeFormats()),
            val_transforms);
        return new[] { key_meta, val_meta };
    }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using Sera.Runtime.Emit.Deps;
using Sera.Runtime.Utils;
using Sera.Runtime.Utils.Internal;

namespace Sera.Runtime.Emit.Ser.Jobs._Seq._Generic;

internal abstract class _Generic(Type item_type) : _Seq
{
    public Type ItemType => item_type;

    protected virtual NullabilityInfo? GetElementNullabilityInfo(EmitMeta target)
    {
        if (target.Type.IsSZArray) return target.TypeMeta.Nullability?.NullabilityInfo?.ElementType;
        var type = target.Type;
        if (type.IsGenericType)
        {
            var del = type.GetGenericTypeDefinition();
            if (ReflectionUtils.SingleGenericTypes.Contains(del))
            {
                return target.TypeMeta.Nullability?.NullabilityInfo?.GenericTypeArguments[0];
            }
        }
        return null;
    }

    public override DepMeta[] CollectDeps(EmitStub stub, EmitMeta target)
    {
        var item_nullable = GetElementNullabilityInfo(target);
        var transforms = !ItemType.IsValueType && item_nullable is not
            { ReadState: NullabilityState.NotNull }
            ? NullableClassImplTransforms
            : EmitTransform.EmptyTransforms;
        var meta = new DepMeta(
            new(TypeMetas.GetTypeMeta(ItemType, new NullabilityMeta(item_nullable)), target.Styles.TakeFormats()),
            transforms);
        return new[] { meta };
    }
}

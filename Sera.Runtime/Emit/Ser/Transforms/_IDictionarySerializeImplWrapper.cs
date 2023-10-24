using System;
using Sera.Core.Impls;

namespace Sera.Runtime.Emit.Ser.Transforms;

internal class _IDictionarySerializeImplWrapper(Type KeyType, Type ValueType) : EmitTransform
{
    public override Type TransformType(EmitMeta target, Type prevType)
    {
        return typeof(IDictionarySerializeImplWrapper<,,>).MakeGenericType(target.Type, KeyType, ValueType);
    }

    public override object TransformInst(EmitMeta target, Type type, Type prevType, object prevInst)
    {
        var ctor = type.GetConstructor(new[]
            { typeof(IDictionarySerializeImplBase<,,>).MakeGenericType(target.Type, KeyType, ValueType) })!;
        return ctor.Invoke(new[] { prevInst });
    }
}

internal class _IReadOnlyDictionarySerializeImplWrapper(Type KeyType, Type ValueType) : EmitTransform
{
    public override Type TransformType(EmitMeta target, Type prevType)
    {
        return typeof(IReadOnlyDictionarySerializeImplWrapper<,,>).MakeGenericType(target.Type, KeyType, ValueType);
    }

    public override object TransformInst(EmitMeta target, Type type, Type prevType, object prevInst)
    {
        var ctor = type.GetConstructor(new[]
            { typeof(IReadOnlyDictionarySerializeImplBase<,,>).MakeGenericType(target.Type, KeyType, ValueType) })!;
        return ctor.Invoke(new[] { prevInst });
    }
}

internal class _IEnumerableMapSerializeImplWrapper(Type KeyType, Type ValueType) : EmitTransform
{
    public override Type TransformType(EmitMeta target, Type prevType)
    {
        return typeof(IEnumerableMapSerializeImplWrapper<,,>).MakeGenericType(target.Type, KeyType, ValueType);
    }

    public override object TransformInst(EmitMeta target, Type type, Type prevType, object prevInst)
    {
        var ctor = type.GetConstructor(new[]
            { typeof(IEnumerableMapSerializeImplBase<,,>).MakeGenericType(target.Type, KeyType, ValueType) })!;
        return ctor.Invoke(new[] { prevInst });
    }
}

internal class _ICollectionMapSerializeImplWrapper(Type KeyType, Type ValueType) : EmitTransform
{
    public override Type TransformType(EmitMeta target, Type prevType)
    {
        return typeof(ICollectionMapSerializeImplWrapper<,,>).MakeGenericType(target.Type, KeyType, ValueType);
    }

    public override object TransformInst(EmitMeta target, Type type, Type prevType, object prevInst)
    {
        var ctor = type.GetConstructor(new[]
            { typeof(ICollectionMapSerializeImplBase<,,>).MakeGenericType(target.Type, KeyType, ValueType) })!;
        return ctor.Invoke(new[] { prevInst });
    }
}

internal class _IReadOnlyCollectionMapSerializeImplWrapper(Type KeyType, Type ValueType) : EmitTransform
{
    public override Type TransformType(EmitMeta target, Type prevType)
    {
        return typeof(IReadOnlyCollectionMapSerializeImplWrapper<,,>).MakeGenericType(target.Type, KeyType, ValueType);
    }

    public override object TransformInst(EmitMeta target, Type type, Type prevType, object prevInst)
    {
        var ctor = type.GetConstructor(new[]
            { typeof(IReadOnlyCollectionMapSerializeImplBase<,,>).MakeGenericType(target.Type, KeyType, ValueType) })!;
        return ctor.Invoke(new[] { prevInst });
    }
}

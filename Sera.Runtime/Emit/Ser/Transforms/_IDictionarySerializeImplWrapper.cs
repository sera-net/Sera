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

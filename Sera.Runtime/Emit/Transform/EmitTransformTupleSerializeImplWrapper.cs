using System;
using BindingFlags = System.Reflection.BindingFlags;

namespace Sera.Runtime.Emit.Transform;

internal class EmitTransformTupleSerializeImplWrapper(Type Wrapper, Type Base) : EmitTransform
{
    public override Type TransformType(EmitMeta target, Type prevType)
    {
        var item_types = target.Type.GetGenericArguments();
        return Wrapper.MakeGenericType(item_types);
    }

    public override object TransformInst(EmitMeta target, Type type, Type prevType, object prevInst)
    {
        var item_types = target.Type.GetGenericArguments();
        var base_type = Base.MakeGenericType(item_types);
        var ctor = type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, new[] { base_type })!;
        return ctor.Invoke(new[] { prevInst });
    }
}

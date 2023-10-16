using System;

namespace Sera.Runtime.Emit;

internal abstract class EmitTransform
{
    public static EmitTransform[] EmptyTransforms { get; } = Array.Empty<EmitTransform>();
    
    public abstract Type TransformType(EmitMeta target, Type prevType);
    public abstract object TransformInst(EmitMeta target, Type type, Type prevType, object prevInst);
}

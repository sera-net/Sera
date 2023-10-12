using System;
using Sera.Core.Impls;
using Sera.Core.Ser;
using Sera.Runtime.Emit.Deps;
using Sera.Runtime.Emit.Transform;
using System.Reflection;

namespace Sera.Runtime.Emit.Ser.Jobs;

internal class _Primitive : _Base
{
    private Type Type = null!;
    private SerializerPrimitiveHint? Hint;

    public override void Init(EmitStub stub, EmitMeta target)
    {
        Type = typeof(PrimitiveImpl<>).MakeGenericType(target.Type);
        Hint = target.Data.Hint;
    }

    public override EmitTransform[] CollectTransforms(EmitStub stub, EmitMeta target)
        => Array.Empty<EmitTransform>();

    public override DepMeta[] CollectDeps(EmitStub stub, EmitMeta target)
        => Array.Empty<DepMeta>();

    public override Type GetEmitType(EmitStub stub, EmitMeta target, DepItem[] deps)
        => Type;

    public override Type GetEmitPlaceholderType(EmitStub stub, EmitMeta target)
        => Type;

    public override Type GetRuntimeType(EmitStub stub, EmitMeta target, DepItem[] deps)
        => Type;

    public override Type GetRuntimePlaceholderType(EmitStub stub, EmitMeta target)
        => Type;

    public override void Emit(EmitStub stub, EmitMeta target, EmitDeps deps) { }

    public override object CreateInst(EmitStub stub, EmitMeta target, RuntimeDeps deps)
    {
        var ctor = Type.GetConstructor(BindingFlags.Public | BindingFlags.Instance,
            new[] { typeof(SerializerPrimitiveHint?) })!;
        return ctor.Invoke(new object?[] { Hint });
    }
}

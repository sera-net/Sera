using System;
using System.Reflection;
using Sera.Core.Impls.Ser;
using Sera.Runtime.Emit.Deps;
using Sera.Runtime.Utils;

namespace Sera.Runtime.Emit.Ser.Jobs;

internal class _Nullable(Type UnderlyingType) : _Base
{
    private Type EmitType { get; set; } = null!;
    private Type RuntimeType { get; set; } = null!;

    public override void Init(EmitStub stub, EmitMeta target) { }

    public override EmitTransform[] CollectTransforms(EmitStub stub, EmitMeta target)
        => EmitTransform.EmptyTransforms;

    public override DepMeta[] CollectDeps(EmitStub stub, EmitMeta target)
    {
        var transforms = EmitTransform.EmptyTransforms;
        var meta = new DepMeta(
            new(TypeMetas.GetTypeMeta(UnderlyingType, new NullabilityMeta(null)), target.Styles.TakeFormats()),
            transforms);
        return new[] { meta };
    }

    public override Type GetEmitType(EmitStub stub, EmitMeta target, EmitDeps deps)
        => EmitType =
            typeof(NullableImpl<,>).MakeGenericType(UnderlyingType, deps.Get(0).MakeSerWrapper(UnderlyingType));

    public override Type GetRuntimeType(EmitStub stub, EmitMeta target, RuntimeDeps deps)
        => RuntimeType =
            typeof(NullableImpl<,>).MakeGenericType(UnderlyingType, deps.Get(0).MakeSerWrapper(UnderlyingType));

    public override void Emit(EmitStub stub, EmitMeta target, EmitDeps deps) { }

    public override object CreateInst(EmitStub stub, EmitMeta target, RuntimeDeps deps)
    {
        var dep = deps.Get(0);
        var wrapper = dep.MakeSerWrapper(UnderlyingType);
        var ctor = RuntimeType.GetConstructor(BindingFlags
                .Public | BindingFlags.Instance,
            new[] { wrapper })!;
        var inst = ctor.Invoke(new object[] { null! });
        return inst;
    }
}

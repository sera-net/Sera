using System;
using Sera.Core.Impls;
using Sera.Runtime.Emit.Deps;

namespace Sera.Runtime.Emit.Ser.Jobs._IEnumerable;

internal class _Legacy : _IEnumerable
{
    public Type ImplType { get; set; } = null!;

    public override void Init(EmitStub stub, EmitMeta target)
    {
        ImplType = typeof(IEnumerableSerializeRuntimeImpl<>).MakeGenericType(target.Type);
    }

    public override EmitTransform[] CollectTransforms(EmitStub stub, EmitMeta target)
        => target.IsValueType ? EmitTransform.EmptyTransforms : SerializeEmitProvider.ReferenceTypeTransforms;

    public override DepMeta[] CollectDeps(EmitStub stub, EmitMeta target)
        => Array.Empty<DepMeta>();

    public override Type GetEmitPlaceholderType(EmitStub stub, EmitMeta target)
        => ImplType;

    public override Type GetEmitType(EmitStub stub, EmitMeta target, EmitDeps deps)
        => ImplType;

    public override Type GetRuntimePlaceholderType(EmitStub stub, EmitMeta target)
        => ImplType;

    public override Type GetRuntimeType(EmitStub stub, EmitMeta target, RuntimeDeps deps)
        => ImplType;

    public override void Emit(EmitStub stub, EmitMeta target, EmitDeps deps) { }

    public override object CreateInst(EmitStub stub, EmitMeta target, RuntimeDeps deps)
        => Activator.CreateInstance(ImplType)!;
}

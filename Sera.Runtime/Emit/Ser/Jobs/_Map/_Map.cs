namespace Sera.Runtime.Emit.Ser.Jobs._Map;

internal abstract class _Map : _Base
{
    public override EmitTransform[] CollectTransforms(EmitStub stub, EmitMeta target)
        => target.IsValueType ? EmitTransform.EmptyTransforms : SerializeEmitProvider.ReferenceTypeTransforms;
}

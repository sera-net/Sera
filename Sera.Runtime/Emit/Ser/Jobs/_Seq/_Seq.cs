namespace Sera.Runtime.Emit.Ser.Jobs._Seq;

internal abstract class _Seq : _Base
{
    public override EmitTransform[] CollectTransforms(EmitStub stub, EmitMeta target)
        => target.IsValueType ? EmitTransform.EmptyTransforms : SerializeEmitProvider.ReferenceTypeTransforms;
}

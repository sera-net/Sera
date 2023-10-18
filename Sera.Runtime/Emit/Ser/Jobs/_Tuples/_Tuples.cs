using System;
using System.Linq;
using System.Reflection;
using Sera.Runtime.Emit.Deps;
using Sera.Runtime.Utils;

namespace Sera.Runtime.Emit.Ser.Jobs._Tuples;

internal abstract class _Tuples(bool IsValueTuple, Type[] ItemTypes) : _Base
{
    protected int TupleSize { get; private set; }

    protected int Size => Math.Min(TupleSize, 8);

    public override void Init(EmitStub stub, EmitMeta target)
    {
        TupleSize = GetTupleSize(target.Type);
    }

    private int GetTupleSize(Type target, int size = 0)
    {
        for (;;)
        {
            if (!target.IsGenericType) return size;
            var generics = target.GenericTypeArguments;
            if (generics.Length < 8) return generics.Length + size;
            target = generics[7];
            if (!(IsValueTuple ? ReflectionUtils.ValueTuples : ReflectionUtils.ClassTuples).Contains(
                    target.GetGenericTypeDefinition()))
                return generics.Length + size;
            size += 7;
        }
    }

    public override DepMeta[] CollectDeps(EmitStub stub, EmitMeta target)
    {
        var nullable = target.TypeMeta.Generics?.Nullabilities;
        return target.Type.GetGenericArguments().Select((t, i) =>
        {
            var item_nullable = nullable?[i];
            var transforms = i != 7 && !t.IsValueType && item_nullable is not
                { NullabilityInfo.ReadState: NullabilityState.NotNull }
                ? SerializeEmitProvider.NullableReferenceTypeTransforms
                : EmitTransform.EmptyTransforms;
            return new DepMeta(new(TypeMetas.GetTypeMeta(t, item_nullable), target.Data),
                transforms, KeepRaw: i == 7);
        }).ToArray();
    }
}

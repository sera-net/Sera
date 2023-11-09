using System;
using System.Linq;
using System.Reflection;
using Sera.Core.Impls.Ser;
using Sera.Runtime.Emit.Deps;
using Sera.Runtime.Utils;
using Sera.Runtime.Utils.Internal;

namespace Sera.Runtime.Emit.Ser.Jobs;

internal class _Tuples(bool IsValueTuple, Type[] ItemTypes) : _Base
{
    private Type EmitType { get; set; } = null!;
    private Type RuntimeType { get; set; } = null!;
    protected int TupleSize { get; private set; }

    protected int Size => Math.Min(TupleSize, 8);

    public override void Init(EmitStub stub, EmitMeta target)
    {
        TupleSize = GetTupleSize(target.Type);
    }

    public override EmitTransform[] CollectTransforms(EmitStub stub, EmitMeta target)
        => target.IsValueType ? EmitTransform.EmptyTransforms : ReferenceTypeTransforms;

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
                ? NullableClassImplTransforms
                : EmitTransform.EmptyTransforms;
            return new DepMeta(new(TypeMetas.GetTypeMeta(t, item_nullable), target.Styles.TakeFormats()),
                transforms, KeepRaw: i == 7);
        }).ToArray();
    }

    public override Type GetEmitType(EmitStub stub, EmitMeta target, EmitDeps deps)
    {
        var generics = ItemTypes
            .Concat(ItemTypes.Select((t, i) =>
                i == 7 ? deps.Get(i).MakeSerTupleWrapper(t) : deps.Get(i).MakeSerWrapper(t)))
            .ToArray();
        if (Size == 8)
        {
            return EmitType = IsValueTuple
                ? typeof(TupleRestValueImpl<,,,,,,,,,,,,,,,>).MakeGenericType(generics)
                : typeof(TupleRestClassImpl<,,,,,,,,,,,,,,,>).MakeGenericType(generics);
        }
        else
        {
            var impl = ReflectionUtils.TupleSerImpls[Size];
            return EmitType = impl.MakeGenericType(generics);
        }
    }

    public override Type GetRuntimeType(EmitStub stub, EmitMeta target, RuntimeDeps deps)
    {
        var generics = ItemTypes
            .Concat(ItemTypes.Select((t, i) =>
                i == 7 ? deps.Get(i).MakeSerTupleWrapper(t) : deps.Get(i).MakeSerWrapper(t)))
            .ToArray();
        if (Size == 8)
        {
            return RuntimeType = IsValueTuple
                ? typeof(TupleRestValueImpl<,,,,,,,,,,,,,,,>).MakeGenericType(generics)
                : typeof(TupleRestClassImpl<,,,,,,,,,,,,,,,>).MakeGenericType(generics);
        }
        else
        {
            var impl = ReflectionUtils.TupleSerImpls[Size];
            return RuntimeType = impl.MakeGenericType(generics);
        }
    }

    public override void Emit(EmitStub stub, EmitMeta target, EmitDeps deps) { }

    public override object CreateInst(EmitStub stub, EmitMeta target, RuntimeDeps deps)
    {
        var wrappers = ItemTypes.Select((t, i) =>
                i == 7 ? deps.Get(i).MakeSerTupleWrapper(t) : deps.Get(i).MakeSerWrapper(t))
            .ToArray();
        if (Size == 8)
        {
            var ctor = RuntimeType.GetConstructor(BindingFlags.Public | BindingFlags.Instance,
                wrappers.Append(typeof(int)).ToArray())!;
            var inst = ctor.Invoke(ItemTypes.Select(_ => (object?)null).Append(TupleSize).ToArray());
            return inst;
        }
        else
        {
            var ctor = RuntimeType.GetConstructor(BindingFlags.Public | BindingFlags.Instance,
                wrappers)!;
            var inst = ctor.Invoke(ItemTypes.Select(_ => (object?)null).ToArray());
            return inst;
        }
    }
}

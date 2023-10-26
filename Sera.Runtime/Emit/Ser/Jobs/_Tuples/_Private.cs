using System;
using System.Linq;
using Sera.Core.Impls.Tuples;
using Sera.Runtime.Emit.Deps;
using Sera.Runtime.Utils;
using BindingFlags = System.Reflection.BindingFlags;

namespace Sera.Runtime.Emit.Ser.Jobs._Tuples;

internal class _Private(bool IsValueTuple, Type[] ItemTypes) : _Tuples(IsValueTuple, ItemTypes)
{
    public Type BaseType { get; set; } = null!;
    public Type[] ItemTypes { get; set; } = null!;
    
    public override void Init(EmitStub stub, EmitMeta target)
    {
        base.Init(stub, target);

        ItemTypes = target.Type.GetGenericArguments();
        var type = Size == 8
            ? IsValueTuple
                ? typeof(ValueTupleRestSerializeImplBase<,,,,,,,>)
                : typeof(TupleRestSerializeImplBase<,,,,,,,>)
            : IsValueTuple
                ? ReflectionUtils.ValueTupleSerBaseImpls[Size]
                : ReflectionUtils.ClassTupleSerBaseImpls[Size];

        BaseType = type.MakeGenericType(ItemTypes);
    }

    public override EmitTransform[] CollectTransforms(EmitStub stub, EmitMeta target)
    {
        var wrapper = IsValueTuple
            ? ReflectionUtils.ValueTupleSerImplWrappers[Size]
            : ReflectionUtils.ClassTupleSerImplWrappers[Size];
        if (target.IsValueType || Size == 8) return new EmitTransform[] { wrapper };
        else return new EmitTransform[] { wrapper, new Transforms._ReferenceTypeWrapperSerializeImpl() };
    }

    public override Type GetEmitType(EmitStub stub, EmitMeta target, DepItem[] deps)
        => BaseType;

    public override Type GetRuntimeType(EmitStub stub, EmitMeta target, DepItem[] deps)
        => BaseType;

    public override Type GetEmitPlaceholderType(EmitStub stub, EmitMeta target)
        => BaseType;

    public override Type GetRuntimePlaceholderType(EmitStub stub, EmitMeta target)
        => BaseType;

    public override void Emit(EmitStub stub, EmitMeta target, EmitDeps deps) { }

    public override object CreateInst(EmitStub stub, EmitMeta target, RuntimeDeps deps)
    {
        var item_types = ItemTypes.Select((t, i) =>
        {
            var dep = deps.Get(i);
            var wrapper = i == 7 ? dep.MakeSeqSerializerReceiverWrapper(t) : dep.MakeSerializeWrapper(t);
            return wrapper;
        }).ToArray();

        var generics = ItemTypes.Concat(item_types).ToArray();

        var type = Size == 8
            ? IsValueTuple
                ? typeof(ValueTupleRestSerializeImpl<,,,,,,,,,,,,,,,>)
                : typeof(TupleRestSerializeImpl<,,,,,,,,,,,,,,,>)
            : IsValueTuple
                ? ReflectionUtils.ValueTupleSerImpls[Size]
                : ReflectionUtils.ClassTupleSerImpls[Size];

        var applied_type = type.MakeGenericType(generics);

        var ctor_types = ItemTypes.Length == 8 ? item_types.Append(typeof(nuint)).ToArray() : item_types;

        var ctor = applied_type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, ctor_types)!;

        var ctor_args = new object?[ItemTypes.Length == 8 ? 9 : ItemTypes.Length];
        if (ItemTypes.Length == 8) ctor_args[8] = (nuint)TupleSize;

        var inst = ctor.Invoke(ctor_args);
        return inst;
    }
}

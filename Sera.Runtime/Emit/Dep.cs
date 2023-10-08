using System;
using System.Collections.Generic;
using System.Linq;
using Sera.Runtime.Emit.Transform;

namespace Sera.Runtime.Emit.Deps;

internal readonly record struct DepMeta(EmitMeta Meta, EmitTransform[] Transforms)
{
    public bool Equals(DepMeta other)
    {
        return Meta.Equals(other.Meta) && Transforms.SequenceEqual(other.Transforms);
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(Meta);
        foreach (var transform in Transforms)
        {
            hash.Add(transform);
        }
        return hash.ToHashCode();
    }
}

internal record struct DepMetaGroup(DepMeta DepMeta, int[] RawIndexes);

internal class DepItem(EmitStub Stub, DepMeta Meta)
{
    public EmitStub Stub { get; } = Stub;
    public DepMeta Meta { get; } = Meta;
    public EmitMeta Target => Meta.Meta;

    public bool UsePlaceholder { get; set; }

    #region Forward

    public bool EmitTypeIsTypeBuilder => Stub.EmitTypeIsTypeBuilder;

    #endregion

    #region Raw

    public Type? RawEmitType => UsePlaceholder ? Stub.RawEmitPlaceholderType : Stub.RawEmitType;
    public Type? RawRuntimeType => UsePlaceholder ? Stub.RawRuntimePlaceholderType : Stub.RawRuntimeType;

    public object? RawInst => Stub.RawInst;

    #endregion

    #region Transforms

    private EmitTransform[] Transforms => Meta.Transforms;
    private int TransformsCount => Stub.Transforms.Length + Transforms.Length;
    private bool HasTransforms => Stub.Transforms.Length > 0 || Transforms.Length > 0;
    private IEnumerable<EmitTransform> AllTransforms => Stub.Transforms.Concat(Transforms);

    public Type[] TransformedEmitType { get; private set; } = Array.Empty<Type>();
    public Type[] TransformedRuntimeType { get; private set; } = Array.Empty<Type>();
    public object[] TransformedInst { get; private set; } = Array.Empty<object>();

    public Type ApplyTypeTransformOnEmitType()
    {
        TransformedEmitType = ApplyTypeTransform(RawEmitType!);
        return TransformedEmitType[^1];
    }

    public Type ApplyTypeTransformOnRuntimeType()
    {
        TransformedRuntimeType = ApplyTypeTransform(RawRuntimeType!);
        return TransformedRuntimeType[^1];
    }

    public object ApplyTypeTransformOnInst()
    {
        TransformedInst = ApplyInstTransform(TransformedRuntimeType, RawInst!);
        return TransformedInst[^1];
    }

    private Type[] ApplyTypeTransform(Type type)
    {
        if (!HasTransforms) return new[] { type };
        var arr = new Type[TransformsCount + 1];
        arr[0] = type;
        foreach (var (transform, i) in AllTransforms.Select((a, i) => (a, i)))
        {
            arr[i + 1] = transform.TransformType(Target, arr[i]);
        }
        return arr;
    }

    private object[] ApplyInstTransform(Type[] types, object inst)
    {
        if (!HasTransforms) return new[] { inst };
        var arr = new object[TransformsCount + 1];
        arr[0] = inst;
        foreach (var (transform, i) in AllTransforms.Select((a, i) => (a, i)))
        {
            arr[i + 1] = transform.TransformInst(Target, types[i + 1], types[i], arr[i]);
        }
        return arr;
    }

    #endregion
}

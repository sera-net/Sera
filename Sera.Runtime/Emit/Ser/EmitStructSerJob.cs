﻿using System;
using System.Linq;
using System.Reflection;
using Sera.Runtime.Emit.Deps;
using Sera.Runtime.Emit.Transform;
using Sera.Runtime.Utils;

namespace Sera.Runtime.Emit.Ser;

internal abstract record EmitStructSerJob(StructMember[] Members) : EmitSerJob
{
    private readonly EmitTransform[] ReferenceTypeTransforms =
        { new ReferenceTypeWrapperSerializeImplEmitTransform() };

    public override EmitTransform[] CollectTransforms(EmitStub stub, EmitMeta target)
    {
        if (target.IsValueType) return Array.Empty<EmitTransform>();
        else return ReferenceTypeTransforms;
    }

    public override DepMeta[] CollectDeps(EmitStub stub, EmitMeta target)
    {
        return Members.AsParallel()
            .Select(m =>
            {
                var null_meta = m.Kind switch
                {
                    PropertyOrField.Property => TypeMetas.GetNullabilityMeta(m.Property!),
                    PropertyOrField.Field => TypeMetas.GetNullabilityMeta(m.Field!),
                    _ => throw new ArgumentOutOfRangeException()
                };
                var transforms = !m.Type.IsValueType && null_meta is not
                    { NullabilityInfo.ReadState: NullabilityState.NotNull }
                    ? SerializeEmitProvider.NullableReferenceTypeTransforms
                    : EmitTransform.EmptyTransforms;
                return new DepMeta(new(TypeMetas.GetTypeMeta(m.Type, null_meta), EmitData.Default), transforms);
            }).ToArray();
    }
}

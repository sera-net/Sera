using System;
using System.Runtime.CompilerServices;

namespace Sera.Core.Impls.Ser;

public readonly struct EmptyStructImpl<T>(string? StructName) : ISeraVision<T>, IStructSeraVision<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, T value) where V : ASeraVisitor<R>
        => visitor.VStruct(this, value.NotNull());

    public string Name => StructName ?? typeof(T).Name;
    public int Count => 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R AcceptField<R, V>(V visitor, T value, int field) where V : AStructSeraVisitor<R>
        => visitor.VNone();
}

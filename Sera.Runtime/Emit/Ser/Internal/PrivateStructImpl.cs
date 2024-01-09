using System.Runtime.CompilerServices;
using Sera.Core;

namespace Sera.Runtime.Emit.Ser.Internal;

public readonly struct PrivateStructImpl<T> : ISeraVision<T>, IStructSeraVision<T>
{
    private sealed record Data(object meta_key, string name, int field_count);

    private readonly Data data;

    internal PrivateStructImpl(object meta_key, string name, int field_count)
        => data = new(meta_key, name, field_count);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, T value) where V : ASeraVisitor<R>
        => visitor.VStruct(this, value);

    public string? Name => data.name;
    public int Count => data.field_count;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R AcceptField<R, V>(V visitor, ref T value, int field) where V : AStructSeraVisitor<R>
        => Jobs._Struct._Private.Impl<T, R, V>.AcceptField(visitor, ref value, field, data.meta_key);
}

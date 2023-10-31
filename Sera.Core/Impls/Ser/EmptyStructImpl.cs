namespace Sera.Core.Impls.Ser;

public readonly struct EmptyStructImpl<T> : ISeraVision<T>, IStructSeraVision<T>
{
    public R Accept<R, V>(V visitor, T value) where V : ASeraVisitor<R>
        => visitor.VStruct(this, value);

    public string Name => typeof(T).Name;
    public int Count => 0;

    public R AcceptField<R, V>(V visitor, T value, int field) where V : AStructSeraVisitor<R>
        => visitor.VNone();
}

namespace Sera.Core;

public interface ISeraVision<in T>
{
    public R Accept<R, V>(V visitor, T value) where V : ASeraVisitor<R>;
}

public interface ITupleSeraVision<in T>
{
    public int Size { get; }

    public R AcceptItem<R, V>(V visitor, T value, int index) where V : ATupleSeraVisitor<R>;
}

public interface ISeqSeraVision
{
    public int? Count { get; }

    public bool HasNext { get; }

    public R AcceptNext<R, V>(V visitor) where V : ASeqSeraVisitor<R>;
}

public interface IMapSeraVision
{
    public int? Count { get; }

    public bool HasNext { get; }

    public R AcceptNext<R, V>(V visitor) where V : AMapSeraVisitor<R>;
}

public interface IStructSeraVision<in T>
{
    public string Name { get; }

    public int Count { get; }

    public R AcceptField<R, V>(V visitor, T value, int field) where V : AStructSeraVisitor<R>;
}

public interface IUnionSeraVision<in T>
{
    public string Name { get; }

    public R Accept<R, V>(V visitor, T value) where V : AUnionSeraVisitor<R>;
}

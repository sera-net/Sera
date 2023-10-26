namespace Sera.Core;

public interface ITypeVision<in T>
{
    public R Accept<R, V>(V visitor, T value) where V : ATypeVisitor<R>;
}

public interface ITupleTypeVision<in T>
{
    public int Size { get; }

    public R AcceptItem<R, V>(V visitor, T value, int index) where V : ATupleTypeVisitor<R>;
}

public interface ISeqTypeVision
{
    public int? Count { get; }

    public bool HasNext { get; }

    public R AcceptNext<R, V>(V visitor) where V : ASeqTypeVisitor<R>;
}

public interface IMapTypeVision
{
    public int? Count { get; }

    public bool HasNext { get; }

    public R AcceptNext<R, V>(V visitor) where V : AMapTypeVisitor<R>;
}

public interface IStructTypeVision<in T>
{
    public string Name { get; }

    public int Count { get; }

    public R AcceptField<R, V>(V visitor, T value, int field) where V : AStructTypeVisitor<R>;
}

public interface IUnionTypeVision<in T>
{
    public string Name { get; }

    public R Accept<R, V>(V visitor, T value) where V : AUnionTypeVisitor<R>;
}

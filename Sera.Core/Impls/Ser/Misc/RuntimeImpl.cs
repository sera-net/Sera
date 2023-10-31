namespace Sera.Core.Impls.Ser;

public readonly struct RuntimeImpl<T, P>(P provider) : ISeraVision<T>
    where P : IRuntimeProvider<ISeraVision<object?>>
{
    public R Accept<R, V>(V visitor, T value) where V : ASeraVisitor<R>
        => provider.Get().Accept<R, V>(visitor, value); 
}

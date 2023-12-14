using System.Runtime.CompilerServices;

namespace Sera.Core.Builders.De;

public readonly record struct DeBuilder<B>(B Target)
    where B : IDeBuilder
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public DeBuilder<B, T> Deserialize<T>()
        => new(Target);
}

public readonly record struct From<B>(B Target);

public readonly record struct Static<B>(B Target)
{
    public From<Static<B>> From
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(this);
    }
}

public readonly record struct DeBuilder<B, T>(B Target)
    where B : IDeBuilder
{
    public Static<DeBuilder<B, T>> Static
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(this);
    }

    public From<DeBuilder<B, T>> From
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public DeBuilder<B, T, C> Use<C>(C colion) where C : ISeraColion<T>
        => new(Target, colion);
}

public readonly record struct DeBuilder<B, T, C>(B Target, C Colion)
    where B : IDeBuilder where C : ISeraColion<T>
{
    public Static<DeBuilder<B, T, C>> Static
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(this);
    }

    public From<DeBuilder<B, T, C>> From
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(this);
    }
}

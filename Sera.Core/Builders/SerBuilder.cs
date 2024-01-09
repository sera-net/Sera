using System.Runtime.CompilerServices;

namespace Sera.Core.Builders.Ser;

public readonly record struct SerBuilder<B>(B Target)
    where B : ISerBuilder
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SerBuilder<B, T> Serialize<T>(T value)
        => new(Target, value);
}

public readonly record struct To<B>(B Target);

public readonly record struct Static<B>(B Target)
{
    public To<Static<B>> To
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(this);
    }
}

public readonly record struct SerBuilder<B, T>(B Target, T Value)
    where B : ISerBuilder
{
    public Static<SerBuilder<B, T>> Static
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(this);
    }
    public To<SerBuilder<B, T>> To
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SerBuilder<B, T, V> Use<V>(V vision) where V : ISeraVision<T>
        => new(Target, Value, vision);
}

public readonly record struct SerBuilder<B, T, V>(B Target, T Value, V Vision)
    where B : ISerBuilder where V : ISeraVision<T>
{
    public Static<SerBuilder<B, T, V>> Static
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(this);
    }
    public To<SerBuilder<B, T, V>> To
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(this);
    }
}

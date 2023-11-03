using System;
using System.Runtime.CompilerServices;

namespace Sera.Core.Builders;

public readonly record struct To<B>(B Target);

public readonly record struct Static<B>(B Target)
{
    public To<Static<B>> To => new(this);
}

public readonly record struct SerBuilder<B, T>(B Target, T Value)
    where B : ISerBuilder
{
    public Static<SerBuilder<B, T>> Static => new(this);
    public To<SerBuilder<B, T>> To => new(this);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SerBuilder<B, T, V> Use<V>(V vision) where V : ISeraVision<T>
        => new(Target, Value, vision);
}

public readonly record struct SerBuilder<B, T, V>(B Target, T Value, V Vision)
    where B : ISerBuilder where V : ISeraVision<T>
{
    public Static<SerBuilder<B, T, V>> Static => new(this);
    public To<SerBuilder<B, T, V>> To => new(this);
}

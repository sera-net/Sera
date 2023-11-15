using System;
using System.Runtime.CompilerServices;
using Sera.Utils;

namespace Sera.Core.Impls.De;

public readonly struct LazyAsmable<D, A> : ISeraAsmable<A>, IRef<A>
    where D : ISeraAsmable<A>
{
    [AssocType("A")]
    public abstract class _A(A type);
    
    private readonly Lazy<Box<A>> lazy;

    public LazyAsmable(D dep)
    {
        lazy = new(() => new(dep.Asmer()));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref A GetRef() => ref lazy.Value.Value;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public A Asmer() => lazy.Value.Value;
}

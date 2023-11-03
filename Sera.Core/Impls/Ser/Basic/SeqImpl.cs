using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Sera.Core.Impls.Ser;

#region Generic

#region IEnumerable

public readonly struct SeqIEnumerableImpl<T, I, D>(D dep) : ISeraVision<T>
    where T : IEnumerable<I>
    where D : ISeraVision<I>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, T value) where V : ASeraVisitor<R>
        => visitor.VSeq<Wrapper, T, I>(new(new(value, dep)));

    public readonly struct Wrapper(Impl Impl) : ISeqSeraVision
    {
        public int? Count => null;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() => Impl.MoveNext();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R AcceptNext<R, V>(V visitor) where V : ASeqSeraVisitor<R>
            => Impl.AcceptNext<R, V>(visitor);
    }

    public sealed class Impl(T value, D dep) : ISeqSeraVision
    {
        private readonly IEnumerator<I> enumerator = value.GetEnumerator();

        public int? Count => null;

        private bool HasNext;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() => HasNext = enumerator.MoveNext();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R AcceptNext<R, V>(V visitor) where V : ASeqSeraVisitor<R>
        {
            if (HasNext) return visitor.VItem(dep, enumerator.Current);
            return visitor.VEnd();
        }
    }
}

#endregion

#region ICollection

public readonly struct SeqICollectionImpl<T, I, D>(D dep) : ISeraVision<T>
    where T : ICollection<I>
    where D : ISeraVision<I>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, T value) where V : ASeraVisitor<R>
        => visitor.VSeq<Wrapper, T, I>(new(new(value, dep)));

    public readonly struct Wrapper(Impl Impl) : ISeqSeraVision
    {
        public int? Count => Impl.Count;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() => Impl.MoveNext();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R AcceptNext<R, V>(V visitor) where V : ASeqSeraVisitor<R>
            => Impl.AcceptNext<R, V>(visitor);
    }

    public sealed class Impl(T value, D dep) : ISeqSeraVision
    {
        private readonly IEnumerator<I> enumerator = value.GetEnumerator();

        public int? Count => value.Count;
        private bool HasNext;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() => HasNext = enumerator.MoveNext();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R AcceptNext<R, V>(V visitor) where V : ASeqSeraVisitor<R>
        {
            if (HasNext) return visitor.VItem(dep, enumerator.Current);
            return visitor.VEnd();
        }
    }
}

#endregion

#region IReadOnlyCollection

public readonly struct SeqIReadOnlyCollectionImpl<T, I, D>(D dep) : ISeraVision<T>
    where T : IReadOnlyCollection<I>
    where D : ISeraVision<I>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, T value) where V : ASeraVisitor<R>
        => visitor.VSeq<Wrapper, T, I>(new(new(value, dep)));

    public readonly struct Wrapper(Impl Impl) : ISeqSeraVision
    {
        public int? Count => Impl.Count;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() => Impl.MoveNext();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R AcceptNext<R, V>(V visitor) where V : ASeqSeraVisitor<R>
            => Impl.AcceptNext<R, V>(visitor);
    }

    public sealed class Impl(T value, D dep) : ISeqSeraVision
    {
        private readonly IEnumerator<I> enumerator = value.GetEnumerator();

        public int? Count => value.Count;
        private bool HasNext;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() => HasNext = enumerator.MoveNext();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R AcceptNext<R, V>(V visitor) where V : ASeqSeraVisitor<R>
        {
            if (HasNext) return visitor.VItem(dep, enumerator.Current);
            return visitor.VEnd();
        }
    }
}

#endregion

#endregion

#region Legacy

#region IEnumerable

public readonly struct SeqIEnumerableLegacyRuntimeImpl<T> : ISeraVision<T>
    where T : IEnumerable
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, T value) where V : ASeraVisitor<R>
        => visitor.VSeq(new Wrapper(new(value, visitor.RuntimeImpl)));

    public readonly struct Wrapper(Impl Impl) : ISeqSeraVision
    {
        public int? Count => null;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() => Impl.MoveNext();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R AcceptNext<R, V>(V visitor) where V : ASeqSeraVisitor<R>
            => Impl.AcceptNext<R, V>(visitor);
    }

    public sealed class Impl(T value, ISeraVision<object?> dep) : ISeqSeraVision
    {
        private readonly IEnumerator enumerator = value.GetEnumerator();
        public int? Count => null;

        private bool HasNext;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() => HasNext = enumerator.MoveNext();

        public R AcceptNext<R, V>(V visitor) where V : ASeqSeraVisitor<R>
        {
            if (HasNext) return visitor.VItem(dep, enumerator.Current);
            return visitor.VEnd();
        }
    }
}

#endregion

#region ICollection

public readonly struct SeqICollectionLegacyRuntimeImpl<T> : ISeraVision<T>
    where T : ICollection
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, T value) where V : ASeraVisitor<R>
        => visitor.VSeq(new Wrapper(new(value, visitor.RuntimeImpl)));

    public readonly struct Wrapper(Impl Impl) : ISeqSeraVision
    {
        public int? Count => Impl.Count;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() => Impl.MoveNext();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R AcceptNext<R, V>(V visitor) where V : ASeqSeraVisitor<R>
            => Impl.AcceptNext<R, V>(visitor);
    }

    public sealed class Impl(T value, ISeraVision<object?> dep) : ISeqSeraVision
    {
        private readonly IEnumerator enumerator = value.GetEnumerator();
        public int? Count => value.Count;

        private bool HasNext;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() => HasNext = enumerator.MoveNext();

        public R AcceptNext<R, V>(V visitor) where V : ASeqSeraVisitor<R>
        {
            if (HasNext) return visitor.VItem(dep, enumerator.Current);
            return visitor.VEnd();
        }
    }
}

#endregion

#endregion

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Sera.Core.Impls.Ser;

#region Gerneric

#region IEnumerable

public readonly struct MapIEnumerableImpl<T, IK, IV, DK, DV>(DK dk, DV dv) : ISeraVision<T>
    where T : IEnumerable<KeyValuePair<IK, IV>>
    where DK : ISeraVision<IK>
    where DV : ISeraVision<IV>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, T value) where V : ASeraVisitor<R>
        => visitor.VMap<Wrapper, T, IK, IV>(new(new(value, dk, dv)));

    public readonly struct Wrapper(Impl Impl) : IMapSeraVision
    {
        public int? Count => null;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() => Impl.MoveNext();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R AcceptNext<R, V>(V visitor) where V : AMapSeraVisitor<R>
            => Impl.AcceptNext<R, V>(visitor);
    }

    public sealed class Impl(T value, DK dk, DV dv) : IMapSeraVision
    {
        private readonly IEnumerator<KeyValuePair<IK, IV>> enumerator = value.GetEnumerator();

        public int? Count => null;
        private bool HasNext;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() => HasNext = enumerator.MoveNext();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R AcceptNext<R, V>(V visitor) where V : AMapSeraVisitor<R>
        {
            if (!HasNext) return visitor.VEnd();
            var current = enumerator.Current;
            return visitor.VEntry(dk, dv, current.Key, current.Value);
        }
    }
}

#endregion

#region ICollection

public readonly struct MapICollectionImpl<T, IK, IV, DK, DV>(DK dk, DV dv) : ISeraVision<T>
    where T : ICollection<KeyValuePair<IK, IV>>
    where DK : ISeraVision<IK>
    where DV : ISeraVision<IV>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, T value) where V : ASeraVisitor<R>
        => visitor.VMap<Wrapper, T, IK, IV>(new(new(value, dk, dv)));

    public readonly struct Wrapper(Impl Impl) : IMapSeraVision
    {
        public int? Count => Impl.Count;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() => Impl.MoveNext();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R AcceptNext<R, V>(V visitor) where V : AMapSeraVisitor<R>
            => Impl.AcceptNext<R, V>(visitor);
    }

    public sealed class Impl(T value, DK dk, DV dv) : IMapSeraVision
    {
        private readonly IEnumerator<KeyValuePair<IK, IV>> enumerator = value.GetEnumerator();

        public int? Count => value.Count;
        private bool HasNext;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() => HasNext = enumerator.MoveNext();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R AcceptNext<R, V>(V visitor) where V : AMapSeraVisitor<R>
        {
            if (!HasNext) return visitor.VEnd();
            var current = enumerator.Current;
            return visitor.VEntry(dk, dv, current.Key, current.Value);
        }
    }
}

#endregion

#region IReadOnlyCollection

public readonly struct MapIReadOnlyCollectionImpl<T, IK, IV, DK, DV>(DK dk, DV dv) : ISeraVision<T>
    where T : IReadOnlyCollection<KeyValuePair<IK, IV>>
    where DK : ISeraVision<IK>
    where DV : ISeraVision<IV>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, T value) where V : ASeraVisitor<R>
        => visitor.VMap<Wrapper, T, IK, IV>(new(new(value, dk, dv)));

    public readonly struct Wrapper(Impl Impl) : IMapSeraVision
    {
        public int? Count => Impl.Count;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() => Impl.MoveNext();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R AcceptNext<R, V>(V visitor) where V : AMapSeraVisitor<R>
            => Impl.AcceptNext<R, V>(visitor);
    }

    public sealed class Impl(T value, DK dk, DV dv) : IMapSeraVision
    {
        private readonly IEnumerator<KeyValuePair<IK, IV>> enumerator = value.GetEnumerator();

        public int? Count => value.Count;
        private bool HasNext;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() => HasNext = enumerator.MoveNext();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R AcceptNext<R, V>(V visitor) where V : AMapSeraVisitor<R>
        {
            if (!HasNext) return visitor.VEnd();
            var current = enumerator.Current;
            return visitor.VEntry(dk, dv, current.Key, current.Value);
        }
    }
}

#endregion

#endregion

#region Legacy

#region IDictionary

public readonly struct MapIDictionaryLegacyRuntimeImpl<T> : ISeraVision<T>
    where T : IDictionary
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, T value) where V : ASeraVisitor<R>
        => visitor.VMap<Wrapper, T>(new(new(value, visitor.RuntimeImpl)));

    public readonly struct Wrapper(Impl Impl) : IMapSeraVision
    {
        public int? Count => Impl.Count;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() => Impl.MoveNext();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R AcceptNext<R, V>(V visitor) where V : AMapSeraVisitor<R>
            => Impl.AcceptNext<R, V>(visitor);
    }

    public sealed class Impl(T value, ISeraVision<object?> dep) : IMapSeraVision
    {
        private readonly IDictionaryEnumerator enumerator = value.GetEnumerator();

        public int? Count => value.Count;
        private bool HasNext;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() => HasNext = enumerator.MoveNext();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R AcceptNext<R, V>(V visitor) where V : AMapSeraVisitor<R>
        {
            if (!HasNext) return visitor.VEnd();
            var key = enumerator.Key;
            var value = enumerator.Value;
            return visitor.VEntry(dep, dep, key, value);
        }
    }
}

#endregion

#endregion

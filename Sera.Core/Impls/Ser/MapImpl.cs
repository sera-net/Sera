using System.Collections.Generic;

namespace Sera.Core.Impls.Ser;

#region IEnumerable

public readonly struct MapIEnumerableImpl<T, IK, IV, DK, DV>(DK dk, DV dv) : ISeraVision<T>
    where T : IEnumerable<KeyValuePair<IK, IV>>
    where DK : ISeraVision<IK>
    where DV : ISeraVision<IV>
{
    public R Accept<R, V>(V visitor, T value) where V : ASeraVisitor<R>
        => visitor.VMap<Wrapper, T, IK, IV>(new(new(value, dk, dv)));

    public readonly struct Wrapper(Impl Impl) : IMapSeraVision
    {
        public int? Count => null;
        public bool HasNext => Impl.HasNext;

        public R AcceptNext<R, V>(V visitor) where V : AMapSeraVisitor<R>
            => Impl.AcceptNext<R, V>(visitor);
    }

    public sealed class Impl : IMapSeraVision
    {
        private readonly DK dk;
        private readonly DV dv;
        private readonly IEnumerator<KeyValuePair<IK, IV>> enumerator;

        public Impl(T value, DK dk, DV dv)
        {
            this.dk = dk;
            this.dv = dv;
            enumerator = value.GetEnumerator();
            HasNext = enumerator.MoveNext();
        }

        public int? Count => null;
        public bool HasNext { get; set; }

        public R AcceptNext<R, V>(V visitor) where V : AMapSeraVisitor<R>
        {
            if (HasNext)
            {
                var current = enumerator.Current;
                var r = visitor.VEntry(dk, dv, current.Key, current.Value);
                HasNext = enumerator.MoveNext();
                return r;
            }
            return visitor.VEnd();
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
    public R Accept<R, V>(V visitor, T value) where V : ASeraVisitor<R>
        => visitor.VMap<Wrapper, T, IK, IV>(new(new(value, dk, dv)));

    public readonly struct Wrapper(Impl Impl) : IMapSeraVision
    {
        public int? Count => Impl.Count;
        public bool HasNext => Impl.HasNext;

        public R AcceptNext<R, V>(V visitor) where V : AMapSeraVisitor<R>
            => Impl.AcceptNext<R, V>(visitor);
    }

    public sealed class Impl : IMapSeraVision
    {
        private readonly DK dk;
        private readonly DV dv;
        private readonly IEnumerator<KeyValuePair<IK, IV>> enumerator;

        public Impl(T value, DK dk, DV dv)
        {
            this.dk = dk;
            this.dv = dv;
            Count = value.Count;
            enumerator = value.GetEnumerator();
            HasNext = enumerator.MoveNext();
        }

        public int? Count { get; }
        public bool HasNext { get; set; }

        public R AcceptNext<R, V>(V visitor) where V : AMapSeraVisitor<R>
        {
            if (HasNext)
            {
                var current = enumerator.Current;
                var r = visitor.VEntry(dk, dv, current.Key, current.Value);
                HasNext = enumerator.MoveNext();
                return r;
            }
            return visitor.VEnd();
        }
    }
}

#endregion

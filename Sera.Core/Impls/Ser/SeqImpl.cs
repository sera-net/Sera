using System.Collections.Generic;

namespace Sera.Core.Impls.Ser;

#region IEnumerable

public readonly struct SeqIEnumerableImpl<T, I, D>(D dep) : ITypeVision<T>
    where T : IEnumerable<I>
    where D : ITypeVision<I>
{
    public R Accept<R, V>(V visitor, T value) where V : ATypeVisitor<R>
        => visitor.VSeq<Wrapper, T, I>(new(new(value, dep)));

    public readonly struct Wrapper(Impl Impl) : ISeqTypeVision
    {
        public int? Count => null;
        public bool HasNext => Impl.HasNext;

        public R AcceptNext<R, V>(V visitor) where V : ASeqTypeVisitor<R>
            => Impl.AcceptNext<R, V>(visitor);
    }

    public sealed class Impl : ISeqTypeVision
    {
        private readonly D dep;
        private readonly IEnumerator<I> enumerator;

        public Impl(T value, D dep)
        {
            this.dep = dep;
            enumerator = value.GetEnumerator();
            HasNext = enumerator.MoveNext();
        }

        public int? Count => null;
        public bool HasNext { get; set; }

        public R AcceptNext<R, V>(V visitor) where V : ASeqTypeVisitor<R>
        {
            if (HasNext)
            {
                var r = visitor.VItem(dep, enumerator.Current);
                HasNext = enumerator.MoveNext();
                return r;
            }
            return visitor.VEnd();
        }
    }
}

#endregion

#region ICollection

public readonly struct SeqICollectionImpl<T, I, D>(D dep) : ITypeVision<T>
    where T : ICollection<I>
    where D : ITypeVision<I>
{
    public R Accept<R, V>(V visitor, T value) where V : ATypeVisitor<R>
        => visitor.VSeq<Wrapper, T, I>(new(new(value, dep)));

    public readonly struct Wrapper(Impl Impl) : ISeqTypeVision
    {
        public int? Count => Impl.Count;
        public bool HasNext => Impl.HasNext;

        public R AcceptNext<R, V>(V visitor) where V : ASeqTypeVisitor<R>
            => Impl.AcceptNext<R, V>(visitor);
    }

    public sealed class Impl : ISeqTypeVision
    {
        private readonly D dep;
        private readonly IEnumerator<I> enumerator;

        public Impl(T value, D dep)
        {
            this.dep = dep;
            Count = value.Count;
            enumerator = value.GetEnumerator();
            HasNext = enumerator.MoveNext();
        }

        public int? Count { get; }
        public bool HasNext { get; set; }

        public R AcceptNext<R, V>(V visitor) where V : ASeqTypeVisitor<R>
        {
            if (HasNext)
            {
                var r = visitor.VItem(dep, enumerator.Current);
                HasNext = enumerator.MoveNext();
                return r;
            }
            return visitor.VEnd();
        }
    }
}

#endregion

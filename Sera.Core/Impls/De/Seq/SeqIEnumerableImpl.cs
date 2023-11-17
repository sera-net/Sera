using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sera.Utils;

namespace Sera.Core.Impls.De;

public readonly struct SeqIEnumerableImpl<I, D>(D d) : ISeraColion<IEnumerable<I>>
    where D : ISeraColion<I>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<IEnumerable<I>>? t = null) where C : ISeraColctor<IEnumerable<I>, R>
        => colctor.CSeq(new SeqListImpl<I, D>(d), new List2IEnumerableMapper(), new Type<List<I>>());
    
    private readonly struct List2IEnumerableMapper : ISeraMapper<List<I>, IEnumerable<I>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<I> Map(List<I> value, InType<IEnumerable<I>>? u = null) => value;
    }
}

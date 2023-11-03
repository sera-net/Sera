using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Sera.Core;

internal static class Utils
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T NotNull<T>(this T value)
    {
        if (value == null) throw new NullReferenceException();
        return value;
    }
    
    public static bool DictEq<K, V>(this Dictionary<K, V> self, Dictionary<K, V> other) where K : notnull
        => self.Count == other.Count && !self
            .AsParallel()
            .Except(other.AsParallel())
            .Any();

    public static int DictHash<K, V>(this Dictionary<K, V> self) where K : notnull
        => self.AsParallel()
            .OrderBy(static a => a.Key)
            .Aggregate(new HashCode(), static (code, pair) =>
            {
                code.Add(pair.Key);
                code.Add(pair.Value);
                return code;
            })
            .ToHashCode();

    public static int SeqHash<T>(this IEnumerable<T> self)
        => self.AsParallel()
            .AsOrdered()
            .Aggregate(new HashCode(), static (code, item) =>
            {
                code.Add(item);
                return code;
            })
            .ToHashCode();
}

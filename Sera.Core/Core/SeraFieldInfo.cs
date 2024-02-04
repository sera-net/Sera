using System;
using System.Collections.Frozen;
using System.Linq;

namespace Sera.Core;

public readonly record struct SeraFieldInfo(string Name, long Key);

public readonly record struct SeraFieldInfos
{
    public SeraFieldInfo[] Infos { get; }
    public FrozenDictionary<string, int> NameToIndex { get; }
    public FrozenDictionary<long, int> KeyToIndex { get; }

    public int Count => Infos.Length;

    public SeraFieldInfos(SeraFieldInfo[] infos)
    {
        Infos = infos;
        NameToIndex = infos.Select((a, b) => (a, b))
            .ToFrozenDictionary(a => a.a.Name, a => a.b);
        KeyToIndex = infos.Select((a, b) => (a, b))
            .ToFrozenDictionary(a => a.a.Key, a => a.b);
    }

    public static SeraFieldInfos Empty { get; } = new(Array.Empty<SeraFieldInfo>());

    public bool TryGet(string name, out (SeraFieldInfo info, int index) info)
    {
        if (!NameToIndex.TryGetValue(name, out var i))
        {
            info = default;
            return false;
        }
        else
        {
            info = (Infos[i], i);
            return true;
        }
    }

    public bool TryGet(long key, out (SeraFieldInfo info, int index) info)
    {
        if (!KeyToIndex.TryGetValue(key, out var i))
        {
            info = default;
            return false;
        }
        else
        {
            info = (Infos[i], i);
            return true;
        }
    }

    public bool Equals(SeraFieldInfos other) => Infos.SequenceEqual(other.Infos);

    public override int GetHashCode() => Infos.SeqHash();
}

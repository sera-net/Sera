using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace Sera.Core;

public record AnyStruct(string? Name, Any[] values, SeraFieldInfos infos) :
    IReadOnlyDictionary<int, Any>,
    IReadOnlyDictionary<string, Any>
{
    private Any[] values { get; } = values;
    private SeraFieldInfos infos { get; } = infos;

    public int Count => values.Length;

    int IReadOnlyCollection<KeyValuePair<int, Any>>.Count => Count;

    public bool ContainsKey(int key) => key >= 0 && key < Count;

    public bool TryGet(int key, out SeraFieldInfo info, out Any value)
    {
        if (!ContainsKey(key))
        {
            info = default;
            value = default;
            return false;
        }
        info = infos.Infos[key];
        value = values[key];
        return true;
    }

    public bool TryGetValue(int key, out Any value)
    {
        if (ContainsKey(key))
        {
            value = values[key];
            return true;
        }
        else
        {
            value = default;
            return false;
        }
    }

    public Any this[int key] => values[key];

    public bool ContainsKey(string key) => infos.NameToIndex.ContainsKey(key);

    public bool TryGetValue(string key, out Any value)
    {
        if (infos.NameToIndex.TryGetValue(key, out var index))
        {
            value = values[index];
            return true;
        }
        else
        {
            value = default;
            return false;
        }
    }

    public Any this[string key] => values[infos.NameToIndex[key]];

    IEnumerable<string> IReadOnlyDictionary<string, Any>.Keys => Names;
    IEnumerable<Any> IReadOnlyDictionary<string, Any>.Values => Values;
    IEnumerable<int> IReadOnlyDictionary<int, Any>.Keys => Indexes;
    IEnumerable<Any> IReadOnlyDictionary<int, Any>.Values => Values;
    int IReadOnlyCollection<KeyValuePair<string, Any>>.Count => Count;

    public IEnumerable<int> Indexes => Enumerable.Range(0, Count);
    public ImmutableArray<string> Names => infos.NameToIndex.Keys;
    public IEnumerable<Any> Values => values;

    public IEnumerable<(SeraFieldInfo info, Any value)> Fields => GetFields();

    private IEnumerable<(SeraFieldInfo info, Any value)> GetFields()
    {
        var values = this.values;
        var infos = this.infos;
        for (var i = 0; i < values.Length; i++)
        {
            yield return (infos.Infos[i], values[i]);
        }
    }

    IEnumerator<KeyValuePair<string, Any>> IEnumerable<KeyValuePair<string, Any>>.GetEnumerator()
    {
        var values = this.values;
        var infos = this.infos;
        for (var i = 0; i < values.Length; i++)
        {
            yield return new(infos.Infos[i].Name, values[i]);
        }
    }

    IEnumerator<KeyValuePair<int, Any>> IEnumerable<KeyValuePair<int, Any>>.GetEnumerator()
    {
        var values = this.values;
        var infos = this.infos;
        for (var i = 0; i < values.Length; i++)
        {
            yield return new(i, values[i]);
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => Fields.GetEnumerator();

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append(Name ?? "<Anonymous>");
        sb.Append(" { ");
        var first = true;
        for (var i = 0; i < values.Length; i++)
        {
            var value = values[i];
            var info = infos.Infos[i];
            if (first) first = false;
            else sb.Append(", ");
            sb.Append(info.Key);
            sb.Append(" : \"");
            sb.Append(info.Name.Replace("\"", "\\\""));
            sb.Append("\" = ");
            sb.Append(value);
        }
        sb.Append(" }");
        return sb.ToString();
    }

    public virtual bool Equals(AnyStruct? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return values.SequenceEqual(other.values) && infos.Equals(other.infos) && Name == other.Name;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(values.SeqHash(), infos, Name);
    }

    public class Builder(string? Name)
    {
        private readonly List<(string? name, long? key, Any value)> items = new();

        public Builder Add((string? name, long? key, Any value) item)
        {
            items.Add(item);
            return this;
        }

        public Builder Add(string? name, long? key, Any value)
        {
            items.Add((name, key, value));
            return this;
        }

        public AnyStruct Build()
        {
            var values = items.Select(static a => a.value).ToArray();
            var infos = items.Select(static (a, i) => new SeraFieldInfo(a.name ?? $"{i}", a.key ?? i)).ToArray();
            return new(Name, values, new SeraFieldInfos(infos));
        }
    }
}

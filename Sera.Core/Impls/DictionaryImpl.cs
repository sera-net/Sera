using System.Collections.Generic;
using System.Threading.Tasks;
using Sera.Core.De;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

#region Serialize

public record DictionarySerializeImpl<M, K, V, SK, SV>(SK KeySerialize, SV ValueSerialize) : ISerialize<M>,
    IMapSerializerReceiver<M>
    where M : Dictionary<K, V> where SK : ISerialize<K> where SV : ISerialize<V> where K : notnull
{
    public void Write<S>(S serializer, M value, ISeraOptions options) where S : ISerializer
        => serializer.StartMap<K, V, M, DictionarySerializeImpl<M, K, V, SK, SV>>((nuint)value.Count, value, this);

    public void Receive<S>(M value, S serializer) where S : IMapSerializer
    {
        foreach (var (k, v) in value)
        {
            serializer.WriteEntry(k, v, KeySerialize, ValueSerialize);
        }
    }
}

#endregion

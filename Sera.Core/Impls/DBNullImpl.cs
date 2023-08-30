using System;
using System.Threading.Tasks;
using Sera.Core.De;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

public record DBNullImpl : ISerialize<DBNull>, IDeserialize<DBNull>
{
    public static DBNullImpl Instance { get; } = new();

    public void Write<S>(S serializer, in DBNull value, SeraOptions options) where S : ISerializer
        => serializer.WriteNull();

    public ValueTask WriteAsync<S>(S serializer, DBNull value, SeraOptions options) where S : IAsyncSerializer
        => serializer.WriteNullAsync();

    public void Read<D>(D deserializer, out DBNull value, SeraOptions options) where D : IDeserializer
    {
        deserializer.ReadNull();
        value = DBNull.Value;
    }

    public async ValueTask<DBNull> ReadAsync<D>(D deserializer, SeraOptions options) where D : IAsyncDeserializer
    {
        await deserializer.ReadNullAsync();
        return DBNull.Value;
    }
}

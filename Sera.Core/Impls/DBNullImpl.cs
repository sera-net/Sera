﻿using System;
using System.Threading.Tasks;
using Sera.Core.De;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

public struct DBNullImpl : ISerialize<DBNull>, IAsyncSerialize<DBNull>, IDeserialize<DBNull>, IAsyncDeserialize<DBNull>
{
    public static DBNullImpl Instance { get; } = new();

    public void Write<S>(S serializer, DBNull value, ISeraOptions options) where S : ISerializer
        => serializer.WriteUnit();

    public ValueTask WriteAsync<S>(S serializer, DBNull value, ISeraOptions options) where S : IAsyncSerializer
        => serializer.WriteUnitAsync();

    public DBNull Read<D>(D deserializer, ISeraOptions options) where D : IDeserializer
    {
        deserializer.ReadUnit();
        return DBNull.Value;
    }

    public async ValueTask<DBNull> ReadAsync<D>(D deserializer, ISeraOptions options) where D : IAsyncDeserializer
    {
        await deserializer.ReadUnitAsync();
        return DBNull.Value;
    }
}

using System;
using System.Threading.Tasks;
using Sera.Core.De;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

public struct DBNullImpl : ISerialize<DBNull>
{
    public static DBNullImpl Instance { get; } = new();

    public void Write<S>(S serializer, DBNull value, ISeraOptions options) where S : ISerializer
        => serializer.WriteUnit();
}

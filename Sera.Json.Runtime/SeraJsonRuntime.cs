using System.Text;
using Sera.Core.Impls;
using Sera.Json.Builders;
using Sera.Json.Builders.Ser;
using Sera.Json.Ser;
using Sera.Runtime.Emit;
using Sera.Runtime.Utils;

namespace Sera.Json.Runtime;

#region Serializer

#region Utils

public static partial class SeraJsonRuntime
{
    private static ISerialize<T> GetSerialize<T>(this EmitRuntimeProvider rt, SeraJsonOptions options)
    {
        if (options.RootReferenceNullable) return rt.GetMayReferenceNullableSerialize<T>();
        else return rt.GetSerialize<T>();
    }
}

#endregion

#region ToStream

public static partial class SeraJsonRuntime
{
    public static void Serialize<T>(this Builder<ToStream> self, T value)
    {
        var rt = EmitRuntimeProvider.Instance;
        var ser = new JsonSerializer(StreamJsonWriter.Create(self))
        {
            RuntimeProvider = rt,
        };
        rt.GetSerialize<T>(self.Options).Write(ser, value, self.Options);
    }
}

#endregion

#region ToString

public static partial class SeraJsonRuntime
{
    public static string Serialize<T>(this Builder<ToString> self, T value)
    {
        var rt = EmitRuntimeProvider.Instance;
        var builder = new StringBuilder();
        var ser = new JsonSerializer(StringBuilderJsonWriter.Create(self, builder))
        {
            RuntimeProvider = rt,
        };
        rt.GetSerialize<T>(self.Options).Write(ser, value, self.Options);
        return builder.ToString();
    }
}

#endregion

#region ToStringBuilder

public static partial class SeraJsonRuntime
{
    public static void Serialize<T>(this Builder<ToStringBuilder> self, T value)
    {
        var rt = EmitRuntimeProvider.Instance;
        var ser = new JsonSerializer(StringBuilderJsonWriter.Create(self))
        {
            RuntimeProvider = rt,
        };
        rt.GetSerialize<T>(self.Options).Write(ser, value, self.Options);
    }
}

#endregion

#endregion

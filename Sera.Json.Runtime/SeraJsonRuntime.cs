using System.Text;
using Sera.Json.Builders;
using Sera.Json.Builders.Ser;
using Sera.Json.Ser;
using Sera.Runtime;
using Sera.Runtime.Emit;
using Sera.Runtime.Utils;

namespace Sera.Json.Runtime;

#region Serializer

#region Utils

public static partial class SeraJsonRuntime
{
    private static ISerialize<T> GetSerialize<T>(this EmitRuntimeProvider rt, SeraJsonOptions options,
        SeraHints hints)
    {
        if (options.RootReferenceNullable) return rt.GetMayReferenceNullableSerialize<T>(hints);
        else return rt.GetSerialize<T>();
    }
}

#endregion

#region ToStream

public static partial class SeraJsonRuntime
{
    public static void Serialize<T>(this Builder<ToStream> self, T value, SeraHints hints = default)
    {
        var rt = EmitRuntimeProvider.Instance;
        var ser = new JsonSerializer(StreamJsonWriter.Create(self))
        {
            RuntimeProvider = rt,
        };
        rt.GetSerialize<T>(self.Options, hints).Write(ser, value, self.Options);
    }

    public static void Serialize<T, S>(this Builder<ToStream> self, T value, S serialize) where S : ISerialize<T>
    {
        var rt = EmitRuntimeProvider.Instance;
        var ser = new JsonSerializer(StreamJsonWriter.Create(self))
        {
            RuntimeProvider = rt,
        };
        serialize.Write(ser, value, self.Options);
    }
}

#endregion

#region ToString

public static partial class SeraJsonRuntime
{
    public static string Serialize<T>(this Builder<ToString> self, T value, SeraHints hints = default)
    {
        var rt = EmitRuntimeProvider.Instance;
        var builder = new StringBuilder();
        var ser = new JsonSerializer(StringBuilderJsonWriter.Create(self, builder))
        {
            RuntimeProvider = rt,
        };
        rt.GetSerialize<T>(self.Options, hints).Write(ser, value, self.Options);
        return builder.ToString();
    }

    public static string Serialize<T, S>(this Builder<ToString> self, T value, S serialize) where S : ISerialize<T>
    {
        var rt = EmitRuntimeProvider.Instance;
        var builder = new StringBuilder();
        var ser = new JsonSerializer(StringBuilderJsonWriter.Create(self, builder))
        {
            RuntimeProvider = rt,
        };
        serialize.Write(ser, value, self.Options);
        return builder.ToString();
    }
}

#endregion

#region ToStringBuilder

public static partial class SeraJsonRuntime
{
    public static void Serialize<T>(this Builder<ToStringBuilder> self, T value, SeraHints hints = default)
    {
        var rt = EmitRuntimeProvider.Instance;
        var ser = new JsonSerializer(StringBuilderJsonWriter.Create(self))
        {
            RuntimeProvider = rt,
        };
        rt.GetSerialize<T>(self.Options, hints).Write(ser, value, self.Options);
    }

    public static void Serialize<T, S>(this Builder<ToStringBuilder> self, T value, S serialize) where S : ISerialize<T>
    {
        var rt = EmitRuntimeProvider.Instance;
        var ser = new JsonSerializer(StringBuilderJsonWriter.Create(self))
        {
            RuntimeProvider = rt,
        };
        serialize.Write(ser, value, self.Options);
    }
}

#endregion

#endregion

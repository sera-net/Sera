using System.Text;
using Sera.Json.Builders;
using Sera.Json.Builders.Ser;
using Sera.Json.Ser;
using Sera.Runtime.Emit;

namespace Sera.Json.Runtime;

#region Serializer

#region ToStream

public static partial class SeraJsonRuntime
{
    public static void Serialize<T>(this Builder<ToStream> self, T value)
    {
        var rt = EmitRuntimeProvider.Instance;
        var ser = new JsonSerializer(self.Options, new StreamJsonWriter(self.Options.Formatter, self.Value.Stream))
        {
            RuntimeProvider = rt,
        };
        rt.GetSerialize<T>().Write(ser, value, self.Options);
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
        var ser = new JsonSerializer(self.Options, new StringBuilderJsonWriter(self.Options.Formatter, builder))
        {
            RuntimeProvider = rt,
        };
        rt.GetSerialize<T>().Write(ser, value, self.Options);
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
        var ser = new JsonSerializer(self.Options,
            new StringBuilderJsonWriter(self.Options.Formatter, self.Value.Builder)
        )
        {
            RuntimeProvider = rt,
        };
        rt.GetSerialize<T>().Write(ser, value, self.Options);
    }
}

#endregion

#endregion

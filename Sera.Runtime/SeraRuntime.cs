using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Sera.Runtime.Emit;

namespace Sera.Runtime;

public static class SeraRuntime
{
    internal static readonly ConcurrentDictionary<ISeraRuntimePlugin, byte> plugins = new();

    public static void Reg(ISeraRuntimePlugin plugin) => plugins.TryAdd(plugin, 0);

    internal static IEnumerable<ISeraEmitProvider> GetSerEmitProviders() => plugins.Select(a => a.Key)
        .Select(a => a.GetSerEmitProvider());

    internal static IEnumerable<ISeraEmitProvider> GetDeEmitProviders() => plugins.Select(a => a.Key)
        .Select(a => a.GetDeEmitProvider());
}

public interface ISeraRuntimePlugin
{
    public ISeraEmitProvider GetSerEmitProvider();

    public ISeraEmitProvider GetDeEmitProvider();
}

public interface ISeraEmitProvider
{
    public EmitJob? TryCreateJob(EmitMeta target);
}

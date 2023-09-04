using System;
using System.Text;
using System.Threading;
using Sera.Core;
using Sera.Json.Ser;

namespace Sera.Json;

public record SeraJsonOptions : ASeraOptions
{
    public static SeraJsonOptions Default { get; } = new();
    
    /// <summary>Formatter during serialization</summary>
    public AJsonFormatter Formatter { get; set; } = CompactJsonFormatter.Default;
}

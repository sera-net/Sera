using System;
using System.Text;
using System.Threading;

namespace Sera.Core;

public interface ISeraOptions
{
    /// <summary>
    /// Cancel operation
    /// </summary>
    public virtual CancellationToken CancellationToken => CancellationToken.None;
    /// <summary>
    /// <para>Indicates the encoding to use when processing strings.</para>
    /// <para>Ignored when outputting to dotnet <see cref="string"/> type.</para>
    /// </summary>
    public virtual Encoding Encoding => Encoding.UTF8;
    /// <summary>
    /// The time zone to use when (de)serializing a DateTime
    /// </summary>
    public virtual TimeZoneInfo TimeZone => TimeZoneInfo.Local;
    /// <summary>
    /// Indicates whether a property's name uses a case-insensitive comparison during deserialization.
    /// </summary>
    public virtual bool PropertyNameCaseInsensitive => false;
    /// <summary>
    /// Runtime provider
    /// </summary>
    public virtual IRuntimeProvider RuntimeProvider => StaticRuntimeProvider.Instance;
    /// <summary>
    /// Async Runtime provider
    /// </summary>
    public virtual IAsyncRuntimeProvider AsyncRuntimeProvider => StaticRuntimeProvider.Instance;
}

public sealed class DefaultSeraOptions : ISeraOptions
{
    public static ISeraOptions Default { get; } = new DefaultSeraOptions();
}

public abstract record ASeraOptions : ISeraOptions
{
    public CancellationToken CancellationToken { get; set; } = DefaultSeraOptions.Default.CancellationToken;
    public Encoding Encoding { get; set; } = DefaultSeraOptions.Default.Encoding;
    public TimeZoneInfo TimeZone { get; set; } = DefaultSeraOptions.Default.TimeZone;
    public bool PropertyNameCaseInsensitive { get; set; } = DefaultSeraOptions.Default.PropertyNameCaseInsensitive;
    public IRuntimeProvider RuntimeProvider { get; set; } = DefaultSeraOptions.Default.RuntimeProvider;
    public IAsyncRuntimeProvider AsyncRuntimeProvider { get; set; } = DefaultSeraOptions.Default.AsyncRuntimeProvider;
}

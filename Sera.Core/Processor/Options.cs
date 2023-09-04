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
    /// Indicates the encoding to use when processing strings.
    /// </summary>
    public virtual Encoding Encoding => Encoding.UTF8;
    /// <summary>
    /// The time zone to use when (de)serializing a DateTime
    /// </summary>
    public virtual TimeZoneInfo TimeZone => TimeZoneInfo.Local;
    /// <summary>
    /// Indicates whether read-only fields are ignored during serialization. A field is read-only if it is marked with the readonly keyword.
    /// </summary>
    public virtual bool IgnoreReadOnlyFields => false;
    /// <summary>
    /// Indicates whether read-only properties are ignored during serialization.
    /// </summary>
    public virtual bool IgnoreReadOnlyProperties => false;
    /// <summary>
    /// Indicates whether fields are handled during serialization and deserialization. 
    /// </summary>
    public virtual bool IncludeFields => false;
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
    public bool IgnoreReadOnlyFields { get; set; } = DefaultSeraOptions.Default.IgnoreReadOnlyFields;
    public bool IgnoreReadOnlyProperties { get; set; } = DefaultSeraOptions.Default.IgnoreReadOnlyProperties;
    public bool IncludeFields { get; set; } = DefaultSeraOptions.Default.IncludeFields;
    public bool PropertyNameCaseInsensitive { get; set; } = DefaultSeraOptions.Default.PropertyNameCaseInsensitive;
    public IRuntimeProvider RuntimeProvider { get; set; } = DefaultSeraOptions.Default.RuntimeProvider;
    public IAsyncRuntimeProvider AsyncRuntimeProvider { get; set; } = DefaultSeraOptions.Default.AsyncRuntimeProvider;
}

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
    [Obsolete("Internal use only, use serializer.RuntimeProvider")]
    public virtual IRuntimeProvider RuntimeProvider => StaticRuntimeProvider.Instance;
    /// <summary>
    /// 标记根（传入的实际参数）对象时引用类型时是否可空
    /// </summary>
    public virtual bool RootReferenceNullable => true;
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
#pragma warning disable CS0618
    public IRuntimeProvider RuntimeProvider
    {
        [Obsolete("Internal use only, use serializer.RuntimeProvider")]
        get;
        set;
    } = DefaultSeraOptions.Default.RuntimeProvider;
#pragma warning restore CS0618
    public virtual bool RootReferenceNullable { get; set; } = DefaultSeraOptions.Default.RootReferenceNullable;
}

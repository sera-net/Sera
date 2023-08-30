using System.Text;
using System.Threading;

namespace Sera.Core;

public abstract class SeraOptions
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
    /// Determine whether (De)Serialize implementations should (de)serialize in human-readable form.
    /// </summary>
    public virtual bool IsHumanReadable => false;
    /// <summary>
    /// Indicates whether a property's name uses a case-insensitive comparison during deserialization.
    /// </summary>
    public virtual bool PropertyNameCaseInsensitive => false;
    /// <summary>
    /// Runtime provider
    /// </summary>
    public virtual IRuntimeProvider RuntimeProvider => StaticRuntimeProvider.Instance;
}

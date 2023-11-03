namespace Sera.Core.SerDe;

public interface ISeraAbility<out RT>
{
    /// <summary>
    /// Get the format of the current (de)serializer
    /// </summary>
    public string FormatName { get; }

    /// <summary>
    /// Get the format MIME of the current (de)serializer
    /// </summary>
    public string FormatMIME { get; }

    /// <summary>
    /// Get what type of format the current (de)serializer is
    /// </summary>
    public SeraFormatType FormatType { get; }
    
    /// <summary>
    /// Get options
    /// </summary>
    public ISeraOptions Options { get; }

    /// <summary>
    /// Runtime impl
    /// </summary>
    public RT RuntimeImpl { get; }
}

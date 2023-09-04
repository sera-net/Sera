namespace Sera.Core.Processor;

public interface ISeraAbility
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
}

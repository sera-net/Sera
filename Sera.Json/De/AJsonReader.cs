using System.Text;

namespace Sera.Json.De;

public abstract class AJsonReader(SeraJsonOptions options)
{
    public SeraJsonOptions Options { get; } = options;
    public Encoding Encoding => Options.Encoding;

    #region Seek

    /// <summary>
    /// Can use <see cref="Save"/> <see cref="Load"/>
    /// </summary>
    public abstract bool CanSeek { get; }

    /// <summary>Returns a save point that can be loaded</summary>
    public abstract long Save();
    /// <summary>Load save point</summary>
    public abstract void Load(long pos);
    /// <summary>Delete save point</summary>
    public abstract void UnSave(long pos);

    #endregion
    
    public abstract bool Has();

    public abstract void MoveNext();

    public abstract JsonToken CurrentToken();

    public abstract void SkipValue();

    /// <summary>Read <c>null</c> and move next</summary>
    public abstract void ReadNull();

    /// <summary>Read <c>string</c> and move next</summary>
    public abstract string ReadString();

    /// <summary>Read <c>,</c> and move next</summary>
    public abstract void ReadComma();

    /// <summary>Read <c>:</c> and move next</summary>
    public abstract void ReadColon();

    /// <summary>Read <c>[</c> and move next</summary>
    public abstract void ReadArrayStart();

    /// <summary>Read <c>]</c> and move next</summary>
    public abstract void ReadArrayEnd();

    /// <summary>Read <c>{</c> and move next</summary>
    public abstract void ReadObjectStart();

    /// <summary>Read <c>}</c> and move next</summary>
    public abstract void ReadObjectEnd();
}

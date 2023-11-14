using System.Text;

namespace Sera.Json.De;

public abstract class AJsonReader(SeraJsonOptions options)
{
    public SeraJsonOptions Options { get; } = options;
    public Encoding Encoding => Options.Encoding;

    public abstract bool Has();

    public abstract void MoveNext();

    public abstract JsonToken CurrentToken();

    /// <summary>,</summary>
    public abstract void ReadComma();

    /// <summary>:</summary>
    public abstract void ReadColon();

    /// <summary>[</summary>
    public abstract void ReadArrayStart();

    /// <summary>]</summary>
    public abstract void ReadArrayEnd();

    /// <summary>{</summary>
    public abstract void ReadObjectStart();

    /// <summary>}</summary>
    public abstract void ReadObjectEnd();
}

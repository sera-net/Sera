using System.Text;
using System.Threading.Tasks;

namespace Sera.Json.De;

public abstract class AAsyncJsonReader(SeraJsonOptions options)
{
    public SeraJsonOptions Options { get; } = options;
    public Encoding Encoding => Options.Encoding;

    public abstract ValueTask<bool> Has();

    public abstract ValueTask MoveNext();

    public abstract ValueTask<JsonToken> CurrentToken();

    /// <summary>,</summary>
    public abstract ValueTask ReadComma();

    /// <summary>:</summary>
    public abstract ValueTask ReadColon();

    /// <summary>[</summary>
    public abstract ValueTask ReadArrayStart();

    /// <summary>]</summary>
    public abstract ValueTask ReadArrayEnd();

    /// <summary>{</summary>
    public abstract ValueTask ReadObjectStart();

    /// <summary>}</summary>
    public abstract ValueTask ReadObjectEnd();
}

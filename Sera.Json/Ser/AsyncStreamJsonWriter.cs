using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Sera.Json.Ser;

public class AsyncStreamJsonWriter
    (SeraJsonOptions Options, AJsonFormatter Formatter, Stream Stream) : AAsyncJsonWriter(Options, Formatter)
{
    private readonly StreamWriter Writer = new(Stream, Options.Encoding, leaveOpen: true);

    private Stream? tmpStream;

    public override async ValueTask Flush() => await Writer.FlushAsync();

    public override async ValueTask<Stream> StartBase64()
    {
        await Write("\"");
        if (Equals(Encoding, Encoding.UTF8))
        {
            return tmpStream = new CryptoStream(Stream, new ToBase64Transform(), CryptoStreamMode.Write, true);
        }
        else
        {
            var code_stream = Encoding.CreateTranscodingStream(Stream, Encoding, Encoding.UTF8, true);
            return tmpStream = new CryptoStream(code_stream, new ToBase64Transform(), CryptoStreamMode.Write);
        }
    }

    public override async ValueTask EndBase64()
    {
        await tmpStream!.FlushAsync();
        await tmpStream.DisposeAsync();
        tmpStream = null;
        await Write("\"");
    }

    public override async ValueTask Write(ReadOnlyMemory<char> str)
    {
        await Writer.WriteAsync(str);
    }

    public override async ValueTask WriteEncoded(ReadOnlyMemory<byte> str, Encoding encoding)
    {
        var encode = Options.Encoding;
        if (encode.Equals(encoding))
        {
            await Stream.WriteAsync(str);
        }
        else
        {
            await using var stream = Encoding.CreateTranscodingStream(Stream, Options.Encoding, Encoding, true);
            await stream.WriteAsync(str);
        }
    }
}

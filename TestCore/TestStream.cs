using System.Buffers;
using System.Runtime.InteropServices;
using System.Text;
using Sera.Utils;

namespace TestCore;

public class TestStream
{
    [SetUp]
    public void Setup() { }

    [Test]
    public void TestStringBuilderStream1()
    {
        var utf8 = "123啊😀"u8;

        var builder = new StringBuilder();
        using var stream = new StringBuilderStream(builder);
        using var code_stream = Encoding.CreateTranscodingStream(stream, Encoding.Unicode, Encoding.UTF8, true);
        code_stream.Write(utf8);

        var str = builder.ToString();
        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("123啊😀"));
    }

    [Test]
    public void TestReadOnlySequenceStream1()
    {
        var utf8 = "123啊😀"u8;

        var seq = new ReadOnlySequence<byte>(utf8.ToArray());
        using var stream = new ReadOnlySequenceStream<byte>(seq);
        using var code_stream = Encoding.CreateTranscodingStream(stream, Encoding.UTF8, Encoding.Unicode, true);

        Span<char> buf = stackalloc char[8];
        var count = code_stream.Read(MemoryMarshal.AsBytes(buf));
        var str = buf[..(count / 2)].ToString();
        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("123啊😀"));
    }

    [Test]
    public void TestReadOnlySequenceStream2()
    {
        var utf8 = "123啊😀"u8;

        var seq = new ReadOnlySequence<byte>(utf8.ToArray());
        using var stream = new ReadOnlySequenceStream<byte>(seq);

        Span<byte> buf = stackalloc byte[16];
        var count = stream.Read(buf[..3]);
        var count2 = stream.Read(buf[3..10]);
        var count3 = stream.Read(buf[10..]);
        
        var str = Encoding.UTF8.GetString(buf[..(count + count2 + count3)]);
        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("123啊😀"));
    }
}

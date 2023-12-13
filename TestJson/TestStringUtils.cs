using Sera.Json.Utils;

namespace TestJson;

public class TestStringUtils
{
    [Test, Parallelizable]
    public void TestCountLeadingSpace1([Range(0, 256)] int count)
    {
        Span<char> span = stackalloc char[256];
        span[..count].Fill(' ');
        var len = StringUtils.CountLeadingSpace(span);
        Console.WriteLine(len);
        Assert.That(len, Is.EqualTo(count));
    }
    
    [Test, Parallelizable]
    public void TestCountLeadingSpace2([Range(0, 333)] int size)
    {
        Span<char> span = stackalloc char[size];
        span[..size].Fill(' ');
        var len = StringUtils.CountLeadingSpace(span);
        Console.WriteLine(len);
        Assert.That(len, Is.EqualTo(size));
    }
    
    [Test, Parallelizable]
    public void TestCountLeadingStringContent1([Range(0, 256)] int count)
    {
        Span<char> span = stackalloc char[256];
        span.Fill('"');
        span[..count].Fill('a');
        var len = StringUtils.CountLeadingStringContent(span);
        Console.WriteLine(len);
        Assert.That(len, Is.EqualTo(count));
    }
    
    [Test, Parallelizable]
    public void TestCountLeadingStringContent2([Range(0, 256)] int count)
    {
        Span<char> span = stackalloc char[256];
        span.Fill('\n');
        span[..count].Fill('a');
        var len = StringUtils.CountLeadingStringContent(span);
        Console.WriteLine(len);
        Assert.That(len, Is.EqualTo(count));
    }
    
    [Test, Parallelizable]
    public void TestFindFirstControlCharacterIndex1([Range(0, 255)] int count)
    {
        Span<char> span = stackalloc char[256];
        span.Fill('\n');
        span[..count].Fill('a');
        var len = StringUtils.FindFirstControlCharacterIndex(span);
        Console.WriteLine(len);
        Assert.That(len, Is.EqualTo(count));
    }
    
    [Test, Parallelizable]
    public void TestFindFirstControlCharacterIndex2()
    {
        Span<char> span = stackalloc char[256];
        span.Fill('a');
        var len = StringUtils.FindFirstControlCharacterIndex(span);
        Console.WriteLine(len);
        Assert.That(len, Is.EqualTo(-1));
    }
    
    [Test, Parallelizable]
    public void TestFindFirstControlCharacterIndex3([Range(0, 333)] int size)
    {
        Span<char> span = stackalloc char[size];
        span.Fill('a');
        var len = StringUtils.FindFirstControlCharacterIndex(span);
        Console.WriteLine(len);
        Assert.That(len, Is.EqualTo(-1));
    }
}

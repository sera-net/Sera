namespace TestCore;

public class TestType
{
    [Test]
    public void Test1()
    {
        var a = typeof(List<>).MakeGenericType(typeof(int));
        var b = typeof(List<>).MakeGenericType(typeof(int));

        Console.WriteLine($"ReferenceEquals of Type: {ReferenceEquals(a, b)}");
    }
}

using System.Reflection;
using Sera.Runtime.Emit;

namespace TestCore;

public class TestEmitPrivateAccess
{
    [SetUp]
    public void Setup() { }

    #region Test1

    public class Test1Struct
    {
        private int Member;

        public Test1Struct(int member)
        {
            Member = member;
        }
    }

    [Test]
    public void Test1()
    {
        var obj = new Test1Struct(123);

        var type = typeof(Test1Struct);
        var field = type.GetField("Member", BindingFlags.NonPublic | BindingFlags.Instance)!;

        var (del, method) = EmitPrivateAccess.Instance.AccessGetField(type, field);
        var f = (AccessGet<Test1Struct, int>)del;

        var r = f(ref obj);
        Assert.That(r, Is.EqualTo(123));

        r = (int)method.Invoke(null, new object[] { obj })!;
        Assert.That(r, Is.EqualTo(123));
    }

    #endregion

    #region Test2

    public class Test2Struct
    {
        private int Member { get; set; }

        public Test2Struct(int member)
        {
            Member = member;
        }
    }

    [Test]
    public void Test2()
    {
        var obj = new Test2Struct(123);

        var type = typeof(Test2Struct);
        var property = type.GetProperty("Member", BindingFlags.NonPublic | BindingFlags.Instance)!;

        var (del, method) = EmitPrivateAccess.Instance.AccessGetProperty(type, property);
        var f = (AccessGet<Test2Struct, int>)del;

        var r = f(ref obj);
        Assert.That(r, Is.EqualTo(123));

        r = (int)method.Invoke(null, new object[] { obj })!;
        Assert.That(r, Is.EqualTo(123));
    }

    #endregion

    #region Test3

    public struct Test3Struct
    {
        private int Member;

        public Test3Struct(int member)
        {
            Member = member;
        }
    }

    [Test]
    public void Test3()
    {
        var obj = new Test3Struct(123);

        var type = typeof(Test3Struct);
        var field = type.GetField("Member", BindingFlags.NonPublic | BindingFlags.Instance)!;

        var (del, method) = EmitPrivateAccess.Instance.AccessGetField(type, field);
        var f = (AccessGet<Test3Struct, int>)del;

        var r = f(ref obj);
        Assert.That(r, Is.EqualTo(123));

        r = (int)method.Invoke(null, new object[] { obj })!;
        Assert.That(r, Is.EqualTo(123));
    }

    #endregion

    #region Test4

    public struct Test4Struct
    {
        private int Member { get; set; }

        public Test4Struct(int member)
        {
            Member = member;
        }
    }

    [Test]
    public void Test4()
    {
        var obj = new Test4Struct(123);

        var type = typeof(Test4Struct);
        var property = type.GetProperty("Member", BindingFlags.NonPublic | BindingFlags.Instance)!;

        var (del, method) = EmitPrivateAccess.Instance.AccessGetProperty(type, property);
        var f = (AccessGet<Test4Struct, int>)del;

        var r = f(ref obj);
        Assert.That(r, Is.EqualTo(123));

        r = (int)method.Invoke(null, new object[] { obj })!;
        Assert.That(r, Is.EqualTo(123));
    }

    #endregion
}

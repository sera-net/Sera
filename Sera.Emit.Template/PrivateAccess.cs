namespace Sera.Emit.Template;

public class PrivateAccess
{
    public class Struct1
    {
        public int Foo;
        public int Bar { get; }
    }

    public struct Struct2
    {
        public int Foo;
        public int Bar { get; }
    }

    public static int Get1(ref Struct1 target) => target.Foo;

    public static int Get2(ref Struct2 target) => target.Foo;


    public static int Get3(ref Struct1 target) => target.Bar;

    public static int Get4(ref Struct2 target) => target.Bar;
}

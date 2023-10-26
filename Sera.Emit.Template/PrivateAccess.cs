namespace Sera.Emit.Template;

public class PrivateAccess
{
    public class Struct1
    {
        public int Foo;
        public int Bar { get; set; }
    }

    public struct Struct2
    {
        public int Foo;
        public int Bar { get; set; }
    }

    public static int Get1(ref Struct1 target) => target.Foo;

    public static int Get2(ref Struct2 target) => target.Foo;


    public static int Get3(ref Struct1 target) => target.Bar;

    public static int Get4(ref Struct2 target) => target.Bar;


    public static void Set1(ref Struct1 target, int value) => target.Foo = value;

    public static void Set2(ref Struct2 target, int value) => target.Foo = value;


    public static void Set3(ref Struct1 target, int value) => target.Bar = value;

    public static void Set4(ref Struct2 target, int value) => target.Bar = value;
}

namespace Sera.Emit.Template;

public class DelegateCall
{
    public static int Foo(Func<int, int> a, int b) => a(b);
}

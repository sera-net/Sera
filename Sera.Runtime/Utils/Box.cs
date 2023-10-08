using System.Reflection;
using System.Runtime.CompilerServices;

namespace Sera.Runtime.Utils;

public class Box<T>
{
    public T Value;

    public Box(T value)
    {
        Value = value;
    }
}

public static class Box
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Get<T>(this Box<T> self) => self.Value;

    public static MethodInfo GetMethodInfo { get; } =
        typeof(Box).GetMethod(nameof(Get), BindingFlags.Public | BindingFlags.Static)!;
}

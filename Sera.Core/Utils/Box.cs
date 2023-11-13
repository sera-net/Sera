using System.Runtime.CompilerServices;

namespace Sera.Utils;

public class Box<T>(T value) : IRef<T>
{
    public T Value = value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T GetRef() => ref Value;
}

public interface IRef<T>
{
    public ref T GetRef();
}

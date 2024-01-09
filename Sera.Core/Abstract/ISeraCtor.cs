using Sera.Utils;

namespace Sera.Core;

public interface ISeraCtor<out T>
{
    public T Ctor(InType<T>? t = null);
}

public interface ICapSeraCtor<out T>
{
    public T Ctor(int? cap, InType<T>? t = null);
}

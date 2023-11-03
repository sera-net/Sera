namespace Sera.Core;

public interface IRuntimeProvider<out T>
{
    public T Get();
}

using System.Runtime.CompilerServices;

namespace Sera.Core;

public readonly record struct Unit
{
    public static Unit New
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new();
    }
}

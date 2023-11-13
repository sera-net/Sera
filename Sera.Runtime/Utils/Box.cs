using System.Reflection;
using System.Runtime.CompilerServices;
using Sera.Utils;

namespace Sera.Runtime.Utils;

public static class Box
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Get<T>(this Box<T> self) => self.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T GetRef<T>(this Box<T> self) => ref self.Value;

    public static MethodInfo GetMethodInfo { get; } =
        typeof(Box).GetMethod(nameof(Get), BindingFlags.Public | BindingFlags.Static)!;

    public static MethodInfo GetRefMethodInfo { get; } =
        typeof(Box).GetMethod(nameof(GetRef), BindingFlags.Public | BindingFlags.Static)!;
}

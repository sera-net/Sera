using System.Reflection;

namespace Sera.Core;

public static class SeraReflectionUtils
{
    public static MethodInfo ISerializable_GetSerialize { get; } =
        typeof(ISerializable<,>).GetMethod("GetSerialize")!;

    public static MethodInfo IDeserializable_GetDeserialize { get; } =
        typeof(IDeserializable<,>).GetMethod("GetDeserialize")!;
    
    public static MethodInfo IAsyncSerializable_GetAsyncSerialize { get; } =
        typeof(IAsyncSerializable<,>).GetMethod("GetAsyncSerialize")!;

    public static MethodInfo IAsyncDeserializable_GetAsyncDeserialize { get; } =
        typeof(IAsyncDeserializable<,>).GetMethod("GetAsyncDeserialize")!;
}

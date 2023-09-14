using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Sera.Core;

public static class SeraReflectionUtils
{
    public static MethodInfo ISerializable_GetSerialize { get; } =
        typeof(ISerializable<,>).GetMethod(nameof(ISerializable<Unit>.GetSerialize))!;

    public static MethodInfo IDeserializable_GetDeserialize { get; } =
        typeof(IDeserializable<,>).GetMethod(nameof(IDeserializable<Unit>.GetDeserialize))!;

    public static MethodInfo IAsyncSerializable_GetAsyncSerialize { get; } =
        typeof(IAsyncSerializable<,>).GetMethod(nameof(IAsyncSerializable<Unit>.GetAsyncSerialize))!;

    public static MethodInfo IAsyncDeserializable_GetAsyncDeserialize { get; } =
        typeof(IAsyncDeserializable<,>).GetMethod(nameof(IAsyncDeserializable<Unit>.GetAsyncDeserialize))!;
}

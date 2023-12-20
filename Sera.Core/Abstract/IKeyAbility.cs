namespace Sera.Core.Abstract;

/// <summary>
/// The key ability hint for deserializers
/// </summary>
public interface IKeyAbility;

/// <summary>
/// Hint that deserializers can treat keys as type As
/// </summary>
// ReSharper disable once UnusedTypeParameter
public interface IKeyAbility<As> : IKeyAbility;

public sealed class AsKeyAbility<T> : IKeyAbility<T>
{
    public static AsKeyAbility<T> Instance { get; } = new();
}

public sealed class StringKeyAbility : IKeyAbility<string>
{
    public static StringKeyAbility Instance { get; } = new();
}

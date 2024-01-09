using System.Runtime.CompilerServices;

namespace Sera.Core.Abstract;

/// <summary>
/// The type ability hint for deserializers
/// </summary>
public abstract class ASeraTypeAbility
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public abstract bool IsAccept(SeraKinds kinds, SeraPrimitiveKinds primitiveKinds = SeraPrimitiveKinds.None);
}

public sealed class StringSeraTypeAbility : ASeraTypeAbility
{
    public static StringSeraTypeAbility Instance { get; } = new();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool IsAccept(SeraKinds kinds, SeraPrimitiveKinds primitiveKinds = SeraPrimitiveKinds.None)
        => kinds.Has(SeraKinds.String);
}

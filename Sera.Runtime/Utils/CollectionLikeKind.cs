namespace Sera.Runtime.Utils.Internal;

public enum CollectionLikeKind : byte
{
    None,
    IEnumerable,
    ICollection,
    IReadOnlyCollection,
    IDictionary,
    IReadOnlyDictionary,
}

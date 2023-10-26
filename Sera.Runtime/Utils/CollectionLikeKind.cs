namespace Sera.Runtime.Utils;

internal enum CollectionLikeKind : byte
{
    None,
    IEnumerable,
    ICollection,
    IReadOnlyCollection,
    IDictionary,
    IReadOnlyDictionary,
}

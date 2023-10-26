namespace Sera.Json.Ser;

internal enum JsonSerializerState
{
    None = 0,
    ArrayItem,
    ObjectKey,
    ObjectValue,
    Field,
}

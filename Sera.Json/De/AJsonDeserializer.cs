using System;
using Sera.Core;

namespace Sera.Json.De;

public class AJsonDeserializer(SeraJsonOptions options) : SeraBase<ISeraColion<object?>>
{
    public override string FormatName => "json";
    public override string FormatMIME => "application/json";
    public override SeraFormatType FormatType => SeraFormatType.HumanReadableText;
    public override ISeraOptions Options => options;

    public override IRuntimeProvider<ISeraColion<object?>> RuntimeProvider =>
        RuntimeProviderOverride ?? throw new NotImplementedException("todo"); // EmptyDeRuntimeProvider.Instance

    public IRuntimeProvider<ISeraColion<object?>>? RuntimeProviderOverride { get; set; }

    #region SelectPriorities

    private static readonly Any.Kind[] SelectPrioritiesArray =
    {
        Any.Kind.Unit,
        Any.Kind.Bytes,
        Any.Kind.Primitive,
        Any.Kind.String,
        Any.Kind.Seq,
        Any.Kind.Map,
    };

    public static ReadOnlyMemory<Any.Kind> SelectPriorities => SelectPrioritiesArray;

    #endregion
}

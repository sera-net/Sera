// using System;
// using System.Runtime.CompilerServices;
// using System.Threading.Tasks;
// using Sera.Core;
// using Sera.Utils;
//
// namespace Sera.Json.De;
//
// public class AsyncJsonDeserializer(SeraJsonOptions options, AAsyncJsonReader reader) : SeraBase<ISeraColion<object?>>
// {
//     public override string FormatName => "json";
//     public override string FormatMIME => "application/json";
//     public override SeraFormatType FormatType => SeraFormatType.HumanReadableText;
//     public override ISeraOptions Options => options;
//     internal readonly AAsyncJsonReader reader = reader;
//
//     public override IRuntimeProvider<ISeraColion<object?>> RuntimeProvider =>
//         RuntimeProviderOverride ?? throw new NotImplementedException("todo"); // EmptyDeRuntimeProvider.Instance
//
//     public IRuntimeProvider<ISeraColion<object?>>? RuntimeProviderOverride { get; set; }
//
//     public ValueTask<T> Collect<C, T>(C colion) where C : ISeraColion<T>
//     {
//         var c = new AsyncJsonDeserializer<T>(this);
//         return colion.Collect<ValueTask<T>, AsyncJsonDeserializer<T>>(ref c);
//     }
// }
//
// public readonly struct AsyncJsonDeserializer<T>(AsyncJsonDeserializer impl) : ISeraColctor<T, ValueTask<T>>
// {
//     #region Ability
//
//     public string FormatName => impl.FormatName;
//     public string FormatMIME => impl.FormatMIME;
//     public SeraFormatType FormatType => impl.FormatType;
//     public ISeraOptions Options => impl.Options;
//     public ISeraColion<object?> RuntimeImpl => impl.RuntimeImpl;
//     private AAsyncJsonReader reader => impl.reader;
//
//     #endregion
// }

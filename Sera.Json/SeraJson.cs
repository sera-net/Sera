using System.IO;
using Sera.Json.Builder;
using Sera.Json.Ser;

#region Basic

namespace Sera.Json
{

    public static partial class SeraJson
    {
        public static SerializerBuilder Serializer => default;
    }

}

#endregion

#region ToStream

namespace Sera.Json.Builder
{

    public readonly partial record struct SerializerBuilder
    {
        public ToStreamSerializer ToStream(Stream stream) => new(stream, SeraJsonOptions.Default);

        public ToStreamSerializer ToStream(Stream stream, SeraJsonOptions options) => new(stream, options);
    }

    public readonly record struct ToStreamSerializer(Stream Stream, SeraJsonOptions Options)
    {
        public ToStreamSerializer WithOptions(SeraJsonOptions options) => this with { Options = options };
    }

}

namespace Sera.Json
{

    public static partial class SeraJson
    {
        public static void SerializeStatic<T>(this ToStreamSerializer self, T value) where T : ISerializable<T>
        {
            var ser = new JsonSerializer(self.Options, new StreamJsonWriter(self.Options.Formatter, self.Stream));
            T.GetSerialize().Write(ser, value, self.Options);
        }

        public static void SerializeStatic<T, S>(this ToStreamSerializer self, T value)
            where T : ISerializable<T, S> where S : ISerialize<T>
        {
            var ser = new JsonSerializer(self.Options, new StreamJsonWriter(self.Options.Formatter, self.Stream));
            T.GetSerialize().Write(ser, value, self.Options);
        }

        public static void SerializeStatic<T, S>(this ToStreamSerializer self, T value, S serialize)
            where S : ISerialize<T>
        {
            var ser = new JsonSerializer(self.Options, new StreamJsonWriter(self.Options.Formatter, self.Stream));
            serialize.Write(ser, value, self.Options);
        }
    }

}

#endregion

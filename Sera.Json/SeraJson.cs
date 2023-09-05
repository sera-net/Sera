using System.IO;
using System.Text;
using Sera.Json.Builders;
using Sera.Json.Builders.Ser;
using Sera.Json.Ser;

#region Basic

namespace Sera.Json
{

    public static partial class SeraJson
    {
        public static SerializerBuilder Serializer => default;
    }

}

namespace Sera.Json.Builders
{

    public readonly record struct Builder<T>(SeraJsonOptions Options, T Value);

}

namespace Sera.Json
{

    public static partial class SeraJson
    {
        public static Builder<T> WithOptions<T>(this Builder<T> self, SeraJsonOptions options) =>
            self with { Options = options };

        public static Builder<T> WithFormatter<T>(this Builder<T> self, AJsonFormatter formatter) =>
            self with { Options = self.Options with { Formatter = formatter } };
    }

}

#endregion

#region ToStream

namespace Sera.Json.Builders
{

    public readonly partial struct SerializerBuilder
    {
        public Builder<ToStream> ToStream(Stream stream) => new(SeraJsonOptions.Default, new(stream));

        public Builder<ToStream> ToStream(Stream stream, SeraJsonOptions options) =>
            new(options, new(stream));
    }

    namespace Ser
    {

        public readonly record struct ToStream(Stream Stream);

    }

}

namespace Sera.Json
{

    public static partial class SeraJson
    {
        public static void SerializeStatic<T>(this Builder<ToStream> self, T value) where T : ISerializable<T>
        {
            var ser = new JsonSerializer(self.Options, new StreamJsonWriter(self.Options.Formatter, self.Value.Stream));
            T.GetSerialize().Write(ser, value, self.Options);
        }

        public static void SerializeStatic<T, S>(this Builder<ToStream> self, T value)
            where T : ISerializable<T, S> where S : ISerialize<T>
        {
            var ser = new JsonSerializer(self.Options, new StreamJsonWriter(self.Options.Formatter, self.Value.Stream));
            T.GetSerialize().Write(ser, value, self.Options);
        }

        public static void SerializeStatic<T, S>(this Builder<ToStream> self, T value, S serialize)
            where S : ISerialize<T>
        {
            var ser = new JsonSerializer(self.Options, new StreamJsonWriter(self.Options.Formatter, self.Value.Stream));
            serialize.Write(ser, value, self.Options);
        }
    }

}

#endregion

#region ToString

namespace Sera.Json.Builders
{

    public readonly partial struct SerializerBuilder
    {
        public new Builder<ToString> ToString() => new(SeraJsonOptions.Default, new());

        public Builder<ToString> ToString(SeraJsonOptions options) => new(options, new());
    }

    namespace Ser
    {

        public readonly struct ToString { }

    }

}

namespace Sera.Json
{

    public static partial class SeraJson
    {
        public static string SerializeStatic<T>(this Builder<ToString> self, T value) where T : ISerializable<T>
        {
            var builder = new StringBuilder();
            var ser = new JsonSerializer(self.Options, new StringBuilderJsonWriter(self.Options.Formatter, builder));
            T.GetSerialize().Write(ser, value, self.Options);
            return builder.ToString();
        }

        public static string SerializeStatic<T, S>(this Builder<ToString> self, T value)
            where T : ISerializable<T, S> where S : ISerialize<T>
        {
            var builder = new StringBuilder();
            var ser = new JsonSerializer(self.Options, new StringBuilderJsonWriter(self.Options.Formatter, builder));
            T.GetSerialize().Write(ser, value, self.Options);
            return builder.ToString();
        }

        public static string SerializeStatic<T, S>(this Builder<ToString> self, T value, S serialize)
            where S : ISerialize<T>
        {
            var builder = new StringBuilder();
            var ser = new JsonSerializer(self.Options, new StringBuilderJsonWriter(self.Options.Formatter, builder));
            serialize.Write(ser, value, self.Options);
            return builder.ToString();
        }
    }

}

#endregion

#region ToStringBuilder

namespace Sera.Json.Builders
{

    public readonly partial struct SerializerBuilder
    {
        public Builder<ToStringBuilder> ToString(StringBuilder builder) =>
            new(SeraJsonOptions.Default, new(builder));

        public Builder<ToStringBuilder> ToString(StringBuilder builder, SeraJsonOptions options) =>
            new(options, new(builder));
    }

    namespace Ser
    {

        public readonly record struct ToStringBuilder(StringBuilder Builder);

    }

}

namespace Sera.Json
{

    public static partial class SeraJson
    {
        public static void SerializeStatic<T>(this Builder<ToStringBuilder> self, T value) where T : ISerializable<T>
        {
            var ser = new JsonSerializer(self.Options,
                new StringBuilderJsonWriter(self.Options.Formatter, self.Value.Builder));
            T.GetSerialize().Write(ser, value, self.Options);
        }

        public static void SerializeStatic<T, S>(this Builder<ToStringBuilder> self, T value)
            where T : ISerializable<T, S> where S : ISerialize<T>
        {
            var ser = new JsonSerializer(self.Options,
                new StringBuilderJsonWriter(self.Options.Formatter, self.Value.Builder));
            T.GetSerialize().Write(ser, value, self.Options);
        }

        public static void SerializeStatic<T, S>(this Builder<ToStringBuilder> self, T value, S serialize)
            where S : ISerialize<T>
        {
            var ser = new JsonSerializer(self.Options,
                new StringBuilderJsonWriter(self.Options.Formatter, self.Value.Builder));
            serialize.Write(ser, value, self.Options);
        }
    }

}

#endregion

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

    public readonly record struct Builder<T>(SeraJsonOptions Options, AJsonFormatter Formatter, T Value)
    {
        public Builder(T value) : this(SeraJsonOptions.Default, CompactJsonFormatter.Default, value) { }
    }

}

namespace Sera.Json
{

    public static partial class SeraJson
    {
        public static Builder<T> WithOptions<T>(this Builder<T> self, SeraJsonOptions options) =>
            self with { Options = options };

        public static Builder<T> WithFormatter<T>(this Builder<T> self, AJsonFormatter formatter) =>
            self with { Formatter = formatter };
    }

}

#endregion

#region Serializer

#region ToStream

namespace Sera.Json.Builders
{

    public readonly partial struct SerializerBuilder
    {
        public Builder<ToStream> ToStream(Stream stream) =>
            new(new(stream));
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
            var ser = new JsonSerializer(StreamJsonWriter.Create(self));
            T.GetSerialize().Write(ser, value, self.Options);
        }

        public static void SerializeStatic<T, S>(this Builder<ToStream> self, T value)
            where T : ISerializable<T, S> where S : ISerialize<T>
        {
            var ser = new JsonSerializer(StreamJsonWriter.Create(self));
            T.GetSerialize().Write(ser, value, self.Options);
        }

        public static void SerializeStatic<T, S>(this Builder<ToStream> self, T value, S serialize)
            where S : ISerialize<T>
        {
            var ser = new JsonSerializer(StreamJsonWriter.Create(self));
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
        public new Builder<ToString> ToString() => new(new());
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
            var ser = new JsonSerializer(StringBuilderJsonWriter.Create(self, builder));
            T.GetSerialize().Write(ser, value, self.Options);
            return builder.ToString();
        }

        public static string SerializeStatic<T, S>(this Builder<ToString> self, T value)
            where T : ISerializable<T, S> where S : ISerialize<T>
        {
            var builder = new StringBuilder();
            var ser = new JsonSerializer(StringBuilderJsonWriter.Create(self, builder));
            T.GetSerialize().Write(ser, value, self.Options);
            return builder.ToString();
        }

        public static string SerializeStatic<T, S>(this Builder<ToString> self, T value, S serialize)
            where S : ISerialize<T>
        {
            var builder = new StringBuilder();
            var ser = new JsonSerializer(StringBuilderJsonWriter.Create(self, builder));
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
        public Builder<ToStringBuilder> ToString(StringBuilder builder) => new(new(builder));
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
            var ser = new JsonSerializer(StringBuilderJsonWriter.Create(self));
            T.GetSerialize().Write(ser, value, self.Options);
        }

        public static void SerializeStatic<T, S>(this Builder<ToStringBuilder> self, T value)
            where T : ISerializable<T, S> where S : ISerialize<T>
        {
            var ser = new JsonSerializer(StringBuilderJsonWriter.Create(self));
            T.GetSerialize().Write(ser, value, self.Options);
        }

        public static void SerializeStatic<T, S>(this Builder<ToStringBuilder> self, T value, S serialize)
            where S : ISerialize<T>
        {
            var ser = new JsonSerializer(StringBuilderJsonWriter.Create(self));
            serialize.Write(ser, value, self.Options);
        }
    }

}

#endregion

#endregion

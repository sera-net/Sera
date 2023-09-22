using Sera.Core;
using Sera.Core.Impls;
using Sera.Core.Ser;

namespace Sera.Emit.Template;

public class Struct1
{
    public int Property1 { get; set; }
    public int Field1;

    public string? Property2 { get; set; }
    public string? Field2;
}

public class Struct1Impl : ISerialize<Struct1>, IStructSerializerReceiver<Struct1>
{
    public static PrimitiveImpl<int> _impl_Int32 = PrimitiveImpls.Int32;
    public static StringImpl _impl_String = StringImpl.Instance;

    public void Write<S>(S serializer, Struct1 value, ISeraOptions options) where S : ISerializer
    {
        serializer.StartStruct<Struct1, Struct1, Struct1Impl>(nameof(Struct1), 6, value, this);
    }

    public void Receive<S>(Struct1 value, S serializer) where S : IStructSerializer
    {
        // serializer.WriteField("Property1", value.Property1, _impl_Int32);
        serializer.WriteField("Field1", null, value.Field1, _impl_Int32);
        // serializer.WriteField("Property2", value.Property2!, _impl_String);
        // serializer.WriteField("Field2", value.Field2!, _impl_String);
    }

    public void Receive2<S>(Struct1 value, S serializer) where S : struct, IStructSerializer
    {
        // serializer.WriteField("Property1", value.Property1, _impl_Int32);
        serializer.WriteField("Field1", null, value.Field1, _impl_Int32);
        // serializer.WriteField("Property2", value.Property2!, _impl_String);
        // serializer.WriteField("Field2", value.Field2!, _impl_String);
    }

    public void Receive3<S>(Struct1 value, S serializer) where S : class, IStructSerializer
    {
        // serializer.WriteField("Property1", value.Property1, _impl_Int32);
        serializer.WriteField("Field1", null, value.Field1, _impl_Int32);
        // serializer.WriteField("Property2", value.Property2!, _impl_String);
        // serializer.WriteField("Field2", value.Field2!, _impl_String);
    }
}

using Sera.Core;
using Sera.Core.Ser;

namespace Sera.Emit.Template;

public class StructEmpty1 { }

public class StructEmpty1Impl : ISerialize<StructEmpty1>, IStructSerializerReceiver<StructEmpty1>
{
    public void Write<S>(S serializer, StructEmpty1 value, ISeraOptions options) where S : ISerializer
    {
        serializer.StartStruct<StructEmpty1, StructEmpty1, StructEmpty1Impl>(nameof(StructEmpty1), 0, value, this);
    }

    public void Receive<S>(StructEmpty1 value, S serializer) where S : IStructSerializer { }
}

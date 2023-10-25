namespace Sera;

#region Sync

public interface ISerializable<T, out Impl> where Impl : ISerialize<T>
{
    public static abstract Impl GetSerialize();
}

public interface IDeserializable<T, out Impl> where Impl : IDeserialize<T>
{
    public static abstract Impl GetDeserialize();
}

public interface ISerializable<T> : ISerializable<T, ISerialize<T>> { }

public interface IDeserializable<T> : IDeserializable<T, IDeserialize<T>> { }

#endregion

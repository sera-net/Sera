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

#region Async

public interface IAsyncSerializable<T, out Impl> where Impl : IAsyncSerialize<T>
{
    public static abstract Impl GetAsyncSerialize();
}

public interface IAsyncDeserializable<T, out Impl> where Impl : IAsyncDeserialize<T>
{
    public static abstract Impl GetAsyncDeserialize();
}

public interface IAsyncSerializable<T> : IAsyncSerializable<T, IAsyncSerialize<T>> { }

public interface IAsyncDeserializable<T> : IAsyncDeserializable<T, IAsyncDeserialize<T>> { }

#endregion

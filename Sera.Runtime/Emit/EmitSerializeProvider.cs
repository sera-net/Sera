using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Sera.Runtime.Emit;

internal partial class EmitSerializeProvider
{
    public readonly ConditionalWeakTable<Type, CacheCell> cache = new();

    public ISerialize<T> GetSerialize<T>()
    {
        var cell = GetSerialize(typeof(T), Thread.CurrentThread);
        if (cell.state != CacheCell.CreateState.Created)
        {
            cell.WaitCreate.WaitOne();
        }
        return (ISerialize<T>)cell.ser_inst!;
    }

    internal class CacheCell
    {
        public enum CreateState
        {
            Idle,
            Creating,
            Created,
        }

        public volatile CreateState state = CreateState.Idle;
        public readonly Thread CreateThread;

        public readonly ManualResetEvent WaitType = new(false);
        public readonly ManualResetEvent WaitCreate = new(false);

        public Type? ser_type;
        public object? ser_inst;

        public CacheCell(Thread createThread)
        {
            CreateThread = createThread;
        }
    }

    private CacheCell GetSerialize(Type type, Thread current_thread)
    {
        var cell = cache.GetValue(type, _ => new(current_thread));
        if (cell.CreateThread == current_thread)
        {
#pragma warning disable CS0420
            if (Interlocked.CompareExchange(ref Unsafe.As<CacheCell.CreateState, int>(ref cell.state),
                    (int)CacheCell.CreateState.Creating, (int)CacheCell.CreateState.Idle) == (int)CacheCell.CreateState.Idle)
            {
                CreateSerialize(type, cell);
            }
#pragma warning restore CS0420
        }
        return cell;
    }

    private void CreateSerialize(Type type, CacheCell cell)
    {
        try
        {
            // todo other type
            GenStruct(type, cell);
        }
        finally
        {
            cell.state = CacheCell.CreateState.Created;
            cell.WaitCreate.Set();
        }
    }

    private void GenStruct(Type type, CacheCell cell)
    {
        if (type.IsVisible) GenPublicStruct(type, cell);
        else GenPrivateStruct(type, cell);
    }
}

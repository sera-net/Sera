using System;
using System.Collections.Generic;
using System.Reflection;
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
        cell.CheckDepsReady();
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

        public Type? dep_container_type;
        public Dictionary<Type, CacheCellDeps>? deps;
        private volatile bool deps_ready;
        private readonly object deps_ready_lock = new();

        public void CheckDepsReady()
        {
            if (deps_ready) return;
            lock (deps_ready_lock)
            {
                if (deps_ready) return;
                try
                {
                    foreach (var (_, dep) in deps!)
                    {
                        var field_name = dep.Field.Name;
                        var field = dep_container_type!.GetField(field_name)!;

                        if (dep.ImplInst != null)
                        {
                            field.SetValue(null, dep.ImplInst);
                        }
                        else
                        {
                            dep.ImplCell!.WaitCreate.WaitOne();
                            field.SetValue(null, dep.ImplCell.ser_inst);
                        }
                    }
                }
                finally
                {
                    deps_ready = true;
                }
            }
            foreach (var (_, dep) in deps)
            {
                if (dep.ImplInst == null)
                {
                    dep.ImplCell!.CheckDepsReady();
                }
            }
        }
    }

    internal record CacheCellDeps(FieldInfo Field, Type ImplType, CacheCell? ImplCell, object? ImplInst)
    {
        public FieldInfo Field { get; set; } = Field;
        public Type ImplType { get; set; } = ImplType;
        public CacheCell? ImplCell { get; set; } = ImplCell;
        public object? ImplInst { get; set; } = ImplInst;
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

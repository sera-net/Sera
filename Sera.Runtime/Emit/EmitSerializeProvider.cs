using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Sera.Runtime.Emit;

internal partial class EmitSerializeProvider
{
    public readonly ConditionalWeakTable<Type, CacheStub> cache = new();

    public ISerialize<T> GetSerialize<T>()
    {
        var stub = GetSerializeStub(typeof(T), Thread.CurrentThread);
        if (stub.state != CacheStub.CreateState.Created)
        {
            stub.WaitCreate.WaitOne();
        }
        stub.EnsureDepsReady();
        return (ISerialize<T>)stub.ser_inst!;
    }

    internal class CacheStub
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

        public CacheStub(Thread createThread)
        {
            CreateThread = createThread;
        }

        public Type? dep_container_type;
        public Dictionary<Type, CacheCellDeps>? deps;
        private volatile bool deps_ready;
        private readonly object deps_ready_lock = new();

        public void EnsureDepsReady()
        {
            if (deps == null || dep_container_type == null) return;
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
                    dep.ImplCell!.EnsureDepsReady();
                }
            }
        }
    }

    internal record CacheCellDeps(FieldInfo Field, Type ImplType, CacheStub? ImplCell, object? ImplInst)
    {
        public FieldInfo Field { get; set; } = Field;
        public Type ImplType { get; set; } = ImplType;
        public CacheStub? ImplCell { get; set; } = ImplCell;
        public object? ImplInst { get; set; } = ImplInst;
    }

    private CacheStub GetSerializeStub(Type type, Thread current_thread)
    {
        var stub = cache.GetValue(type, _ => new(current_thread));
        if (stub.CreateThread == current_thread)
        {
#pragma warning disable CS0420
            if (Interlocked.CompareExchange(ref Unsafe.As<CacheStub.CreateState, int>(ref stub.state),
                    (int)CacheStub.CreateState.Creating, (int)CacheStub.CreateState.Idle) ==
                (int)CacheStub.CreateState.Idle)
            {
                CreateSerialize(type, stub);
            }
#pragma warning restore CS0420
        }
        return stub;
    }

    private void CreateSerialize(Type type, CacheStub stub)
    {
        try
        {
            // todo other type
            GenStruct(type, stub);
        }
        finally
        {
            stub.state = CacheStub.CreateState.Created;
            stub.WaitCreate.Set();
        }
    }

    private void GenStruct(Type target, CacheStub stub)
    {
        var members = GetStructMembers(target, true);
        if (target.IsVisible && members.All(m => m.Type.IsVisible))
        {
            GenPublicStruct(target, members, stub);
        }
        else
        {
            GenPrivateStruct(target, members, stub);
        }
    }
}

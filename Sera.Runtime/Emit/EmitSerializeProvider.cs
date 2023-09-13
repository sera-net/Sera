using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using Sera.Core.Impls;
using Sera.Runtime.Utils;

namespace Sera.Runtime.Emit;

internal partial class EmitSerializeProvider
{
    public readonly ConditionalWeakTable<Type, CacheStub> cache = new();

    public ISerialize<T> GetSerialize<T>()
    {
        var stub = GetSerializeStub(typeof(T), Thread.CurrentThread);
        stub.EnsureInstProvided();
        stub.EnsureDepsReady();
        return (ISerialize<T>)stub.SerInst;
    }

    internal class CacheStub
    {
        private enum CreateState
        {
            Idle,
            Creating,
            Created,
        }

        private volatile CreateState state = CreateState.Idle;
        public Thread CreateThread { get; }

        private readonly ManualResetEvent WaitType = new(false);
        private readonly ManualResetEvent WaitCreate = new(false);

        private Type? ser_type;
        private object? ser_inst;

        public Type SerType => ser_type!;
        public object SerInst => ser_inst!;

        public CacheStub(Thread createThread)
        {
            CreateThread = createThread;
        }

        private Type? dep_container_type;
        private Dictionary<Type, CacheStubDeps>? deps;
        private volatile bool deps_ready;
        private readonly object deps_ready_lock = new();

#pragma warning disable CS0420
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryEnterCreating() =>
            Interlocked.CompareExchange(ref Unsafe.As<CreateState, int>(ref state),
                (int)CreateState.Creating, (int)CreateState.Idle) == (int)CreateState.Idle;
#pragma warning restore CS0420

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WaitTypeProvided()
        {
            WaitType.WaitOne();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WaitInstProvided()
        {
            WaitType.WaitOne();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EnsureInstProvided()
        {
            if (state != CreateState.Created)
            {
                WaitInstProvided();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void MarkTypeProvided()
        {
            WaitType.Set();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void MarkInstProvided()
        {
            state = CreateState.Created;
            WaitCreate.Set();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ProvideType(Type type)
        {
            ser_type = type;
            MarkTypeProvided();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ProvideInst(object inst)
        {
            ser_inst = inst;
            MarkInstProvided();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ProvideDeps(Type dep_container_type, Dictionary<Type, CacheStubDeps> deps)
        {
            this.dep_container_type = dep_container_type;
            this.deps = deps;
        }

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
                        dep.Ready(dep_container_type);
                    }
                }
                finally
                {
                    deps_ready = true;
                }
            }
            foreach (var (_, dep) in deps)
            {
                dep.WaitInstReady();
            }
        }
    }

    internal record CacheStubDeps(
        FieldInfo Field, Type ImplType, Type RawImplType, Type ValueType,
        CacheStub? ImplStub, object? ImplInst, bool RefNullable
    )
    {
        public FieldInfo Field { get; set; } = Field;
        public Type ImplType { get; set; } = ImplType;
        public Type RawImplType { get; set; } = RawImplType;
        public Type ValueType { get; set; } = ValueType;

        private CacheStub? ImplStub { get; set; } = ImplStub;
        private object? ImplInst { get; set; } = ImplInst;

        /// <summary>
        /// If <c>true</c>
        /// <code>
        /// ImplType == typeof(NullableReferenceTypeImpl&lt;typeof(ImplInst)&gt;)
        /// </code>
        /// </summary>
        private bool RefNullable { get; set; } = RefNullable;

        internal void Ready(Type dep_container_type)
        {
            var field_name = Field.Name;
            var field = dep_container_type.GetField(field_name)!;

            if (ImplInst != null)
            {
                var inst = ImplInst;
                if (RefNullable)
                {
                    var ctor = ImplType.GetConstructor(BindingFlags.Public | BindingFlags.Instance,
                        new[] { RawImplType })!;
                    inst = ctor.Invoke(new[] { inst });
                }
                field.SetValue(null, inst);
            }
            else
            {
                ImplStub!.WaitInstProvided();
                var inst = ImplStub.SerInst;
                if (RefNullable)
                {
                    var impl_type = field.FieldType;
                    var ctor = impl_type.GetConstructor(BindingFlags.Public | BindingFlags.Instance,
                        new[] { ImplStub.SerType })!;
                    inst = ctor.Invoke(new[] { inst });
                }
                field.SetValue(null, inst);
            }
        }

        internal void WaitInstReady()
        {
            if (ImplInst == null)
            {
                ImplStub!.EnsureDepsReady();
            }
        }
    }

    private CacheStub GetSerializeStub(Type type, Thread current_thread)
    {
        var stub = cache.GetValue(type, _ => new(current_thread));
        if (stub.CreateThread == current_thread)
        {
            if (stub.TryEnterCreating())
            {
                CreateSerialize(type, stub);
            }
        }
        return stub;
    }

    private void CreateSerialize(Type type, CacheStub stub)
    {
        try
        {
            // todo other type
            if (type.IsEnum)
            {
                GenEnum(type, stub);
            }
            else
            {
                GenStruct(type, stub);
            }
        }
        finally
        {
            stub.MarkInstProvided();
        }
    }
}

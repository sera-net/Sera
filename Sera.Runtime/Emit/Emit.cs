using System;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading;
using Sera.Core.Ser;
using Sera.Runtime.Emit.Deps;
using Sera.Runtime.Utils;

namespace Sera.Runtime.Emit;

internal record EmitData(
    SerializerPrimitiveHint? Hint = null,
    // use bytes on arrays
    bool UserBytes = true
)
{
    public static EmitData Default { get; } = new();
}

internal readonly record struct EmitMeta(TypeMeta TypeMeta, EmitData Data)
{
    public Type Type => TypeMeta.Type;

    public bool IsValueType => Type.IsValueType;

    public bool IsEnum => Type.IsEnum;

    public bool IsArray => Type.IsArray;
    public bool IsSZArray => Type.IsSZArray;
}

internal abstract class AEmitProvider
{
    private readonly ConditionalWeakTable<Type, ConcurrentDictionary<EmitMeta, EmitStub>> cache = new();

    internal EmitStub GetStub(EmitMeta target)
    {
        var cache_group = cache.GetValue(target.Type, static _ => new());
        var job = CreateJob(target);
        var stub = cache_group.GetOrAdd(
            target,
            static (_, a) =>
                new(a.emitProvider, a.target, a.job),
            (emitProvider: this, target, job)
        );
        stub.EnsureJobInit();
        return stub;
    }

    protected abstract EmitJob CreateJob(EmitMeta target);

    #region Emit

    protected EmitStub Emit(EmitMeta target)
    {
        var stub = GetStub(target);
        if (stub.IsCompleted) return stub;
        var ctx = new EmitCtx(Thread.CurrentThread);
        stub.EnsureCompleted(ctx);
        return stub;
    }

    #endregion
}

internal record struct EmitCtx(Thread CurrentThread);

internal sealed class EmitStub(AEmitProvider emitProvider, EmitMeta target, EmitJob job)
{
    public AEmitProvider EmitProvider { get; } = emitProvider;

    public EmitMeta Target { get; } = target;
    private EmitJob Job { get; } = job;

    private volatile EmitStubState state = EmitStubState.None;
    public EmitStubState State => state;

    private volatile int currentThread = -1;

    public Type? RawEmitType { get; private set; }
    public Type? RawRuntimeType { get; private set; }
    public Type? RawEmitPlaceholderType { get; private set; }
    public Type? RawRuntimePlaceholderType { get; private set; }
    public object? RawInst { get; private set; }

    private readonly object MakeRawEmitTypeLock = new();
    private readonly object MakeRawEmitPlaceholderTypeLock = new();

    private readonly object MakeRawRuntimeTypeLock = new();
    private readonly object MakeRawRuntimePlaceholderTypeLock = new();

    public EmitTransform[] Transforms { get; private set; } = Array.Empty<EmitTransform>();
    private DepMetaGroup[] DepsGroups { get; set; } = Array.Empty<DepMetaGroup>();
    public FrozenDictionary<int, int> DepsIndexMap { get; private set; } = FrozenDictionary<int, int>.Empty;
    private DepItem[] Deps { get; set; } = Array.Empty<DepItem>();
    private EmitDeps? emitDeps;
    private RuntimeDeps? runtimeDeps;

    public bool EmitTypeIsTypeBuilder { get; private set; }

    public ExceptionDispatchInfo? Exception { get; private set; }

    public object? GetResult()
    {
        WaitState(EmitStubState.Created);
        return RawInst;
    }

    #region State

    public bool IsInited => state >= EmitStubState.Inited;
    public bool IsCompleted => state >= EmitStubState.Created;
    public bool IsCreated => state == EmitStubState.Created;
    public bool IsFaulted => state == EmitStubState.Error;

    #region TryEnterState

#pragma warning disable CS0420
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool TryEnterState(EmitStubState before, EmitStubState after)
        => Interlocked.CompareExchange(ref Unsafe.As<EmitStubState, uint>(ref state), (uint)after,
            (uint)before) == (uint)before;
#pragma warning restore CS0420

    #endregion

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SetState(EmitStubState state) => this.state = state;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SpinWait(EmitStubState target)
    {
        while (state < target) { }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WaitState(EmitStubState target)
    {
        SpinWait(target);
        if (Exception != null) Exception!.Throw();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void MarkError(ExceptionDispatchInfo e)
    {
        Exception = e;
        state = EmitStubState.Error;
    }

    #endregion

    #region InitJob

    internal void EnsureJobInit()
    {
        if (TryEnterState(EmitStubState.None, EmitStubState.Initing))
        {
            try
            {
                InitJob();
                SetState(EmitStubState.Inited);
            }
            catch (Exception e)
            {
                MarkError(ExceptionDispatchInfo.Capture(e));
            }
        }
        else
        {
            WaitState(EmitStubState.Inited);
        }
    }

    private void InitJob()
    {
        Job.Init(this, Target);
        Transforms = Job.CollectTransforms(this, Target);
        var RequestedDeps = Job.CollectDeps(this, Target);

        if (RequestedDeps.Length != 0)
        {
            DepsGroups = RequestedDeps
                .Select((a, i) => (a, i))
                .GroupBy(a => a.a)
                .Select(g => new DepMetaGroup(g.First().a, g.Select(a => a.i).ToArray()))
                .ToArray();
            DepsIndexMap = DepsGroups
                .SelectMany((a, n) => a.RawIndexes.Select(i => (i, n)))
                .ToDictionary(a => a.i, a => a.n)
                .ToFrozenDictionary();
        }
    }

    #endregion

    #region EnsureCompleted

    internal void EnsureCompleted(EmitCtx ctx)
    {
        if (TryStart(ctx))
        {
            WaitState(EmitStubState.Created);
            return;
        }
        EnsureDepsReady(ctx);
        EnsureEmitStart(ctx);
        EnsureRtDepsReady(ctx);
        EnsureInstCreated(ctx);
        EnsureDepsInstSet(ctx);
    }

    private bool TryStart(EmitCtx ctx) =>
        Interlocked.CompareExchange(ref currentThread, ctx.CurrentThread.ManagedThreadId, -1) != -1;

    #endregion

    #region ReadyDeps

    private void EnsureDepsReady(EmitCtx ctx)
    {
        TryStart(ctx);
        if (TryEnterState(EmitStubState.Inited, EmitStubState.BuildingDeps))
        {
            try
            {
                ReadyDeps();
                ReadyChildDeps(ctx);
                CheckDepsCircular();
                BuildEmitType(false);
                CheckEmitTypeIsTypeBuilder();
                SetState(EmitStubState.DepsReady);
            }
            catch (Exception e)
            {
                MarkError(ExceptionDispatchInfo.Capture(e));
            }
        }
        else
        {
            if (ctx.CurrentThread.ManagedThreadId != currentThread)
            {
                WaitState(EmitStubState.DepsReady);
            }
        }
    }

    private void ReadyDeps()
    {
        Deps = DepsGroups.AsParallel().AsOrdered()
            .Select(dep =>
            {
                var stub = EmitProvider.GetStub(dep.DepMeta.Meta);
                return new DepItem(stub, dep.DepMeta);
            })
            .ToArray();
    }

    private void ReadyChildDeps(EmitCtx ctx)
    {
        Deps.AsParallel().ForAll(dep =>
        {
            dep.Stub.EnsureDepsReady(ctx); //
        });
    }

    private void CheckDepsCircular()
    {
        Deps.AsParallel().ForAll(dep =>
        {
            var cache = new ConcurrentDictionary<EmitStub, byte>();
            cache.TryAdd(this, 0);
            cache.TryAdd(dep.Stub, 0);
            dep.UsePlaceholder = HasCircular(dep);

            bool HasCircular(DepItem dep) => dep.Stub.Deps.Length != 0 && dep.Stub.Deps.AsParallel().Any(a =>
            {
                if (!cache.TryAdd(a.Stub, 0))
                {
                    if (a.Stub == dep.Stub || a.Stub == this) return true;
                    return false;
                }
                return HasCircular(a);
            });
        });
    }

    private void BuildEmitType(bool usePlaceholder)
    {
        if (usePlaceholder)
        {
            lock (MakeRawEmitTypeLock)
            {
                RawEmitPlaceholderType ??= Job.GetEmitPlaceholderType(this, Target);
            }
        }
        else
        {
            var deps = Deps;
            if (RawEmitType == null)
            {
                Deps.AsParallel().ForAll(dep => dep.Stub.BuildEmitType(dep.UsePlaceholder));
                lock (MakeRawEmitPlaceholderTypeLock)
                {
                    RawEmitType ??= Job.GetEmitType(this, Target, deps);
                }
            }
        }
    }

    private void CheckEmitTypeIsTypeBuilder()
    {
        EmitTypeIsTypeBuilder = IsTypeBuilder(RawEmitType!);
    }

    private bool IsTypeBuilder(Type type) =>
        type is TypeBuilder ||
        type is { IsGenericType: true } && type.GetGenericArguments().AsParallel().Any(IsTypeBuilder);

    #endregion

    #region Emit

    private void EnsureEmitStart(EmitCtx ctx)
    {
        if (TryEnterState(EmitStubState.DepsReady, EmitStubState.Emitting))
        {
            try
            {
                Emit();
                SetState(EmitStubState.Emitted);
                EmitDeps(ctx);
            }
            catch (Exception e)
            {
                MarkError(ExceptionDispatchInfo.Capture(e));
            }
        }
        else
        {
            if (ctx.CurrentThread.ManagedThreadId != currentThread)
            {
                WaitState(EmitStubState.Emitted);
            }
        }
    }

    private void Emit()
    {
        var deps = emitDeps = EmitDepContainer.BuildEmitDeps(this, Deps);
        Job.Emit(this, Target, deps);
    }

    private void EmitDeps(EmitCtx ctx)
    {
        Deps.AsParallel().ForAll(dep =>
        {
            dep.Stub.EnsureEmitStart(ctx); //
        });
    }

    #endregion

    #region ReadyRtDeps

    private void EnsureRtDepsReady(EmitCtx ctx)
    {
        if (TryEnterState(EmitStubState.Emitted, EmitStubState.BuildingRtDeps))
        {
            try
            {
                ReadyChildRtDeps(ctx);
                BuildRuntimeType(false);
                SetState(EmitStubState.RtDepsReady);
            }
            catch (Exception e)
            {
                MarkError(ExceptionDispatchInfo.Capture(e));
            }
        }
        else
        {
            if (ctx.CurrentThread.ManagedThreadId != currentThread)
            {
                WaitState(EmitStubState.RtDepsReady);
            }
        }
    }

    private void ReadyChildRtDeps(EmitCtx ctx)
    {
        Deps.AsParallel().ForAll(dep =>
        {
            dep.Stub.EnsureRtDepsReady(ctx); //
        });
    }


    private void BuildRuntimeType(bool usePlaceholder)
    {
        if (usePlaceholder)
        {
            lock (MakeRawRuntimeTypeLock)
            {
                RawRuntimePlaceholderType ??= Job.GetRuntimePlaceholderType(this, Target);
            }
        }
        else
        {
            var deps = Deps;
            if (RawRuntimeType == null)
            {
                Deps.AsParallel().ForAll(dep => dep.Stub.BuildRuntimeType(dep.UsePlaceholder));
                lock (MakeRawRuntimePlaceholderTypeLock)
                {
                    RawRuntimeType ??= Job.GetRuntimeType(this, Target, deps);
                }
            }
        }
    }

    #endregion

    #region CreateInst

    private void EnsureInstCreated(EmitCtx ctx)
    {
        if (TryEnterState(EmitStubState.RtDepsReady, EmitStubState.CreatingInst))
        {
            try
            {
                CreateInst();
                CreateDepsInst(ctx);
                SetState(EmitStubState.InstCreated);
            }
            catch (Exception e)
            {
                MarkError(ExceptionDispatchInfo.Capture(e));
            }
        }
        else
        {
            if (ctx.CurrentThread.ManagedThreadId != currentThread)
            {
                WaitState(EmitStubState.InstCreated);
            }
        }
    }

    private void CreateInst()
    {
        var deps = runtimeDeps = EmitDepContainer.BuildRuntimeDeps(emitDeps!);
        RawInst = Job.CreateInst(this, Target, deps);
        Debug.Assert(RawInst != null);
    }

    private void CreateDepsInst(EmitCtx ctx)
    {
        Deps.AsParallel().ForAll(dep =>
        {
            dep.Stub.EnsureInstCreated(ctx); //
        });
    }

    #endregion

    #region SetDepsInst

    private void EnsureDepsInstSet(EmitCtx ctx)
    {
        if (TryEnterState(EmitStubState.InstCreated, EmitStubState.SettingDepsInst))
        {
            try
            {
                SetDepsInst(ctx);
                Clear();
                SetState(EmitStubState.Created);
            }
            catch (Exception e)
            {
                MarkError(ExceptionDispatchInfo.Capture(e));
            }
        }
        else
        {
            if (ctx.CurrentThread.ManagedThreadId != currentThread)
            {
                WaitState(EmitStubState.Created);
            }
        }
    }

    private void SetDepsInst(EmitCtx ctx)
    {
        EmitDepContainer.SetDepsInst(runtimeDeps!);
        Deps.AsParallel().ForAll(dep =>
        {
            dep.Stub.EnsureDepsInstSet(ctx); //
        });
    }

    #endregion

    #region Clear

    private void Clear()
    {
        DepsGroups = null!;

        emitDeps = null!;
        runtimeDeps = null!;
    }

    #endregion
}

internal enum EmitStubState : uint
{
    None = 0,
    Initing = None + 1,
    Inited = Initing + 1,
    BuildingDeps = Inited + 1,
    DepsReady = BuildingDeps + 1,
    Emitting = DepsReady + 1,
    Emitted = Emitting + 1,
    BuildingRtDeps = Emitted + 1,
    RtDepsReady = BuildingRtDeps + 1,
    CreatingInst = RtDepsReady + 1,
    InstCreated = CreatingInst + 1,
    SettingDepsInst = InstCreated + 1,
    Created = uint.MaxValue - 1,
    Error = uint.MaxValue,
}

internal abstract class EmitJob
{
    public Guid Guid { get; } = Guid.NewGuid();

    protected TypeBuilder CreateTypeBuilder(string name)
    {
        var module = ReflectionUtils.CreateAssembly($"_{Guid:N}_");
        return module.DefineType(
            $"{module.Assembly.GetName().Name}.{name}",
            TypeAttributes.Public | TypeAttributes.Sealed
        );
    }

    protected TypeBuilder CreateTypeBuilderStruct(string name)
    {
        var module = ReflectionUtils.CreateAssembly($"_{Guid:N}_");
        return module.DefineType(
            $"{module.Assembly.GetName().Name}.{name}",
            TypeAttributes.Public | TypeAttributes.Sealed,
            typeof(ValueType)
        );
    }

    /// <summary>Initialize miscellaneous data</summary>
    public abstract void Init(EmitStub stub, EmitMeta target);

    /// <summary>Collect transforms</summary>
    public abstract EmitTransform[] CollectTransforms(EmitStub stub, EmitMeta target);

    /// <summary>Collect dependencies</summary>
    public abstract DepMeta[] CollectDeps(EmitStub stub, EmitMeta target);

    /// <summary>Create emit placeholder type</summary>
    public abstract Type GetEmitPlaceholderType(EmitStub stub, EmitMeta target);

    /// <summary>Create runtime placeholder type</summary>
    public abstract Type GetRuntimePlaceholderType(EmitStub stub, EmitMeta target);

    /// <summary>Create emit type</summary>
    public abstract Type GetEmitType(EmitStub stub, EmitMeta target, DepItem[] deps);

    /// <summary>Create runtime type</summary>
    public abstract Type GetRuntimeType(EmitStub stub, EmitMeta target, DepItem[] deps);

    /// <summary>Emit if need</summary>
    public abstract void Emit(EmitStub stub, EmitMeta target, EmitDeps deps);

    /// <summary>Complete instance creation</summary>
    public abstract object CreateInst(EmitStub stub, EmitMeta target, RuntimeDeps deps);
}

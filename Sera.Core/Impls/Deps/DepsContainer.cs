namespace Sera.Core.Impls.Deps;

#region interfaces

public interface IDepsContainer { }

public interface IDepsContainer1<out D1> : IDepsContainer
{
    public static abstract D1? Impl1 { get; }
}

public interface IDepsContainer2<out D2> : IDepsContainer
{
    public static abstract D2? Impl2 { get; }
}

public interface IDepsContainer3<out D3> : IDepsContainer
{
    public static abstract D3? Impl3 { get; }
}

public interface IDepsContainer4<out D4> : IDepsContainer
{
    public static abstract D4? Impl4 { get; }
}

public interface IDepsContainer5<out D5> : IDepsContainer
{
    public static abstract D5? Impl5 { get; }
}

public interface IDepsContainer6<out D6> : IDepsContainer
{
    public static abstract D6? Impl6 { get; }
}

public interface IDepsContainer7<out D7> : IDepsContainer
{
    public static abstract D7? Impl7 { get; }
}

public interface IDepsContainer8<out D8> : IDepsContainer
{
    public static abstract D8? Impl8 { get; }
}

public interface IDepsContainer<out D1>
    : IDepsContainer1<D1> { }

public interface IDepsContainer<out D1, out D2>
    : IDepsContainer<D1>, IDepsContainer2<D2> { }

public interface IDepsContainer<out D1, out D2, out D3>
    : IDepsContainer<D1, D2>, IDepsContainer3<D3> { }

public interface IDepsContainer<out D1, out D2, out D3, out D4>
    : IDepsContainer<D1, D2, D3>, IDepsContainer4<D4> { }

public interface IDepsContainer<out D1, out D2, out D3, out D4, out D5>
    : IDepsContainer<D1, D2, D3, D4>, IDepsContainer5<D5> { }

public interface IDepsContainer<out D1, out D2, out D3, out D4, out D5, out D6>
    : IDepsContainer<D1, D2, D3, D4, D5>, IDepsContainer6<D6> { }

public interface IDepsContainer<out D1, out D2, out D3, out D4, out D5, out D6, out D7>
    : IDepsContainer<D1, D2, D3, D4, D5, D6>, IDepsContainer7<D7> { }

public interface IDepsContainer<out D1, out D2, out D3, out D4, out D5, out D6, out D7, out D8>
    : IDepsContainer<D1, D2, D3, D4, D5, D6, D7>, IDepsContainer8<D8> { }

#endregion

#region Classes

public sealed class DepsContainer : IDepsContainer { }

public sealed class DepsContainer<D1> : IDepsContainer<D1>
{
    public static D1? Impl1 { get; set; }
}

public sealed class DepsContainer<D1, D2> : IDepsContainer<D1, D2>
{
    public static D1? Impl1 { get; set; }
    public static D2? Impl2 { get; set; }
}

public sealed class DepsContainer<D1, D2, D3> : IDepsContainer<D1, D2, D3>
{
    public static D1? Impl1 { get; set; }
    public static D2? Impl2 { get; set; }
    public static D3? Impl3 { get; set; }
}

public sealed class DepsContainer<D1, D2, D3, D4> : IDepsContainer<D1, D2, D3, D4>
{
    public static D1? Impl1 { get; set; }
    public static D2? Impl2 { get; set; }
    public static D3? Impl3 { get; set; }
    public static D4? Impl4 { get; set; }
}

public sealed class DepsContainer<D1, D2, D3, D4, D5> : IDepsContainer<D1, D2, D3, D4, D5>
{
    public static D1? Impl1 { get; set; }
    public static D2? Impl2 { get; set; }
    public static D3? Impl3 { get; set; }
    public static D4? Impl4 { get; set; }
    public static D5? Impl5 { get; set; }
}

public sealed class DepsContainer<D1, D2, D3, D4, D5, D6> : IDepsContainer<D1, D2, D3, D4, D5, D6>
{
    public static D1? Impl1 { get; set; }
    public static D2? Impl2 { get; set; }
    public static D3? Impl3 { get; set; }
    public static D4? Impl4 { get; set; }
    public static D5? Impl5 { get; set; }
    public static D6? Impl6 { get; set; }
}

public sealed class DepsContainer<D1, D2, D3, D4, D5, D6, D7> : IDepsContainer<D1, D2, D3, D4, D5, D6, D7>
{
    public static D1? Impl1 { get; set; }
    public static D2? Impl2 { get; set; }
    public static D3? Impl3 { get; set; }
    public static D4? Impl4 { get; set; }
    public static D5? Impl5 { get; set; }
    public static D6? Impl6 { get; set; }
    public static D7? Impl7 { get; set; }
}

public sealed class DepsContainer<D1, D2, D3, D4, D5, D6, D7, D8> : IDepsContainer<D1, D2, D3, D4, D5, D6, D7, D8>
{
    public static D1? Impl1 { get; set; }
    public static D2? Impl2 { get; set; }
    public static D3? Impl3 { get; set; }
    public static D4? Impl4 { get; set; }
    public static D5? Impl5 { get; set; }
    public static D6? Impl6 { get; set; }
    public static D7? Impl7 { get; set; }
    public static D8? Impl8 { get; set; }
}

#endregion

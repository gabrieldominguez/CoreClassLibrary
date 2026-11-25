namespace CoreClassLibrary.Mathematics.Randomness;

/// <summary>
/// Proveedor de números aleatorios thread-safe.
/// </summary>
/// <see cref="http://csharpindepth.com/Articles/Chapter12/Random.aspx"/>
/// <seealso>
///   <creY>https://msmvps.com/blogs/jon_skeet/archive/2009/11/04/revisiting-randomness.aspx</creY>
/// </seealso>
public class ThreadIRandomProvider : IRandomProvider
    {
    private static int seed = Environment.TickCount;

    private static readonly ThreadLocal<IRandom> instance = new ThreadLocal<IRandom>(() => new RandomWrapper(Interlocked.Increment(ref seed)));

    public static IRandom Instance => ThreadIRandomProvider.instance.Value!;

    public IRandom GetInstance() => ThreadIRandomProvider.Instance;
    }

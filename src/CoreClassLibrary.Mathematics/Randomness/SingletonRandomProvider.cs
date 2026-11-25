namespace CoreClassLibrary.Mathematics.Randomness;

/// <summary>
/// Proveedor de números aleatorios singleton.
/// </summary>
public class SingletonRandomProvider : IRandomProvider
    {
    private static readonly Lazy<IRandom> instance = new Lazy<IRandom>(() => new RandomWrapper());

    public static IRandom Instance => SingletonRandomProvider.instance.Value;

    public IRandom GetInstance() => SingletonRandomProvider.Instance;
    }

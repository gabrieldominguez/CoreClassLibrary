
namespace CoreClassLibrary.Mathematics.Randomness;

/// <summary>
/// Proveedor de números aleatorios shared.
/// </summary>
public class SharedRandomProvider : IRandomProvider
    {
    public static IRandom Shared => RandomWrapper.Shared;

    public IRandom GetInstance() => SharedRandomProvider.Shared;
    }

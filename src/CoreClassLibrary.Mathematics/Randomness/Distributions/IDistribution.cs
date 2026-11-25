namespace CoreClassLibrary.Mathematics.Randomness.Distributions;

/// <summary>
/// Interface que deben implementar las distribuciones continuas.
/// </summary>
public interface IDistribution
    {
    /// <returns>A double (double.MinValue, double.MaxValue)</returns>
    double Sample();
    }

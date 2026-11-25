namespace CoreClassLibrary.Mathematics.Randomness.Distributions;

/// <summary>
/// Interface que deben implementar las distribuciones discretas.
/// </summary>
public interface IDiscreteDistribution
    {
    uint DiscreteSample();
    }

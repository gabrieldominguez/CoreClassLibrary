namespace CoreClassLibrary.Mathematics.Randomness.Distributions.Continuous;

/// <summary>
/// Generates rayleigh distributed random numbers.
/// </summary>
/// <see cref="http://en.wikipedia.org/wiki/Rayleigh_distribution"/>
public class RayleighDistribution(IRandom random, double mean = 0.0, double standardDeviation = 1.0) : IDistribution
    {
    private readonly NormalDistribution normalDistribution1 = new NormalDistribution(random, mean, standardDeviation);
    private readonly NormalDistribution normalDistribution2 = new NormalDistribution(random, mean, standardDeviation);

    public double Sample() => Math.Sqrt(this.normalDistribution1.Sample().Pow2() + this.normalDistribution2.Sample().Pow2());
    }

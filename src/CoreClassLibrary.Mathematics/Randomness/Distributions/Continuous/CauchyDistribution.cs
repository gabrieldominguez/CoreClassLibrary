namespace CoreClassLibrary.Mathematics.Randomness.Distributions.Continuous;

/// <summary>
/// Generates cauchy distributed random numbers.
/// </summary>
/// <see cref="http://en.wikipedia.org/wiki/Cauchy_distribution"/>
/// <see cref="http://www.xycoon.com/cauchy2p_random.htm"/>
public class CauchyDistribution(IRandom random, double median = 1.0, double scale = 1.0) : IDistribution
    {
    public double Sample() => median + scale * Math.Tan(Math.PI * (random.Sample() - 0.5));
    }

namespace CoreClassLibrary.Mathematics.Randomness.Distributions.Continuous;

/// <summary>
/// Generates exponential distributed random numbers.
/// </summary>
/// <see cref="http://en.wikipedia.org/wiki/Exponential_distribution"/>
public class ExponentialDistribution(IRandom random, double lambda = 1.0) : IDistribution
    {
    public double Sample() => -Math.Log(random.PositiveSample()) / lambda;
    }

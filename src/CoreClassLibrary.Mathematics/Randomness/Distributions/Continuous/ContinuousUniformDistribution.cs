namespace CoreClassLibrary.Mathematics.Randomness.Distributions.Continuous;

/// <summary>
/// Generates continuous uniformly distributed random numbers.
/// </summary>
/// <see cref="http://en.wikipedia.org/wiki/Uniform_distribution_%28continuous%29"/>
public class ContinuousUniformDistribution(IRandom random, double a = double.MinValue, double b = double.MaxValue) : IDistribution
    {
    public double Sample() => random.Next(a, b);
    }

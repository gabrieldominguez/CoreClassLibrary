namespace CoreClassLibrary.Mathematics.Randomness.Distributions.Discrete;

/// <summary>
/// Generates binomial distributed random numbers.
/// </summary>
/// <see cref="http://en.wikipedia.org/wiki/binomial_distribution"/>
public class BinomialDistribution(IRandom random, double trueRate = 0.5, int trialsCount = 1) : IDiscreteDistribution
    {
    public uint DiscreteSample() => (uint)Enumerable.Range(0, trialsCount).Count(n => random.NextBool(trueRate));
    }

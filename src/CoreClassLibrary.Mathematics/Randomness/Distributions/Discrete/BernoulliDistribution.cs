namespace CoreClassLibrary.Mathematics.Randomness.Distributions.Discrete;

/// <summary>
/// Generates bernoulli distributed random numbers.
/// </summary>
/// <see cref="http://en.wikipedia.org/wiki/Bernoulli_distribution"/>
public class BernoulliDistribution(IRandom random, double trueRate = 0.5) : IDiscreteDistribution
    {
    public uint DiscreteSample() => random.NextBool(trueRate) ? 1U : 0U;
    }

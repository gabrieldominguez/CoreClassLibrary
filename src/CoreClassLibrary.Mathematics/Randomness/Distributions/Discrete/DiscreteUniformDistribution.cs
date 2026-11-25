namespace CoreClassLibrary.Mathematics.Randomness.Distributions.Discrete;

/// <summary>
/// Generates discrete uniformly distributed random numbers.
/// </summary>
/// <see cref="http://en.wikipedia.org/wiki/Uniform_distribution_%28discrete%29"/>
public class DiscreteUniformDistribution(IRandom random, uint a = 0, uint b = 1) : IDiscreteDistribution
    {
    private readonly uint b = b + 1;

    public uint DiscreteSample() => random.Next<uint>(a, this.b);
    }

namespace CoreClassLibrary.Mathematics.Randomness.Distributions.Discrete;

/// <summary>
/// Generate geometric distributed random numbers.
/// </summary>
/// <see cref="http://en.wikipedia.org/wiki/Geometric_distribution"/>
public class GeometricDistribution(IRandom random, double trueRate = 0.5) : IDiscreteDistribution
    {
    public uint DiscreteSample()
        {
        uint count = 1;

        while (random.NextBool(trueRate))
            {
            count++;
            }

        return count;
        }
    }

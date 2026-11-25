namespace CoreClassLibrary.Mathematics.Randomness.Distributions.Discrete;

/// <summary>
/// Generate poisson distributed random numbers.
/// </summary>
/// <see cref="http://en.wikipedia.org/wiki/Poisson_distribution"/>
public class PoissonDistribution(IRandom random, double lambda = 1.0) : IDiscreteDistribution
    {
    private readonly double e_lambda = Math.Exp(-lambda);

    public uint DiscreteSample()
        {
        uint count = 0;

        double product = random.Sample();
        while (product >= this.e_lambda)
            {
            product *= random.Sample();
            count++;
            }

        return count;
        }
    }

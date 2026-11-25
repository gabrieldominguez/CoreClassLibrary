namespace CoreClassLibrary.Mathematics.Randomness.Distributions.Continuous;

/// <summary>
/// Generates chi distributed random numbers.
/// </summary>
/// <see cref="http://en.wikipedia.org/wiki/Chi_distribution"/>
public class ChiDistribution(IRandom random, int alpha = 1) : ChiSquareDistribution(random, alpha)
    {
    public override double Sample() => Math.Sqrt(base.Sample());
    }

/// <summary>
/// Generates chi-squared distributed random numbers.
/// </summary>
/// <see cref="http://en.wikipedia.org/wiki/Chi_distribution"/>
public class ChiSquareDistribution(IRandom random, int alpha = 1) : IDistribution
    {
    public readonly int alpha = alpha;
    private readonly NormalDistribution normalDistribution = new NormalDistribution(random);

    public virtual double Sample()
        {
        double sum = 0.0;
        for (int i = 0; i < this.alpha; i++)
            {
            sum += this.normalDistribution.Sample().Pow2();
            }

        return sum;
        }
    }

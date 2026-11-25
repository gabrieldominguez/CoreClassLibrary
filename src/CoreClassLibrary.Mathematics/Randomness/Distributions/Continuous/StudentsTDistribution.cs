namespace CoreClassLibrary.Mathematics.Randomness.Distributions.Continuous;

/// <summary>
/// Generates t-distributed random numbers.
/// </summary>
/// <see cref="http://en.wikipedia.org/wiki/Student%27s_t-distribution"/>
/// <see cref="http://www.xycoon.com/stt_random.htm"/>
public class StudentsTDistribution(IRandom random, int n = 1) : IDistribution
    {
    private readonly NormalDistribution normalDistribution = new NormalDistribution(random);
    private readonly ChiSquareDistribution chiSquareDistribution = new ChiSquareDistribution(random, n);

    public double Sample() => this.normalDistribution.Sample() / Math.Sqrt(this.chiSquareDistribution.Sample() / n);
    }

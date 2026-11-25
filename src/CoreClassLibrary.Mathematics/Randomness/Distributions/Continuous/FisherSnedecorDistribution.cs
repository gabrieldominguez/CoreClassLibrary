namespace CoreClassLibrary.Mathematics.Randomness.Distributions.Continuous;

/// <summary>
/// Generates Fisher-Snedecor distributed random numbers.
/// </summary>
/// <see cref="http://en.wikipedia.org/wiki/F-distribution"/>
public class FisherSnedecorDistribution(IRandom random, int alpha = 1, int beta = 1) : IDistribution
    {
    private readonly ChiSquareDistribution chiSquareDistributionAlpha = new ChiSquareDistribution(random, alpha);
    private readonly ChiSquareDistribution chiSquareDistributionBeta = new ChiSquareDistribution(random, beta);

    public double Sample() => this.chiSquareDistributionAlpha.Sample() / this.chiSquareDistributionBeta.Sample() * this.chiSquareDistributionBeta.alpha / this.chiSquareDistributionAlpha.alpha;
    }

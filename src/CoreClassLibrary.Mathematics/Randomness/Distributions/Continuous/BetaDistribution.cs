namespace CoreClassLibrary.Mathematics.Randomness.Distributions.Continuous;

public class BetaPrimeDistribution(IRandom random, double alphaShape = 1.0, double betaShape = 1.0) : BetaDistribution(random, alphaShape, betaShape)
    {
    public override double Sample()
        {
        double x = base.Sample();

        return x / (1.0 - x);
        }
    }


/// <summary>
/// Generates beta distributed random numbers.
/// </summary>
/// <see cref="http://en.wikipedia.org/wiki/Beta_distribution"/>
/// <see cref="http://www.xycoon.com/beta_randomnumbers.htm"/>
public class BetaDistribution(IRandom random, double alphaShape = 1.0, double betaShape = 1.0) : IDistribution
    {
    private readonly GammaDistribution gammaDistributionAlpha = new GammaDistribution(random, alphaShape);
    private readonly GammaDistribution gammaDistributionBeta = new GammaDistribution(random, betaShape);

    public virtual double Sample()
        {
        double x = this.gammaDistributionAlpha.Sample();

        return x / (x + this.gammaDistributionBeta.Sample());
        }
    }

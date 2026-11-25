namespace CoreClassLibrary.Mathematics.Randomness.Distributions.Continuous;

/// <summary>
/// Generates laplace distributed random numbers.
/// </summary>
/// <see cref="http://en.wikipedia.org/wiki/Laplace_distribution"/>
public class LaplaceDistribution(IRandom random, double mean = 0.0, double scale = 1.0) : IDistribution
    {
    public double Sample()
        {
        double u = random.Next(-0.5, 0.5);
        return mean - scale * Math.Sign(u) * Math.Log(2.0 * Math.Abs(u));
        }
    }

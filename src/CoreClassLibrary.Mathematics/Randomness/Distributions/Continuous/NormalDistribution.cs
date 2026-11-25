namespace CoreClassLibrary.Mathematics.Randomness.Distributions.Continuous;

/// <summary>
/// Generates lognormal distributed random numbers.
/// </summary>
/// <see cref="http://en.wikipedia.org/wiki/Log-normal_distribution"/>
public class LogNormalDistribution(IRandom random, double mean = 0.0, double standardDeviation = 1.0) : NormalDistribution(random, mean, standardDeviation)
    {
    public override double Sample() => Math.Exp(base.Sample());
    }

/// <summary>
/// Returns the next pseudorandom, Gaussian ("normally") distributed double value with mean 0.0 and standard deviation 1.0 from this random number generator's sequence.
/// </summary>
/// <remarks>
/// From java library: http://docs.oracle.com/javase/7/docs/api/java/util/Random.html
/// 
/// This uses the polar method of G. E. P. Box, M. E. Muller, and G. Marsaglia, 
/// as described by Donald E. Knuth in The Art of Computer Programming, Volume 3: Seminumerical Algorithms, section 3.4.1, subsection C, algorithm P. 
/// 
/// Note that it generates two independent values at the cost of only one call to log and one call to sqrt.
/// </remarks>
/// <returns>The next pseudorandom, Gaussian ("normally") distributed double value with mean 0.0 and standard deviation 1.0 from this random number generator's sequence</returns>
/// <see cref="http://en.wikipedia.org/wiki/Normal_distribution"/>
public class NormalDistribution(IRandom random, double mean = 0.0, double standardDeviation = 1.0) : IDistribution
    {
    private double nextNextGaussian;
    private bool haveNextNextGaussian = false;

    public virtual double Sample()
        {
        double nextGaussian;
        if (this.haveNextNextGaussian)
            {
            this.haveNextNextGaussian = false;
            nextGaussian = this.nextNextGaussian;
            }
        else
            {
            double v1, v2, s;
            do
                {
                s = (v1 = random.Next(-1.0, 1.0)).Pow2() + (v2 = random.Next(-1.0, 1.0)).Pow2();
                }
            while (s >= 1 || s == 0);

            double multiplier = Math.Sqrt(-2 * Math.Log(s) / s);
            this.nextNextGaussian = v2 * multiplier;
            this.haveNextNextGaussian = true;
            nextGaussian = v1 * multiplier;
            }

        return mean + standardDeviation * nextGaussian;
        }
    }

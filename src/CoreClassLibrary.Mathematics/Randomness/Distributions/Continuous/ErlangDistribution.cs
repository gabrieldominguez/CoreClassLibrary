namespace CoreClassLibrary.Mathematics.Randomness.Distributions.Continuous;

/// <summary>
/// Generates erlang distributed random numbers.
/// </summary>
/// <see cref="http://en.wikipedia.org/wiki/Erlang_distribution"/>
/// <see cref="http://www.xycoon.com/erlang_random.htm"/>
public class ErlangDistribution(IRandom random, int shape = 1, double rate = 1.0) : IDistribution
    {
    public double Sample()
        {
        double product = 1.0;
        for (int i = 0; i < shape; i++)
            {
            product *= random.Sample();
            }

        // Subtract product from 1.0 to avoid Math.Log(0.0)
        return -Math.Log(1.0 - product) / rate;
        }
    }

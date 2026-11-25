namespace CoreClassLibrary.Mathematics.Randomness.Distributions.Continuous;

/// <summary>
/// Generates Fisher-Tippett distributed random numbers.
/// </summary>
public class FisherTippettDistribution(IRandom random, double mean = 0.0, double scale = 1.0) : IDistribution
    {
    public double Sample() => mean - scale * Math.Log(-Math.Log(random.PositiveSample()));
    }

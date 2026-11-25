namespace CoreClassLibrary.Mathematics.Randomness.Distributions.Continuous;

/// <summary>
/// Generates weibull distributed random numbers.
/// </summary>
/// <see cref="http://en.wikipedia.org/wiki/Weibull_distribution"/>
public class WeibullDistribution(IRandom random, double shape = 1.0, double scale = 1.0) : IDistribution
    {
    private readonly double invShape = 1.0 / shape;

    public double Sample() => scale * Math.Pow(-Math.Log(random.PositiveSample()), this.invShape);
    }

namespace CoreClassLibrary.Mathematics.Randomness.Distributions.Continuous;

/// <summary>
/// Generates pareto distributed random numbers.
/// </summary>
/// <see cref="http://en.wikipedia.org/wiki/Pareto_distribution"/>
/// <see cref="http://www.xycoon.com/par_random.htm"/>
public class ParetoDistribution(IRandom random, double shape = 1.0, double scale = 1.0) : IDistribution
    {
    private readonly double invShape = 1.0 / shape;

    public double Sample() => scale / Math.Pow(random.PositiveSample(), this.invShape);
    }

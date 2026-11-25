namespace CoreClassLibrary.Mathematics.Randomness.Distributions.Continuous;

/// <summary>
/// Generates power distributed random numbers.
/// </summary>
/// <see cref="http://www.xycoon.com/power.htm"/>
public class PowerDistribution(IRandom random, double shape = 1.0, double scale = 1.0) : IDistribution
    {
    private readonly double invShape = 1.0 / shape;

    public double Sample() => Math.Pow(random.Sample(), this.invShape) / scale;
    }

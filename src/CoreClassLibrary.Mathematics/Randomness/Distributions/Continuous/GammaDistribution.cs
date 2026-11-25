namespace CoreClassLibrary.Mathematics.Randomness.Distributions.Continuous;

/// <summary>
/// Inverse Gamma distribution
/// </summary>
/// <remarks>
/// If X is gamma(shape, scale) then
/// 1/Y is inverse gamma(shape, 1/scale)
/// </remarks>
public class InverseGammaDistribution(IRandom random, double shape = 1, double scale = 1) : GammaDistribution(random, shape, 1.0 / scale)
    {
    public override double Sample() => 1.0 / base.Sample();
    }

/// <summary>
/// Generates gamma distributed random numbers.
/// </summary>
/// <see cref="http://en.wikipedia.org/wiki/Gamma_distribution"/>
public class GammaDistribution : IDistribution
    {
    private readonly IRandom random;

    private readonly double shape;
    private readonly double scale;

    private readonly double delta;
    private readonly double v0;

    public GammaDistribution(IRandom random, double shape = 1.0, double scale = 1.0)
        {
        this.random = random;

        this.shape = shape;
        this.scale = scale;

        this.delta = this.shape - Math.Floor(this.shape);
        this.v0 = Math.E / (Math.E + this.delta);
        }

    public virtual double Sample()
        {
        double xi, eta;
        do
            {
            double gen1 = this.random.PositiveSample();
            double gen2 = this.random.PositiveSample();

            if (gen1 <= this.v0)
                {
                xi = Math.Pow(gen1 / this.v0, 1.0 / this.delta);
                eta = gen2 * Math.Pow(xi, this.delta - 1.0);
                }
            else
                {
                xi = 1.0 - Math.Log((gen1 - this.v0) / (1.0 - this.v0));
                eta = gen2 * Math.Pow(Math.E, -xi);
                }
            } 
        while (eta > Math.Pow(xi, this.delta - 1.0) * Math.Pow(Math.E, -xi));

        for (int i = 1; i <= this.shape; i++)
            {
            xi -= Math.Log(this.random.Sample());
            }

        return xi * this.scale;
        }
    }

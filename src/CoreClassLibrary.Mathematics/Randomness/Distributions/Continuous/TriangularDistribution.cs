namespace CoreClassLibrary.Mathematics.Randomness.Distributions.Continuous;

/// <summary>
/// Generates triangular distributed random numbers.
/// </summary>
/// <see cref="http://en.wikipedia.org/wiki/Triangular_distribution"/>
public class TriangularDistribution : IDistribution
    {
    private readonly IRandom random;

    private readonly double a;
    private readonly double b;

    private readonly double fc;

    private readonly double sqrt_ba_ca;
    private readonly double sqrt_ba_bc;

    public TriangularDistribution(IRandom random, double a = 0.0, double b = 1.0, double c = 0.5)
        {
        this.random = random;

        double b_a = (this.b = b) - (this.a = a);
        double c_a = c - a;
        
        this.fc = b_a / c_a;

        this.sqrt_ba_ca = Math.Sqrt(b_a * c_a);
        this.sqrt_ba_bc = Math.Sqrt(b_a * (b - c));
        }

    public double Sample()
        {
        double u = this.random.Sample();

        return u < this.fc?
            this.a + Math.Sqrt(u) * this.sqrt_ba_ca:
            this.b - Math.Sqrt(1-u) * this.sqrt_ba_bc;
        }
    }

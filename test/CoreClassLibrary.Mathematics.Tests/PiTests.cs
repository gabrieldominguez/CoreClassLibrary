
using CoreClassLibrary.Extensions;
using CoreClassLibrary.Mathematics.Distances;
using CoreClassLibrary.Mathematics.Randomness;
using Xunit;

namespace CoreClassLibrary.Mathematics.Tests;

/// <summary>
/// Pi
/// </summary>
/// <see cref="https://mathworld.wolfram.com/PiFormulas.html"/>
/// <see cref="https://es.wikipedia.org/wiki/N%C3%BAmero_%CF%80"/>
/// <see cref="https://en.wikipedia.org/wiki/Approximations_of_%CF%80"/>
/// <see cref="https://stackoverflow.com/questions/39395/how-do-i-calculate-pi-in-c"/>
public class PiTests(ITestOutputHelper output)
    {
    [Fact]
    public void Mesopotamia() => this.AssertOutput(3 + 1.0 / 7, 2);

    [Fact]
    public void Egipto() => this.AssertOutput(256.0 / 81, 0);

    [Fact]
    public void Vitruvio() => this.AssertOutput(25.0 / 8, 1);

    [Fact]
    public void Ptolomeo() => this.AssertOutput(377.0 / 120, 3);

    [Fact]
    public void ZhangHeng() => this.AssertOutput(Math.Sqrt(10), 0);

    [Fact]
    public void WangFang() => this.AssertOutput(142.0 / 45, 0);

    [Fact]
    public void ZuChongzhi1() => this.AssertOutput(22.0 / 7, 2);

    [Fact]
    public void ZuChongzhi2() => this.AssertOutput(355.0 / 113, 6);

    [Fact]
    public void Newton() => this.AssertOutput(6 * Math.Asin(0.5), 14);

    [Fact]
    public void Gregory() => this.AssertOutput(6 * Math.Atan(1.0 / Math.Sqrt(3)), 14);

    // PI = 2 * (1 + 1/3 * (1 + 2/5 * (1 + 3/7 * (...))))
    private static double F(int i, int max) => i == max? 1: 1 + i / (2.0 * i + 1) * PiTests.F(i + 1, max);

    [Theory]
    [InlineData(100, 14)]
    public void NewtonRecursiveSerie(int count, int precision) => this.AssertOutput(2 * PiTests.F(1, count), precision);

    [Theory]
    [InlineData(100, 1)]
    [InlineData(1000, 2)]
    [InlineData(10000, 3)]
    [InlineData(100000, 4)]
    public void Wallis(int count, int precision)
        {
        double pi = 2 * Enumerable.Range(1, count).Select(n => (2.0 * n / (2.0 * n - 1.0)) * (2.0 * n / (2.0 * n + 1.0))).Prod();

        this.AssertOutput(pi, precision);
        }

    /// <summary>
    /// Estimación del Valor de Pi.
    /// </summary>
    /// <remarks>
    /// pi^2 / 6 = sum(1/i^2) => pi = sqrt(6 * sum(1/i^2))
    /// </remarks>
    [Theory]
    [InlineData(100, 1)]
    [InlineData(1000, 2)]
    [InlineData(10000, 2)]
    public void EulerSerie(int count, int precision)
        {
        double pi = Math.Sqrt(6 * Enumerable.Range(1, count).Sum(i => 1.0 / (i * i)));

        this.AssertOutput(pi, precision);
        }

    [Theory]
    [InlineData(10, 2)]
    [InlineData(100, 14)]
    public void EulerSerie2(int count, int precision)
        {
        double pi = 1 + Enumerable.Range(1, count).Sum(i => Enumerable.Range(1, i).Prod() / Enumerable.Range(1, i).Select(i => i * 2.0 + 1.0).Prod());

        this.AssertOutput(2 * F(1, count), precision);
        }

    /// <summary>
    /// Gregory-Leibniz Serie
    /// </summary>
    /// <remarks>
    /// p/4 = 1 - 1/3 + 1/5 - 1/7 + 1/9 - ...
    /// </remarks>
    [Theory]
    [InlineData(100, 1)]
    [InlineData(1000, 2)]
    [InlineData(10000, 2)]
    public void GregoryLeibnizSerie(int count, int precision)
        {
        double pi = 4.0 * Enumerable.Range(0, count).Sum(i => 1.0 / (i * 2 + 1) * Math.Pow(-1, i));

        this.AssertOutput(pi, precision);
        }

    /// <summary>
    /// Nilakantha Serie
    /// </summary>
    /// <remarks>
    /// p = 3 + 4 / (2x3x4) - 4 / (4x5x6) + 4 / (6x7x8) - 4 / (8x9x10) + ...
    /// </remarks>
    [Theory]
    [InlineData(10, 2)]
    [InlineData(100, 5)]
    [InlineData(1000, 5)]
    public void NilakanthaSerie(int count, int precision)
        {
        double pi = 3 + Enumerable.Range(1, count).Sum(i => 4.0 / ((i*2) * ((i*2)+1) * ((i*2)+2)) * (-Math.Pow(-1, i)));

        this.AssertOutput(pi, precision);
        }

    /// <summary>
    /// Estimación del Valor de Pi usando la relacion entre las areas de un círculo inscrito en un cuadrado de lado 1.
    /// </summary>
    /// <remarks>
    /// Area circulo: Ao = pi r^2
    /// Area cuadrado: Ac = l^2
    /// l = 2r => Ac = (2r)^2 = 4 r^2 => r^2 = Ac / 4
    /// Ao = pi Ac / 4 => pi = 4 Ao / Ac
    /// </remarks>
    /// <see cref="https://www.geogebra.org/m/cF7RwK3H"/>
    [Theory]
    [InlineData(100, 0)]
    [InlineData(1000, 0)]
    [InlineData(10000, 0)]
    [InlineData(100000, 0)]
    [InlineData(1000000, 1)]
    [InlineData(10000000, 2)]
    public void RandomPi(int count, int precision)
        {
        double[] center = [0.5, 0.5];

        double pi = 4.0 *
            EnumerableExtensions.EnumerateInArrays(RandomWrapper.Shared.EnumerateSamples(TestContext.Current.CancellationToken), RandomWrapper.Shared.EnumerateSamples(TestContext.Current.CancellationToken))
            .Take(count)
            .Count(p => EuclideanDistance<double>.ComputeDistance(p, center) < 0.5) / count;

        this.AssertOutput(pi, precision);
        }

    private void AssertOutput(double pi, int precision)
        { 
        output.WriteLine(Math.PI.ToString());
        output.WriteLine(pi.ToString());
        output.WriteLine(Math.Abs(Math.PI - pi).ToString());

        Assert.Equal(Math.PI, pi, precision);
        }
    }

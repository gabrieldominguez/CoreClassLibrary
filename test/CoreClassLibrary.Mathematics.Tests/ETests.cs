using Xunit;

namespace CoreClassLibrary.Mathematics.Tests;

/// <summary>
/// e
/// </summary>
/// <see cref="https://es.wikipedia.org/wiki/N%C3%BAmero_e"/>
/// <see cref="https://stackoverflow.com/questions/43563912/calculating-e-number-c-sharp"/>
public class ETests(ITestOutputHelper output)
    {
    [Fact]
    public void Exp() => this.AssertOutput(Math.Exp(1.0), 15);

    [Theory]
    [InlineData(10, 0)]
    [InlineData(100, 1)]
    [InlineData(1_000, 2)]
    [InlineData(10_000, 3)]
    [InlineData(100_000, 4)]
    [InlineData(1_000_000, 5)]
    public void EulerNumberLimit(int n, int precision)
        {
        double e = Math.Pow(1.0 + 1.0 / n, n);

        this.AssertOutput(e, precision);
        }

    [Theory]
    [InlineData(5, 1)]
    [InlineData(6, 2)]
    [InlineData(7, 3)]
    [InlineData(8, 4)]
    [InlineData(10, 6)]
    [InlineData(15, 11)]
    [InlineData(20, 13)]
    // https://stackoverflow.com/questions/43563912/calculating-e-number-c-sharp
    public void Taylor(int count, int precision)
        {
        double e = 1.0;
        double item = 1.0;      

        for (int i = 1; i < count; i++)
            { 
            e += (item /= i);
            }

        this.AssertOutput(e, precision);
        }
    
    [Theory]
    [InlineData(10, 6)]
    [InlineData(20, 13)]
    public void Euler(int count, int precision)
        {
        double e = Enumerable.Range(0, count).Sum(n => 1.0 / Maths.Fact(n));

        this.AssertOutput(e, precision);
        }

    private void AssertOutput(double e, int precision)
        { 
        output.WriteLine(Math.E.ToString());
        output.WriteLine(e.ToString());
        output.WriteLine(Math.Abs(Math.E - e).ToString());

        Assert.Equal(Math.E, e, precision);
        }
    }

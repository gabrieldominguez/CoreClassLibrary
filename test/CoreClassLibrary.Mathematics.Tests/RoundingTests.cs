
using Xunit;

namespace CoreClassLibrary.Mathematics.Tests;

public class RoundingTests
    {
    [Theory]
    [InlineData(0.50004, 0.5000)]
    [InlineData(0.50005, 0.5000)]
    [InlineData(0.50006, 0.5001)]
    [InlineData(0.50014, 0.5001)]
    [InlineData(0.50015, 0.5002)]
    [InlineData(0.50016, 0.5002)]
    public void Round(decimal d, decimal expected)
        {
        Assert.Equal(expected, Math.Round(d, 4));   // MidpointRounding.ToEven
        }

    [Theory]
    [InlineData(0.50004, 0.5000)]
    [InlineData(0.50005, 0.5001)]
    [InlineData(0.50006, 0.5001)]
    [InlineData(0.50014, 0.5001)]
    [InlineData(0.50015, 0.5002)]
    [InlineData(0.50016, 0.5002)]
    public void RoundAwayFromZero(decimal d, decimal expected)
        {
        Assert.Equal(expected, Math.Round(d, 4, MidpointRounding.AwayFromZero));
        }

    [Theory]
    [InlineData(3.45, 1, MidpointRounding.ToEven, 3.4)]
    [InlineData(3.45, 1, MidpointRounding.AwayFromZero, 3.5)]
    [InlineData(3.47, 1, MidpointRounding.ToZero, 3.4)]
    [InlineData(-3.45, 1, MidpointRounding.ToEven, -3.4)]
    [InlineData(-3.45, 1, MidpointRounding.AwayFromZero, -3.5)]
    [InlineData(-3.47, 1, MidpointRounding.ToZero, -3.4)]
    public void TestDecimalRounding(decimal value, int precision, MidpointRounding mode, decimal expected)
        {
        Assert.Equal(expected, Math.Round(value, precision, mode));
        }

    [Theory]
    [InlineData(12.30, 0.25, 12.25)]
    [InlineData(12.40, 0.25, 12.50)]
    [InlineData(12.70, 0.25, 12.75)]
    [InlineData(12.90, 0.25, 13.00)]
    [InlineData(12.40, 0.50, 12.50)]
    [InlineData(12.70, 0.50, 12.50)]
    [InlineData(12.90, 0.50, 13.00)]
    public void RoundRange(decimal d, decimal scale, decimal expected)
        {
        Assert.Equal(expected, d.RoundRange(scale));   // MidpointRounding.ToEven
        }

    [Theory]
    [InlineData(12.30, 0.25, 12.25)]
    [InlineData(12.40, 0.25, 12.25)]
    [InlineData(12.70, 0.25, 12.50)]
    [InlineData(12.90, 0.25, 12.75)]
    [InlineData(12.40, 0.50, 12.00)]
    [InlineData(12.70, 0.50, 12.50)]
    [InlineData(12.90, 0.50, 12.50)]
    public void TruncateRange(decimal d, decimal scale, decimal expected)
        {
        Assert.Equal(expected, d.TruncateRange(scale));   // MidpointRounding.ToEven
        }

    [Theory]
    [InlineData(12.35, 0, 12)]
    [InlineData(12.35, 1, 12.3)]
    [InlineData(12.35, 2, 12.35)]
    [InlineData(12.35, 3, 12.35)]
    public void Truncate(decimal d, int digits, decimal expected)
        {
        Assert.Equal(expected, d.Truncate(digits));   // MidpointRounding.ToEven
        }    
    }

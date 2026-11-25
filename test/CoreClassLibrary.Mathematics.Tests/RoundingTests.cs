
using Xunit;

namespace CoreClassLibrary.Mathematics.Tests;

public class RoundingTests
    {
    public static TheoryData<decimal, decimal> RoundData => new()
        {
        { 0.50004M, 0.5000M },
        { 0.50005M, 0.5000M },
        { 0.50006M, 0.5001M },
        { 0.50014M, 0.5001M },
        { 0.50015M, 0.5002M },
        { 0.50016M, 0.5002M }
        };

    [Theory]
    [MemberData(nameof(RoundData))]
    public void Round(decimal d, decimal expected)
        {
        Assert.Equal(expected, Math.Round(d, 4));   // MidpointRounding.ToEven
        }

    public static TheoryData<decimal, decimal> RoundAwayFromZeroData => new()
        {
        { 0.50004M, 0.5000M },
        { 0.50005M, 0.5001M },
        { 0.50006M, 0.5001M },
        { 0.50014M, 0.5001M },
        { 0.50015M, 0.5002M },
        { 0.50016M, 0.5002M }
        };

    [Theory]
    [MemberData(nameof(RoundAwayFromZeroData))]
    public void RoundAwayFromZero(decimal d, decimal expected)
        {
        Assert.Equal(expected, Math.Round(d, 4, MidpointRounding.AwayFromZero));
        }

    public static TheoryData<decimal, int, MidpointRounding, decimal> DecimalRoundingData => new()
        {
        { 3.45M, 1, MidpointRounding.ToEven, 3.4M },
        { 3.45M, 1, MidpointRounding.AwayFromZero, 3.5M },
        { 3.47M, 1, MidpointRounding.ToZero, 3.4M },
        { -3.45M, 1, MidpointRounding.ToEven, -3.4M },
        { -3.45M, 1, MidpointRounding.AwayFromZero, -3.5M },
        { -3.47M, 1, MidpointRounding.ToZero, -3.4M }
        };

    [Theory]
    [MemberData(nameof(DecimalRoundingData))]
    public void TestDecimalRounding(decimal value, int precision, MidpointRounding mode, decimal expected)
        {
        Assert.Equal(expected, Math.Round(value, precision, mode));
        }

    public static TheoryData<decimal, decimal, decimal> RoundRangeData => new()
        {
        { 12.30M, 0.25M, 12.25M },
        { 12.40M, 0.25M, 12.50M },
        { 12.70M, 0.25M, 12.75M },
        { 12.90M, 0.25M, 13.00M },
        { 12.40M, 0.50M, 12.50M },
        { 12.70M, 0.50M, 12.50M },
        { 12.90M, 0.50M, 13.00M }
        };

    [Theory]
    [MemberData(nameof(RoundRangeData))]
    public void RoundRange(decimal d, decimal scale, decimal expected)
        {
        Assert.Equal(expected, d.RoundRange(scale));   // MidpointRounding.ToEven
        }

    public static TheoryData<decimal, decimal, decimal> TruncateRangeData => new()
        {
        { 12.30M, 0.25M, 12.25M },
        { 12.40M, 0.25M, 12.25M },
        { 12.70M, 0.25M, 12.50M },
        { 12.90M, 0.25M, 12.75M },
        { 12.40M, 0.50M, 12.00M },
        { 12.70M, 0.50M, 12.50M },
        { 12.90M, 0.50M, 12.50M }
        };

    [Theory]
    [MemberData(nameof(TruncateRangeData))]
    public void TruncateRange(decimal d, decimal scale, decimal expected)
        {
        Assert.Equal(expected, d.TruncateRange(scale));   // MidpointRounding.ToEven
        }

    public static TheoryData<decimal, int, decimal> TruncateData => new()
        {
        { 12.35M, 0, 12M },
        { 12.35M, 1, 12.3M },
        { 12.35M, 2, 12.35M },
        { 12.35M, 3, 12.35M }
        };

    [Theory]
    [MemberData(nameof(TruncateData))]
    public void Truncate(decimal d, int digits, decimal expected)
        {
        Assert.Equal(expected, d.Truncate(digits));   // MidpointRounding.ToEven
        }    
    }

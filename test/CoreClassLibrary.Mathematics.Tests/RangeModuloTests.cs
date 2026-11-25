
using CoreClassLibrary.Objects.Ranges;
using Xunit;

namespace CoreClassLibrary.Mathematics.Tests;

public class RangeTests
    {
    [Theory]
    [InlineData(0, 10, 5, 5)]
    [InlineData(0, 10, 15, 4)]
    [InlineData(5, 10, 0, 5)]
    [InlineData(5, 10, 1, 6)]
    [InlineData(5, 10, 2, 7)]
    [InlineData(5, 10, 3, 8)]
    [InlineData(5, 10, 4, 9)]
    [InlineData(5, 10, 5, 10)]
    [InlineData(5, 10, 6, 5)]
    [InlineData(5, 10, 7, 6)]
    [InlineData(5, 10, 8, 7)]
    public void RangeModulo(long start, long end, long input, long output)
        {
        Range<long> range = new Range<long>(start, end);
        Assert.Equal(output, range.Modulo(input));
        }
    }

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CA1861 // Avoid constant arrays as arguments

using CoreClassLibrary.Mathematics.Statistics;
using Xunit;

namespace CoreClassLibrary.Mathematics.Tests;

/// <summary>
/// Contains unit tests for verifying the behavior of the <see cref="Extensions.ComputeMeanAndStandardDeviation"/> method.
/// </summary>
/// <remarks>
/// This class includes tests to ensure the correctness of the average and standard deviation
/// calculations for various input scenarios, including valid inputs, edge cases such as empty arrays, and arrays with a
/// single value. The tests use the xUnit framework and include both [Fact] and [Theory] attributes for comprehensive
/// coverage.
/// </remarks>
/// <see cref="Extensions.ComputeMeanAndStandardDeviation"/>
/// <see cref="https://www.calculator.net/standard-deviation-calculator.html"/>
/// <see cref="https://en.wikipedia.org/wiki/Standard_deviation"/>
/// <see cref="https://en.wikipedia.org/wiki/Algorithms_for_calculating_variance"/>"
/// <see cref="https://en.wikipedia.org/wiki/Mean"/>
/// <see cref="https://en.wikipedia.org/wiki/Population_and_sample"/>
public class StatsTests
    {
    [Theory]
    [InlineData(new double[] { 1.0, 2.0, 3.0 }, 2.0)]     // (1+3)/2 = 2
    [InlineData(new double[] { -1.0, -2.0, -3.0 }, -2.0)] // (-1-3)/2 = -2
    [InlineData(new double[] { -5.0, 5.0 }, 0.0)]         // (-5+5)/2 = 0
    [InlineData(new double[] { 7.0 }, 7.0)]               // Single element: midrange = that element
    public void MidRange_ReturnsExpected(double[] values, double expected)
        {
        Assert.Equal(expected, values.MidRange<double>(), precision: 5);
        }

    [Fact]
    public void MidRange_EmptySequence_Throws()
        {
        Assert.Throws<InvalidOperationException>(() => Array.Empty<double>().MidRange<double>());
        }

    [Theory]
    [InlineData(new double[] { 2, 4, 4, 4, 5, 5, 7, 9 }, 8, 40)]
    [InlineData(new double[] { 10, 12, 23, 23, 16, 23, 21, 16 }, 8, 144)]
    [InlineData(new double[] { 25, 27, 23, 26.5, 24.5 }, 5, 25.2 * 5)]
    [InlineData(new double[] { 1, 2, 3, 4, 5 }, 5, 15)]
    [InlineData(new double[] { 1, 1, 1, 1 }, 4, 4)]
    [InlineData(new double[] { 10 }, 1, 10)]
    [InlineData(new double[] { }, 0, 0)]
    [InlineData(new double[] { 1, 2, 3 }, 3, 6)]
    [InlineData(new double[] { -1, -2, -3 }, 3, -6)]
    public void CountAndSum(double[] values, int expectedCount, double expectedSum)
        {
        // Act
        (int Count, double Sum) result = values.CountAndSum();

        // Assert
        Assert.Equal(expectedCount, result.Count);
        Assert.Equal(expectedSum, result.Sum, precision: 5);
        }    

    [Theory]
    [InlineData(new double[] { 2, 4, 4, 4, 5, 5, 7, 9 }, 5)]
    [InlineData(new double[] { 10, 12, 23, 23, 16, 23, 21, 16 }, 18)]
    [InlineData(new double[] { 25, 27, 23, 26.5, 24.5 }, 25.2)]
    [InlineData(new double[] { 1, 2, 3, 4, 5 }, 3)]
    [InlineData(new double[] { 1, 1, 1, 1 }, 1)]
    [InlineData(new double[] { 10 }, 10)]
    [InlineData(new double[] { 1, 2, 3 }, 2)]
    [InlineData(new double[] { -1, -2, -3 }, -2)]
    public void Mean(double[] values, double expectedMean)
        {
        Assert.Equal(expectedMean, values.Mean(), precision: 5);
        } 
    
    [Theory]
    [InlineData(new double[] { 2, 4, 4, 4, 5, 5, 7, 9 }, 4)]
    [InlineData(new double[] { 10, 12, 23, 23, 16, 23, 21, 16 }, 24)]
    [InlineData(new double[] { 25, 27, 23, 26.5, 24.5 }, 2.06)]
    [InlineData(new double[] { 1, 2, 3, 4, 5 }, 2)]
    [InlineData(new double[] { 1, 1, 1, 1 }, 0)]
    [InlineData(new double[] { 10 }, 0)]
    [InlineData(new double[] { 1, 2, 3 }, 0.666666)]
    [InlineData(new double[] { -1, -2, -3 }, 0.666666)]
    public void Variance(double[] values, double expectedVariance)
        {
        Assert.Equal(expectedVariance, values.ComputeMeanAndVariance().Variance, precision: 5);
        } 

    [Theory]
    [InlineData(new double[] { 2, 4, 4, 4, 5, 5, 7, 9 }, 2)]
    [InlineData(new double[] { 10, 12, 23, 23, 16, 23, 21, 16 }, 4.8989794855664)]
    [InlineData(new double[] { 25, 27, 23, 26.5, 24.5 }, 1.4352700094407)]
    [InlineData(new double[] { 1, 2, 3, 4, 5 }, 1.4142135623731)]
    [InlineData(new double[] { 1, 1, 1, 1 }, 0)]
    [InlineData(new double[] { 10 }, 0)]
    [InlineData(new double[] { 1, 2, 3 }, 0.81649658092773)]
    [InlineData(new double[] { -1, -2, -3 }, 0.81649658092773)]
    public void StandardDeviation(double[] values, double expectedStandardDeviation)
        {
        Assert.Equal(expectedStandardDeviation, values.ComputeMeanAndStandardDeviation().StandardDeviation, precision: 5);
        } 

    [Fact]
    public void ComputeMeanAndStandardDeviation_WithValidInput_ReturnsCorrectValues()
        {
        // Arrange
        float[] values = [2, 4, 4, 4, 5, 5, 7, 9];

        // Act
        (float Mean, float StandardDeviation) result = values.ComputeMeanAndStandardDeviation();

        // Assert
        Assert.Equal(5.0, result.Mean, precision: 5);
        Assert.Equal(2.0, result.StandardDeviation, precision: 5);
        }

    [Fact]
    public void ComputeMeanAndStandardDeviation_EmptyList_ThrowsInvalidOperationException()
        {
        // Arrange
        float[] empty = [];

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => empty.ComputeMeanAndStandardDeviation());
        }

    [Fact]
    public void ComputeMeanAndStandardDeviation_WithSingleValue_ReturnsZeroDeviation()
        {
        // Arrange
        float[] values = [ 10 ];

        // Act
        (float Mean, float StandardDeviation) result = values.ComputeMeanAndStandardDeviation();

        // Assert
        Assert.Equal(10.0, result.Mean);
        Assert.Equal(0.0, result.StandardDeviation);
        }

    [Theory]
    [InlineData(new double[] { 1, 2, 3, 4, 5 }, new double[] { 1, 1.5, 2, 2.5, 3 })]
    [InlineData(new double[] { 10, 20, 30 }, new double[] { 10, 15, 20 })]
    [InlineData(new double[] { -1, -2, -3 }, new double[] { -1, -1.5, -2 })]
    [InlineData(new double[] { 5 }, new double[] { 5 })]
    [InlineData(new double[] { }, new double[] { })]
    public void MovingMean_Double_ReturnsExpected(double[] values, double[] expected)
        {
        Assert.Equal(expected, values.MovingMean());
        }

    [Theory]
    [InlineData(new int[] { 1, 2, 3, 4, 5 }, new int[] { 1, 1, 2, 2, 3 })]
    [InlineData(new int[] { 10, 20, 30 }, new int[] { 10, 15, 20 })]
    [InlineData(new int[] { -1, -2, -3 }, new int[] { -1, -1, -2 })]
    [InlineData(new int[] { 5 }, new int[] { 5 })]
    [InlineData(new int[] { }, new int[] { })]
    public void MovingMean_Int_ReturnsExpected(int[] values, int[] expected)
        {
        Assert.Equal(expected, values.MovingMean());
        }
    }

#pragma warning disable IDE0305 // Simplify collection initialization

using CoreClassLibrary.Extensions;
using CoreClassLibrary.Mathematics.Randomness;
using CoreClassLibrary.RegularExpressions;
using CoreClassLibrary.Objects.Pooling;
using CoreClassLibrary.Objects.Ranges;
using Xunit;
using CoreClassLibrary.Tests;

namespace CoreClassLibrary.Mathematics.Tests;

public class RandomTests(ITestOutputHelper output)
    {
    [Fact]
    public void RNG()
        {
        byte[] expected = [0x01, 0x02, 0x03, 0x01, 0x02, 0x03, 0x01, 0x02];

        TestRandomNumberGenerator testRNG = new TestRandomNumberGenerator([0x01, 0x02, 0x03]);

        byte[] actual = new byte[expected.Length];

        testRNG.GetBytes(actual);

        Assert.Equal(expected, actual);
        }

    [Fact]
    public void IRandom()
        {
        double[] expected = [0.1, 0.2, 0.3, 0.1, 0.2, 0.3, 0.1];

        TestRandom testRandom = new TestRandom([0.1, 0.2, 0.3]);

        double[] actual = testRandom.EnumerateSamples(cancellationToken: TestContext.Current.CancellationToken).Take(expected.Length).ToArray();

        Assert.Equal(expected, actual);
        }

    [Fact]
    public void IDiscreteRandom()
        {
        uint[] expected = [1, 2, 3, 1, 2, 3, 1];

        TestDiscreteRandom testDiscreteRandom = new TestDiscreteRandom([1, 2, 3]);

        uint[] actual = testDiscreteRandom.EnumerateDiscreteSamples(cancellationToken: TestContext.Current.CancellationToken).Take(expected.Length).ToArray();

        Assert.Equal(expected, actual);
        }

    [Theory]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(10)]
    public void IntRandom(int maxValue)
        {
        int[] expected = [1, 2, 3, 1, 2, 3, 1];

        TestIntRandom testIntRandom = new TestIntRandom([1, 2, 3], maxValue);

        int[] actual = EnumerableExtensions.Repeat(() => testIntRandom.Next(maxValue), expected.Length).ToArray();

        Assert.Equal(expected, actual);
        }

    [Theory]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(11)]
    public void IntRandomRange(int count)
        {
        Range<int> outerRange = new Range<int>(0, 10);
        Range<int> innerRange = RandomWrapper.Shared.NextRange(outerRange, count);

        output.WriteLine(innerRange.ToString());

        Assert.True(outerRange.Contains(innerRange));
        }

    [Fact]
    public void TestLoremIpsumFile()
        {
        using PooledStringBuilder builder = new PooledStringBuilder();
        foreach (string word in File.ReadAllText("LoremIpsum.txt").ToLowerInvariant().GetWords().Distinct())
            {
            builder.StringBuilder.Append('"').Append(word).Append("\", ");
            }
        output.WriteLine(builder.StringBuilder.ToString());
        }

    [Theory]
    [InlineData(1000)]
    public void TestLoremIpsum(int wordsCount)
        => output.WriteLine(RandomWrapper.Shared.LoremIpsum(wordsCount));

    [Fact]
    public void Shuffle()
        {
        for (int i = 0; i < 10; i++)
            {
            int[] array = Enumerable.Range(0, 10).ToArray();
            RandomWrapper.Shared.Shuffle<int>(array);
            output.WriteEnumerable(array);
            }
        }
    
    [Fact]
    public void ChiSquareTest()
        {
        int r = 100;
        float result = RandomWrapper.Shared.ChiSquareTest(1000, r);
        output.WriteLine(result.ToString());
        Assert.Equal(result, r, r * 0.25);
        }
    }

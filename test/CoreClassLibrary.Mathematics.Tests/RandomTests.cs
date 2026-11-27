#pragma warning disable IDE0305 // Simplify collection initialization

using CoreClassLibrary.Extensions;
using CoreClassLibrary.Mathematics.Randomness;
using CoreClassLibrary.Objects.Pooling;
using CoreClassLibrary.Objects.Ranges;
using CoreClassLibrary.RegularExpressions;
using CoreClassLibrary.Tests;
using Xunit;

namespace CoreClassLibrary.Mathematics.Tests;

public class RandomTests(ITestOutputHelper output)
    {
    [Theory]
    [InlineData(10, 100)]
    [InlineData(10, 1000)]
    [InlineData(10, 10000)]
    [InlineData(10, 100000)]
    public void PowerOfTwoRandomChoices(int serverCount, int requestCount)
        {
        int[] randomLoads = new int[serverCount];
        int[] powerOfTwoLoads = new int[serverCount];

        for (int i = 0; i < requestCount; i++)
            {
            randomLoads[RandomWrapper.Shared.NextIndex(randomLoads)]++;
            powerOfTwoLoads[RandomWrapper.Shared.NextMinOfTwoIndex(powerOfTwoLoads)]++;
            }

        int randomMinLoad = randomLoads.Min();
        int randomMaxLoad = randomLoads.Max();
        int powerOfTwoMinLoad = powerOfTwoLoads.Min();
        int powerOfTwoMaxLoad = powerOfTwoLoads.Max();

        output.WriteLine("Traditional Random load balancing");
        output.WriteEnumerable(randomLoads);
        output.WriteLine($"Min load: {randomMinLoad}");
        output.WriteLine($"Max load: {randomMaxLoad}");
        output.WriteLine($"Load difference: {randomMaxLoad - randomMinLoad}");
        output.WriteLine("");
        output.WriteLine("Power of Two Random Choices load balancing");
        output.WriteEnumerable(powerOfTwoLoads);
        output.WriteLine($"Min load: {powerOfTwoMinLoad}");
        output.WriteLine($"Max load: {powerOfTwoMaxLoad}");
        output.WriteLine($"Load difference: {powerOfTwoMaxLoad - powerOfTwoMinLoad}");

        Assert.True(powerOfTwoMinLoad > randomMinLoad);
        Assert.True(powerOfTwoMaxLoad < randomMaxLoad);
        }

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

    [Fact]
    public void ShuffleArray()
        {
        int[] array = Enumerable.Range(0, 9).AsArray();

        RandomWrapper.Shared.Shuffle<int>(array);

        output.WriteEnumerable(array);
        Assert.NotEqual(Enumerable.Range(0, 9).AsArray(), array);
        }

    [Fact]
    public void ShuffleSpan()
        {
        int[] array = Enumerable.Range(0, 9).AsArray();
        Span<int> span = array.AsSpan();

        RandomWrapper.Shared.Shuffle<int>(span);

        output.WriteEnumerable(array);
        Assert.NotEqual(Enumerable.Range(0, 9).AsArray(), array);
        }

    [Fact]
    public void ShuffleSpan2()
        {
        int[] array = Enumerable.Range(0, 9).AsArray();
        Span<int> span = array.AsSpan();

        RandomWrapper.Shared.Shuffle<int>(span);

        output.WriteEnumerable(array);
        Assert.False(array.IsOrdered());
        }

    [Fact]
    public void IsAlphaNumeric()
        {
        Assert.True(RandomWrapper.Shared.NextAlphaNumericString(10).IsAlphaNumeric());
        Assert.False(RandomWrapper.Shared.NextSymbolsString(10).IsAlphaNumeric());
        }

    [Fact]
    public void ContainsSymbols()
        {
        Assert.True(RandomWrapper.Shared.NextSymbolsString(10).ContainsSymbols());
        Assert.False(RandomWrapper.Shared.NextAlphaNumericString(10).ContainsSymbols());
        }
    }

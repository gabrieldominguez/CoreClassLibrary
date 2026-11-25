
using Xunit;

namespace CoreClassLibrary.Mathematics.Tests;

public class ShuffleTests
	{
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
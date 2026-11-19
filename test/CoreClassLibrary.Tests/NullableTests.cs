
using CoreClassLibrary.Objects.Disposables;
using Xunit;

namespace CoreClassLibrary.Tests;

public class NullableTests
    {
    [Fact]
    public void NotNullPatternMatchingTest()
        {
        Assert.Equal(-1, new Data(          ).Num is { } num1 ? num1.N : -1);
        Assert.Equal( 0, new Data(new Num( )).Num is { } num2 ? num2.N : -1);
        Assert.Equal( 1, new Data(new Num(1)).Num is { } num3 ? num3.N : -1);
        }

    [Fact]
    public void TestNullDisposable()
        {
        using (NullDisposable.Instance) { }
        }

    public record Data (Num? Num = null);
    public record Num (int? N = 0);

    [Fact]
    public void NullBoolean()
        {
        Assert.False(new Data().Num?.N < 0);
        Assert.False(new Data().Num?.N > 0);

        Assert.NotEqual(0, new Data().Num?.N);

        Assert.NotEqual(0, new Data(new Num(null)).Num?.N);

        Assert.Equal(0, new Data(new Num()).Num?.N);
        Assert.Equal(0, new Data(new Num(0)).Num?.N);
        }

    [Fact]
    public void NullNumber()
        {
        Assert.Equal(0, default(float));
        Assert.Null(default(float?));
        }
    }

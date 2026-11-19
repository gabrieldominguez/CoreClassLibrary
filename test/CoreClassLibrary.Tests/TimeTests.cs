
using Microsoft.Extensions.Time.Testing;
using CoreClassLibrary.Extensions;
using CoreClassLibrary.Objects.Ranges;
using Xunit;

namespace CoreClassLibrary.Tests;

public class TimeTests(ITestOutputHelper output)
    {
    [Fact]
    public void FakeTimeProvider()
        {
        DateTimeOffset dateTimeOffset = new DateTimeOffset(1975, 11, 12, 12, 0, 0, TimeSpan.FromHours(-3));
        FakeTimeProvider fakeTimeProvider = new FakeTimeProvider(dateTimeOffset);

        Assert.Equal(dateTimeOffset, fakeTimeProvider.GetLocalNow());
        Assert.Equal(dateTimeOffset.UtcDateTime, fakeTimeProvider.GetUtcNow());

        TimeSpan increment = TimeSpan.FromSeconds(1);
        fakeTimeProvider.Advance(increment);
        DateTimeOffset dateTimeOffset2 = dateTimeOffset.Add(increment);

        Assert.Equal(dateTimeOffset2, fakeTimeProvider.GetLocalNow());
        Assert.Equal(dateTimeOffset2.UtcDateTime, fakeTimeProvider.GetUtcNow());
        }

    [Fact]
    public void FakeTimeProviderAutoAdvanceAmount()
        {
        FakeTimeProvider fakeTimeProvider = new FakeTimeProvider
            {
            AutoAdvanceAmount = TimeSpan.FromSeconds(1)
            };

        DateTimeOffset defaultDateTimeOffset = fakeTimeProvider.GetLocalNow();
        Assert.Equal(new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero), defaultDateTimeOffset);

        int n = 10;
        for (int i = 0; i < n; i++)
            {
            output.WriteLine(fakeTimeProvider.GetLocalNow().ToLocalDateTimeString());
            }

        Assert.Equal(defaultDateTimeOffset.Add(fakeTimeProvider.AutoAdvanceAmount * (n+1)), fakeTimeProvider.GetLocalNow());
        }

    [Fact]
    public void TestTimeProvider()
        {
        output.WriteLine("GetLocalNow:");
        output.WriteLine(TimeProvider.System.GetLocalNow().ToString(DateTimeExtensions.DateTimeZoneFormat));

        output.WriteLine("");        
        output.WriteLine("GetUtcNow:");
        output.WriteLine(TimeProvider.System.GetUtcNow().ToString(DateTimeExtensions.DateTimeZoneFormat));

        output.WriteLine("");        
        output.WriteLine("LocalTimeZone:");
        output.WriteLine(TimeProvider.System.LocalTimeZone.ToString());

        output.WriteLine("");        
        output.WriteLine("TimestampFrequency:");
        output.WriteLine(TimeProvider.System.TimestampFrequency.ToString());

        long startingTimestamp = TimeProvider.System.GetTimestamp();
        output.WriteLine("");        
        output.WriteLine("Timestamp:");
        output.WriteLine(startingTimestamp.ToString());

        output.WriteLine("");        
        output.WriteLine("GetElapsedTime:");
        output.WriteLine(TimeProvider.System.GetElapsedTime(startingTimestamp).TotalMilliseconds.ToString());

        output.WriteLine("");        
        output.WriteLine("Timestamp:");
        output.WriteLine(TimeProvider.System.GetTimestamp().ToString());
        }    

    [Theory]
    [InlineData("01:00:00", "20:00:00", "05:00:00")]
    public void CircularRange(string time, string rangeStart, string rangeEnd)
        {
        Assert.True(new CircularRange<TimeSpan>(TimeSpan.Parse(rangeStart), TimeSpan.Parse(rangeEnd)).Contains(TimeSpan.Parse(time)));
        Assert.False(new Range<TimeSpan>(TimeSpan.Parse(rangeStart), TimeSpan.Parse(rangeEnd)).Contains(TimeSpan.Parse(time)));
        }

    // Display the TimeSpan value and the results of the 
    // Duration and Negate methods.
    private void ShowDurationNegate(TimeSpan interval)
        => output.WriteLine("{0,22}{1,22}{2,22}", interval, interval.Duration(), interval.Negate());

    // https://msdn.microsoft.com/en-us/library/system.timespan.duration(v=vs.110).aspx
    [Fact]
    public void LogInterval()
        {
        output.WriteLine("TimeSpan Duration() Negate()");
        output.WriteLine("-------- ---------- --------");

        this.ShowDurationNegate(new TimeSpan(1));
        this.ShowDurationNegate(new TimeSpan(-1234567));
        this.ShowDurationNegate(new TimeSpan(0, 0, 10, -20, -30));
        this.ShowDurationNegate(new TimeSpan(0, -10, 20, -30, 40));
        this.ShowDurationNegate(-new TimeSpan(1, 10, 20, 40, 160));
        this.ShowDurationNegate(-new TimeSpan(-10, -20, -30, -40, -50));
        }

    [Fact]
    public void Modulo()
        {
        TimeSpan a = TimeSpan.FromSeconds(90);
        TimeSpan b = TimeSpan.FromSeconds(20);

        Assert.Equal(new TimeSpan(0, 7, 30), TimeExtensions.Multiply(a, 5));
        Assert.Equal(4.5, TimeExtensions.Divide(a, b));
        Assert.Equal(new TimeSpan(0, 0, 10), a.Modulo(b));
        }

    [Fact]
    public void TimeSpanAreEqual()
        {
        TimeSpan midnight = TimeSpan.FromDays(1);
        TimeSpan oneSecBeforeMidnight = midnight.Subtract(TimeSpan.FromSeconds(1));
        TimeSpan oneSecAfterMidnight = midnight.Add(TimeSpan.FromSeconds(1));

        TimeSpan zeroMidnight = new TimeSpan();
        TimeSpan oneSecBeforeZeroMidnight = zeroMidnight.Subtract(TimeSpan.FromSeconds(1));
        TimeSpan oneSecAfterZeroMidnight = zeroMidnight.Add(TimeSpan.FromSeconds(1));

        TimeSpan insideWindow = TimeSpan.FromSeconds(1.1);
        TimeSpan outsideWindow = TimeSpan.FromSeconds(0.9);

        Assert.True(TimeExtensions.AreEqual(midnight, oneSecBeforeMidnight, insideWindow));
        Assert.True(TimeExtensions.AreEqual(midnight, oneSecAfterMidnight, insideWindow));
        Assert.False(TimeExtensions.AreEqual(midnight, oneSecBeforeMidnight, outsideWindow));
        Assert.False(TimeExtensions.AreEqual(midnight, oneSecAfterMidnight, outsideWindow));

        Assert.True(TimeExtensions.AreEqual(oneSecBeforeMidnight, midnight, insideWindow));
        Assert.True(TimeExtensions.AreEqual(oneSecAfterMidnight, midnight, insideWindow));
        Assert.False(TimeExtensions.AreEqual(oneSecBeforeMidnight, midnight, outsideWindow));
        Assert.False(TimeExtensions.AreEqual(oneSecAfterMidnight, midnight, outsideWindow));

        Assert.True(TimeExtensions.AreEqual(zeroMidnight, oneSecBeforeZeroMidnight, insideWindow));
        Assert.True(TimeExtensions.AreEqual(zeroMidnight, oneSecAfterZeroMidnight, insideWindow));
        Assert.False(TimeExtensions.AreEqual(zeroMidnight, oneSecBeforeZeroMidnight, outsideWindow));
        Assert.False(TimeExtensions.AreEqual(zeroMidnight, oneSecAfterZeroMidnight, outsideWindow));

        Assert.False(TimeExtensions.AreEqual(midnight, zeroMidnight, insideWindow));
        Assert.False(TimeExtensions.AreEqual(midnight, oneSecBeforeZeroMidnight, insideWindow));
        Assert.False(TimeExtensions.AreEqual(midnight, oneSecAfterZeroMidnight, insideWindow));
        Assert.False(TimeExtensions.AreEqual(zeroMidnight, oneSecBeforeMidnight, insideWindow));
        Assert.False(TimeExtensions.AreEqual(zeroMidnight, oneSecAfterMidnight, insideWindow));

        Assert.True(TimeExtensions.AreEqualDaily(midnight, zeroMidnight, insideWindow));
        Assert.True(TimeExtensions.AreEqualDaily(midnight, oneSecBeforeZeroMidnight, insideWindow));
        Assert.True(TimeExtensions.AreEqualDaily(midnight, oneSecAfterZeroMidnight, insideWindow));
        Assert.True(TimeExtensions.AreEqualDaily(zeroMidnight, oneSecBeforeMidnight, insideWindow));
        Assert.True(TimeExtensions.AreEqualDaily(zeroMidnight, oneSecAfterMidnight, insideWindow));

        Assert.False(TimeExtensions.AreEqualDaily(midnight, oneSecBeforeZeroMidnight, outsideWindow));
        Assert.False(TimeExtensions.AreEqualDaily(midnight, oneSecAfterZeroMidnight, outsideWindow));
        Assert.False(TimeExtensions.AreEqualDaily(zeroMidnight, oneSecBeforeMidnight, outsideWindow));
        Assert.False(TimeExtensions.AreEqualDaily(zeroMidnight, oneSecAfterMidnight, outsideWindow));

        Assert.True(TimeExtensions.AreEqualDaily(oneSecBeforeZeroMidnight, midnight, insideWindow));
        Assert.True(TimeExtensions.AreEqualDaily(oneSecAfterZeroMidnight, midnight, insideWindow));
        Assert.True(TimeExtensions.AreEqualDaily(oneSecBeforeMidnight, zeroMidnight, insideWindow));
        Assert.True(TimeExtensions.AreEqualDaily(oneSecAfterMidnight, zeroMidnight, insideWindow));
        }


    }

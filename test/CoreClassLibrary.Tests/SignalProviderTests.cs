
using Microsoft.Extensions.Time.Testing;
using CoreClassLibrary.Extensions;
using CoreClassLibrary.Signaling.Handlers;
using CoreClassLibrary.Signaling.Providers;
using Xunit;

namespace CoreClassLibrary.Tests;

/// <summary>
/// SignalProviderTests
/// </summary>
/// <example>
///     DailySignalProvider locksEvictionSignalProvider = new DailySignalProvider();
///     locksEvictionSignalProvider.Signaled += (sender, args) => locksDictionary.Clear();
/// </example>
/// <see cref="https://andrewlock.net/exploring-the-dotnet-8-preview-avoiding-flaky-tests-with-timeprovider-and-itimer/"/>
public class SignalProviderTests(ITestOutputHelper output)
    {
    [Theory]
    [InlineData(30)]
    public async Task TimeSpanSignalProvider(int milliseconds)
        {
        DateTime start = DateTime.Now;
        DateTime signaled = start;

        TimeSpanSignalProvider signalProvider = new TimeSpanSignalProvider(TimeSpan.FromMilliseconds(milliseconds));

        signalProvider.Signaled += (sender, e) => signaled = DateTime.Now;

        await Task.Delay(milliseconds * 2, TestContext.Current.CancellationToken);

        Assert.NotEqual(start, signaled);
        Assert.True(start < signaled);
        Assert.True(start.AddMilliseconds(milliseconds) < signaled);
        }

    [Theory]
    [InlineData(800)]
    public async Task DailySignalProvider(int milliseconds)
        {
        TimeOnly start = TimeProvider.System.GetNow();
        TimeOnly end = start.Add(TimeSpan.FromMilliseconds(milliseconds));
        TimeOnly signaled = start;

        DailySignalProvider signalProvider = new DailySignalProvider(TimeProvider.System, end);

        signalProvider.Signaled += (sender, e) => signaled = TimeProvider.System.GetNow();

        await Task.Delay(milliseconds * 2, TestContext.Current.CancellationToken);

        output.WriteLine(start.ToString());
        output.WriteLine(signaled.ToString());
        
        Assert.NotEqual(start, signaled);
        Assert.True(start < signaled);
        Assert.True(end < signaled);
        }

    [Theory]
    [InlineData(800)]
    public async Task DateTimeSignalProvider(int milliseconds)
        {
        DateTime start = DateTime.Now;
        DateTime end = start.Add(TimeSpan.FromMilliseconds(milliseconds));
        DateTime signaled = start;

        DateTimeSignalProvider signalProvider = new DateTimeSignalProvider(end);

        signalProvider.Signaled += (sender, e) => signaled = DateTime.Now;

        await Task.Delay(milliseconds * 2, TestContext.Current.CancellationToken);

        Assert.NotEqual(start, signaled);
        Assert.True(start < signaled);
        Assert.True(end < signaled);
        }

    [Fact]
    public void HourlySignalProvider()
        {
        FakeTimeProvider fakeTimeProvider = new FakeTimeProvider();

        DateTimeOffset start = fakeTimeProvider.GetLocalNow();
        DateTimeOffset signaled = start;

        HourlySignalProvider signalProvider = new HourlySignalProvider(fakeTimeProvider);

        signalProvider.Signaled += (sender, e) => signaled = fakeTimeProvider.GetLocalNow();

        TimeSpan increment = TimeSpan.FromHours(1);
        fakeTimeProvider.Advance(increment);

        Assert.Equal(start.Add(increment), signaled);

        TimeSpan increment2 = TimeSpan.FromMinutes(30);
        fakeTimeProvider.Advance(increment2);

        Assert.Equal(start.Add(increment), signaled);

        fakeTimeProvider.Advance(increment2);

        Assert.Equal(start.Add(increment).Add(increment2).Add(increment2), signaled);
        }

    [Theory]
    [InlineData(30, 45)]
    public async Task ReentrantSignalProvider(int msPeriod, int msTask)
        {
        int count = 0;

        TimeSpanSignalProvider signalProvider = new TimeSpanSignalProvider(TimeSpan.FromMilliseconds(msPeriod));

        signalProvider.Signaled += async (sender, e) => { await Task.Delay(msTask); count++; };

        await Task.Delay((int)(msTask * 3.2), TestContext.Current.CancellationToken);

        //Assert.Equal(4, count);
        Assert.True(count > 2);
        }

    [Theory]
    [InlineData(30, 45)]
    public async Task ReentrantSignalHandler(int msPeriod, int msTask)
        {
        int count = 0;

        ConcurrentCounterSignalHandler signalHandler = new ConcurrentCounterSignalHandler((sender, e) => { Thread.Sleep(msTask); count++; });
        TimeSpanSignalProvider signalProvider = new TimeSpanSignalProvider(TimeSpan.FromMilliseconds(msPeriod));

        signalProvider.Signaled += signalHandler.EventHandler;

        await Task.Delay((int)(msTask * 3.2), TestContext.Current.CancellationToken);

        //Assert.Equal(4, count);
        Assert.True(count > 2);
        }

    [Theory]
    [InlineData(30, 45)]
    public async Task ReentrantAsyncSignalHandler(int msPeriod, int msTask)
        {
        int count = 0;

        ConcurrentCounterSignalHandler signalHandler = new ConcurrentCounterSignalHandler(async (sender, e) => { await Task.Delay(msTask); count++; });
        TimeSpanSignalProvider signalProvider = new TimeSpanSignalProvider(TimeSpan.FromMilliseconds(msPeriod));

        signalProvider.Signaled += signalHandler.EventHandler;

        await Task.Delay((int)(msTask * 3.2), TestContext.Current.CancellationToken);

        //Assert.Equal(4, count);
        Assert.True(count > 2);
        }

    /*
    [Theory]
    [InlineData(30, 45)]
    public async Task NoReentrantSignalProvider(int msPeriod, int msTask)
        {
        int count = 0;

        NonReentrantSignalHandler nonReentrantsignalHandler = new NonReentrantSignalHandler((sender, e) => { Thread.Sleep(msTask); count++; });
        TimeSpanSignalProvider signalProvider = new TimeSpanSignalProvider(TimeSpan.FromMilliseconds(msPeriod));

        signalProvider.Signaled += nonReentrantsignalHandler.EventHandler;

        await Task.Delay((int)(msTask * 3.2));

        Assert.Equal(2, count);
        }
    */
    }

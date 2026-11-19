
using CoreClassLibrary.Exceptions;
using Xunit;

namespace CoreClassLibrary.Tests;

public class ExceptionTests(ITestOutputHelper output)
    {
    [Fact]
    public void Try()
        {
        ExceptionAggregator aggregator = new ExceptionAggregator();

        aggregator.Try(() => output.WriteLine("TryTest"));

        aggregator.ThrowIfExceptionsCatched();

        aggregator.Try(() => throw new InvalidOperationException("TryTest"));

        Assert.Throws<AggregateException>(aggregator.ThrowIfExceptionsCatched);
        }

    [Fact]
    public async Task TryAsync()
        {
        ExceptionAggregator aggregator = new ExceptionAggregator();
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        await aggregator.TryAsync(cancellation => { output.WriteLine("TryAsyncTest"); return Task.CompletedTask; }, cancellationTokenSource.Token);

        aggregator.ThrowIfExceptionsCatched();

        await aggregator.TryAsync(cancellation => throw new InvalidOperationException("TryAsyncTest"), cancellationTokenSource.Token);

        Assert.Throws<AggregateException>(aggregator.ThrowIfExceptionsCatched);
        }

    [Fact]
    public void TryContext()
        {
        ExceptionAggregator aggregator = new ExceptionAggregator();

        aggregator.Try<ITestOutputHelper>(output, output => output.WriteLine("TryContextTest"));

        aggregator.ThrowIfExceptionsCatched();

        aggregator.Try<ITestOutputHelper>(output, output => throw new InvalidOperationException("TryContextTest"));

        Assert.Throws<AggregateException>(aggregator.ThrowIfExceptionsCatched);
        }

    [Fact]
    public async Task TryContextAsync()
        {
        ExceptionAggregator aggregator = new ExceptionAggregator();

        await aggregator.TryAsync<ITestOutputHelper>(output, (output, cancellation) => { output.WriteLine("TryContextAsyncTest"); return Task.CompletedTask; }, TestContext.Current.CancellationToken);

        aggregator.ThrowIfExceptionsCatched();

        await aggregator.TryAsync<ITestOutputHelper>(output, (output, cancellation) => throw new InvalidOperationException("TryContextAsyncTest"), TestContext.Current.CancellationToken);

        Assert.Throws<AggregateException>(aggregator.ThrowIfExceptionsCatched);
        }

    [Fact]
    public void TryForEach()
        {
        ExceptionAggregator aggregator = new ExceptionAggregator();
        string[] strings = ["A", "B", "C"];

        aggregator.TryForEach(strings, output.WriteLine, TestContext.Current.CancellationToken);

        aggregator.ThrowIfExceptionsCatched();

        aggregator.TryForEach(strings, str => throw new InvalidOperationException(str), TestContext.Current.CancellationToken);

        Assert.Throws<AggregateException>(aggregator.ThrowIfExceptionsCatched);
        }

    [Fact]
    public async Task TryForEachAsync()
        {
        ExceptionAggregator aggregator = new ExceptionAggregator();
        string[] strings = ["A", "B", "C"];

        await aggregator.TryForEachAsync(strings, (str, cancellation) => { output.WriteLine(str); return Task.CompletedTask; }, TestContext.Current.CancellationToken);

        aggregator.ThrowIfExceptionsCatched();

        await aggregator.TryForEachAsync(strings, (str, cancellation) => throw new InvalidOperationException(str), TestContext.Current.CancellationToken);

        Assert.Throws<AggregateException>(aggregator.ThrowIfExceptionsCatched);
        }

    [Fact]
    public void TryForEachContext()
        {
        ExceptionAggregator aggregator = new ExceptionAggregator();
        string[] strings = ["A", "B", "C"];

        aggregator.TryForEach<string, ITestOutputHelper>(strings, output, (str, output, cancellation) => output.WriteLine("TryForEachContextTest"), TestContext.Current.CancellationToken);

        aggregator.ThrowIfExceptionsCatched();

        aggregator.TryForEach<string, ITestOutputHelper>(strings, output, (str, output, cancellation) => throw new InvalidOperationException("TryForEachContextTest"), TestContext.Current.CancellationToken);

        Assert.Throws<AggregateException>(aggregator.ThrowIfExceptionsCatched);
        }

    [Fact]
    public async Task TryForEachContextAsync()
        {
        ExceptionAggregator aggregator = new ExceptionAggregator();
        string[] strings = ["A", "B", "C"];

        await aggregator.TryForEachAsync<string, ITestOutputHelper>(strings, output, (str, output, cancellation) => { output.WriteLine("TryForEachContextAsyncTest"); return Task.CompletedTask; }, TestContext.Current.CancellationToken);

        aggregator.ThrowIfExceptionsCatched();

        await aggregator.TryForEachAsync<string, ITestOutputHelper>(strings, output, (str, output, cancellation) => throw new InvalidOperationException("TryForEachContextAsyncTest"), TestContext.Current.CancellationToken);

        Assert.Throws<AggregateException>(aggregator.ThrowIfExceptionsCatched);
        }
    }

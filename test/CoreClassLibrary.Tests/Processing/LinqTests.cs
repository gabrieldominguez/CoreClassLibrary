
using CoreClassLibrary.Extensions;
using System.Reactive.Linq;
using Xunit;

namespace CoreClassLibrary.Tests.Processing;

public class LinqTests(ITestOutputHelper output)
    {
    [Fact]
    public void Linq()
        => Shared.Source
            .Select(Shared.Process)
            .Do(output.WriteLine)
            .ForEach(result => Assert.Contains(result, Shared.Result), TestContext.Current.CancellationToken);

    [Fact]
    public Task LinqAsync()
        => Shared.AsyncSource
            .Select<string, string>((line, i, cancellation) => new ValueTask<string>(Shared.ProcessAsync(line, cancellation)))
            .DoAsync(output.WriteLine, TestContext.Current.CancellationToken).ForEachAsync(async (result, cancellation) => Assert.Contains(result, Shared.AsyncResult), cancellationToken: TestContext.Current.CancellationToken);
    
    [Fact]
    public void Linq_Parallel_ForAll()
        => Shared.Source
            .AsParallel()
            //.WithDegreeOfParallelism(4)
            .Select(Shared.Process)
            .Do(output.WriteLine)
            .ForAll(result => Assert.Contains(result, Shared.Result));

    [Fact]
    public async Task WhenAll()
        {
        (await Task.WhenAll(Shared.Source.Select(input => Shared.ProcessAsync(input, TestContext.Current.CancellationToken))))
        .Do(output.WriteLine)
        .ForEach(result => Assert.Contains(result, Shared.Result), TestContext.Current.CancellationToken);
        }

    [Fact]
    // https://stackoverflow.com/questions/23064745/exception-using-rx-and-await-to-accomplish-reading-file-line-by-line-async
    public void Reactive()
        => Shared.Source
            .ToObservable()
            .Select(Shared.Process)
            .Do(output.WriteLine)
            .Subscribe(result => Assert.Contains(result, Shared.Result));

    [Fact]
    public void ReactiveAsync()
        => Shared.AsyncSource
            .ToObservable()
            .Select(Shared.Process)
            .Do(output.WriteLine)
            .Subscribe(result => Assert.Contains(result, Shared.AsyncResult));
    }

public static class LinqExtensions
    {
    public static IObservable<T> DoAsync<T>(this IObservable<T> source, Func<T, Task> action)
        => Observable.Create<T>(observer =>
            source.Subscribe(
                async item =>
                    {
                    await action(item).ConfigureAwait(false);
                    observer.OnNext(item);
                    },
                ex => observer.OnError(ex),
                () => observer.OnCompleted()));

    public static IObservable<T> ToObservable<T>(this IAsyncEnumerable<T> source)
        => Observable.Create<T>(async (observer, cancellationToken) =>
                {
                try
                    {
                    await foreach (T item in source.WithCancellation(cancellationToken))
                        {
                        observer.OnNext(item);
                        }

                    observer.OnCompleted();
                    }
                catch (Exception ex)
                    {
                    observer.OnError(ex);
                    }
                });
}
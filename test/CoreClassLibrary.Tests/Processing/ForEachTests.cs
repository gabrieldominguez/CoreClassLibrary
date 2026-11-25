using CoreClassLibrary.Extensions;
using Xunit;

namespace CoreClassLibrary.Tests.Processing;

public class ForEachTests(ITestOutputHelper output)
    {
    [Fact]
    public void Do()
        {
        Shared.Source.ForEach(line => Shared.Test(line, output), cancellationToken: TestContext.Current.CancellationToken);
        }

    [Fact]
    public void ForEach()
        {
        foreach (string line in Shared.Source)
            {
            Shared.Test(line, output);
            }
        }

    [Fact]
    // https://learn.microsoft.com/en-us/archive/msdn-magazine/2019/november/csharp-iterating-with-async-enumerables-in-csharp-8
    public async Task ForEachAsyncSource() 
        {
        await foreach (string line in Shared.AsyncSource)
            {
            Shared.Test(line, output);
            }
        }

    [Fact]
    // https://learn.microsoft.com/en-us/archive/msdn-magazine/2019/november/csharp-iterating-with-async-enumerables-in-csharp-8
    public async Task ForEachAsyncSourceAsyncTest()
        {
        await foreach (string line in Shared.AsyncSource)
            {
            await Shared.TestAsync(line, output, TestContext.Current.CancellationToken);
            }
        }

    [Fact]
    public void ParallelForEach()
        {
        output.WriteLine(Parallel.ForEach(Shared.Source, line => Shared.Test(line, output)).IsCompleted.ToString());
        }

    [Fact]
    public void ParallelForEachAsync()
        {
        output.WriteLine(Parallel.ForEachAsync(Shared.AsyncSource, (line, cancellationToken) => Shared.TestValueAsync(line, output, cancellationToken)).IsCompleted.ToString());
        }
    }

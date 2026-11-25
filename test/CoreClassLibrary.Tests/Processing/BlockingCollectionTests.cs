#pragma warning disable IDE0028 // Simplify collection initialization

using CoreClassLibrary.Extensions;
using System.Collections.Concurrent;
using Xunit;

namespace CoreClassLibrary.Tests.Processing;

/// <summary>
/// BlockingCollection Tests
/// </summary>
/// <see cref="https://learn.microsoft.com/en-us/dotnet/standard/collections/thread-safe/blockingcollection-overview"/>
/// <see cref="https://learn.microsoft.com/en-us/dotnet/standard/collections/thread-safe/how-to-use-arrays-of-blockingcollections"/>
public class BlockingCollectionTests(ITestOutputHelper output)
    {
    [Fact]
    public async Task BlockingCollection()
        {
        BlockingCollection<string> blockingCollection = new BlockingCollection<string>();

        Task dequeueWorker = Task.Run(() =>
            {
            while (!blockingCollection.IsCompleted)
                {
                Shared.Test(blockingCollection.Take(), output);    // blockingCollection.Take() blocks if dataItems.Count == 0
                }
            }, TestContext.Current.CancellationToken);

        // Begin producing
        foreach (string line in Shared.Source)
            {
            blockingCollection.Add(line, TestContext.Current.CancellationToken);
            }
        blockingCollection.CompleteAdding();
        // End producing

        // Begin consuming
        await dequeueWorker;
        // End consuming
        }

    [Fact]
    public async Task BlockingCollectionLINQ()
        {
        BlockingCollection<string> blockingCollection = new BlockingCollection<string>();

        Task dequeueWorker = Task.Run(() =>
            {
            blockingCollection
                .GetConsumingEnumerable()
                .Select(Shared.Process)
                .Do(output.WriteLine)
                .ForEach(result => Assert.Contains(result, Shared.Result));
            }, TestContext.Current.CancellationToken);

        // Begin producing
        foreach (string line in Shared.Source)
            {
            blockingCollection.Add(line, TestContext.Current.CancellationToken);
            }
        blockingCollection.CompleteAdding();
        // End producing

        // Begin consuming
        await dequeueWorker;
        // End consuming
        }
    }

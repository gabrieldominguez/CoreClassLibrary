
using CoreClassLibrary.Threading.AsyncQueues;
using Xunit;

namespace CoreClassLibrary.Tests.Processing;

public class AsyncQueueTests(ITestOutputHelper output)
    {
    // https://java-design-patterns.com/patterns/poison-pill/
    private async Task PoisonPillAsyncQueue(IAsyncQueue<string> asyncQueue)
        {
        Task dequeueWorker = Task.Run(async () =>
            {
            string line;
            while ((line = await asyncQueue.DequeueAsync()) != "")
                {
                await Shared.TestAsync(line, output);
                }
            }, TestContext.Current.CancellationToken);

        foreach (string line in Shared.Source)
            {
            await asyncQueue.EnqueueAsync(line);
            }
        await asyncQueue.EnqueueAsync("");

        await dequeueWorker;
        }

    private async Task CompleteAsyncQueue(IAsyncQueue<string> asyncQueue)
        {
        Task dequeueWorker = Task.Run(async () =>
            {
            while (!asyncQueue.Completion.IsCompleted)
                {
                //await Shared.AsyncTest(await asyncQueue.DequeueAsync(), output);
                string? line = await asyncQueue.TryDequeueAsync();
                if (line != null)
                    {
                    await Shared.TestAsync(line, output);
                    }
                }
            });

        foreach (string line in Shared.Source)
            {
            await asyncQueue.EnqueueAsync(line);
            }

        await Task.Delay(100);

        asyncQueue.Complete();

        await dequeueWorker;
        }

    private async Task FaultAsyncQueue(IAsyncQueue<string> asyncQueue)
        {
        Task dequeueWorker = Task.Run(async () =>
            {
            while (!asyncQueue.Completion.IsCompleted)
                {
                await Shared.TestAsync(await asyncQueue.DequeueAsync(), output);
                }
            });

        foreach (string line in Shared.Source)
            {
            await asyncQueue.EnqueueAsync(line);
            }

        await Task.Delay(100);

        asyncQueue.Complete(new InvalidOperationException());

        await Assert.ThrowsAnyAsync<InvalidOperationException>(() => dequeueWorker);
        }

    [Fact]
    public Task PoisonPillChannelAsyncQueue()
        => this.PoisonPillAsyncQueue(new ChannelAsyncQueue<string>());

    [Fact]
    public Task PoisonPillConcurrentAsyncQueue()
        => this.PoisonPillAsyncQueue(new ConcurrentAsyncQueue<string>());

    //[Fact] public Task PoisonPillBufferBlockAsyncQueue() => this.PoisonPillAsyncQueue(new BufferBlockAsyncQueue<string>());

    [Fact]
    public Task CompleteChannelAsyncQueue()
        => this.CompleteAsyncQueue(new ChannelAsyncQueue<string>());

    [Fact]
    public Task CompleteConcurrentAsyncQueue()
        => this.CompleteAsyncQueue(new ConcurrentAsyncQueue<string>());
    
    //[Fact] public Task CompleteBufferBlockAsyncQueue() => this.CompleteAsyncQueue(new BufferBlockAsyncQueue<string>());

    [Fact]
    public Task FaultChannelAsyncQueue()
        => this.FaultAsyncQueue(new ChannelAsyncQueue<string>());

    [Fact]
    public Task FaultConcurrentAsyncQueue()
        => this.FaultAsyncQueue(new ConcurrentAsyncQueue<string>());
    
    //[Fact] public Task FaultBufferBlockAsyncQueue() => this.FaultAsyncQueue(new BufferBlockAsyncQueue<string>());
    }

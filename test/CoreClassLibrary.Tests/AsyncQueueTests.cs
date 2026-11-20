using CoreClassLibrary.Threading.AsyncQueues;
using System.Threading.Channels;
using Xunit;

namespace CoreClassLibrary.Tests;

public class AsyncQueueTests
    {
    [Fact]
    public async Task ChannelQueue_EnqueueAndDequeue_PreservesItem()
        {
        ChannelAsyncQueue<int> queue = new();

        await queue.EnqueueAsync(42, TestContext.Current.CancellationToken);
        int item = await queue.DequeueAsync(TestContext.Current.CancellationToken);

        Assert.Equal(42, item);
        }

    [Fact]
    public async Task ChannelQueue_CompleteWithoutError_DequeueThrowsInvalidOperation()
        {
        ChannelAsyncQueue<int> queue = new();
        queue.Complete();

        await Assert.ThrowsAsync<ChannelClosedException>(() => queue.DequeueAsync(TestContext.Current.CancellationToken).AsTask());
        }

    [Fact]
    public async Task ChannelQueue_FaultedCompletion_DequeueThrowsOriginalError()
        {
        ChannelAsyncQueue<int> queue = new();
        InvalidOperationException error = new("queue failed");
        queue.Complete(error);

        ChannelClosedException exception = await Assert.ThrowsAsync<ChannelClosedException>(
            () => queue.DequeueAsync(TestContext.Current.CancellationToken).AsTask());

        Assert.IsType<InvalidOperationException>(exception.InnerException);
        Assert.Equal("queue failed", exception.InnerException.Message);
    }

    [Fact]
    public async Task ConcurrentQueue_EnqueueAndDequeue_PreservesItem()
        {
        using ConcurrentAsyncQueue<int> queue = new();

        await queue.EnqueueAsync(7, TestContext.Current.CancellationToken);

        Assert.Equal(7, await queue.DequeueAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task ConcurrentQueue_CompleteWithPendingItems_DrainsThenCompletes()
        {
        using ConcurrentAsyncQueue<int> queue = new();
        await queue.EnqueueAsync(1, TestContext.Current.CancellationToken);
        await queue.EnqueueAsync(2, TestContext.Current.CancellationToken);

        queue.Complete();

        Assert.Equal(1, await queue.DequeueAsync(TestContext.Current.CancellationToken));
        Assert.Equal(2, await queue.DequeueAsync(TestContext.Current.CancellationToken));
        await Assert.ThrowsAsync<InvalidOperationException>(() => queue.DequeueAsync(TestContext.Current.CancellationToken).AsTask());
        await queue.Completion;
    }

    [Fact]
    public async Task ConcurrentQueue_Fault_CompletionContainsError()
        {
        using ConcurrentAsyncQueue<int> queue = new();
        InvalidOperationException error = new("queue failed");

        queue.Complete(error);

        await Assert.ThrowsAsync<InvalidOperationException>(() => queue.Completion);
    }

    [Fact]
    public async Task ConcurrentQueue_EnqueueAfterCompletion_Throws()
        {
        using ConcurrentAsyncQueue<int> queue = new();
        queue.Complete();

        Assert.Throws<InvalidOperationException>(() =>
            {
            queue.EnqueueAsync(1, TestContext.Current.CancellationToken);
            });
    }
}

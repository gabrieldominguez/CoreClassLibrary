using CoreClassLibrary.Threading.AsyncQueues;
using CoreClassLibrary.Threading.Messaging;
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

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>            
            await queue.EnqueueAsync(1, TestContext.Current.CancellationToken)
            );
        }

    [Fact]
    public async Task MessageBus_PublishAsync_DeliversToEverySubscriber()
        {
        QueueMessageBus<int> messageBus = new QueueMessageBus<int>();
        IAsyncQueue<int> firstSubscriber = messageBus.Subscribe();
        IAsyncQueue<int> secondSubscriber = messageBus.Subscribe();

        await messageBus.PublishAsync(1, TestContext.Current.CancellationToken);
        await messageBus.PublishAsync(2, TestContext.Current.CancellationToken);

        Assert.Equal(1, await firstSubscriber.DequeueAsync(TestContext.Current.CancellationToken));
        Assert.Equal(2, await firstSubscriber.DequeueAsync(TestContext.Current.CancellationToken));
        Assert.Equal(1, await secondSubscriber.DequeueAsync(TestContext.Current.CancellationToken));
        Assert.Equal(2, await secondSubscriber.DequeueAsync(TestContext.Current.CancellationToken));

        messageBus.Complete();
        }

    [Fact]
    public async Task MessageBus_Complete_ClosesSubscribersAndRejectsFutureOperations()
        {
        QueueMessageBus<int> messageBus = new QueueMessageBus<int>();
        IAsyncQueue<int> subscriber = messageBus.Subscribe();

        messageBus.Complete();

        await Assert.ThrowsAsync<ChannelClosedException>(
            () => subscriber.DequeueAsync(TestContext.Current.CancellationToken).AsTask());
        await Assert.ThrowsAsync<ObjectDisposedException>(
            () => messageBus.PublishAsync(1, TestContext.Current.CancellationToken).AsTask());
        Assert.Throws<ObjectDisposedException>(() => messageBus.Subscribe());
        }

    [Fact]
    public async Task MessageBusT_PublishAsync_RoutesMessagesByType()
        {
        QueueMessageBus messageBus = new QueueMessageBus();
        IAsyncQueue<int> firstSubscriber = messageBus.Subscribe<int>();
        IAsyncQueue<int> secondSubscriber = messageBus.Subscribe<int>();
        IAsyncQueue<string> stringSubscriber = messageBus.Subscribe<string>();

        await messageBus.PublishAsync(42, TestContext.Current.CancellationToken);
        await messageBus.PublishAsync("hello", TestContext.Current.CancellationToken);

        Assert.Equal(42, await firstSubscriber.DequeueAsync(TestContext.Current.CancellationToken));
        Assert.Equal(42, await secondSubscriber.DequeueAsync(TestContext.Current.CancellationToken));
        Assert.Equal("hello", await stringSubscriber.DequeueAsync(TestContext.Current.CancellationToken));
        }

    [Fact]
    public async Task MessageBusT_Unsubscribe_CompletesQueueAndStopsDelivery()
        {
        QueueMessageBus messageBus = new QueueMessageBus();
        IAsyncQueue<int> subscriber = messageBus.Subscribe<int>();

        Assert.True(messageBus.Unsubscribe(subscriber));
        Assert.False(messageBus.Unsubscribe(subscriber));
        await messageBus.PublishAsync(42, TestContext.Current.CancellationToken);

        await Assert.ThrowsAsync<ChannelClosedException>(
            () => subscriber.DequeueAsync(TestContext.Current.CancellationToken).AsTask());
        }
    }

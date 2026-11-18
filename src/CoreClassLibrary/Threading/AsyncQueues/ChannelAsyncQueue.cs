using System.Threading.Channels;

namespace CoreClassLibrary.Threading.AsyncQueues;

/// <summary>
/// Asynchronous Queue implementation using Channel
/// </summary>
/// <remarks>
/// These options are performance optimizations for Channel<T>:
/// SingleWriter = true: Optimizes for scenarios where only one thread is writing to the channel.
/// SingleReader = true: Optimizes for scenarios where only one thread is reading from the channel.
/// When set to false, the channel allows multiple threads to safely read/write concurrently, 
/// but with slightly more overhead due to synchronization.
/// 
/// Important: Each message is consumed by one reader, not broadcast.
/// </remarks>
/// <see cref="https://devblogs.microsoft.com/dotnet/an-introduction-to-system-threading-channels"/>
public class ChannelAsyncQueue<T>(Channel<T> channel) : IAsyncQueue<T>
    {
    // Unbounded channel: no limit on the number of items
    public ChannelAsyncQueue(bool singleEnqueuer = false, bool singleDequeuer = true):
        this(new UnboundedChannelOptions { SingleWriter = singleEnqueuer, SingleReader = singleDequeuer}) { }
    
    // Unbounded channel: no limit on the number of items
    public ChannelAsyncQueue(UnboundedChannelOptions options):
        this(Channel.CreateUnbounded<T>(options)) { }

    public ValueTask EnqueueAsync(T item, CancellationToken cancellationToken = default)
        => channel.Writer.WriteAsync(item, cancellationToken);

    public ValueTask<T> DequeueAsync(CancellationToken cancellationToken = default)
        => channel.Reader.ReadAsync(cancellationToken);

    public void Complete(Exception? error = null)
        {
        channel.Writer.Complete(error);
        }

    public Task Completion => channel.Reader.Completion;
    }

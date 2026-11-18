using System.Collections.Concurrent;

namespace CoreClassLibrary.Threading.AsyncQueues;

/// <summary>
/// Asynchronous Queue implementation using ConcurrentQueue
/// </summary>
/// <see cref="ChannelAsyncQueue{T}"/>
/// <see cref="https://devblogs.microsoft.com/dotnet/an-introduction-to-system-threading-channels"/>
/// <see cref="BlockingCollection{T}"/>
/// <see cref="IProducerConsumerCollection{T}"/>
public class ConcurrentAsyncQueue<T>(ConcurrentQueue<T> concurrentQueue) : IAsyncQueue<T>, IDisposable
    {
    public ConcurrentAsyncQueue(): this(new ConcurrentQueue<T>()) { }

    private readonly SemaphoreSlim itemSemaphore = new SemaphoreSlim(0);
    private readonly TaskCompletionSource enqueueCompletion = new TaskCompletionSource();
    private readonly TaskCompletionSource dequeueCompletion = new TaskCompletionSource();

    public Task Completion => this.dequeueCompletion.Task;

    public ValueTask EnqueueAsync(T item, CancellationToken cancellationToken = default)
        {
        if (this.enqueueCompletion.Task.IsCompleted)
            {
            throw new InvalidOperationException("Queue has been completed.");
            }

        concurrentQueue.Enqueue(item);
        this.itemSemaphore.Release();   // notify any consumers that more data is available
        return ValueTask.CompletedTask;
        }

    public async ValueTask<T> DequeueAsync(CancellationToken cancellationToken = default)
        {
        try
            {
            await Task.WhenAny(
                this.itemSemaphore.WaitAsync(cancellationToken),
                this.enqueueCompletion.Task
                );
            }
        catch(Exception ex) 
            {
            if (concurrentQueue.IsEmpty && this.enqueueCompletion.Task.IsFaulted)
                {
                this.dequeueCompletion.TrySetException(ex);
                throw;
                }
            }

        if (concurrentQueue.TryDequeue(out T? item))
            {
            return item;
            }
        else
            {
            if (this.enqueueCompletion.Task.IsCompleted)
                {
                this.dequeueCompletion.TrySetResult();
                throw new InvalidOperationException("Queue has been completed.");
                }

            throw new InvalidOperationException("Failed to dequeue an item.");
            }
        }

    public void Complete(Exception? error = null)
        {
        if (error == null)
            {
            this.CompletedSuccessfully();
            }
        else
            {
            this.Fault(error);
            }
        }

    public void CompletedSuccessfully()
        {        
        this.enqueueCompletion.TrySetResult();

        if (concurrentQueue.IsEmpty)
            {
            this.dequeueCompletion.TrySetResult();
            }
        }

    public void Fault(Exception error)
        {        
        this.enqueueCompletion.TrySetException(error);

        if (concurrentQueue.IsEmpty)
            {
            this.dequeueCompletion.TrySetException(error);
            }
        }

    public void Dispose()
        {
        this.itemSemaphore.Dispose();
        GC.SuppressFinalize(this);
        }
    }

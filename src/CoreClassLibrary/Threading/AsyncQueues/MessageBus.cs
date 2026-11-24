#pragma warning disable IDE0305 // Simplify collection initialization

namespace CoreClassLibrary.Threading.AsyncQueues;

public class MessageBus<T>(IList<IAsyncQueue<T>> subscribers)
    {
    private readonly Lock gate = new Lock();
    private bool isCompleted;

    public IAsyncQueue<T> Subscribe()
        {
        IAsyncQueue<T> subscriber = new ChannelAsyncQueue<T>();
        this.Subscribe(subscriber);
        return subscriber;
        }

    public void Subscribe(IAsyncQueue<T> subscriber)
        {
        lock (this.gate)
            {
            ObjectDisposedException.ThrowIf(this.isCompleted, this);
            if (subscribers.Contains(subscriber))
                {
                throw new InvalidOperationException("The queue is already subscribed.");
                }

            subscribers.Add(subscriber);
            }
        }

    public bool Unsubscribe(IAsyncQueue<T> subscriber, Exception? error = null)
        {
        lock (this.gate)
            {
            if (!subscribers.Remove(subscriber))
                {
                return false;
                }
            }

        subscriber.Complete(error);
        return true;
        }

    public async ValueTask PublishAsync(T message, CancellationToken cancellationToken = default)
        {
        IAsyncQueue<T>[] subscribersSnapshot;
        lock (this.gate)
            {
            ObjectDisposedException.ThrowIf(this.isCompleted, this);
            subscribersSnapshot = subscribers.ToArray();
            }

        foreach (IAsyncQueue<T> subscriber in subscribersSnapshot)
            {
            await subscriber.EnqueueAsync(message, cancellationToken);
            }
        }

    public void Complete(Exception? error = null)
        {
        IAsyncQueue<T>[] subscribersSnapshot;
        lock (this.gate)
            {
            if (this.isCompleted)
                {
                return;
                }

            this.isCompleted = true;
            subscribersSnapshot = subscribers.ToArray();
            subscribers.Clear();
            }

        foreach (IAsyncQueue<T> subscriber in subscribersSnapshot)
            {
            subscriber.Complete(error);
            }
        }
    }

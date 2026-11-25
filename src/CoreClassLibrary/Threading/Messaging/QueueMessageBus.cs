using CoreClassLibrary.Threading.AsyncQueues;

namespace CoreClassLibrary.Threading.Messaging;

public sealed class QueueMessageBus()
    {
    private readonly Lock gate = new Lock();
    private readonly Dictionary<Type, ISubscriberCollection> subscribers = [];

    public IAsyncQueue<T> Subscribe<T>()
        {
        IAsyncQueue<T> subscriber = new ChannelAsyncQueue<T>();

        lock (this.gate)
            {
            this.GetSubscribers<T>().Queues.Add(subscriber);
            }

        return subscriber;
        }

    public bool Unsubscribe<T>(IAsyncQueue<T> subscriber, Exception? error = null)
        {
        ArgumentNullException.ThrowIfNull(subscriber);

        lock (this.gate)
            {
            if (!this.subscribers.TryGetValue(typeof(T), out ISubscriberCollection? subscribersForType)
                || !((SubscriberCollection<T>)subscribersForType).Queues.Remove(subscriber))
                {
                return false;
                }
            }

        subscriber.Complete(error);
        return true;
        }

    public async ValueTask PublishAsync<T>(T message, CancellationToken cancellationToken = default)
        {
        IAsyncQueue<T>[] subscribersSnapshot;
        lock (this.gate)
            {
            subscribersSnapshot = this.subscribers.TryGetValue(typeof(T), out ISubscriberCollection? subscribersForType)
                ? ((SubscriberCollection<T>)subscribersForType).Queues.ToArray()
                : [];
            }

        foreach (IAsyncQueue<T> subscriber in subscribersSnapshot)
            {
            await subscriber.EnqueueAsync(message, cancellationToken);
            }
        }

    private SubscriberCollection<T> GetSubscribers<T>()
        {
        if (!this.subscribers.TryGetValue(typeof(T), out ISubscriberCollection? subscribersForType))
            {
            subscribersForType = new SubscriberCollection<T>();
            this.subscribers.Add(typeof(T), subscribersForType);
            }

        return (SubscriberCollection<T>)subscribersForType;
        }

    private interface ISubscriberCollection;

    private sealed class SubscriberCollection<T> : ISubscriberCollection
        {
        public List<IAsyncQueue<T>> Queues { get; } = [];
        }
    }

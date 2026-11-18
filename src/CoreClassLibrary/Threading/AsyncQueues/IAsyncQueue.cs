namespace CoreClassLibrary.Threading.AsyncQueues;

/// <summary>
/// Async Queue Interface for inter-thread communication.
/// </summary>
/// <remarks>
/// Preferred implementation is ChannelAsyncQueue&lt;T&gt;.
/// </remarks>
/// <see cref="ChannelAsyncQueue{T}"/>
public interface IAsyncQueue<T>: IAsyncEnqueuer<T>, IAsyncDequeuer<T>;

public interface IAsyncEnqueuer<T>
	{
	ValueTask EnqueueAsync(T item, CancellationToken cancellationToken = default);
	void Complete(Exception? error = null);
	}

public interface IAsyncDequeuer<T>
	{
	ValueTask<T> DequeueAsync(CancellationToken cancellationToken = default);
	Task Completion { get; }

	async ValueTask<T?> TryDequeueAsync(CancellationToken cancellationToken = default)
		{
		try 
			{	        
			return await this.DequeueAsync(cancellationToken);
			}   
		catch (Exception)
			{
			return default;
			}
		}
	}

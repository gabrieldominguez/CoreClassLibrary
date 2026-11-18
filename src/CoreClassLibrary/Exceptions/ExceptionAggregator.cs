namespace CoreClassLibrary.Exceptions;

/// <summary>
/// Almacena excepciones y las agrupa dentro de una única AggregateException.
/// </summary>
/// <see cref="AggregateException"/>
public class ExceptionAggregator
    {
    private Lazy<List<Exception>> catchedExceptions = new Lazy<List<Exception>>(() => []);

    public ExceptionAggregator Try(Action action)
        {
        try
            {
            action();
            }
        catch (Exception ex)
            {
            this.catchedExceptions.Value.Add(ex);
            }

        return this;
        }

    public ExceptionAggregator Try<TContext>(TContext context, Action<TContext> action)
        {
        try
            {
            action(context);
            }
        catch (Exception ex)
            {
            this.catchedExceptions.Value.Add(ex);
            }

        return this;
        }

    public ExceptionAggregator TryForEach<T>(IEnumerable<T> arr, Action<T> action, CancellationToken cancellationToken = default)
        {
        foreach (T item in arr)
            {
            try
                {
                cancellationToken.ThrowIfCancellationRequested();
                action(item);
                }
            catch (Exception ex)
                {
                ex.Data.Add("ExceptionAggregator.ForEach.Item", item);

                this.catchedExceptions.Value.Add(ex);
                }
            }

        return this;
        }

    public async Task<ExceptionAggregator> TryAsync(Func<CancellationToken, Task> actionAsync, CancellationToken cancellationToken = default)
        {
        try
            {
            await actionAsync(cancellationToken);
            }
        catch (Exception ex)
            {
            this.catchedExceptions.Value.Add(ex);
            }

        return this;
        }

    public async Task<ExceptionAggregator> TryAsync<TContext>(TContext context, Func<TContext, CancellationToken, Task> actionAsync, CancellationToken cancellationToken = default)
        {
        try
            {
            await actionAsync(context, cancellationToken);
            }
        catch (Exception ex)
            {
            this.catchedExceptions.Value.Add(ex);
            }

        return this;
        }

    public async Task<ExceptionAggregator> TryForEachAsync<T>(IEnumerable<T> arr, Func<T, CancellationToken, Task> actionAsync, CancellationToken cancellationToken = default)
        {
        foreach (T item in arr)
            {
            try
                {
                await actionAsync(item, cancellationToken);
                }
            catch (Exception ex)
                {
                ex.Data.Add("ExceptionAggregator.ForEach.Item", item);
                this.catchedExceptions.Value.Add(ex);
                }
            }

        return this;
        }

    public ExceptionAggregator TryForEach<T, TContext>(IEnumerable<T> arr, TContext context, Action<T, TContext, CancellationToken> action, CancellationToken cancellationToken = default)
        {
        foreach (T item in arr)
            {
            try
                {
                action(item, context, cancellationToken);
                }
            catch (Exception ex)
                {
                ex.Data.Add("ExceptionAggregator.ForEach.Item", item);
                this.catchedExceptions.Value.Add(ex);
                }
            }

        return this;
        }

    public async Task<ExceptionAggregator> TryForEachAsync<T, TContext>(IEnumerable<T> arr, TContext context, Func<T, TContext, CancellationToken, Task> actionAsync, CancellationToken cancellationToken = default)
        {
        foreach (T item in arr)
            {
            try
                {
                await actionAsync(item, context, cancellationToken);
                }
            catch (Exception ex)
                {
                ex.Data.Add("ExceptionAggregator.ForEach.Item", item);
                this.catchedExceptions.Value.Add(ex);
                }
            }

        return this;
        }

    public void ThrowIfExceptionsCatched()
        {
        if (this.catchedExceptions.IsValueCreated)
            {
            AggregateException aggregateException = new AggregateException(this.catchedExceptions.Value);
            this.catchedExceptions = new Lazy<List<Exception>>(() => []);
            throw aggregateException;
            }
        }
    }

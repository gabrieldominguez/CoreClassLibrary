namespace CoreClassLibrary.Objects.Disposables;

public class CompositeDisposable(IReadOnlyCollection<IDisposable> disposables) : IDisposable
    {
    public void Dispose()
        {
        foreach (IDisposable disposable in disposables)
            {
            disposable.Dispose();
            }

        GC.SuppressFinalize(this);
        }
    }

public class CompositeAsyncDisposable(IReadOnlyCollection<IAsyncDisposable> disposables) : IAsyncDisposable
    {
    public async ValueTask DisposeAsync()
        {
        foreach (IAsyncDisposable disposable in disposables)
            {
            await disposable.DisposeAsync();
            }

        GC.SuppressFinalize(this);
        }
    }

namespace CoreClassLibrary.Objects.Disposables;

/// <summary>
/// Controla el acceso a un Disposable
/// </summary>
/// <typeparam name="TValue"></typeparam>
public class DisposableEnumerableWrapper<TValue>(IEnumerable<TValue> values) : IDisposable where TValue: IDisposable
    {
    /// <summary>
    /// Track whether Dispose is been executed.
    /// </summary>
    public bool IsDisposing { get; private set; } = false;

    /// <summary>
    /// Track whether Dispose has been called.
    /// </summary>
    public bool IsDisposed { get; private set; } = false;

    public void ValidateDisposed()
        {
        ObjectDisposedException.ThrowIf(this.IsDisposed, nameof(DisposableEnumerableWrapper<>));
        }

    public void Dispose()
        {
        if (!this.IsDisposed)
            {
            this.IsDisposing = true;

            foreach (IDisposable value in values) 
                {
                value.Dispose();
                }

            GC.SuppressFinalize(this);

            this.IsDisposing = false;
            this.IsDisposed = true;
            }
        }
    }

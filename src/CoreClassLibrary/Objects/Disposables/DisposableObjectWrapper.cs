namespace CoreClassLibrary.Objects.Disposables;

/// <summary>
/// Controla el acceso a un Disposable
/// </summary>
/// <typeparam name="TValue"></typeparam>
public class DisposableObjectWrapper<TValue>(TValue value) : IDisposable where TValue: IDisposable
    {
    /// <summary>
    /// Track whether Dispose is been executed.
    /// </summary>
    public bool IsDisposing { get; private set; } = false;

    /// <summary>
    /// Track whether Dispose has been called.
    /// </summary>
    public bool IsDisposed { get; private set; } = false;

    public TValue Value 
        => this.IsDisposing || this.IsDisposed? throw new ObjectDisposedException(typeof(TValue).FullName) : value;

    public void Dispose()
        {
        if (!this.IsDisposed)
            {
            this.IsDisposing = true;

            value.Dispose();
            GC.SuppressFinalize(this);

            this.IsDisposing = false;
            this.IsDisposed = true;
            }
        }
    }

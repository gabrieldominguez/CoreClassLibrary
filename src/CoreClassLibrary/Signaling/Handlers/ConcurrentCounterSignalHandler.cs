namespace CoreClassLibrary.Signaling.Handlers;

/// <summary>
/// Mantiene la cantidad especificada de ejecuciones concurrentes.
/// </summary>
public class ConcurrentCounterSignalHandler(Action<object?, EventArgs> handler) : ISignalHandler
    {
    private int count = 0;
    public int Count => this.count;
    public bool IsRunning => this.count > 0;

    public void EventHandler(object? sender, EventArgs e)
        {
        Interlocked.Increment(ref this.count);
        try
            {
            handler.Invoke(sender, e);
            }
        finally
            {
            Interlocked.Decrement(ref this.count);
            }
        }
    }

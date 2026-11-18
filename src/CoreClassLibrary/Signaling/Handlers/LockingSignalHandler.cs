namespace CoreClassLibrary.Signaling.Handlers;

/// <summary>
/// Bloquea la ejecución del handler especificado hasta que termine la ejecución previa.
/// </summary>
public class LockingSignalHandler(Action<object?, EventArgs> handler) : ISignalHandler
    {
    private readonly Lock syncRoot = new Lock();

    public void EventHandler(object? sender, EventArgs e)
        {
        lock (this.syncRoot)
            {
            handler.Invoke(sender, e);
            }
        }
    }

namespace CoreClassLibrary.Signaling.Handlers;

/// <summary>
/// No ejecuta el handler especificado si la ejecución previa no a terminado.
/// </summary>
/// <remarks>
/// No ejecutar el handler manualmente (sin usar un ISignalProvider)
/// </remarks>
public class NonReentrantSignalHandler(Action<object?, EventArgs> handler) : ISignalHandler
    {
    public bool IsRunning { get; private set; }

    public void EventHandler(object? sender, EventArgs e)
        {
        if (!this.IsRunning)
            {
            this.IsRunning = true;
            try
                {
                handler.Invoke(sender, e);
                }
            finally
                {
                this.IsRunning = false;
                }
            }
        }
    }

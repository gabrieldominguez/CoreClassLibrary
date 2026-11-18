namespace CoreClassLibrary.Signaling.Providers;

/// <summary>
/// Clase base para implementar ISignalProvider
/// </summary>
public abstract class SignalProvider : ISignalProvider
    {
    public event EventHandler? Signaled;
    protected void OnSignaled() => this.Signaled?.Invoke(this, EventArgs.Empty);
    }

/*
 * Action base implementation
 * 
public abstract class SignalProvider : ISignalProvider
    {
    public Action Signaled { get; set; }

    protected void OnSignaled()
        {
        this.Signaled?.Invoke();
        }
    }
*/

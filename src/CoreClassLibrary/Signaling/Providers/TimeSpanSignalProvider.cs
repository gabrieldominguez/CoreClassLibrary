namespace CoreClassLibrary.Signaling.Providers;

/// <summary>
/// Permite emitir un evento para señalizar un suceso.
/// </summary>
public sealed class TimeSpanSignalProvider : SignalProvider, IDisposable
    {
    private readonly ITimer timer;

    public TimeSpanSignalProvider(TimeSpan period): this(TimeProvider.System, period) { }

    public TimeSpanSignalProvider(TimeProvider timeProvider, TimeSpan period): this(timeProvider, period, TimeSpan.Zero) { }

    /// <summary>
    /// Contructor.
    /// </summary>
    /// <param name="period">Intervalo de tiempo entre las distintas invocaciones del evento.</param>
    /// <param name="due">Intervalo de tiempo desde que arranca el timer hasta la primer invocación del evento.</param>
    public TimeSpanSignalProvider(TimeProvider timeProvider, TimeSpan period, TimeSpan due)
        {
        this.timer = timeProvider.CreateTimer(signalProvider => ((TimeSpanSignalProvider?)signalProvider)?.OnSignaled(), this, due, period);
        }

    public void Dispose() => this.timer.Dispose();
    }

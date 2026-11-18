using CoreClassLibrary.Extensions;

namespace CoreClassLibrary.Signaling.Providers;

/// <summary>
/// Emite un evento en la fecha/hora especificada, con precision de 1 seg.
/// </summary>
public sealed class DateTimeSignalProvider : SignalProvider, IDisposable
    {
    private readonly ITimer timer;
    private readonly TimeProvider timeProvider;
    private readonly DateTimeOffset time;

    public DateTimeSignalProvider(DateTime time): this(TimeProvider.System, time) { }

    /// <summary>
    /// Contructor.
    /// </summary>
    /// <param name="time">Hora en el que emitir el evento.</param>
    /// <param name="period">Periodo en el que controlar la hora.</param>
    public DateTimeSignalProvider(TimeProvider timeProvider, DateTimeOffset time)
        {        
        this.time = time;
        this.timer = (this.timeProvider = timeProvider).CreateTimer(
            signalProvider =>
                {
                if (signalProvider is DateTimeSignalProvider dateTimeSignalProvider && dateTimeSignalProvider.timeProvider.IsNow(this.time, TimeSpan.FromSeconds(0.5)))
                    {
                    dateTimeSignalProvider.OnSignaled();
                    }
                },
            this,
            TimeSpan.Zero,
            TimeSpan.FromSeconds(1));
        }

    public void Dispose() 
        => this.timer.Dispose();
    }


using CoreClassLibrary.Extensions;

namespace CoreClassLibrary.Signaling.Providers;

/// <summary>
/// Emite un evento una vez por dia, con precision de 1 seg.
/// </summary>
public sealed class DailySignalProvider : SignalProvider, IDisposable
    {
    private readonly ITimer timer;
    private readonly TimeProvider timeProvider;
    private readonly TimeOnly time;

    public DailySignalProvider() : this(TimeProvider.System) { }

    public DailySignalProvider(TimeProvider timeProvider) : this(timeProvider, TimeOnly.MinValue) { }

    /// <summary>
    /// Contructor.
    /// </summary>
    /// <param name="time">Hora en el que emitir el evento.</param>
    /// <param name="period">Periodo en el que controlar la hora.</param>
    public DailySignalProvider(TimeProvider timeProvider, TimeOnly time)
        {
        this.time = time;
        this.timer = (this.timeProvider = timeProvider).CreateTimer(
            signalProvider =>
                {
                if (signalProvider is DailySignalProvider dailySignalProvider && dailySignalProvider.timeProvider.IsNow(dailySignalProvider.time, TimeSpan.FromSeconds(0.5)))
                    {
                    dailySignalProvider.OnSignaled();
                    }
                },
            this,
            TimeSpan.Zero,
            TimeSpan.FromSeconds(1));
        }

    public void Dispose() => this.timer.Dispose();
    }

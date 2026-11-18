namespace CoreClassLibrary.Signaling.Providers;

/// <summary>
/// Emite un evento una vez por hora con precision de 1 minuto.
/// </summary>
/// <see cref="https://andrewlock.net/exploring-the-dotnet-8-preview-avoiding-flaky-tests-with-timeprovider-and-itimer/"/>
public sealed class HourlySignalProvider : SignalProvider, IDisposable
    {
    private readonly ITimer timer;
    private readonly TimeProvider timeProvider;
    private readonly int minute;

    public HourlySignalProvider(int minute = 0): this(TimeProvider.System, minute) { }

    /// <summary>
    /// Contructor.
    /// </summary>
    /// <param name="minute">Minuto en el que emitir el evento.</param>
    public HourlySignalProvider(TimeProvider timeProvider, int minute = 0)
        {
        this.minute = minute;
        this.timer = (this.timeProvider = timeProvider).CreateTimer(
            signalProvider =>
                {
                if (signalProvider is HourlySignalProvider hourlySignalProvider && hourlySignalProvider.timeProvider.GetLocalNow().Minute == hourlySignalProvider.minute)
                    {
                    hourlySignalProvider.OnSignaled();
                    }
                },
            this,
            TimeSpan.Zero, 
            TimeSpan.FromMinutes(1));
        }

    public void Dispose() => this.timer.Dispose();
    }

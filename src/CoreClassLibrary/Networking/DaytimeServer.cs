using CoreClassLibrary.Extensions;

namespace CoreClassLibrary.Networking;

/// <summary>
/// Daytime Client
/// </summary>
/// <see cref="https://en.wikipedia.org/wiki/Daytime_Protocol"/>
/// <see cref="DaytimeClient"/>
public class DaytimeServer(TimeProvider timeProvider, ResponseStringServer server)
    {
    public const int DEFAULT_PORT = 13;

    public DaytimeServer(ResponseStringServer server): this(TimeProvider.System, server) { }

    public ValueTask ListenAsync(CancellationToken cancellationToken)
        => server.ListenAsync(cancellationToken => ValueTask.FromResult(timeProvider.GetLocalNow().ToISO8601String()), cancellationToken);
    }

using CoreClassLibrary.Extensions;

namespace CoreClassLibrary.Networking;

/// <summary>
/// Daytime Client
/// </summary>
/// <see cref="https://en.wikipedia.org/wiki/Daytime_Protocol"/>
/// <see cref="DaytimeServer"/>
public class DaytimeClient(ResponseStringClient client)
    {
    private const int BUFFER_LENGTH = 33;
    public const int DEFAULT_PORT = 13;

    public async ValueTask<DateTime> GetDateTimeAsync(CancellationToken cancellationToken)
        => DateTimeExtensions.ParseISO8601String(await client.ReceiveAsync(DaytimeClient.BUFFER_LENGTH, cancellationToken));
    }

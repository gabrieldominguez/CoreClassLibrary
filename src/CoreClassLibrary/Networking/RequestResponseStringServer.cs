using CoreClassLibrary.Extensions;

namespace CoreClassLibrary.Networking;

/// <summary>
/// String Request/Response Server
/// </summary>
/// <see cref="RequestResponseStringClient"/>
public class RequestResponseStringServer(IRequestResponseServer server)
    {
    public ValueTask ListenAsync(Func<string, CancellationToken, ValueTask<string>> service, CancellationToken cancellationToken)
        => server.ListenAsync(
            async (input, output, cancellationToken) => StringExtensions.UTF8Encoding.GetBytes(await service.Invoke(StringExtensions.UTF8Encoding.GetString(input.Span), cancellationToken), output.Span),
            cancellationToken);
    }

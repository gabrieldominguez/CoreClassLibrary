using CoreClassLibrary.Extensions;

namespace CoreClassLibrary.Networking;

/// <summary>
/// Server that proccess String requests without sending a response
/// </summary>
/// <see cref="RequestStringClient"/>
public class RequestStringServer(IRequestServer server)
    {
    public ValueTask ListenAsync(Func<string, CancellationToken, ValueTask> service, CancellationToken cancellationToken)
        => server.ListenAsync(
            (input, cancellationToken) => service.Invoke(StringExtensions.UTF8Encoding.GetString(input.Span), cancellationToken),
            cancellationToken);
    }

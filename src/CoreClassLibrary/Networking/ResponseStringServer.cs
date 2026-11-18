using CoreClassLibrary.Extensions;

namespace CoreClassLibrary.Networking;

/// <summary>
/// Server that proccess String requests without sending a response
/// </summary>
/// <see cref="RequestStringClient"/>
public class ResponseStringServer(IResponseServer server)
    {
    public ValueTask ListenAsync(Func<CancellationToken, ValueTask<string>> service, CancellationToken cancellationToken)
        => server.ListenAsync(async (output, cancellationToken) => StringExtensions.UTF8Encoding.GetBytes(await service.Invoke(cancellationToken), output.Span), cancellationToken);
    }

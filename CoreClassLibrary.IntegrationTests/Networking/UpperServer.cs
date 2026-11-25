using CoreClassLibrary.Networking;

namespace CoreClassLibrary.IntegrationTests.Networking;

public class UpperServer(RequestResponseStringServer server)
    {
    public ValueTask ListenAsync(CancellationToken cancellationToken)
        => server.ListenAsync(
            (input, cancellationToken) => ValueTask.FromResult(input.ToUpperInvariant()),
            cancellationToken);
    }

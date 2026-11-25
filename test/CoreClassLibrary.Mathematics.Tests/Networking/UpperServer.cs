
using CoreClassLibrary.Networking;

namespace CoreClassLibrary.Tests.Networking;

public class UpperServer(RequestResponseStringServer server)
    {
    public ValueTask ListenAsync(CancellationToken cancellationToken)
        => server.ListenAsync(
            (input, cancellationToken) => ValueTask.FromResult(input.ToUpperInvariant()),
            cancellationToken);
    }

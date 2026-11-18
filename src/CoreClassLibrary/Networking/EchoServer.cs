namespace CoreClassLibrary.Networking;

/// <summary>
/// Echo Server
/// </summary>
/// <see cref="IRequestResponseClient"/>
public class EchoServer(IRequestResponseServer server)
    {
    public ValueTask ListenAsync(CancellationToken cancellationToken)
        => server.ListenAsync((input, output, cancellationToken) => { input.CopyTo(output); return ValueTask.FromResult(input.Length); }, cancellationToken);
    }

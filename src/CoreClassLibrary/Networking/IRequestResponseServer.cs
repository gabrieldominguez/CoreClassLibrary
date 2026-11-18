namespace CoreClassLibrary.Networking;

/// <summary>
/// Request/Response Server
/// </summary>
/// <see cref="IRequestResponseClient"/>
public interface IRequestResponseServer
    {
    public ValueTask ListenAsync(Func<ReadOnlyMemory<byte>, Memory<byte>, CancellationToken, ValueTask<int>> service, CancellationToken cancellationToken);
    }

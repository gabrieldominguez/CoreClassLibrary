namespace CoreClassLibrary.Networking;

/// <summary>
/// Request/Response Client
/// </summary>
/// <see cref="IRequestResponseServer"/>
public interface IRequestResponseClient
    {
    public ValueTask<int> SendReceiveAsync(ReadOnlyMemory<byte> inputBuffer, Memory<byte> outputBuffer, CancellationToken cancellationToken);
    }
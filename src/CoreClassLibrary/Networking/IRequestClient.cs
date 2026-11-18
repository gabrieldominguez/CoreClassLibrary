namespace CoreClassLibrary.Networking;

/// <summary>
/// Client that sends requests without expecting a response.
/// </summary>
/// <see cref="IRequestServer"/>
public interface IRequestClient
    {
    public ValueTask SendAsync(ReadOnlyMemory<byte> inputBuffer, CancellationToken cancellationToken);
    }

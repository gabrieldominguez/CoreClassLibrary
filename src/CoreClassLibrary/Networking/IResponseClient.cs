namespace CoreClassLibrary.Networking;

/// <summary>
/// Client that receive messages without sending a request.
/// </summary>
/// <see cref="IRequestServer"/>
public interface IResponseClient
    {
    public ValueTask<int> ReceiveAsync(Memory<byte> outputBuffer, CancellationToken cancellationToken);
    }
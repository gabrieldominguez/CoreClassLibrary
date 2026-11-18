namespace CoreClassLibrary.Networking.UdpClientServer;

/// <summary>
/// UDP Client that receive messages without sending a request.
/// </summary>
/// <see cref="RequestTcpServer"/>
public class ResponseUdpClient(RequestResponseUdpClient client): IResponseClient
    {
    public ValueTask<int> ReceiveAsync(Memory<byte> outputBuffer, CancellationToken cancellationToken)
        => client.SendReceiveAsync(Array.Empty<byte>(), outputBuffer, cancellationToken);
    }

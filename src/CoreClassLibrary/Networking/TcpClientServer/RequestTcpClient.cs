using System.Net;
using System.Net.Sockets;

namespace CoreClassLibrary.Networking.TcpClientServer;

/// <summary>
/// TCP Client that sends requests without expecting a response.
/// </summary>
/// <see cref="RequestTcpServer"/>
public class RequestTcpClient(IPEndPoint ipEndPoint): IRequestClient
    {
    public async ValueTask SendAsync(ReadOnlyMemory<byte> inputBuffer, CancellationToken cancellationToken)
        { 
        using TcpClient client = new TcpClient();
        await client.ConnectAsync(ipEndPoint, cancellationToken);
        await using NetworkStream stream = client.GetStream();
        await stream.WriteAsync(inputBuffer, cancellationToken);
        }
    }

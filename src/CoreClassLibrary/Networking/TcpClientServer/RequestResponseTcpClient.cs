using System.Net;
using System.Net.Sockets;

namespace CoreClassLibrary.Networking.TcpClientServer;

/// <summary>
/// Request/Response TCP Client
/// </summary>
/// <see cref="RequestResponseTcpServer"/>
public class RequestResponseTcpClient(IPEndPoint ipEndPoint): IRequestResponseClient
    {
    public async ValueTask<int> SendReceiveAsync(ReadOnlyMemory<byte> inputBuffer, Memory<byte> outputBuffer, CancellationToken cancellationToken)
        {
        using TcpClient client = new TcpClient();
        await client.ConnectAsync(ipEndPoint, cancellationToken);
        await using NetworkStream stream = client.GetStream();

        await stream.WriteAsync(inputBuffer, cancellationToken);
        return await stream.ReadAsync(outputBuffer, cancellationToken);
        }
    }

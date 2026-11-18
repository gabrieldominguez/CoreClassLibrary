using System.Net;
using System.Net.Sockets;

namespace CoreClassLibrary.Networking.TcpClientServer;

/// <summary>
/// TCP Client that receive messages without sending a request.
/// </summary>
/// <see cref="RequestTcpServer"/>
public class ResponseTcpClient(IPEndPoint ipEndPoint): IResponseClient
    {
    public async ValueTask<int> ReceiveAsync(Memory<byte> outputBuffer, CancellationToken cancellationToken)
        { 
        using TcpClient client = new TcpClient();
        await client.ConnectAsync(ipEndPoint, cancellationToken);
        await using NetworkStream stream = client.GetStream();
        return await stream.ReadAsync(outputBuffer, cancellationToken);
        }
    }

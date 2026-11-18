using CoreClassLibrary.Networking;
using System.Net;
using System.Net.Sockets;

namespace CoreClassLibrary.Networking.UdpClientServer;

/// <summary>
/// UDP Server that proccess requests without sending a response
/// </summary>
/// <see cref="RequestUdpClient"/>
public class RequestUdpServer(int port): IRequestServer
    {
    public async ValueTask ListenAsync(Func<ReadOnlyMemory<byte>, CancellationToken, ValueTask> service, CancellationToken cancellationToken)
        { 
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, port);
        using UdpClient client = new UdpClient(endPoint);

        try
            {
            while (!cancellationToken.IsCancellationRequested)
                {
                UdpReceiveResult result = await client.ReceiveAsync(cancellationToken);
                await service.Invoke(result.Buffer, cancellationToken);
                }
            }
        catch(OperationCanceledException) { }
        }
    }

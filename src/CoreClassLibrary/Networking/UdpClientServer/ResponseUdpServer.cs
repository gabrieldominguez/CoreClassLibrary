using CoreClassLibrary.Networking;
using CoreClassLibrary.Objects.Pooling;
using CoreClassLibrary.Streams;
using System.Net;
using System.Net.Sockets;

namespace CoreClassLibrary.Networking.UdpClientServer;

/// <summary>
/// UDP Server that sends a messages without expecting a request.
/// </summary>
/// <see cref="RequestUdpClient"/>
public class ResponseUdpServer(int port, int outputBufferSize = StreamExtensions.DefaultWriteBufferSize): IResponseServer
    {
    public async ValueTask ListenAsync(Func<Memory<byte>, CancellationToken, ValueTask<int>> service, CancellationToken cancellationToken)
        { 
        using PooledMemory<byte> outputBuffer = new PooledMemory<byte>(outputBufferSize);

        IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, port);
        using UdpClient client = new UdpClient(endPoint);

        try
            {
            while (!cancellationToken.IsCancellationRequested)
                {
                UdpReceiveResult result = await client.ReceiveAsync(cancellationToken);
                int outputLength = await service.Invoke(outputBuffer.Memory, cancellationToken);
                await client.SendAsync(outputBuffer.Memory[..outputLength], result.RemoteEndPoint, cancellationToken);
                }
            }
        catch(OperationCanceledException) { }
        }
    }

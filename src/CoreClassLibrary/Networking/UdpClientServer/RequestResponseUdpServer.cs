using CoreClassLibrary.Networking;
using CoreClassLibrary.Objects.Pooling;
using CoreClassLibrary.Streams;
using System.Net;
using System.Net.Sockets;

namespace CoreClassLibrary.Networking.UdpClientServer;

/// <summary>
/// Request/Response UDP Server
/// </summary>
/// <see cref="RequestResponseUdpClient"/>
/// <see cref="https://learn.microsoft.com/en-us/dotnet/api/system.net.sockets.udpclient"/>
public class RequestResponseUdpServer(int port, int outputBufferSize = StreamExtensions.DefaultWriteBufferSize): IRequestResponseServer
    {
    public async ValueTask ListenAsync(Func<ReadOnlyMemory<byte>, Memory<byte>, CancellationToken, ValueTask<int>> service, CancellationToken cancellationToken)
        { 
        using PooledMemory<byte> outputBuffer = new PooledMemory<byte>(outputBufferSize);

        IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, port);
        using UdpClient client = new UdpClient(endPoint);

        try
            {
            while (!cancellationToken.IsCancellationRequested)
                {
                UdpReceiveResult result = await client.ReceiveAsync(cancellationToken);
                int outputLength = await service.Invoke(result.Buffer, outputBuffer.Memory, cancellationToken);
                await client.SendAsync(outputBuffer.Memory[..outputLength], result.RemoteEndPoint, cancellationToken);
                }
            }
        catch(OperationCanceledException) { }
        }
    }

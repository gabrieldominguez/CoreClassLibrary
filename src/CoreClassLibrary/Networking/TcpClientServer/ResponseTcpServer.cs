using CoreClassLibrary.Objects.Pooling;
using CoreClassLibrary.Streams;
using System.Net;
using System.Net.Sockets;

namespace CoreClassLibrary.Networking.TcpClientServer;

/// <summary>
/// TCP Server that sends a messages without expecting a request.
/// </summary>
/// <see cref="RequestTcpClient"/>
public class ResponseTcpServer(IPEndPoint ipEndPoint, int outputBufferSize = StreamExtensions.DefaultWriteBufferSize): IResponseServer
    {
    public ResponseTcpServer(int port): this(new IPEndPoint(IPAddress.Any, port)) { }

    public async ValueTask ListenAsync(Func<Memory<byte>, CancellationToken, ValueTask<int>> service, CancellationToken cancellationToken)
        { 
        using PooledMemory<byte> outputBuffer = new PooledMemory<byte>(outputBufferSize);

        using TcpListener listener = new TcpListener(ipEndPoint);
        listener.Start();

        try
            {
            while (!cancellationToken.IsCancellationRequested)
                {
                using TcpClient client = await listener.AcceptTcpClientAsync(cancellationToken);
                await using NetworkStream stream = client.GetStream();

                int outputLength = await service.Invoke(outputBuffer.Memory, cancellationToken);
                await stream.WriteAsync(outputBuffer.Memory[..outputLength], cancellationToken);
                }
            }
        catch(OperationCanceledException) { }
        finally
            {
            listener.Stop();
            }
        }
    }

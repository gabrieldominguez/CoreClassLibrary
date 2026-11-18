using CoreClassLibrary.Objects.Pooling;
using CoreClassLibrary.Streams;
using System.Net;
using System.Net.Sockets;

namespace CoreClassLibrary.Networking.TcpClientServer;

/// <summary>
/// TCP Server that proccess requests without sending a response
/// </summary>
/// <see cref="RequestTcpClient"/>
public class RequestTcpServer(IPEndPoint ipEndPoint, int inputBufferSize = StreamExtensions.DefaultReadBufferSize): IRequestServer
    {
    public RequestTcpServer(int port): this(new IPEndPoint(IPAddress.Any, port)) { }

    public async ValueTask ListenAsync(Func<ReadOnlyMemory<byte>, CancellationToken, ValueTask> service, CancellationToken cancellationToken)
        { 
        using PooledMemory<byte> inputBuffer = new PooledMemory<byte>(inputBufferSize);

        using TcpListener listener = new TcpListener(ipEndPoint);
        listener.Start();

        try
            {
            while (!cancellationToken.IsCancellationRequested)
                {
                using TcpClient client = await listener.AcceptTcpClientAsync(cancellationToken);
                await using NetworkStream stream = client.GetStream();

                int received = await stream.ReadAsync(inputBuffer.Memory, cancellationToken);
                await service.Invoke(inputBuffer.Memory[0..received], cancellationToken);
                }
            }
        catch(OperationCanceledException) { }
        finally
            {
            listener.Stop();
            }
        }
    }

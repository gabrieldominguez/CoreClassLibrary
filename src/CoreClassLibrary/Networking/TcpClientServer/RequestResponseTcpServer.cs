using CoreClassLibrary.Objects.Pooling;
using CoreClassLibrary.Streams;
using System.Net;
using System.Net.Sockets;

namespace CoreClassLibrary.Networking.TcpClientServer;

/// <summary>
/// Request/Response TCP Server
/// </summary>
/// <see cref="RequestResponseTcpClient"/>
/// <see cref="https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/sockets/tcp-classes"/>
/// <see cref="https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/sockets/socket-services"/>
/// <see cref="https://learn.microsoft.com/en-us/dotnet/api/system.net.sockets.tcplistener"/>
public class RequestResponseTcpServer(IPEndPoint ipEndPoint, int inputBufferSize = StreamExtensions.DefaultReadBufferSize, int outputBufferSize = StreamExtensions.DefaultWriteBufferSize): IRequestResponseServer
    {
    public RequestResponseTcpServer(int port, int inputBufferSize = StreamExtensions.DefaultReadBufferSize, int outputBufferSize = StreamExtensions.DefaultWriteBufferSize): this(new IPEndPoint(IPAddress.Any, port), inputBufferSize, outputBufferSize) { }

    public async ValueTask ListenAsync(Func<ReadOnlyMemory<byte>, Memory<byte>, CancellationToken, ValueTask<int>> service, CancellationToken cancellationToken)
        { 
        using PooledMemory<byte> inputBuffer = new PooledMemory<byte>(inputBufferSize);
        using PooledMemory<byte> outputBuffer = new PooledMemory<byte>(outputBufferSize);

        using TcpListener listener = new TcpListener(ipEndPoint);
        listener.Start();

        try
            {
            while (!cancellationToken.IsCancellationRequested)
                {
                using TcpClient client = await listener.AcceptTcpClientAsync(cancellationToken);
                await using NetworkStream stream = client.GetStream();

                int received = await stream.ReadAsync(inputBuffer.Memory, cancellationToken);
                int outputLength = await service.Invoke(inputBuffer.Memory[..received], outputBuffer.Memory, cancellationToken);
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

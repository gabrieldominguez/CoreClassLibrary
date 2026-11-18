using CoreClassLibrary.Extensions;
using CoreClassLibrary.Objects.Pooling;
using System.Net;
using System.Net.Sockets;

namespace CoreClassLibrary.Networking.TcpClientServer;

/// <summary>
/// Request only Length-Prefix String Server
/// </summary>
/// <see cref="RequestLengthPrefixStringClient"/>
public class RequestLengthPrefixStringServer(IPEndPoint ipEndPoint, int prefixLength = 5)
    {
    public RequestLengthPrefixStringServer(int port = 0, int prefixLength = 5): this(new IPEndPoint(IPAddress.Any, port), prefixLength) { }

    public async ValueTask ListenAsync<TState>(TState state, Func<TState, string, CancellationToken, ValueTask> service, CancellationToken cancellationToken)
        { 
        using PooledMemory<byte> prefixBuffer = new PooledMemory<byte>(prefixLength);

        using TcpListener listener = new TcpListener(ipEndPoint);
        listener.Start();

        int port = ((IPEndPoint)listener.LocalEndpoint).Port;
        try
            {
            while (!cancellationToken.IsCancellationRequested)
                {
                using TcpClient client = await listener.AcceptTcpClientAsync(cancellationToken);
                await using NetworkStream stream = client.GetStream();

                await stream.ReadExactlyAsync(prefixBuffer.Memory, cancellationToken);

                int messageLength = int.Parse(StringExtensions.UTF8Encoding.GetString(prefixBuffer.Memory.Span)) - prefixLength;

                using PooledMemory<byte> messageBuffer = new PooledMemory<byte>(messageLength);

                await stream.ReadExactlyAsync(messageBuffer.Memory, cancellationToken);

                await service.Invoke(state, StringExtensions.UTF8Encoding.GetString(messageBuffer.Memory.Span), cancellationToken);
                }
            }
        catch(OperationCanceledException) { }
        finally
            {
            listener.Stop();
            }
        }
    }

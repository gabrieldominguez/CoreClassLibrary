using System.Net;
using System.Net.Sockets;

namespace CoreClassLibrary.Networking.UdpClientServer;

/// <summary>
/// UDP Client that sends requests without expecting a response.
/// </summary>
/// <see cref="RequestUdpServer"/>
public class RequestUdpClient(IPEndPoint ipEndPoint): IRequestClient
    {
    /// <summary>
    /// Send a UDP datagram asynchronously.
    /// </summary>
    /// <param name="inputBuffer">
    /// An <see cref="ReadOnlyMemory{T}"/> of Type <see cref="byte"/> that specifies the UDP datagram that you intend to send.
    /// NO USAR PooledArray<byte>. USAR PooledMemory.
    /// </param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="ValueTask{TResult}"/> representing the asynchronous operation.</returns>
    public async ValueTask SendAsync(ReadOnlyMemory<byte> inputBuffer, CancellationToken cancellationToken)
        {         
        using UdpClient client = new UdpClient();
        client.Connect(ipEndPoint);

        await client.SendAsync(inputBuffer, cancellationToken);
        }
    }

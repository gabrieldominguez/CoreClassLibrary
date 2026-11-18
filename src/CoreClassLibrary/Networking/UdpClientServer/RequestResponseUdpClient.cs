using CoreClassLibrary.Networking;
using System.Net;
using System.Net.Sockets;

namespace CoreClassLibrary.Networking.UdpClientServer;

/// <summary>
/// Request/Response UDP Client
/// </summary>
/// <see cref="RequestResponseUdpServer"/>
/// <see cref="https://learn.microsoft.com/en-us/dotnet/api/system.net.sockets.udpclient"/>
public class RequestResponseUdpClient(IPEndPoint ipEndPoint): IRequestResponseClient
    {
    /// <summary>
    /// Sends and receive a UDP datagram asynchronously to a remote host.
    /// </summary>
    /// <param name="inputBuffer">
    /// An <see cref="ReadOnlyMemory{T}"/> of Type <see cref="byte"/> that specifies the UDP datagram that you intend to send.
    /// NO USAR PooledArray<byte>. USAR PooledMemory.
    /// </param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    public async ValueTask<int> SendReceiveAsync(ReadOnlyMemory<byte> inputBuffer, Memory<byte> outputBuffer, CancellationToken cancellationToken)
        {         
        using UdpClient client = new UdpClient();
        client.Connect(ipEndPoint);

        await client.SendAsync(inputBuffer, cancellationToken);
        UdpReceiveResult result = await client.ReceiveAsync(cancellationToken);
        result.Buffer.CopyTo(outputBuffer);
        return result.Buffer.Length;
        }
    }

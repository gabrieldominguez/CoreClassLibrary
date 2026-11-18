#pragma warning disable IDE0046 // Convert to conditional expression

using CoreClassLibrary.Objects.Ranges;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace CoreClassLibrary.Networking;

/// <summary>
/// Socket Extensions Methods
/// </summary>
/// <remarks>
/// How to: Configure Network Tracing
/// http://msdn.microsoft.com/en-us/library/ty48b824(v=vs.110).aspx
/// </remarks>
public static class SocketExtensions
    {
    /// <summary>
    /// Ports with numbers 1024-49151 (called user or registered ports)
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Registered_port"/>
    /// <see cref="http://www.rfc-editor.org/rfc/rfc6335.txt"/>
    //public static readonly Range UserPortsRange = new Range(1024, 49152);
    public static readonly Range<int> UserPortsRange = new Range<int>(1024, 49151);

    public static IPAddress GetHostAddress(string hostName = "localhost")
        => Dns.GetHostEntry(hostName).AddressList[0];

    /// <summary>
    /// Retorna un puerto libre.
    /// </summary>
    public static int GetFreePort()
        {
        using TcpListener listener = new TcpListener(new IPEndPoint(IPAddress.Any, 0));
        listener.Start();
        IPEndPoint endPoint = (IPEndPoint)listener.LocalEndpoint;
        listener.Stop();

        return endPoint.Port;
        }
    
    /// <summary>
    /// Retorna el siguiente puerto libre.
    /// </summary>
    /// <see cref="https://stackoverflow.com/questions/53815519/get-random-free-opened-port-for-tests"/>
    public static int NextFreePort(int port = 1024) 
        {
        while (!SocketExtensions.IsFreePort(port)) 
            {
            port++;
            }
        return port;
        }

    public static bool IsFreePort(int port) 
        => SocketExtensions.GetActiveListenersEndPoints()
            .Select(endpoint => endpoint.Port).ToArray().All(openPort => openPort != port);

    public static IEnumerable<IPEndPoint> GetActiveListenersEndPoints()
        {
        IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
        return ipProperties.GetActiveTcpListeners()
                    .Concat(ipProperties.GetActiveUdpListeners())
                    //.Concat(ipProperties.GetActiveTcpConnections().Select(connection => connection.LocalEndPoint))
                    ;
        }
       
    public static void EnsureConnected(this Socket socket)
        {
        if (!socket.IsCurrentlyConnected())
            {
            socket.Connect(socket.RemoteEndPoint ?? throw new InvalidOperationException("No RemoteEndPoint."));
            }
        }

    /// <summary>
    /// Gets a value that indicates whether a socket is currently connected to a remote host.
    /// </summary>
    /// <remarks>
    /// The Connected property gets the connection state of the Socket as of the last I/O operation.
    ///
    /// The value of the Connected property reflects the state of the connection as of the most
    /// recent operation. If you need to determine the current state of the connection, make a
    /// nonblocking, zero-byte Send call. If the call returns successfully or throws a WAEWOULDBLOCK
    /// error code (10035), then the socket is still connected; otherwise, the socket is no longer connected.
    /// </remarks>
    /// <returns>true if the socket is currently connected to a remote resource; otherwise, false.</returns>
    /// <see cref="http://support.microsoft.com/es-es/kb/819124"/>
    public static bool IsCurrentlyConnected(this Socket socket)
        {
        bool blockingState = socket.Blocking;
        try
            {
            socket.Blocking = false;
            socket.Send([0]);

            return true;
            }
        catch (SocketException e)
            {
            // e.NativeErrorCode == 10035 (WSAEWOULDBLOCK)
            // True: Still Connected, but the Send would block
            // False: Disconnected
            return e.SocketErrorCode == SocketError.WouldBlock;
            }
        finally
            {
            socket.Blocking = blockingState;
            }
        }


    public static bool CanRead(this Socket socket)
        {
        try
            {
            return socket.Poll(socket.ReceiveTimeout * 10, SelectMode.SelectRead);
            }
        catch
            {
            return false;
            }
        }

    public static bool CanWrite(this Socket socket)
        {
        try
            {
            return socket.Poll(socket.SendTimeout * 10, SelectMode.SelectWrite);
            }
        catch
            {
            return false;
            }
        }

    /// <summary>
    /// Discard the data that has been received from the network and is available to be read.
    /// </summary>
    /// <param name="socket">Berkeley sockets interface implementation.</param>
    public static void Discard(this Socket socket, Span<byte> buffer)
        {
        if (socket.Available > 0)
            {
            socket.ReceiveExactlyAndCheck(buffer, socket.Available);
            }
        }

    /// <summary>
    /// Idem ReceiveAndCheck pero recibe la totalidad de los bytes esperados.
    /// </summary>
    /// <param name="socket">Berkeley sockets interface implementation.</param>
    /// <param name="buffer">An array of type <see cref="T:System.Byte"/> that is the storage location for received data. </param>
    /// <param name="offset">The location in <paramref name="buffer"/> to store the received data. </param>
    /// <param name="totalSize">The total number of bytes to receive. </param>
    /// <remarks>
    /// Garantiza que se reciban la totalidad de bytes pedidos (y no menos).
    /// </remarks>
    public static void ReceiveExactlyAndCheck(this Socket socket, Span<byte> buffer, int totalSize)
        {
        while (totalSize > 0)
            {
            int receivedSize = socket.ReceiveAndCheck(buffer);
            totalSize -= receivedSize;
            }
        }

    /// <summary>
    /// Idem Socket.Receive pero chequea que el host no haya cerrado la conexión.
    /// </summary>
    /// <remarks>
    /// If you are using a connection-oriented Socket, the Receive method will read as much data as
    /// is available, up to the size of the buffer. If the remote host shuts down the Socket connection
    /// with the Shutdown method, and all available data has been received, the Receive method will
    /// complete immediately and return zero bytes.
    ///
    /// receivedSize puede ser menor a maxSize (pero no mayor).
    /// </remarks>
    /// <see cref="http://msdn.microsoft.com/en-us/library/aa329732(VS.71).aspx"/>
    /// <param name="socket">Berkeley sockets interface implementation.</param>
    /// <param name="buffer">An array of type <see cref="T:System.Byte"/> that is the storage location for received data. </param>
    /// <returns>The number of bytes received.</returns>
    public static int ReceiveAndCheck(this Socket socket, Span<byte> buffer)
        {
        int receivedSize = socket.Receive(buffer, SocketFlags.None);

        if (receivedSize == 0)
            {
            throw new Exception("Either the remote host shuts down the socket connection with the Shutdown method and all available data has been received (the Receive method completed immediately and returned zero bytes) or this host shuts down the socket connection with the Shutdown method.");
            }

        return receivedSize;
        }

    public static async Task<int> ReceiveExactlyAsync(this Socket socket, Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
        int totalReceivedBytes = 0;

        while (totalReceivedBytes < buffer.Length)
            {
            int receivedBytes = await socket.ReceiveAsync(buffer[totalReceivedBytes..], SocketFlags.None, cancellationToken);
            totalReceivedBytes += receivedBytes;
            }

        return totalReceivedBytes;
        }

    /*
    /// <summary>
    /// Recibe datos a traves del socket hasta completar totalmente el buffer.
    /// </summary>
    [Obsolete("Use ReadExactlyAsync")]
    public static async ValueTask<int> ReadCompleteBufferAsync(this NetworkStream stream, Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
        int totalReceivedBytes = 0;

        while (totalReceivedBytes < buffer.Length)
            {
            int receivedBytes = await stream.ReadAsync(buffer[totalReceivedBytes..], cancellationToken);
            totalReceivedBytes += receivedBytes;
            }

        return totalReceivedBytes;
        }
    */
    }

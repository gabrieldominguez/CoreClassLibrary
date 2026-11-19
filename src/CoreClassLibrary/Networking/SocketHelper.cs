// Use IRequestResponseServer, IRequestResponseClient, etc

/*
using CoreClassLibrary.Objects.Pooling;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace CoreClassLibrary.Networking;

/// <summary>
/// Socket Networking Helper Class
/// </summary>
/// <remarks>
/// How to: Configure Network Tracing
/// http://msdn.microsoft.com/en-us/library/ty48b824(v=vs.110).aspx
/// </remarks>
public static class SocketHelper
    {
    public static string SendReceive(IPAddress serverAddress, int port, int receiveBufferSize, int sendReceiveTimeout, Encoding encoding, string message)
        => encoding.GetString(SocketHelper.SendReceive(serverAddress, port, receiveBufferSize, sendReceiveTimeout, encoding.GetBytes(message)));

    public static byte[] SendReceive(IPAddress serverAddress, int port, int receiveBufferSize, int sendReceiveTimeout, byte[] requestBuffer)
        {
        byte[] responseBuffer = new byte[receiveBufferSize];

        using Socket tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            
        //socket.ReceiveBufferSize = this.ReceiveBufferSize;
        //tcpSocket.NoDelay = true;
        tcpSocket.SendTimeout = tcpSocket.ReceiveTimeout = sendReceiveTimeout;
        tcpSocket.Connect(serverAddress, port);

        tcpSocket.Send(requestBuffer);

        int lengthResponse = tcpSocket.Receive(responseBuffer);

        return responseBuffer;
        }

    public static void SendString(IPAddress serverAddress, int port, int sendTimeout, Encoding encoding, string message) 
        => SocketHelper.Send(serverAddress, port, sendTimeout, encoding.GetBytes(message));

    public static void Send(IPAddress serverAddress, int port, int sendTimeout, byte[] requestBuffer)
        {
        using Socket tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        tcpSocket.SendTimeout = sendTimeout;
        tcpSocket.Connect(serverAddress, port);
        tcpSocket.Send(requestBuffer);
        }

    public static string ReceiveString(Encoding encoding, IPAddress serverAddress, int port, int receiveBufferSize, int receiveTimeout)
        => encoding.GetString(SocketHelper.Receive(serverAddress, port, receiveBufferSize, receiveTimeout));

    public static byte[] Receive(IPAddress serverAddress, int port, int receiveBufferSize, int receiveTimeout)
        {
        byte[] responseBuffer = new byte[receiveBufferSize];

        using Socket tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) { ReceiveTimeout = receiveTimeout };

        tcpSocket.Connect(serverAddress, port);

        int lengthResponse = tcpSocket.Receive(responseBuffer);
            
        return responseBuffer;
        }

   
    /// <summary>
    /// Envia un mensaje a un puerto otra máquina usando el protocolo udp.
    /// </summary>
    /// <param name="message">Mensaje a enviar.</param>
    /// <param name="address">Dirección ip al que enviar el mensaje.</param>
    /// <param name="port">Puerto del destino al que enviar el mensaje.</param>
    /// <param name="bufferSize">Tamaño del buffer de envio de mensajes.</param>
    public static void UdpSend(Encoding encoding, string message, IPAddress address, int port, int bufferSize = 0x10000) 
        => SocketHelper.Send(encoding, message, address, port, SocketType.Dgram, ProtocolType.Udp, bufferSize);

    /// <summary>
    /// Envia un mensaje a un puerto otra máquina usando el protocolo tcp.
    /// </summary>
    /// <param name="message">Mensaje a enviar.</param>
    /// <param name="address">Dirección ip al que enviar el mensaje.</param>
    /// <param name="port">Puerto del destino al que enviar el mensaje.</param>
    /// <param name="bufferSize">Tamaño del buffer de envio de mensajes.</param>
    public static void TcpSend(Encoding encoding, string message, IPAddress address, int port, int bufferSize = 0x10000) 
        => SocketHelper.Send(encoding, message, address, port, SocketType.Stream, ProtocolType.Tcp, bufferSize);

    public static void Send(Encoding encoding, string message, IPAddress address, int port, SocketType socketType, ProtocolType protocolType, int bufferSize = 0x10000)
        {
        using Socket socket = new Socket(AddressFamily.InterNetwork, socketType, protocolType);
        //socket.SendTimeout = 500;
        socket.SendBufferSize = bufferSize;
        socket.Connect(address, port);
        socket.Send(encoding.GetBytes(message));
        }

    /// <summary>
    /// Escucha en un puerto la llegada de mensajes por protocolo udp.
    /// </summary>
    /// <param name="port">Numero de puerto en el cual esperar los mensajes.</param>
    /// <param name="predicate">Funcion que se evalua con cada mensaje recibido o cuando se excede el tiempo de espera y si devuelve true se continua esperando nuevos mensajes.</param>
    /// <param name="receiveTimeout">Tiempo de espera en milisegundos para la recepción de de mensajes.</param>
    /// <param name="bufferSize">Tamaño del buffer de recepción de mensajes.</param>
    /// <returns>Mensaje recibido.</returns>
    public static IEnumerable<string> UdpListen(Encoding encoding, int port, Predicate<string> predicate, int receiveTimeout = 0, int bufferSize = 0x10000)
        {
        using PooledArray<byte> bufferInMessage = new PooledArray<byte>(bufferSize);
        using Socket receiveSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp) { ReceiveTimeout = receiveTimeout };
        receiveSocket.Bind(new IPEndPoint(IPAddress.Any, port));

        string message;
        do
            {
            int bytesReceived = 0;
            try
                {
                bytesReceived = receiveSocket.Receive(bufferInMessage.Array);
                }
            catch (SocketException) { }

            if (bytesReceived > 0)
                {
                yield return message = encoding.GetString(bufferInMessage.Array, 0, bytesReceived);
                }
            else
                {
                message = "";
                }
            }
        while (predicate(message));
        }

    /// <summary>
    /// Escucha en un puerto la llegada de mensajes por protocolo tcp.
    /// </summary>
    /// <param name="port">Numero de puerto en el cual esperar los mensajes.</param>
    /// <param name="predicate">Funcion que se evalua con cada mensaje recibido y si devuelve true se continua esperando nuevos mensajes.</param>
    /// <param name="bufferSize">Tamaño del buffer de recepción de mensajes.</param>
    /// <returns>Mensaje recibido.</returns>
    public static IEnumerable<string> TcpListen(Encoding encoding, int port, Predicate<string> predicate, int bufferSize = 0x10000)
        {
        using PooledArray<byte> bufferInMessage = new PooledArray<byte>(bufferSize);
        using Socket listeningSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        listeningSocket.Bind(new IPEndPoint(IPAddress.Any, port));
        listeningSocket.Listen(int.MaxValue);
        string message;
        do
            {
            using Socket receiveSocket = listeningSocket.Accept();
            int bytesReceived = receiveSocket.Receive(bufferInMessage.Array);
            message = encoding.GetString(bufferInMessage.Array, 0, bytesReceived);

            yield return message;
            }
        while (predicate(message));
        }

    /// <summary>
    /// Idem UdpListen pero usando UdpClient
    /// </summary>
    public static IEnumerable<string> UdpListen2(int port, Predicate<string> predicate, int bufferSize = 0x10000)
        {
        UdpClient udpClient = new UdpClient(port);            
        string message;
        do
            {
            IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] bufferInMessage = udpClient.Receive(ref remoteIpEndPoint);

            message = Encoding.ASCII.GetString(bufferInMessage);
            yield return message;
            }
        while (predicate(message));
        }

    /// <summary>
    /// Idem TcpListen pero usando TcpListener
    /// </summary>
    public static IEnumerable<string> TcpListen2(int port, Predicate<string> predicate, int bufferSize = 0x10000)
        {
        byte[] bufferInMessage = new byte[bufferSize];

        TcpListener tcpListener = new TcpListener(IPAddress.Any, port);
        tcpListener.Start();

        string message;
        do
            {
            using (Socket receiveSocket = tcpListener.AcceptSocket())
                {
                int bytesReceived = receiveSocket.Receive(bufferInMessage);
                message = Encoding.ASCII.GetString(bufferInMessage, 0, bytesReceived);
                yield return message;
                }
            }
        while (predicate(message));
        }
    }
*/
using CoreClassLibrary.Extensions;
using CoreClassLibrary.Objects.Pooling;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace CoreClassLibrary.Networking.SyncSocketWrappers;

/// <summary>
/// Proporciona un socket conectado a un EndPoint determinado.
/// </summary>
/// <remarks>
/// Mantiene los buffers de entrada y salida y gestiona la conexión y desconexión del socked.
/// Solo permite envío y recepción sincrónicos para poder usarse SyncSocketWrapperPool
/// </remarks>
public class SyncSocketWrapper(
        EndPoint remoteEndPoint,
        AddressFamily addressFamily = AddressFamily.InterNetwork,
        SocketType socketType = SocketType.Stream,
        ProtocolType protocolType = ProtocolType.Tcp,
        int receiveTimeout = Timeout.Infinite,
        int sendTimeout = Timeout.Infinite,
        int receiveBufferSize = 8192,
        int sendBufferSize = 8192
        ) : IDisposable
    {
    private readonly PooledMemory<byte> sendBuffer = new PooledMemory<byte>(sendBufferSize);
    private readonly PooledMemory<byte> receiveBuffer = new PooledMemory<byte>(receiveBufferSize);
    private readonly Lock syncRoot = new Lock();
    private Socket? socket;

    public EndPoint EndPoint => remoteEndPoint;

    /// <summary>
    /// Obtiene un socket, conectándolo si es nuevo.
    /// </summary>
    /// <remarks>
    /// Si el destino cerró la conexión el socket no estará conectado y no hay forma de saberlo hasta que no se envie un paquete y se evalue la respuesta.
    ///
    /// Es más rapido que ConnectedSyncSocketWrapper.GetSocked() pero no asegura que el socket esté actualmente conectado.
    /// </remarks>
    /// <returns>Socket creado.</returns>
    public Socket GetSocked()
        {
        if (this.socket == null)
            {
            lock (this.syncRoot)
                {
                if (this.socket == null)
                    {
                    this.socket = new Socket(addressFamily, socketType, protocolType)
                        {
                        ReceiveTimeout = receiveTimeout,
                        SendTimeout = sendTimeout,
                        ReceiveBufferSize = receiveBufferSize,
                        SendBufferSize = sendBufferSize
                        };
                    this.socket.Connect(remoteEndPoint);
                    }
                }
            }

        return this.socket;
        }

    public void Disconnect()
        {
        if (this.socket != null)
            {
            lock (this.syncRoot)
                {
                if (this.socket != null)
                    {
                    try
                        {
                        this.socket.Shutdown(SocketShutdown.Both);
                        this.socket.Disconnect(false);
                        }
                    /*
                    catch (SocketException ex)
                        {
                        logger.Log(ex);
                        logger.Log(LogLevel.Error, "SocketErrorCode: {0}", ex.SocketErrorCode);
                        throw;
                        }
                    */
                    finally
                        {
                        this.socket.Close();
                        this.socket = null;
                        }
                    }
                }
            }
        }

    public int SendString(string message)
        => this.SendString(message, StringExtensions.UTF8Encoding);

    public int SendString(string message, Encoding encoding)
        {
        Socket socket = this.GetSocked();
        socket.Discard(this.receiveBuffer.Memory.Span);
        int length = encoding.GetBytes(message, this.sendBuffer.Memory.Span);
        return socket.Send(this.sendBuffer.Memory.Span[0..length]);
        }

    public string ReceiveString(int size)
        => this.ReceiveString(size, StringExtensions.UTF8Encoding);

    public string ReceiveString(int size, Encoding encoding)
        {
        this.Receive(size);
        return encoding.GetString(this.receiveBuffer.Memory.Span[0..size]);
        }

    private void Receive(int size)
        {
        this.GetSocked().ReceiveExactlyAndCheck(this.receiveBuffer.Memory.Span, size);
        }

    public void Dispose()
        {
        this.Dispose(true);

        GC.SuppressFinalize(this);
        }

    protected virtual void Dispose(bool disposing)
        {
        if (disposing)
            {
            this.Disconnect();
            this.sendBuffer.Dispose();
            this.receiveBuffer.Dispose();
            }
        }
    }

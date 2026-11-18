using System.Collections.Concurrent;
using System.Net;

namespace CoreClassLibrary.Networking.SyncSocketWrappers;

/// <summary>
/// Lista singleton de SocketWrappers. 
/// </summary>
/// <remarks>
/// Permite sincronizar el accesso a los sockets del pool.
/// </remarks>
/// <remarks>
/// How to: Configure Network Tracing
/// http://msdn.microsoft.com/en-us/library/ty48b824(v=vs.110).aspx
/// 
/// Socket.Receive
/// http://msdn.microsoft.com/en-us/library/8s4y8aff(v=vs.110).aspx
/// </remarks>
public sealed class SyncSocketWrapperPool(Func<EndPoint> endPointProvider, Func<EndPoint, SyncSocketWrapper> socketWrapperProvider) : IDisposable
    {
    private readonly ConcurrentDictionary<EndPoint, SyncSocketWrapper> pool = new ConcurrentDictionary<EndPoint, SyncSocketWrapper>();

    /// <summary>
    /// Proporciona acceso sincronizado a un socket del pool ya conectado a un endpoint.
    /// </summary>
    public void InvokeWithSocket(Action<SyncSocketWrapper> action)
        {
        SyncSocketWrapper socketWrapper = this.GetSocketWrapper();

        lock (socketWrapper)
            {
            action.Invoke(socketWrapper);
            }
        }

    public TOutput InvokeWithSocket<TInput, TOutput>(Func<SyncSocketWrapper, TInput, TOutput> action, TInput input)
        {
        SyncSocketWrapper socketWrapper = this.GetSocketWrapper();

        lock (socketWrapper)
            {
            return action(socketWrapper, input);
            }
        }

    private SyncSocketWrapper GetSocketWrapper()
        => this.pool.GetOrAdd(endPointProvider(), socketWrapperProvider);

    public void Dispose()
        {
        foreach (SyncSocketWrapper socketWrapper in this.pool.Values)
            {
            socketWrapper.Dispose();
            }

        this.pool.Clear();
        }
    }

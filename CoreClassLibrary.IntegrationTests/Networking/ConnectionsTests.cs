using CoreClassLibrary.Networking;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

namespace CoreClassLibrary.IntegrationTests.Networking;

public class ConnectionsTests(ITestOutputHelper output)
    {
    [Fact]
    public void IsFreePort()
        {
        using TcpListener listener = new TcpListener(new IPEndPoint(IPAddress.Any, 0));
        listener.Start();
        int port = ((IPEndPoint)listener.LocalEndpoint).Port;
        Assert.False(SocketExtensions.IsFreePort(port));
        listener.Stop();
        Assert.True(SocketExtensions.IsFreePort(port));
        }
    
    [Fact]
    public void GetActiveListeners()
        {
        IPEndPoint[] endPoints = SocketExtensions.GetActiveListenersEndPoints().ToArray();
        foreach (IPEndPoint endPoint in endPoints)
            {
            output.WriteLine(endPoint.ToString());
            }
        }

    [Fact]
    public void GetActiveTcpConnections()
        {
        IPEndPoint[] endPoints = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpConnections().Select(connection => connection.LocalEndPoint).ToArray();
        foreach (IPEndPoint endPoint in endPoints)
            {
            output.WriteLine(endPoint.ToString());
            }
        }
    }

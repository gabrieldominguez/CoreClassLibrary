#pragma warning disable IDE0305 // Simplify collection initialization

using CoreClassLibrary.Mathematics.Randomness;
using CoreClassLibrary.Networking;
using CoreClassLibrary.Networking.SyncSocketWrappers;
using CoreClassLibrary.Networking.UdpClientServer;
using CoreClassLibrary.Objects.Pooling;
using CoreClassLibrary.Objects.Ranges;
using System.Net;
using System.Net.Sockets;
using Xunit;

namespace CoreClassLibrary.Tests.Networking;

public class UdpTests(ITestOutputHelper outputHelper)
{
    [Fact]
    public Task SntpClientValidateLocalTime()
        => SntpClient.ValidateLocalTimeAsync(TimeProvider.System, [
            IPAddress.Parse("172.16.11.42"),
            IPAddress.Parse("172.16.1.224"),
            IPAddress.Loopback
            ], 100000, TestContext.Current.CancellationToken);

    [Theory]
    [InlineData("172.16.11.42")]
    [InlineData("172.16.1.224")]
    [InlineData("127.0.0.1")]
    public async Task SntpClientLocalValidation(string serverAddress)
    {
        using PooledMemory<byte> dataBuffer = new PooledMemory<byte>(SntpData.Length);
        SntpClient client = new SntpClient(TimeProvider.System, IPAddress.Parse(serverAddress));
        SntpClientData data = await client.GetDataAsync(dataBuffer.Memory, TestContext.Current.CancellationToken);

        outputHelper.WriteLine(data.ToString());
        Assert.True(data.IsValidLocalTime(100000));
    }

    /*
    [Theory]
    [InlineData("pool.ntp.org")]        // https://www.ntp.org/
    [InlineData("ar.pool.ntp.org")]        // https://www.ntppool.org/zone/ar
    [InlineData("ntp.inti.gob.ar")]     // https://www.inti.gob.ar/hora-inti
    public async Task SntpClientInternetValidation(string url)
        {
        using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(1000);

        using PooledMemory<byte> dataBuffer = new PooledMemory<byte>(SntpData.Length);
        SntpClient client = new SntpClient(TimeProvider.System, Dns.GetHostEntry(url).AddressList[0]);
        SntpClientData data = await client.GetDataAsync(dataBuffer.Memory, cancellationTokenSource.Token);

        outputHelper.WriteLine(data.ToString());
        Assert.True(data.IsValidLocalTime(100000));
        }
    */

    [Fact]
    public async Task SntpClientServer()
    {
        using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        int port = RandomWrapper.Shared.NextInclusive(SocketExtensions.UserPortsRange);

        SntpServer server = new SntpServer(TimeProvider.System, port);
        ValueTask listening = server.ListenAsync(cancellationTokenSource.Token);

        using PooledMemory<byte> dataBuffer = new PooledMemory<byte>(SntpData.Length);
        SntpClient client = new SntpClient(TimeProvider.System, new IPEndPoint(IPAddress.Loopback, port));
        SntpClientData data = await client.GetDataAsync(dataBuffer.Memory, TestContext.Current.CancellationToken);

        outputHelper.WriteLine(data.ToString());
        Assert.True(data.IsValidLocalTime(100000));

        cancellationTokenSource.Cancel();
        await listening;
    }

    [Fact]
    public void UdpClientServerSync()
    {
        int port = RandomWrapper.Shared.NextInclusive(SocketExtensions.UserPortsRange);

        IPEndPoint serverEndpoint = new IPEndPoint(IPAddress.Any, port);
        using UdpClient server = new UdpClient(serverEndpoint);

        Task serverTask = Task.Run(() =>
            {
                while (true)
                {
                    byte[] data = server.Receive(ref serverEndpoint); // serverEndpoint sale con la informacion de RemoteEndPoint
                    outputHelper.WriteLine("receive data from " + serverEndpoint.ToString());
                    Assert.Equal([1, 2, 3, 4, 5], data);

                    server.Send(data[..2], 2, serverEndpoint); // reply back
                }
            }, TestContext.Current.CancellationToken);

        using UdpClient client = new UdpClient();
        IPEndPoint ep = new IPEndPoint(IPAddress.Loopback, port); // endpoint where server is listening

        // send data
        client.Send([1, 2, 3, 4, 5], ep);

        // then receive data
        byte[] receivedData = client.Receive(ref ep);

        outputHelper.WriteLine("receive data from " + ep.ToString());
        Assert.Equal([1, 2], receivedData);
    }

    [Fact]
    public async Task UdpClientServerAsync()
    {
        int port = RandomWrapper.Shared.NextInclusive(SocketExtensions.UserPortsRange);

        using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        IPEndPoint serverEndpoint = new IPEndPoint(IPAddress.Any, port);
        using UdpClient server = new UdpClient(serverEndpoint);

        Task serverTask = Task.Run(async () =>
            {
                try
                {
                    while (!cancellationTokenSource.IsCancellationRequested)
                    {
                        UdpReceiveResult result = await server.ReceiveAsync(cancellationTokenSource.Token); // listen
                        outputHelper.WriteLine("receive data from " + result.RemoteEndPoint.ToString());
                        Assert.Equal([1, 2, 3, 4, 5], result.Buffer);

                        await server.SendAsync(result.Buffer.AsMemory()[..2], result.RemoteEndPoint); // reply back
                    }
                }
                catch (OperationCanceledException) { }
            }, TestContext.Current.CancellationToken);

        IPEndPoint ep = new IPEndPoint(IPAddress.Loopback, port); // endpoint where server is listening
        using UdpClient client = new UdpClient();

        // send data
        await client.SendAsync([1, 2, 3, 4, 5], 5, ep);

        // then receive data
        UdpReceiveResult result = await client.ReceiveAsync(TestContext.Current.CancellationToken);

        Assert.Equal([1, 2], result.Buffer);

        cancellationTokenSource.Cancel();
        await serverTask;
    }
    /*
    [Fact]
    public async Task SocketWrapperEcho()
    {
        int port = RandomWrapper.Shared.Next(SocketExtensions.UserPortsRange);

        SyncSocketWrapper syncSocketWrapper = new SyncSocketWrapper(new IPEndPoint(IPAddress.Loopback, port), AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        await new NetworkTests(outputHelper).SocketWrapperEcho(
            new RequestResponseUdpServer(port),
            syncSocketWrapper);
    }
    */
    [Fact]
    public async Task SocketWrapperUpper()
    {
        int port = RandomWrapper.Shared.NextInclusive(SocketExtensions.UserPortsRange);

        await new NetworkTests(outputHelper).SocketWrapperUpper(
            new RequestResponseUdpServer(port),
            new SyncSocketWrapper(new IPEndPoint(IPAddress.Loopback, port), AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp));
    }

    [Fact]
    public async Task SocketWrapperPoolUpper()
    {
        const int poolCount = 10;
        Range<int> portsRange = RandomWrapper.Shared.NextRange(SocketExtensions.UserPortsRange, poolCount);

        await new NetworkTests(outputHelper).SocketWrapperPool(
            portsRange, 
            Enumerable.Range(portsRange.Start, poolCount).Select(port => new RequestResponseUdpServer(port)).ToArray(),
            endpoint => new SyncSocketWrapper(endpoint, AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp));
        }
    
    [Fact]
    public async Task ClientServer()
    {
        int port = RandomWrapper.Shared.NextInclusive(SocketExtensions.UserPortsRange);

        await new NetworkTests(outputHelper).ClientServer(
            new RequestResponseUdpServer(port),
            new RequestResponseUdpClient(new IPEndPoint(IPAddress.Loopback, port)));
    }
    
    [Fact]
    public async Task DaytimeClientServer()
    {
        await new NetworkTests(outputHelper).DaytimeClientServer(
            new ResponseUdpServer(DaytimeServer.DEFAULT_PORT),
            new ResponseUdpClient(new RequestResponseUdpClient(new IPEndPoint(IPAddress.Loopback, DaytimeServer.DEFAULT_PORT))));
    }

    [Fact]
    public async Task EchoClientServer()
    {
        int port = RandomWrapper.Shared.NextInclusive(SocketExtensions.UserPortsRange);

        await new NetworkTests(outputHelper).EchoClientServer(
            new RequestResponseUdpServer(port), 
            new RequestResponseUdpClient(new IPEndPoint(IPAddress.Loopback, port)));
    }

    [Theory]
    [InlineData("Hello")]
    [InlineData("<PedLiqDDV><F ID=\"AltaPrestamosDDV\"><R><SW0IDPED>2776</SW0IDPED><SW0SUC>0</SW0SUC><SW0LINEA>301</SW0LINEA><SW0NOPER>0</SW0NOPER><SW0ZONA1AL>000006000023503021030509046111WFC000000000000000020250916001001000000000000000000000004548410000000000202509160000000000000095319998000000000000000000000199010000010000041002000000000000000000000000000000000000000000000000000010900000010000000000000000000000099      </SW0ZONA1AL><SW0ZONA2AL>00000000000000000000000000000953199980000000000000000000000000000000100000100000410020WFC                                               000000000000000010111000                                                                                                              </SW0ZONA2AL></R></F><F ID=\"ListaDet\"><R><SW0IDPED>2776</SW0IDPED><SW0SUC>0</SW0SUC><SW0LINEA>301</SW0LINEA><SW0NOPER>0</SW0NOPER><SW0NROCHEQ>00053684</SW0NROCHEQ><SW0FECVTO>20250924</SW0FECVTO><SW0IMPORTE>950000.0</SW0IMPORTE><SW0CMC7>33002822000005368400002890005</SW0CMC7><SW0TIPCHE></SW0TIPCHE><SW0FECOBRO>20250916</SW0FECOBRO><SW0CUITFIR>20063078175</SW0CUITFIR><SW0NOMFIR></SW0NOMFIR><SW0CUITCED>30509046111</SW0CUITCED><SW0NOMCED></SW0NOMCED><SW0SISCTA>1</SW0SISCTA><SW0SUCCTA>0</SW0SUCCTA><SW0TIPCTA>1</SW0TIPCTA><SW0CUENTA>410020</SW0CUENTA><SW0ECHEQS>202509161005230000000G0QND05LLDV9EXY</SW0ECHEQS><SW0CFT>58.000000</SW0CFT><SW0CFTST>58.000000</SW0CFTST><SW0TEA>57.997800</SW0TEA><SW0TASA>45.484098</SW0TASA></R><R><SW0IDPED>2776</SW0IDPED><SW0SUC>0</SW0SUC><SW0LINEA>301</SW0LINEA><SW0NOPER>0</SW0NOPER><SW0NROCHEQ>00054523</SW0NROCHEQ><SW0FECVTO>20250924</SW0FECVTO><SW0IMPORTE>1599.99</SW0IMPORTE><SW0CMC7>33000020000005452300007850008</SW0CMC7><SW0TIPCHE></SW0TIPCHE><SW0FECOBRO>20250916</SW0FECOBRO><SW0CUITFIR>30707515127</SW0CUITFIR><SW0NOMFIR></SW0NOMFIR><SW0CUITCED>30509046111</SW0CUITCED><SW0NOMCED></SW0NOMCED><SW0SISCTA>1</SW0SISCTA><SW0SUCCTA>0</SW0SUCCTA><SW0TIPCTA>1</SW0TIPCTA><SW0CUENTA>410020</SW0CUENTA><SW0ECHEQS>202509161005240000000P68NK6LQPRZNR7O</SW0ECHEQS><SW0CFT>58.000000</SW0CFT><SW0CFTST>58.000000</SW0CFTST><SW0TEA>57.997800</SW0TEA><SW0TASA>45.484098</SW0TASA></R><R><SW0IDPED>2776</SW0IDPED><SW0SUC>0</SW0SUC><SW0LINEA>301</SW0LINEA><SW0NOPER>0</SW0NOPER><SW0NROCHEQ>00054524</SW0NROCHEQ><SW0FECVTO>20250924</SW0FECVTO><SW0IMPORTE>1599.99</SW0IMPORTE><SW0CMC7>33000020000005452400007850008</SW0CMC7><SW0TIPCHE></SW0TIPCHE><SW0FECOBRO>20250916</SW0FECOBRO><SW0CUITFIR>30707515127</SW0CUITFIR><SW0NOMFIR></SW0NOMFIR><SW0CUITCED>30509046111</SW0CUITCED><SW0NOMCED></SW0NOMCED><SW0SISCTA>1</SW0SISCTA><SW0SUCCTA>0</SW0SUCCTA><SW0TIPCTA>1</SW0TIPCTA><SW0CUENTA>410020</SW0CUENTA><SW0ECHEQS>202509161005250000000EZ8NP1O80YR24KV</SW0ECHEQS><SW0CFT>58.000000</SW0CFT><SW0CFTST>58.000000</SW0CFTST><SW0TEA>57.997800</SW0TEA><SW0TASA>45.484098</SW0TASA></R></F></PedLiqDDV>")]
    public async Task EchoMessageClientServer(string message)
        {
        int port = RandomWrapper.Shared.NextInclusive(SocketExtensions.UserPortsRange);
        int bufferSize = message.Length * 2;
        await new NetworkTests(outputHelper).EchoClientServer(
            new RequestResponseUdpServer(port, bufferSize),
            new RequestResponseUdpClient(new IPEndPoint(IPAddress.Loopback, port)),
            message);
        }


    [Fact]
    public async Task UpperClientServer()
    {
        int port = RandomWrapper.Shared.NextInclusive(SocketExtensions.UserPortsRange);

        await new NetworkTests(outputHelper).UpperClientUdpServer(new RequestResponseUdpServer(port), new RequestResponseUdpClient(new IPEndPoint(IPAddress.Loopback, port)));
    }

    [Fact]
    public async Task RequestStringServer()
    {
        int port = RandomWrapper.Shared.NextInclusive(SocketExtensions.UserPortsRange);

        await new NetworkTests(outputHelper).RequestStringServer(
            new RequestUdpServer(port),
            new RequestUdpClient(new IPEndPoint(IPAddress.Loopback, port)));
    }
}

#pragma warning disable IDE0305 // Simplify collection initialization

using CoreClassLibrary.Extensions;
using CoreClassLibrary.Mathematics.Randomness;
using CoreClassLibrary.Networking;
using CoreClassLibrary.Networking.TcpClientServer;
using CoreClassLibrary.Objects.Pooling;
using CoreClassLibrary.Objects.Refs;
using System.Buffers;
using System.Net;
using System.Net.Sockets;
using Xunit;

namespace CoreClassLibrary.Tests.Networking;

public class TcpTests(ITestOutputHelper outputHelper)
{
    [Fact]
    public void TcpClientServerSync()
    {
        int port = RandomWrapper.Shared.NextInclusive(SocketExtensions.UserPortsRange);

        Task serverTask = Task.Run(() =>
            {
                using PooledMemory<byte> inputBuffer = new PooledMemory<byte>(5);

                IPEndPoint listeningEndpoint = new IPEndPoint(IPAddress.Any, port);
                using TcpListener server = new TcpListener(listeningEndpoint);
                server.Start();

                while (true)
                {
                    using TcpClient client = server.AcceptTcpClient();
                    using NetworkStream stream = client.GetStream();

                    int received = stream.Read(inputBuffer.Memory.Span);

                    Assert.Equal((byte[])[1, 2, 3, 4, 5], inputBuffer.Memory[..received]);

                    stream.Write(inputBuffer.Memory.Span[..2]);
                }
            }, TestContext.Current.CancellationToken);

        using TcpClient client = new TcpClient();
        IPEndPoint remoteEndpoint = new IPEndPoint(IPAddress.Loopback, port); // endpoint where server is listening
        client.Connect(remoteEndpoint);
        using NetworkStream stream = client.GetStream();

        // send data
        stream.Write([1, 2, 3, 4, 5]);

        // then receive data
        using PooledMemory<byte> outputBuffer = new PooledMemory<byte>(5);
        int received = stream.Read(outputBuffer.Memory.Span);
        
        Assert.Equal((byte[])[1, 2], outputBuffer.Memory[..received]);
    }

    
    [Fact]
    public async Task TcpClientServerAsync()
    {
        int port = RandomWrapper.Shared.NextInclusive(SocketExtensions.UserPortsRange);

        using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        Task serverTask = Task.Run(async () =>
            { 
            using PooledMemory<byte> inputBuffer = new PooledMemory<byte>(5);

            IPEndPoint listeningEndpoint = new IPEndPoint(IPAddress.Any, port);
            using TcpListener server = new TcpListener(listeningEndpoint);
            server.Start();

            try
                {
                while (!cancellationTokenSource.IsCancellationRequested)
                    {
                    using TcpClient client = await server.AcceptTcpClientAsync(cancellationTokenSource.Token);
                    await using NetworkStream stream = client.GetStream();

                    int received = await stream.ReadAsync(inputBuffer.Memory, cancellationTokenSource.Token);
                    Assert.Equal((byte[])[1, 2, 3, 4, 5], inputBuffer.Memory[..received]);
                    await stream.WriteAsync(inputBuffer.Memory[..2], cancellationTokenSource.Token);
                    }
                }
            catch(OperationCanceledException) { }
            finally
                {
                server.Stop();
                }
            }, TestContext.Current.CancellationToken);
        
        IPEndPoint remoteEndpoint = new IPEndPoint(IPAddress.Loopback, port); // endpoint where server is listening
        using TcpClient client = new TcpClient();
        await client.ConnectAsync(remoteEndpoint, cancellationTokenSource.Token);
        await using NetworkStream stream = client.GetStream();

        // send data
        using PooledMemory<byte> inputBuffer = new PooledMemory<byte>(5);
        ((byte[])[1, 2, 3, 4, 5]).WriteTo<byte>(inputBuffer.Memory.Span);        
        await stream.WriteAsync(inputBuffer.Memory, cancellationTokenSource.Token);

        // then receive data
        using PooledMemory<byte> outputBuffer = new PooledMemory<byte>(5);
        int received = await stream.ReadAsync(outputBuffer.Memory, cancellationTokenSource.Token);

        Assert.Equal((byte[])[1, 2], outputBuffer.Memory.Span[..received]);

        cancellationTokenSource.Cancel();
        await serverTask;
    }
    
    /*
     * Falla en el segundo mensaje porque TcpListener cierra la conexion
    [Fact]
    public async Task SocketWrapperEcho()
    {
        int port = RandomWrapper.Shared.Next(SocketExtensions.UserPortsRange);

        SyncSocketWrapper syncSocketWrapper = new SyncSocketWrapper(new IPEndPoint(IPAddress.Loopback, port));

        await new NetworkTests(outputHelper).SocketWrapperEcho(
            new RequestResponseTcpServer(port),
            syncSocketWrapper);
    }

    [Fact]
    public async Task SocketWrapperUpper()
    {
        int port = RandomWrapper.Shared.Next(SocketExtensions.UserPortsRange);

        await new NetworkTests(outputHelper).SocketWrapperUpper(
            new RequestResponseTcpServer(port),
            new SyncSocketWrapper(new IPEndPoint(IPAddress.Loopback, port)));
    }

    [Fact]
    public async Task SocketWrapperPoolUpper()
    {
        const int poolCount = 10;
        Range<int> portsRange = RandomWrapper.Shared.NextRange(SocketExtensions.UserPortsRange, poolCount);
        
        await new NetworkTests(outputHelper).SocketWrapperPool(
            portsRange, 
            Enumerable.Range(portsRange.Start, poolCount).Select(port => new RequestResponseTcpServer(port)).ToArray(),
            endpoint => new SyncSocketWrapper(endpoint));
    }
    */

    [Fact]
    public async Task ClientServer()
    {
        int port = RandomWrapper.Shared.NextInclusive(SocketExtensions.UserPortsRange);

        await new NetworkTests(outputHelper).ClientServer(
            new RequestResponseTcpServer(port),
            new RequestResponseTcpClient(new IPEndPoint(IPAddress.Loopback, port)));
    }
    
    [Fact]
    public async Task DaytimeClientServer()
    {
        await new NetworkTests(outputHelper).DaytimeClientServer(
            new ResponseTcpServer(DaytimeServer.DEFAULT_PORT),
            new ResponseTcpClient(new IPEndPoint(IPAddress.Loopback, DaytimeClient.DEFAULT_PORT)));
    }

    [Fact]
    public async Task EchoClientServer()
    {
        int port = RandomWrapper.Shared.NextInclusive(SocketExtensions.UserPortsRange);

        await new NetworkTests(outputHelper).EchoClientServer(
            new RequestResponseTcpServer(port),
            new RequestResponseTcpClient(new IPEndPoint(IPAddress.Loopback, port)));
    }

    [Theory]
    [InlineData("Hello")]
    [InlineData("<PedLiqDDV><F ID=\"AltaPrestamosDDV\"><R><SW0IDPED>2776</SW0IDPED><SW0SUC>0</SW0SUC><SW0LINEA>301</SW0LINEA><SW0NOPER>0</SW0NOPER><SW0ZONA1AL>000006000023503021030509046111WFC000000000000000020250916001001000000000000000000000004548410000000000202509160000000000000095319998000000000000000000000199010000010000041002000000000000000000000000000000000000000000000000000010900000010000000000000000000000099      </SW0ZONA1AL><SW0ZONA2AL>00000000000000000000000000000953199980000000000000000000000000000000100000100000410020WFC                                               000000000000000010111000                                                                                                              </SW0ZONA2AL></R></F><F ID=\"ListaDet\"><R><SW0IDPED>2776</SW0IDPED><SW0SUC>0</SW0SUC><SW0LINEA>301</SW0LINEA><SW0NOPER>0</SW0NOPER><SW0NROCHEQ>00053684</SW0NROCHEQ><SW0FECVTO>20250924</SW0FECVTO><SW0IMPORTE>950000.0</SW0IMPORTE><SW0CMC7>33002822000005368400002890005</SW0CMC7><SW0TIPCHE></SW0TIPCHE><SW0FECOBRO>20250916</SW0FECOBRO><SW0CUITFIR>20063078175</SW0CUITFIR><SW0NOMFIR></SW0NOMFIR><SW0CUITCED>30509046111</SW0CUITCED><SW0NOMCED></SW0NOMCED><SW0SISCTA>1</SW0SISCTA><SW0SUCCTA>0</SW0SUCCTA><SW0TIPCTA>1</SW0TIPCTA><SW0CUENTA>410020</SW0CUENTA><SW0ECHEQS>202509161005230000000G0QND05LLDV9EXY</SW0ECHEQS><SW0CFT>58.000000</SW0CFT><SW0CFTST>58.000000</SW0CFTST><SW0TEA>57.997800</SW0TEA><SW0TASA>45.484098</SW0TASA></R><R><SW0IDPED>2776</SW0IDPED><SW0SUC>0</SW0SUC><SW0LINEA>301</SW0LINEA><SW0NOPER>0</SW0NOPER><SW0NROCHEQ>00054523</SW0NROCHEQ><SW0FECVTO>20250924</SW0FECVTO><SW0IMPORTE>1599.99</SW0IMPORTE><SW0CMC7>33000020000005452300007850008</SW0CMC7><SW0TIPCHE></SW0TIPCHE><SW0FECOBRO>20250916</SW0FECOBRO><SW0CUITFIR>30707515127</SW0CUITFIR><SW0NOMFIR></SW0NOMFIR><SW0CUITCED>30509046111</SW0CUITCED><SW0NOMCED></SW0NOMCED><SW0SISCTA>1</SW0SISCTA><SW0SUCCTA>0</SW0SUCCTA><SW0TIPCTA>1</SW0TIPCTA><SW0CUENTA>410020</SW0CUENTA><SW0ECHEQS>202509161005240000000P68NK6LQPRZNR7O</SW0ECHEQS><SW0CFT>58.000000</SW0CFT><SW0CFTST>58.000000</SW0CFTST><SW0TEA>57.997800</SW0TEA><SW0TASA>45.484098</SW0TASA></R><R><SW0IDPED>2776</SW0IDPED><SW0SUC>0</SW0SUC><SW0LINEA>301</SW0LINEA><SW0NOPER>0</SW0NOPER><SW0NROCHEQ>00054524</SW0NROCHEQ><SW0FECVTO>20250924</SW0FECVTO><SW0IMPORTE>1599.99</SW0IMPORTE><SW0CMC7>33000020000005452400007850008</SW0CMC7><SW0TIPCHE></SW0TIPCHE><SW0FECOBRO>20250916</SW0FECOBRO><SW0CUITFIR>30707515127</SW0CUITFIR><SW0NOMFIR></SW0NOMFIR><SW0CUITCED>30509046111</SW0CUITCED><SW0NOMCED></SW0NOMCED><SW0SISCTA>1</SW0SISCTA><SW0SUCCTA>0</SW0SUCCTA><SW0TIPCTA>1</SW0TIPCTA><SW0CUENTA>410020</SW0CUENTA><SW0ECHEQS>202509161005250000000EZ8NP1O80YR24KV</SW0ECHEQS><SW0CFT>58.000000</SW0CFT><SW0CFTST>58.000000</SW0CFTST><SW0TEA>57.997800</SW0TEA><SW0TASA>45.484098</SW0TASA></R></F></PedLiqDDV>")]
    public async Task EchoMessageClientServer(string message)
        {
        int port = RandomWrapper.Shared.NextInclusive(SocketExtensions.UserPortsRange);
        int bufferSize = message.Length * 2;
        await new NetworkTests(outputHelper).EchoClientServer(
            new RequestResponseTcpServer(port, bufferSize, bufferSize),
            new RequestResponseTcpClient(new IPEndPoint(IPAddress.Loopback, port)),
            message);
        }

    [Fact]
    public async Task UpperClientServer()
    {
        int port = RandomWrapper.Shared.NextInclusive(SocketExtensions.UserPortsRange);

        await new NetworkTests(outputHelper).UpperClientUdpServer(
            new RequestResponseTcpServer(port),
            new RequestResponseTcpClient(new IPEndPoint(IPAddress.Loopback, port)));
    }

    [Fact]
    public async Task RequestStringServer()
    {
        int port = RandomWrapper.Shared.NextInclusive(SocketExtensions.UserPortsRange);

        await new NetworkTests(outputHelper).RequestStringServer(
            new RequestTcpServer(port),
            new RequestTcpClient(new IPEndPoint(IPAddress.Loopback, port)));
    }

    [Fact]
    public async Task RequestLengthPrefixStringServer()
    {
        ReadWrite<int> serverState = new ReadWrite<int>(0);
        int port = RandomWrapper.Shared.NextInclusive(SocketExtensions.UserPortsRange);

        using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        ValueTask listening = new RequestLengthPrefixStringServer(port).ListenAsync<ReadWrite<int>>(
            serverState, 
            (state, request, cancellationToken) =>
                {
                outputHelper.WriteLine(request);
                state.Value = int.Parse(request);
                cancellationTokenSource.Cancel();
                return ValueTask.CompletedTask;
                }, 
            cancellationTokenSource.Token);

        RequestLengthPrefixStringClient client = new RequestLengthPrefixStringClient(new RequestStringClient(new RequestTcpClient(new IPEndPoint(IPAddress.Loopback, port))));
        await client.SendAsync("1", cancellationToken: TestContext.Current.CancellationToken);

        await listening;

        Assert.Equal(1, serverState.Value);
    }

    [Fact]
    public void LocalHostIPAddress()
        => Assert.NotEqual(IPAddress.Loopback, SocketExtensions.GetHostAddress());

    [Theory]
    [InlineData("172.18.40.65")]
    public void IPAddressV4Equal(string ip1)
    {
        IPAddress ipAddress1 = IPAddress.Parse(ip1);

        outputHelper.WriteLine(ipAddress1.ToString());

        Assert.Equal(ipAddress1, ipAddress1.MapToIPv4());
        Assert.NotEqual(ipAddress1, ipAddress1.MapToIPv6());
        Assert.Equal(ipAddress1, ipAddress1.MapToIPv6().MapToIPv4());
    }

    [Fact]
    public void TestPacketProtocol()
    {
        int messages = 0;

        PacketProtocol packetizer = new PacketProtocol(2000);
        packetizer.MessageArrived += _ => ++messages;

        packetizer.DataReceived(
        PacketProtocol.WrapMessage(new byte[3]).Concat(
            PacketProtocol.WrapMessage(new byte[4])).ToArray());

        Assert.Equal(2, messages);
    }


    [Fact]
    public void PacketProtocolTest2()
    {
        int numMessages = 0;
        string[] messages = new string[2];

        PacketProtocol packetizer = new PacketProtocol(2000);
        packetizer.MessageArrived += message =>
            {
                messages[numMessages] = StringExtensions.UTF8Encoding.GetString(message);
                ++numMessages;
            };

        packetizer.DataReceived(
        PacketProtocol.WrapMessage(StringExtensions.UTF8Encoding.GetBytes("Hello")).Concat(
            PacketProtocol.WrapMessage(StringExtensions.UTF8Encoding.GetBytes("World"))).ToArray());

        Assert.Equal(2, numMessages);
        Assert.Equal("Hello", messages[0]);
        Assert.Equal("World", messages[1]);
    }
}

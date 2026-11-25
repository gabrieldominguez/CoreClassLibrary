#pragma warning disable IDE0305 // Simplify collection initialization

using CoreClassLibrary.Extensions;
using CoreClassLibrary.Mathematics.Randomness;
using CoreClassLibrary.Networking;
using CoreClassLibrary.Networking.SyncSocketWrappers;
using CoreClassLibrary.Objects.Pooling;
using CoreClassLibrary.Objects.Ranges;
using CoreClassLibrary.Threading;
using System.Net;
using Xunit;

namespace CoreClassLibrary.Tests.Networking;

public class NetworkTests(ITestOutputHelper outputHelper)
    {
    public async Task ClientServer(IRequestResponseServer server, IRequestResponseClient client)
        {
        using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        ValueTask listening = server.ListenAsync((input, output, cancellationToken) =>
            {
            Assert.Equal((byte[])[1, 2, 3, 4, 5], input.Span);
            input[..2].CopyTo(output);
            return ValueTask.FromResult(2);
            }, cancellationTokenSource.Token);

        using PooledMemory<byte> outputBuffer = new PooledMemory<byte>(5); 
            
        int outputLength = await client.SendReceiveAsync((byte[])[1, 2, 3, 4, 5], outputBuffer.Memory, CancellationToken.None);
        outputHelper.WriteLine(outputLength.ToString());
        Assert.Equal((byte[])[1, 2], outputBuffer.Memory[..outputLength]);

        cancellationTokenSource.Cancel();
        await listening;
        }
    
    public Task EchoClientServer(IRequestResponseServer server, IRequestResponseClient client)
        => this.EchoClientServer(server, client, RandomWrapper.Shared.NextString(10));

    public async Task EchoClientServer(IRequestResponseServer server, IRequestResponseClient client, string input)
        {
        EchoServer echoServer = new EchoServer(server);
        RequestResponseStringClient echoClient = new RequestResponseStringClient(client);

        using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        ValueTask listening = echoServer.ListenAsync(cancellationTokenSource.Token);

        string output = await echoClient.SendReceiveAsync(input, input.Length * 2, input.Length * 2);
        Assert.Equal(input, output);

        output = await echoClient.SendReceiveAsync(input, input.Length * 2, input.Length * 2);
        Assert.Equal(input, output);

        cancellationTokenSource.Cancel();
        await listening;

        outputHelper.WriteLine(input);
        }

    public async Task UpperClientUdpServer(IRequestResponseServer server, IRequestResponseClient client)
    {
        UpperServer toUpperServer = new UpperServer(new RequestResponseStringServer(server));
        RequestResponseStringClient toUpperClient = new RequestResponseStringClient(client);

        using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        ValueTask listening = toUpperServer.ListenAsync(cancellationTokenSource.Token);

        string input = RandomWrapper.Shared.NextString(10, StringExtensions.LowerCaseAlphaChars);
        string output = await toUpperClient.SendReceiveAsync(input);

        Assert.Equal(input.ToUpperInvariant(), output);
        outputHelper.WriteLine(input);
        outputHelper.WriteLine(output);

        cancellationTokenSource.Cancel();
        await listening;
    }

    public async Task DaytimeClientServer(IResponseServer server, IResponseClient client)
    {
        DaytimeServer daytimeServer = new DaytimeServer(new ResponseStringServer(server));
        DaytimeClient daytimeClient = new DaytimeClient(new ResponseStringClient(client));

        using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        ValueTask listening = daytimeServer.ListenAsync(cancellationTokenSource.Token);

        DateTime dateTime = await daytimeClient.GetDateTimeAsync(cancellationTokenSource.Token);
        outputHelper.WriteLine(dateTime.ToLocalDateTimeString());
        Assert.Equal(DateTime.Now, dateTime, TimeSpan.FromSeconds(0.5));
        outputHelper.WriteLine(DateTime.Now.ToLocalDateTimeString());

        dateTime = await daytimeClient.GetDateTimeAsync(cancellationTokenSource.Token);
        outputHelper.WriteLine(dateTime.ToLocalDateTimeString());
        Assert.Equal(DateTime.Now, dateTime, TimeSpan.FromSeconds(0.5));
        outputHelper.WriteLine(DateTime.Now.ToLocalDateTimeString());

        cancellationTokenSource.Cancel();
        await listening;
    }

    public async Task RequestStringServer(IRequestServer server, IRequestClient client)
    {
        int serverState = 0;
        Task serverStateChanged = TasksExtensions.CreateNullTask();
        int port = RandomWrapper.Shared.NextInclusive(SocketExtensions.UserPortsRange);

        using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        RequestStringServer stringServer = new RequestStringServer(server);

        ValueTask listening = stringServer.ListenAsync((request, cancellationToken) =>
            {
                outputHelper.WriteLine(request);
                serverState = int.Parse(request);
                serverStateChanged.Start();
                return ValueTask.CompletedTask;
            }, cancellationTokenSource.Token);

        RequestStringClient stringClient = new RequestStringClient(client);
        await stringClient.SendAsync("1");

        await serverStateChanged;

        Assert.Equal(1, serverState);

        cancellationTokenSource.Cancel();
    }

    public async Task SocketWrapperEcho(IRequestResponseServer server, SyncSocketWrapper clientSocket)
        {
        EchoServer echoServer = new EchoServer(server);

        using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        ValueTask listening = echoServer.ListenAsync(cancellationTokenSource.Token);

        string input = RandomWrapper.Shared.NextString(10);
        clientSocket.SendString(input);
        string output = clientSocket.ReceiveString(10);
        Assert.Equal(input, output);
        outputHelper.WriteLine(input);

        input = RandomWrapper.Shared.NextString(10);
        clientSocket.SendString(input);
        output = clientSocket.ReceiveString(10);
        Assert.Equal(input, output);
        outputHelper.WriteLine(input);
        
        cancellationTokenSource.Cancel();
        await listening;     
        }

    public async Task SocketWrapperUpper(IRequestResponseServer server, SyncSocketWrapper clientSocket)
        {
        UpperServer toUpperServer = new UpperServer(new RequestResponseStringServer(server));
        
        using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        ValueTask upperListening = toUpperServer.ListenAsync(cancellationTokenSource.Token);

        string input = RandomWrapper.Shared.NextString(10, StringExtensions.LowerCaseAlphaChars);
        clientSocket.SendString(input);
        string output = clientSocket.ReceiveString(10);
        Assert.Equal(input.ToUpperInvariant(), output);
        outputHelper.WriteLine(input);
        outputHelper.WriteLine(output);

        input = RandomWrapper.Shared.NextString(10, StringExtensions.LowerCaseAlphaChars);
        clientSocket.SendString(input);
        output = clientSocket.ReceiveString(10);
        Assert.Equal(input.ToUpperInvariant(), output);
        outputHelper.WriteLine(input);
        outputHelper.WriteLine(output);

        cancellationTokenSource.Cancel();
        await upperListening;        
        }        
    
    public async Task SocketWrapperPool(Range<int> portsRange, IRequestResponseServer[] serverPool, Func<EndPoint, SyncSocketWrapper> socketWrapperFactory)
        {
        UpperServer[] toUpperServerPool = serverPool.Select(server => new UpperServer(new RequestResponseStringServer(server))).ToArray();            
        SyncSocketWrapperPool clientSocketPool = new SyncSocketWrapperPool(() => new IPEndPoint(IPAddress.Loopback, RandomWrapper.Shared.NextInclusive(portsRange)), socketWrapperFactory);
        
        using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        ValueTask[] upperListenings = toUpperServerPool.Select(server => server.ListenAsync(cancellationTokenSource.Token)).ToArray();

        for (int i = 0; i < serverPool.Length * 5; i++)
            {
            string input = RandomWrapper.Shared.NextString(10, StringExtensions.LowerCaseAlphaChars);
            outputHelper.WriteLine(input);

            string output = clientSocketPool.InvokeWithSocket<string, string>((clientSocket, inputData) => 
                {
                outputHelper.WriteLine(clientSocket.EndPoint.ToString() ?? "");
                clientSocket.SendString(inputData);
                return clientSocket.ReceiveString(10);
                }, input);

            Assert.Equal(input.ToUpperInvariant(), output);
            outputHelper.WriteLine(output);
            outputHelper.WriteLine("");
            }

        cancellationTokenSource.Cancel();
        
        await Task.WhenAll(upperListenings.Select(valueTask => valueTask.AsTask()));
        }
    }

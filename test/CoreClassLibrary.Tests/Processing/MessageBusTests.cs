
using CoreClassLibrary.Threading.Messaging;
using Xunit;

namespace CoreClassLibrary.Tests.Processing;

public class MessageBusTests(ITestOutputHelper output)
    {
    private Task AsyncTest(string line, CancellationToken cancellationToken = default)
        => Shared.TestAsync(line, output, cancellationToken);

    private void Test(string line)
        => Shared.Test(line, output);

    [Fact]
    public void MessageBus()
        {
        MessageBus messageBus = new MessageBus();

        messageBus.Subscribe<string>(this.Test);

        foreach (string line in Shared.Source)
            {
            messageBus.Publish(line);
            }
        }

    [Fact]
    public void MessageBusT()
        {
        MessageBus<string> messageBus = new MessageBus<string>();

        messageBus.Subscribe(this.Test);

        foreach (string line in Shared.Source)
            {
            messageBus.Publish(line);
            }
        }

    [Fact]
    public void ResponseMessageBus()
        {
        ResponseMessageBus messageBus = new ResponseMessageBus();
        messageBus.Subscribe<string, string>(Shared.Process);

        foreach (string line in Shared.Source)
            {
            string result = messageBus.Publish<string, string>(line);
            output.WriteLine(result);
            Assert.Contains(result, Shared.Result);
            }
        }

    [Fact]
    public void MultipleMessageBuss()
        {
        ResponseMessageBus processService = new ResponseMessageBus();
        processService.Subscribe<string, string>(Shared.Process);

        MessageBus<string> outputService = new MessageBus<string>();
        outputService.Subscribe(output.WriteLine);

        MessageBus assertService = new MessageBus();
        assertService.Subscribe<string>(result => Assert.Contains(result, Shared.Result));

        foreach (string line in Shared.Source)
            {
            string result = processService.Publish<string, string>(line);
            outputService.Publish(result);
            assertService.Publish(result);
            }
        }

    [Fact]
    public async Task AsyncMessageBus()
        {
        AsyncMessageBus messageBus = new AsyncMessageBus();

        messageBus.Subscribe<string>(this.AsyncTest);

        foreach (string line in Shared.Source)
            {
            await messageBus.PublishAsync(line);
            }
        }

    [Fact]
    public async Task AsyncMessageBusT()
        {
        AsyncMessageBus<string> messageBus = new AsyncMessageBus<string>();

        messageBus.Subscribe(this.AsyncTest);

        foreach (string line in Shared.Source)
            {
            await messageBus.PublishAsync(line, TestContext.Current.CancellationToken);
            }
        }

    [Fact]
    public async Task AsyncResponseMessageBus()
        {
        AsyncResponseMessageBus messageBus = new AsyncResponseMessageBus();
        messageBus.Subscribe<string, string>(Shared.ProcessAsync);

        foreach (string line in Shared.Source)
            {
            string result = await messageBus.PublishAsync<string, string>(line);
            output.WriteLine(result);
            Assert.Contains(result, Shared.Result);
            }
        }
    }

using System.Threading.Channels;
using Xunit;

namespace CoreClassLibrary.Tests.Processing;

/// <summary>
/// Channel Tests
/// </summary>
/// <remarks>
/// 
/// Important: Each message is consumed by one reader, not broadcast.
/// 
/// How Channels compare to other options:
///  ----------------------------------------------------------------------- 
/// | Option                  | Notes                                       |
/// | ----------------------- | --------------------------------------------|
/// | Channel<T>              | Best general-purpose in-process async bus   |
/// |                         |                                             |
/// | BlockingCollection<T>   | Legacy, sync only                           |
/// | ConcurrentQueue<T>      | No backpressure                             |
/// | IObservable<T>          | Push-only, no backpressure                  |
/// | TPL Dataflow            | Powerful, but heavier                       |
/// | MediatR                 | Great abstraction, but sync-centric         |
/// | Message brokers         | Overkill for in-process                     |
///  -----------------------------------------------------------------------
///  
/// Pipeline Mental model
/// A pipeline is:
///     Producer -> Stage A -> Stage B -> Stage C -> Sink
/// 
/// Each stage:
///     reads from an input channel
///     processes items
///     writes to an output channel
///     runs concurrently
///     propagates completion and failure
/// 
/// This maps perfectly to ChannelReader<T> / ChannelWriter<T>.
/// 
/// One last pro tip
/// Think in terms of ChannelReader in, ChannelWriter out.
/// Never pass the channel itself — that enforces directionality and avoids bugs.
/// </remarks>
/// <see cref="https://devblogs.microsoft.com/dotnet/an-introduction-to-system-threading-channels"/>
/// <see cref="https://gist.github.com/AlgorithmsAreCool/b0960ce8a3400305e43fe8ffdf89b32c"/>
public class ChannelTests(ITestOutputHelper output)
    {
    [Fact]
    public async Task WhileReaderIsNotCompleted()
        {
        // Bounded channels are important if the consumer/reader is slower than the producer. 
        // You don't want your app to keep buffering until explode with an OutOfMemoryException.
        // Channel<string> channel = Channel.CreateBounded<string>(100);
        Channel<string> channel = Channel.CreateUnbounded<string>(new UnboundedChannelOptions() { SingleWriter = true, SingleReader = true });

        // You typically would need to run a dedicated thread to await and proccess message from the channel, 
        // but we can use Task.Run to 'borrow' a thread from the thread pool. 
        Task readerWorker = Task.Run(async () =>
            {
            while (!channel.Reader.Completion.IsCompleted)
                {
                Shared.Test(await channel.Reader.ReadAsync(), output);
                }
            }, TestContext.Current.CancellationToken);

        Task writerWorker = Task.Run(async () =>
            {
            foreach (string line in Shared.Source)
                {
                await channel.Writer.WriteAsync(line);
                }

            channel.Writer.Complete();
            }, TestContext.Current.CancellationToken);

        await readerWorker;
        }

    [Fact]
    public async Task ReaderWaitToReadAsync()
        {
        Channel<string> channel = Channel.CreateUnbounded<string>(new UnboundedChannelOptions() { SingleWriter = true, SingleReader = true });

        Task readerWorker = Task.Run(async () =>
            {
            while (await channel.Reader.WaitToReadAsync())
                {
                while (channel.Reader.TryRead(out string? line))
                    {
                    await Shared.TestAsync(line, output);
                    }
                }
            }, TestContext.Current.CancellationToken);

        Task writerWorker = Task.Run(async () =>
        {
            foreach (string line in Shared.Source)
                {
                await channel.Writer.WriteAsync(line);
                }

            channel.Writer.Complete();
        }, TestContext.Current.CancellationToken);

        await readerWorker;
        }

    [Fact]
    public async Task ReaderReadAllAsync()
        {
        Channel<string> channel = Channel.CreateUnbounded<string>(new UnboundedChannelOptions() { SingleWriter = true, SingleReader = true });

        Task readerWorker = Task.Run(async () =>
            {
            await foreach (string line in channel.Reader.ReadAllAsync())
                {
                await Shared.TestAsync(line, output);
                }
            }, TestContext.Current.CancellationToken);

        Task writerWorker = Task.Run(async () =>
        {
            foreach (string line in Shared.Source)
                {
                await channel.Writer.WriteAsync(line);
                }

            channel.Writer.Complete();
        }, TestContext.Current.CancellationToken);

        await readerWorker;
        }

    [Fact]
    // Al igual que con una cola, solo uno de los receptores recibe cada mensaje.
    public async Task ChannelTwoReaderTestAsync()
        {
        Channel<string> channel = Channel.CreateUnbounded<string>(new UnboundedChannelOptions() { SingleWriter = true, SingleReader = false });

        async Task reader(string name)
            {
            try
                {
                while (!channel.Reader.Completion.IsCompleted)
                    {
                    output.WriteLine(String.Concat(name, ": ", await channel.Reader.ReadAsync()));
                    }
                }
            catch (ChannelClosedException) { }
            }

        Task readerWorker1 = Task.Run(() => reader("readerWorker1"), TestContext.Current.CancellationToken);
        Task readerWorker2 = Task.Run(() => reader("readerWorker2"), TestContext.Current.CancellationToken);

        Task writerWorker = Task.Run(async () =>
            {
            await channel.Writer.WriteAsync("A");
            await channel.Writer.WriteAsync("B");

            await Task.Delay(100);

            channel.Writer.Complete();
            }, TestContext.Current.CancellationToken);

        await Task.WhenAll(readerWorker1, readerWorker2);
        }
    }

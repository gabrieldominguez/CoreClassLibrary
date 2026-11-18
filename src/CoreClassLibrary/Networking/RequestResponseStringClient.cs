using CoreClassLibrary.Extensions;
using CoreClassLibrary.Objects.Pooling;
using CoreClassLibrary.Streams;

namespace CoreClassLibrary.Networking;

/// <summary>
/// String Request/Response Client
/// </summary>
/// <see cref="RequestResponseStringServer"/>
public class RequestResponseStringClient(IRequestResponseClient client)
    {
    public async ValueTask<string> SendReceiveAsync(string input, int inputBufferSize = StreamExtensions.DefaultReadBufferSize, int outputBufferSize = StreamExtensions.DefaultWriteBufferSize, CancellationToken cancellationToken = default)
        {
        using PooledMemory<byte> inputBuffer = new PooledMemory<byte>(inputBufferSize);
        using PooledMemory<byte> outputBuffer = new PooledMemory<byte>(outputBufferSize);

        int inputLength = StringExtensions.UTF8Encoding.GetBytes(input, inputBuffer.Memory.Span);
        int outputLength = await client.SendReceiveAsync(inputBuffer.Memory[..inputLength], outputBuffer.Memory, cancellationToken);
        return StringExtensions.UTF8Encoding.GetString(outputBuffer.Memory.Span[..outputLength]);
        }
    }
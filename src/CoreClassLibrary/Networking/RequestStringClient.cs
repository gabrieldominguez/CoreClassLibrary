using CoreClassLibrary.Extensions;
using CoreClassLibrary.Objects.Pooling;
using CoreClassLibrary.Streams;

namespace CoreClassLibrary.Networking;

/// <summary>
/// Client that sends String requests without expecting a response.
/// </summary>
/// <see cref="RequestStringServer"/>
public class RequestStringClient(IRequestClient client)
{
    public ValueTask SendAsync(string input, int inputBufferSize = StreamExtensions.DefaultReadBufferSize, CancellationToken cancellationToken = default)
        {
        using PooledMemory<byte> inputBuffer = new PooledMemory<byte>(inputBufferSize);
        int inputLength = StringExtensions.UTF8Encoding.GetBytes(input, inputBuffer.Memory.Span);
        return client.SendAsync(inputBuffer.Memory[..inputLength], cancellationToken);
        }
    }

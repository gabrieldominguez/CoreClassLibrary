using CoreClassLibrary.Extensions;
using CoreClassLibrary.Objects.Pooling;
using CoreClassLibrary.Streams;

namespace CoreClassLibrary.Networking;

/// <summary>
/// Client that receive String messages without sending a request.
/// </summary>
/// <see cref="RequestStringServer"/>
public class ResponseStringClient(IResponseClient client)
    {
    public async ValueTask<string> ReceiveAsync(int outputBufferSize = StreamExtensions.DefaultWriteBufferSize, CancellationToken cancellationToken = default)
        {
        using PooledMemory<byte> outputBuffer = new PooledMemory<byte>(outputBufferSize);
        int outputLength = await client.ReceiveAsync(outputBuffer.Memory, cancellationToken);
        return StringExtensions.UTF8Encoding.GetString(outputBuffer.Memory.Span[..outputLength]);
        }
    }

using CoreClassLibrary.Streams;
using System.Globalization;

namespace CoreClassLibrary.Networking.TcpClientServer;

/// <summary>
/// Request only Length-Prefix String Client
/// </summary>
/// <see cref="RequestLengthPrefixStringServer"/>
public class RequestLengthPrefixStringClient(RequestStringClient client, int prefixLength = 5)
    {
    private readonly string prefixFormat = new String('0', prefixLength);
    
    public ValueTask SendAsync(string input, int inputBufferSize = StreamExtensions.DefaultReadBufferSize, CancellationToken cancellationToken = default)
        => client.SendAsync(String.Concat((prefixLength + input.Length).ToString(this.prefixFormat, CultureInfo.InvariantCulture), input), inputBufferSize, cancellationToken);
    }

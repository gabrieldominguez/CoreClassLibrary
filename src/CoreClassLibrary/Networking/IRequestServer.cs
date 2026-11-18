namespace CoreClassLibrary.Networking;

/// <summary>
/// Server that proccess requests without sending a response
/// </summary>
/// <see cref="IRequestClient"/>
public interface IRequestServer
    {
    public ValueTask ListenAsync(Func<ReadOnlyMemory<byte>, CancellationToken, ValueTask> service, CancellationToken cancellationToken);    
    }

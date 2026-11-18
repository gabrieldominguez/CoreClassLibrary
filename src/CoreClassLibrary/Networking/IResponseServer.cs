namespace CoreClassLibrary.Networking;

/// <summary>
/// Server that proccess requests without sending a response
/// </summary>
/// <see cref="IRequestClient"/>
public interface IResponseServer
    {
    public ValueTask ListenAsync(Func<Memory<byte>, CancellationToken, ValueTask<int>> service, CancellationToken cancellationToken);
    }

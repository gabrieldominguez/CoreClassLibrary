namespace CoreClassLibrary.Signaling;

public interface IAsyncSignal: IAsyncSignalProvider, IAsyncSignalAwaiter;

public interface IAsyncSignalAwaiter
    {
    public Task WaitAsync();
    }

public interface IAsyncSignalProvider
    {
    public void Signal();
    }

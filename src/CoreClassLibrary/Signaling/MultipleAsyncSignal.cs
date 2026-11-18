namespace CoreClassLibrary.Signaling;

/// <summary>
/// Implementation of a multiple asynchronous signal.
/// </summary>
/// <remarks>
/// Use a SemaphoreSlim to represent the signal state.
/// So when Signal is called, the SemaphoreSlim is released,
/// any awaiting tasks are unblocked, 
/// AND SUBSEQUENT CALLS TO WAITASYNC WILL AWAIT UNTIL NEW CALLS TO SIGNAL ARE MADE.
/// </remarks>
public class MultipleAsyncSignal: IAsyncSignal, IDisposable
    {
    private readonly SemaphoreSlim semaphore = new SemaphoreSlim(0);

    public Task WaitAsync() 
        => this.semaphore.WaitAsync();

    public void Signal() 
        => this.semaphore.Release();

    public void Dispose()
        {
        this.semaphore.Dispose();
        GC.SuppressFinalize(this);
        }
    }

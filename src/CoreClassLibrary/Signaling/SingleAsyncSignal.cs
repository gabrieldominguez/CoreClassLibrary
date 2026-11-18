namespace CoreClassLibrary.Signaling;

/// <summary>
/// Implementation of a single asynchronous signal.
/// </summary>
/// <remarks>
/// Use a TaskCompletionSource to represent the signal state.
/// So when Signal is called, the TaskCompletionSource is set to completed,
/// any awaiting tasks are unblocked, 
/// AND SUBSEQUENT CALLS TO WAITASYNC WILL COMPLETE IMMEDIATELY.
/// </remarks>
public class SingleAsyncSignal(TaskCompletionSource completionSource): IAsyncSignal
    {
    public SingleAsyncSignal(): this(new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously)) { }

    public Task WaitAsync() 
        => completionSource.Task;

    public void Signal() 
        => completionSource.TrySetResult();
    }

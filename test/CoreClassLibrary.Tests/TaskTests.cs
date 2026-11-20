using CoreClassLibrary.Signaling;
using CoreClassLibrary.Threading;
using System.Runtime.CompilerServices;
using Xunit;
using Xunit.Sdk;

namespace CoreClassLibrary.Tests;

/// <summary>
/// Task / async / await tests
/// </summary>
/// <see cref="https://devblogs.microsoft.com/dotnet/how-async-await-really-works/"/>
public class TaskTests
    {
    [Fact]
    public async Task TestTask()
        {
        // arrange
        const int i = 1;

        // act
        Task<int> task = new Task<int>(() => i + 1);
        task.Start();

        // assert
        Assert.Equal(2, await task);
        }

    [Fact]
    public async Task TaskFactory()
        {
        const int i = 1;

        Assert.Equal(2, await Task.Factory.StartNew(() => i + 1));
        }

    [Fact]
    public void Default()
        {
        Assert.True(CancellationToken.None == default);
        #pragma warning disable xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken
        Assert.True(CancellationToken.None.Equals(default));
        Assert.Equal(default, CancellationToken.None);
        #pragma warning restore xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken
        }

    [Fact]
    public void ReuseCancellationTokenSource()
        {
        using CancellationTokenSource tokenSource = new CancellationTokenSource();

        tokenSource.Cancel();
        tokenSource.Cancel();
        }
    
    [Fact]
    public void InfiniteTimeSpanCancellationTokenSource()
        {
        using CancellationTokenSource tokenSource = new CancellationTokenSource(Timeout.InfiniteTimeSpan);

        tokenSource.Cancel();
        }

    [Fact]
    public async Task TaskCompletionSource()
        {
        // Arrange
        TaskCompletionSource completionSource = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        // Act
        Task backgroundTask = Task.Run(async () =>
            {
            await Task.Delay(50);   // Simulate async work
            completionSource.TrySetResult();
            }, TestContext.Current.CancellationToken);
        
        await completionSource.Task;    // Assert - completionSource.Task should complete once Signal is called
        await backgroundTask;           // optional but ensures clean task completion

        Assert.True(completionSource.Task.IsCompleted);
        Assert.True(completionSource.Task.IsCompletedSuccessfully);
        Assert.False(completionSource.Task.IsCanceled);
        Assert.False(completionSource.Task.IsFaulted);
        }

    [Fact]
    public async Task TaskCompletionSourceTrySetResultTwice()
        {
        // Arrange
        TaskCompletionSource completionSource = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        completionSource.TrySetResult();
        completionSource.TrySetResult();

        await completionSource.Task;

        Assert.True(completionSource.Task.IsCompleted);
        Assert.True(completionSource.Task.IsCompletedSuccessfully);
        Assert.False(completionSource.Task.IsCanceled);
        Assert.False(completionSource.Task.IsFaulted);
        }

    [Fact]
    public async Task TaskCompletionSourceTrySetException()
        {
        // Arrange
        TaskCompletionSource completionSource = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        completionSource.TrySetException(new InvalidOperationException());

        try
            {
            await completionSource.Task;
            }
        catch (InvalidOperationException) { }

        Assert.True(completionSource.Task.IsCompleted);
        Assert.False(completionSource.Task.IsCompletedSuccessfully);
        Assert.False(completionSource.Task.IsCanceled);
        Assert.True(completionSource.Task.IsFaulted);
        }

    [Fact]
    public async Task TaskCompletionSourceTrySetCanceled()
        {
        // Arrange
        TaskCompletionSource completionSource = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        completionSource.TrySetCanceled(TestContext.Current.CancellationToken);

        try
            {
            await completionSource.Task;
            }
        catch (TaskCanceledException) { }

        Assert.True(completionSource.Task.IsCompleted);
        Assert.False(completionSource.Task.IsCompletedSuccessfully);
        Assert.True(completionSource.Task.IsCanceled);
        Assert.False(completionSource.Task.IsFaulted);
        }

    [Fact]
    public Task SingleSingleAsyncSignal()
        => TaskTests.SingleAsyncSignal(new SingleAsyncSignal());
        
    [Fact]
    public Task SingleMultipleAsyncSignal()
        => TaskTests.SingleAsyncSignal(new MultipleAsyncSignal());

    [Fact]
    public Task NonMultipleAsyncSignal()
        => Assert.ThrowsAsync<TrueException>(() => TaskTests.MultipleAsyncSignal(new SingleAsyncSignal()));
        
    [Fact]
    public Task MultipleMultipleAsyncSignal()
        => TaskTests.MultipleAsyncSignal(new MultipleAsyncSignal());

    private static async Task SingleAsyncSignal(IAsyncSignal asyncSignal)
        {
        Task backgroundTask = Task.Run(async () =>
            {
            await Task.Delay(50);   // Simulate async work
            asyncSignal.Signal();
            });
        
        await asyncSignal.WaitAsync();   // Assert - WaitAsync should complete once Signal is called
        await backgroundTask;       // optional but ensures clean task completion
        }

    private static async Task MultipleAsyncSignal(IAsyncSignal asyncSignal)
        {
        Task backgroundTask = Task.Run(async () =>
            {
            await Task.Delay(50);   // Simulate async work
            asyncSignal.Signal();
            await Task.Delay(100);   // Simulate async work
            asyncSignal.Signal();
            });
        
        await asyncSignal.WaitAsync();   // Assert - WaitAsync should complete once Signal first call

        DateTime dateTime = DateTime.Now;
        await asyncSignal.WaitAsync();   // Assert - WaitAsync should complete once Signal second call
        Assert.True((DateTime.Now - dateTime).TotalMilliseconds >= 100);

        await backgroundTask;       // optional but ensures clean task completion
        }

    [Fact]
    public void NotSignalingTask()
        {
        Task signaled = TasksExtensions.CreateNullTask();
        Task delay = Task.Delay(10, TestContext.Current.CancellationToken).ContinueWith(_ => signaled.Start(), TestContext.Current.CancellationToken);
        Assert.False(delay.IsCompleted);
        Assert.False(signaled.IsCompleted);
        }

    [Fact]
    public async Task SignalingTask()
        {
        Task signaled = TasksExtensions.CreateNullTask();
        Task delay = Task.Delay(10, TestContext.Current.CancellationToken).ContinueWith(_ => signaled.Start(), TestContext.Current.CancellationToken);
        await signaled;
        Assert.True(delay.IsCompleted);
        Assert.True(signaled.IsCompleted);
        }
    
    [Fact]
    public async Task IsCancellationRequested()
        {
        using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        Task task = Task.Run(() => 
            {
            while (!cancellationTokenSource.Token.IsCancellationRequested);
            }, TestContext.Current.CancellationToken);

        cancellationTokenSource.Cancel();

        await task;
        }

    [Fact]
    public async Task TaskCanceledException()
        {
        using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        Task task = Task.Run(() => 
            {
            while (true);
            }, cancellationTokenSource.Token);

        cancellationTokenSource.Cancel();

        await Assert.ThrowsAsync<TaskCanceledException>(() => task);
        }
    
    [Fact]
    public async Task LinkedTokenSource()
        {
        using CancellationTokenSource manualTokenSource = new CancellationTokenSource();
        using CancellationTokenSource delayTokenSource = new CancellationTokenSource(TimeSpan.FromHours(1));

        using CancellationTokenSource aggregatedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(manualTokenSource.Token, delayTokenSource.Token);

        Task task = Task.Run(() => 
            {
            while (!aggregatedTokenSource.Token.IsCancellationRequested);
            }, TestContext.Current.CancellationToken);

        manualTokenSource.Cancel();

        await task;
        }

    // https://learn.microsoft.com/en-us/archive/msdn-magazine/2019/november/csharp-iterating-with-async-enumerables-in-csharp-8
    //public static IAsyncEnumerable<int> RangeAsync(int start, int count) => Enumerable.Range(start, count).ToAsyncEnumerable();
    public static async IAsyncEnumerable<int> RangeAsync(int start, int count, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
        for (int i = 0; i < count; i++)
            {
            //await Task.FromResult(0);
            await Task.Delay(0, cancellationToken);
            yield return start + i;
            }
        }

    [Fact]
    public async Task TestRangeAsync()
        {
        await foreach (int n in RangeAsync(10, 3, TestContext.Current.CancellationToken))
            {
            Assert.Contains(n, (int[])[10, 11, 12]);
            }
        }

    [Fact]
    public async Task CreateCompletedTask_ReturnsDefaultValue()
        {
        Task<string?> task = TasksExtensions.CreateCompletedTask<string>();

        Assert.True(task.IsCompletedSuccessfully);
        Assert.Null(await task);
        }

    [Fact]
    public async Task WaitForCancellationAsync_CompletesWhenCanceled()
        {
        using CancellationTokenSource source = new();

        Task waitingTask = source.Token.WaitForCancellationAsync();
        source.Cancel();

        await Assert.ThrowsAsync<TaskCanceledException>(() => waitingTask);
        }

    [Fact]
    public async Task WithCancellation_ThrowsWhenCancellationWins()
        {
        using CancellationTokenSource source = new();
        TaskCompletionSource<int> operation = new(TaskCreationOptions.RunContinuationsAsynchronously);

        Task<int> result = operation.Task.WithCancellation(source.Token);
        source.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() => result);
        }

    [Fact]
    public async Task TimeoutAfter_ThrowsWhenTimeoutWins()
        {
        TaskCompletionSource<int> operation = new(TaskCreationOptions.RunContinuationsAsynchronously);

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => operation.Task.TimeoutAfter(TimeSpan.FromMilliseconds(10)));
        }

    [Fact]
    public async Task TimeoutAfter_ReturnsCompletedOperation()
        {
        Task<int> operation = Task.FromResult(42);

        Assert.Equal(42, await operation.TimeoutAfter(TimeSpan.FromSeconds(1)));
        }
    }

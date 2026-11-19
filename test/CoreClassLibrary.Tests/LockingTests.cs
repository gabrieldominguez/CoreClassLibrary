
using CoreClassLibrary.Extensions;
using CoreClassLibrary.Objects.Locking;
using Xunit;

namespace CoreClassLibrary.Tests;

/// <summary>
/// Locker tests.
/// </summary>
/// <remarks>
/// ESTOS TESTS NO PUEDEN CORRER EN PARALELO
/// </remarks>
public class LockingTests(ITestOutputHelper output) : IDisposable
    {
    //private readonly object syncLock = new object();
    private readonly Locker locker = new Locker();

    private readonly LocksDictionary<int> locksDictionary = new LocksDictionary<int>();
    private readonly LockerDictionary<int> asyncLocksDictionary = new LockerDictionary<int>();

    private int state = 0;
    private int expectedState;

    private void Method() => this.Method(default);

    private void Method(CancellationToken cancellationToken)
        {
        output.WriteLine(DateTime.Now.ToString(DateTimeExtensions.TimeFormat)); output.WriteLine(" entrando. state = "); output.WriteLine(this.state.ToString());

        this.state++;

        try
            {
            //Thread.Sleep(10);
            Task.Delay(10, cancellationToken).Wait(cancellationToken);
            }
        catch { }

        if (this.state == 1)
            {
            this.state = 0;
            }

        output.WriteLine(DateTime.Now.ToString(DateTimeExtensions.TimeFormat)); output.WriteLine(" saliendo. state = "); output.WriteLine(this.state.ToString());
        }

    private async Task MethodAsync(CancellationToken cancellationToken = default)
        {
        output.WriteLine(DateTime.Now.ToString(DateTimeExtensions.TimeFormat)); output.WriteLine(" entrando. state = "); output.WriteLine(this.state.ToString());

        this.state++;

        try
            {
            await Task.Delay(10, cancellationToken);
            }
        catch { }

        if (this.state == 1)
            {
            this.state = 0;
            }

        output.WriteLine(DateTime.Now.ToString(DateTimeExtensions.TimeFormat)); output.WriteLine(" saliendo. state = "); output.WriteLine(this.state.ToString());
        }

    /*
    private Task MethodAsync()
        => Task.Run((Action)this.Method);  //Task.Factory.StartNew(this.Method)
    */
    /*
    private void SyncMethod()
        {            
        lock (this.syncLock)
            {
            this.Method();
            }
        }
    */
    private void SyncMethod()
        => this.locker.Lock(this.Method);

    private void SyncMethod(CancellationToken cancellationToken = default)
        => this.locker.Lock(this.Method, cancellationToken, default);

    private Task SyncMethodAsync(CancellationToken cancellationToken = default)
        => this.locker.LockAsync(this.MethodAsync, cancellationToken);

    private void SyncMethod(int id)
        => this.locksDictionary.Lock(id, this.Method);

    private Task SyncMethodAsync(int id)
        => this.asyncLocksDictionary.LockAsync(id, this.MethodAsync);

    /*
    private void SyncMethod(int id)
        {
        lock (this.locksDictionary.GetOrAddLock(id))
            {
            this.Method();
            }
        }

    private async Task SyncMethodAsync(int id)
        {
        SemaphoreSlim semaphoreLock = this.asyncLocksDictionary.GetOrAddLock(id);

        await semaphoreLock.WaitAsync();
        try
            {
            await this.MethodAsync();
            }
        finally
            {
            semaphoreLock.Release();
            }
        }
    */

    /*
    public void Run()
        {
        this.output.WriteLine("Sin bloqueos. Presione Enter..."); this.output.ReadLine();

        this.RunWithNoLock();
        this.RunWithIdNoLock();
        this.RunWithIdNoAsyncLock();

        this.output.WriteLine("");
        this.output.WriteLine("Con bloqueos. Presione Enter..."); this.output.ReadLine();

        this.RunWithLock();
        this.RunWithLock2();
        this.RunWithIdLock();

        this.RunWithAsyncLock();
        this.RunWithIdAsyncLock();

        this.output.WriteLine("");
        this.output.WriteLine("Cancelable. Presione Enter..."); this.output.ReadLine();

        RunCancellableWithLock(new CancellationTokenSource(500).Token);
        }
    */

    /// <summary>
    /// Sin sincronizacion
    /// </summary>
    [Fact]
    public async Task RunWithNoLock()
        {
        this.state = 0;
        this.expectedState = 2;

        await Task.WhenAll(
            this.MethodAsync(TestContext.Current.CancellationToken),
            this.MethodAsync(TestContext.Current.CancellationToken)
            );

        Assert.Equal(this.expectedState, this.state);
        }

    /// <summary>
    /// Con sincronizacion
    /// </summary>
    [Fact]
    public async Task RunWithLock()
        {
        this.state = 0;
        this.expectedState = 0;

        await Task.WhenAll(
            Task.Run(this.SyncMethod, TestContext.Current.CancellationToken),
            Task.Run(this.SyncMethod, TestContext.Current.CancellationToken)
            );

        Assert.Equal(0, this.state);
        }

    /// <summary>
    /// Con sincronizacion
    /// </summary>
    [Fact]
    public async Task RunWithLock2()
        {
        this.state = 0;
        this.expectedState = 0;

        await Task.WhenAll(
            Task.Run(this.SyncMethod, TestContext.Current.CancellationToken),
            Task.Run(this.SyncMethod, TestContext.Current.CancellationToken)
            );

        Assert.Equal(0, this.state);
        }

    /// <summary>
    /// Con sincronizacion, cancelable
    /// </summary>
    [Fact]
    public async Task RunCancellableWithLock()
        {
        using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(500);

        this.state = 0;
        this.expectedState = 0;

        await Task.WhenAll(
                [
                Task.Run(() => this.SyncMethod(cancellationTokenSource.Token), cancellationTokenSource.Token),
                Task.Run(SyncMethod, cancellationTokenSource.Token)
                //Task.Run(this.SyncMethod)
                ]);

        Assert.Equal(0, this.state);
        }

    /// <summary>
    /// Con sincronizacion asincronica
    /// </summary>
    [Fact]
    public async Task RunWithAsyncLock()
        {
        this.state = 0;
        this.expectedState = 0;

        await Task.WhenAll(
            this.SyncMethodAsync(TestContext.Current.CancellationToken),
            this.SyncMethodAsync(TestContext.Current.CancellationToken)
            );

        Assert.Equal(this.expectedState, this.state);
        }

    /*
    /// <summary>
    /// Con sincronizacion dependiente de un id
    /// </summary>
    [Fact]
    public async Task RunWithIdNoLock()
        {
        this.state = 0;
        this.expectedState = 2;

        await Task.WhenAll(
            Task.Run(() => this.SyncMethod(1)),
            Task.Run(() => this.SyncMethod(2))
            );

        Assert.Equal(this.expectedState, this.state);
        }
    */

    /// <summary>
    /// Con sincronizacion dependiente de un id
    /// </summary>
    [Fact]
    public async Task RunWithIdLock()
        {
        this.state = 0;
        this.expectedState = 0;

        await Task.WhenAll(
            Task.Run(() => this.SyncMethod(1), TestContext.Current.CancellationToken),
            Task.Run(() => this.SyncMethod(1), TestContext.Current.CancellationToken)
            );

        Assert.Equal(this.expectedState, this.state);
        }

    /// <summary>
    /// Con sincronizacion dependiente de un id
    /// </summary>
    [Fact]
    public async Task RunWithIdNoAsyncLock()
        {
        this.state = 0;
        this.expectedState = 2;

        await Task.WhenAll(
            this.SyncMethodAsync(1),
            this.SyncMethodAsync(2)
            );

        Assert.Equal(this.expectedState, this.state);
        }

    /// <summary>
    /// Con sincronizacion dependiente de un id
    /// </summary>
    [Fact]
    public async Task RunWithIdAsyncLock()
        {
        this.state = 0;
        this.expectedState = 0;

        await Task.WhenAll(
            this.SyncMethodAsync(1),
            this.SyncMethodAsync(1)
            );

        Assert.Equal(this.expectedState, this.state);
        }


    [Fact]
    public async Task Locker()
        {
        using Locker locker = new Locker(3);
        int counter = 0;

        //ThreadPool.SetMaxThreads(100, 100);

        Task[] tasks = new Task[10];

        for (int i = 0; i < 10; i++)
            {
            tasks[i] = Task.Run(() =>
            {
                output.WriteLine("Task {0} begins and waits for the semaphore, counter = {1}.", Task.CurrentId ?? 0, counter);

                locker.Lock(() =>
                {
                    Interlocked.Increment(ref counter);

                    output.WriteLine("Task {0} enters the semaphore, counter = {1}.", Task.CurrentId ?? 0, counter);

                    Thread.Sleep(10);

                    Interlocked.Decrement(ref counter);

                    output.WriteLine("Task {0} exits the semaphore, counter = {1}.", Task.CurrentId ?? 0, counter);
                });

                output.WriteLine("Task {0} releases the semaphore, counter = {1}.", Task.CurrentId ?? 0, counter);
            }, TestContext.Current.CancellationToken);
            }

        output.WriteLine("Main thread wait to allow all the tasks to start and block.");
        Thread.Sleep(10);

        output.WriteLine("Main thread waits for the tasks to complete.");
        await Task.WhenAll(tasks);

        output.WriteLine("Main thread exits.");
        }
    /*
    [Fact]
    public async Task LockerTimeout()
        {
        using Locker locker = new Locker();
        int counter = 0;
        bool operationCanceled = false;

        Task[] tasks = new Task[2];

        try
            {
            for (int i = 0; i < 2; i++)
                {
                tasks[i] = Task.Run(() =>
                {
                    locker.Lock(() =>
                    {
                        counter++;
                        Thread.Sleep(100);
                    },
                        new CancellationTokenSource(TimeSpan.FromMilliseconds(50)).Token);
                });
                }
            await Task.WhenAll(tasks);
            }
        catch(OperationCanceledException)
            {
            operationCanceled = true;
            }
        catch(AggregateException ex)
            {
            operationCanceled = ex.InnerExceptions.Any(inner => inner.GetType() == typeof(OperationCanceledException));
            }

        Assert.True(operationCanceled);
        Assert.Equal(1, counter);
        }
    */
    public void Dispose()
        {
        this.locker.Dispose();
        GC.SuppressFinalize(this);
        }
    }

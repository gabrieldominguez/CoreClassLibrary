namespace CoreClassLibrary.Objects.Locking;

/// <summary>
/// Encapsula un semaforo.
/// </summary>
/// <remarks>
/// El CancellationToken especificado es para el thread que ha sido bloqueado, no para la action que esta siendo ejecutado.
///
/// Permite codigo asincronico.
/// No confundir con System.Reactive.Concurrency.AsyncLock.
/// </remarks>
/// <example>
/// REGISTRAR COMO SINGLETON (dentro del contexto que se quiere sincronizar)
/// Locker locker = new Locker();
/// ...
/// SBMAESOJSJ sojData = null;
/// await this.locker.LockAsync(async () => sojData = await this.saldoDAO.SelectByKeyAsync(new SBMAESOJSJ { SXXCODSIS = cuentaAS.TipoCuenta, SXXSUC = cuentaAS.Sucursal, SXXCUENTA = cuentaAS.Numero }));
///
/// Para especificar un timeout usar:
/// TimeSpan timeout;
/// ...
/// locker.LockAsync(action, timeout == Timeout.InfiniteTimeSpan? default: new CancellationTokenSource(timeout).Token
/// </example>
/// <see cref="http://www.albahari.com/threading/part2.aspx#_Semaphore"/>
/// <see cref="https://stackoverflow.com/questions/26203040/semaphore-understanding-initial-and-maximum-number-of-requests"/>
/// <see cref="https://stackoverflow.com/questions/1431349/semaphore-timeout-mechanism-in-c-sharp"/>
/// <see cref="https://stackoverflow.com/questions/32624497/does-semaphoreslims-timeout-defeat-its-own-purpose"/>
/// <param name="count">Cantidad de hilos concurrentes.</param>
public class Locker(int count = 1) : IDisposable
    {
    private readonly SemaphoreSlim semaphore = new SemaphoreSlim(count);

    public void Lock(Action action, CancellationToken cancellationToken = default)
        {
        this.semaphore.Wait(cancellationToken);
        try
            {
            action();
            }
        finally
            {
            this.semaphore.Release();
            }
        }

    public void Lock<T>(Action<T> action, T t, CancellationToken cancellationToken = default)
        {
        this.semaphore.Wait(cancellationToken);
        try
            {
            action(t);
            }
        finally
            {
            this.semaphore.Release();
            }
        }

    public TResult Lock<TResult>(Func<TResult> func, CancellationToken cancellationToken = default)
        {
        this.semaphore.Wait(cancellationToken);
        try
            {
            return func();
            }
        finally
            {
            this.semaphore.Release();
            }
        }

    public TResult Lock<T, TResult>(Func<T, TResult> func, T t, CancellationToken cancellationToken = default)
        {
        this.semaphore.Wait(cancellationToken);
        try
            {
            return func(t);
            }
        finally
            {
            this.semaphore.Release();
            }
        }

    public async Task LockAsync(Func<CancellationToken, Task> actionAsync, CancellationToken cancellationToken = default)
        {
        await this.semaphore.WaitAsync(cancellationToken);
        try
            {
            await actionAsync(cancellationToken);
            }
        finally
            {
            this.semaphore.Release();
            }
        }

    public async Task LockAsync<T>(Func<T, CancellationToken, Task> actionAsync, T t, CancellationToken cancellationToken = default)
        {
        await this.semaphore.WaitAsync(cancellationToken);
        try
            {
            await actionAsync(t, cancellationToken);
            }
        finally
            {
            this.semaphore.Release();
            }
        }

    public async Task<TResult> LockAsync<TResult>(Func<Task<TResult>> funcAsync, CancellationToken cancellationToken = default)
        {
        await this.semaphore.WaitAsync(cancellationToken);
        try
            {
            return await funcAsync();
            }
        finally
            {
            this.semaphore.Release();
            }
        }

    public async Task<TResult> LockAsync<T, TResult>(Func<T, Task<TResult>> funcAsync, T t, CancellationToken cancellationToken = default)
        {
        await this.semaphore.WaitAsync(cancellationToken);
        try
            {
            return await funcAsync(t);
            }
        finally
            {
            this.semaphore.Release();
            }
        }

    public void Dispose()
        {
        this.semaphore.Dispose();
        GC.SuppressFinalize(this);
        }
    }

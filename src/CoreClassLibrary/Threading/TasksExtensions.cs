using CoreClassLibrary.Extensions;

namespace CoreClassLibrary.Threading;

/// <summary>
/// Tasks / ValueTasks Extensions
/// </summary>
/// <see cref="https://devblogs.microsoft.com/dotnet/how-async-await-really-works/"/>
public static class TasksExtensions
    {
    public static readonly Task<bool> TrueTask = Task.FromResult(true);
    public static readonly Task<bool> FalseTask = Task.FromResult(false);

    public static Task CreateNullTask()
        => new Task(DelegateExtensions.NullAction);

    // Usar Task.CompletedTask
    //public static Task CreateCompletedTask() => Task.FromResult<object>(null);  // Task.FromResult(0);

    public static Task<T?> CreateCompletedTask<T>()
        => Task.FromResult(default(T));  // Task.FromResult(0);

    public static Task WaitForCancellationAsync(this CancellationToken token)
        {
        TaskCompletionSource completionSource = new TaskCompletionSource();
        token.Register(() => completionSource.TrySetCanceled(token));
        return completionSource.Task;
        }

    /// <summary>
    /// Cancelling an uncancellable operation.
    /// </summary>
    /// <see cref="https://github.com/davidfowl/AspNetCoreDiagnosticScenarios/blob/master/AsyncGuidance.md"/>
    public static async Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken)
        {
        TaskCompletionSource<object> tcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);

        // This disposes the registration as soon as one of the tasks trigger
        using (cancellationToken.Register(state => (state as TaskCompletionSource<object>)?.TrySetResult(new object()), tcs))
            {
            Task resultTask = await Task.WhenAny(task, tcs.Task);
            if (resultTask == tcs.Task)
                {
                // Operation cancelled
                throw new OperationCanceledException(cancellationToken);
                }

            return await task;
            }
        }

    public static async Task<T> TimeoutAfter<T>(this Task<T> task, TimeSpan timeout)
        {
        using CancellationTokenSource cts = new CancellationTokenSource();
        Task delayTask = Task.Delay(timeout, cts.Token);

        Task resultTask = await Task.WhenAny(task, delayTask);
        if (resultTask == delayTask)
            {
            // Operation cancelled
            throw new OperationCanceledException();
            }
        else
            {
            // Cancel the timer task so that it does not fire
            cts.Cancel();
            }

        return await task;
        }

    //public static System.Runtime.CompilerServices.ConfiguredTaskAwaitable<T> ConfigureFalse<T>(this Task<T> task) => task.ConfigureAwait(false);
    //public static System.Runtime.CompilerServices.ConfiguredTaskAwaitable ConfigureFalse(this Task task) => task.ConfigureAwait(false);

    /*
     * NO USAR !!!
     * COSAS EXTRAÑAS PASARAN!!! :)
     * await this.logger.Log(ex);  // await??
     * Hace que se tenga que colocar await en los métodos asincrónicos cuando se quiera descartar un objeto retornado.
     * 
    /// <summary>
    /// Permite escribir de forma simple operadores ternarios que retornan una constante o el resultado de un metodo asincronico.
    /// </summary>
    /// <example>
    /// await "Hello world"
    /// await bit? "Hello world": funcAsync()
    /// </example>
    /// <see cref="https://mariusgundersen.net/duck-extensions/"/>
    public static TaskAwaiter<T> GetAwaiter<T>(this T result) => Task.FromResult(result).GetAwaiter();

    /// <summary>
    /// Permite esperar que terminen todas las tareas sin especificar WhenAll
    /// </summary>
    /// <example>
    /// Before (notice the Task.WhenAll)
    /// var manyThings = await Task.WhenAll(Enumerable.Range(0, 10).Select(GetSomethingAsync));
    ///
    /// After (no more Task.WhenAll)
    /// var manyThings = await Enumerable.Range(0, 10).Select(GetSomethingAsync);
    /// </example>
    /// <see cref="https://mariusgundersen.net/duck-extensions/"/>
    /// <see cref="https://github.com/dotnet/csharplang/issues/380"/>
    /// <see cref="https://gist.github.com/jnm2/3660db29457d391a34151f764bfe6ef7"/>
    /// <see cref="https://www.nuget.org/packages/TaskTupleAwaiter/"/>
    public static TaskAwaiter<T[]> GetAwaiter<T>(this IEnumerable<Task<T>> manyTasks) => Task.WhenAll(manyTasks).GetAwaiter();

    *
    *
    *

    /// <summary>
    /// Para usar cuando la task a esperar puedar ser null
    /// </summary>
    /// <example>
    /// .AppendLine(await (requestMessage.Content?.ReadAsStringAsync() ?? Task.FromResult("")))
    /// .AppendLine(await requestMessage.Content?.ReadAsStringAsync().GetValueOrDefault(""))
    /// </example>
    /// <see cref="https://github.com/dotnet/roslyn/issues/7171"/>
    // Usar directamente await (task ?? Task.FromResult())
    // public static Task<T> GetValueOrDefault<T>(this Task<T> task, T defaultValue = default(T)) => task ?? Task.FromResult(defaultValue);

    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <see cref="https://github.com/dotnet/corefx/issues/2704"/>
    /// <see cref="https://blogs.msdn.microsoft.com/pfxteam/2012/10/05/how-do-i-cancel-non-cancelable-async-operations/"/>
    /// <see cref="https://stackoverflow.com/questions/18670111/task-from-cancellation-token"/>
    public static Task WhenCanceled(this CancellationToken cancellationToken)
        {
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        //cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).SetResult(true), tcs, false);
        cancellationToken.Register(() => tcs.TrySetCanceled(cancellationToken), false);
        return tcs.Task;
        }

    public static Task WhenCanceled(this CancellationToken cancellationToken, int millisecondsTimeout) 
        => Task.Delay(millisecondsTimeout, cancellationToken);
    */
    }

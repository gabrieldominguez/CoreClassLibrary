using System.Runtime.CompilerServices;

namespace CoreClassLibrary.Objects;

/// <summary>
/// Provide Async support to Lazy types.
/// </summary>
/// <see cref="https://blogs.msdn.microsoft.com/pfxteam/2011/01/15/asynclazyt/"/>
/// <see cref="https://mariusgundersen.net/duck-extensions/"/>
public class AsyncLazy<T> : Lazy<Task<T>>
    {
    public AsyncLazy(Func<T> valueFactory) : base(() => Task.Factory.StartNew(valueFactory)) { }
    public AsyncLazy(Func<Task<T>> taskFactory) : base(() => Task.Factory.StartNew(taskFactory).Unwrap()) { }

    public TaskAwaiter<T> GetAwaiter() => this.Value.GetAwaiter();
    }

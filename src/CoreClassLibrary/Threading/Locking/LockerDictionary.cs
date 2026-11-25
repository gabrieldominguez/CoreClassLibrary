using System.Collections.Concurrent;

namespace CoreClassLibrary.Threading.Locking;

/// <summary>
/// Diccionario de objetos para blockear por id bloques de codigo asincronicos.
/// </summary>
/// <see cref="https://stackoverflow.com/questions/45214268/locking-according-to-id-in-async-method"/>
public class LockerDictionary<TKey>(ConcurrentDictionary<TKey, Locker> locks) : IDisposable where TKey : notnull
    {
    public LockerDictionary() : this(new ConcurrentDictionary<TKey, Locker>()) { }

    public Locker GetOrAdd(TKey id)
        => locks.GetOrAdd(id, _ => new Locker());

    public bool Remove(TKey id)
        => locks.TryRemove(id, out _);

    public void Lock(TKey id, Action action, CancellationToken cancellationToken = default)
        => this.GetOrAdd(id).Lock(action, cancellationToken);

    public TResult Lock<TResult>(TKey id, Func<TResult> func, CancellationToken cancellationToken = default)
        => this.GetOrAdd(id).Lock(func, cancellationToken);

    public Task LockAsync(TKey id, Func<CancellationToken, Task> actionAsync, CancellationToken cancellationToken = default)
        => this.GetOrAdd(id).LockAsync(actionAsync, cancellationToken);

    public Task<TResult> LockAsync<TResult>(TKey id, Func<Task<TResult>> funcAsync, CancellationToken cancellationToken = default)
        => this.GetOrAdd(id).LockAsync(funcAsync, cancellationToken);

    public void Clear()
        {
        foreach (Locker locker in locks.Values)
            {
            locker.Dispose();
            }
        locks.Clear();
        }

    public void Dispose()
        {
        GC.SuppressFinalize(this);
        this.Clear();
        }
    }

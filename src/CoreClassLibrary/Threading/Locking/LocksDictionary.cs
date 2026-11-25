using System.Collections.Concurrent;

namespace CoreClassLibrary.Threading.Locking;

/// <summary>
/// Diccionario de objetos para blockear por id bloques de codigo no asincronicos.
/// </summary>
/// <see cref="https://stackoverflow.com/questions/19008894/lock-only-if-value-is-same"/>
public class LocksDictionary<TKey>(ConcurrentDictionary<TKey, object> locks) where TKey : notnull
    {
    public LocksDictionary() : this(new ConcurrentDictionary<TKey, object>()) { }

    public object GetOrAdd(TKey id)
        => locks.GetOrAdd(id, _ => new object());

    public bool Remove(TKey id)
        => locks.TryRemove(id, out _);

    public void Lock(TKey id, Action action)
        {
        lock (this.GetOrAdd(id))
            {
            action();
            }
        }

    public TResult Lock<TResult>(TKey id, Func<TResult> func)
        {
        lock (this.GetOrAdd(id))
            {
            return func();
            }
        }

    public void Clear()
        => locks.Clear();
    }

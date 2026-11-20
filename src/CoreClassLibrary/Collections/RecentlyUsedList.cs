#pragma warning disable IDE0028 // Simplify collection initialization

namespace CoreClassLibrary.Collections;

public class RecentlyUsedList<T> : IReadOnlyList<T>
    {
    private readonly List<T> items = new List<T>();
    public int Count => this.items.Count;

    public T this[int index] => this.items[this.Count - index - 1];

    public void Add(T item)
        {
        this.items.Remove(item);
        this.items.Add(item);
        }

    public IEnumerator<T> GetEnumerator() => this.items.GetEnumerator();
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => this.GetEnumerator();
    }

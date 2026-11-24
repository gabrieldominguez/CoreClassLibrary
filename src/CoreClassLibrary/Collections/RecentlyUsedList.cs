
namespace CoreClassLibrary.Collections;

public class RecentlyUsedList<T>(IList<T> items) : IReadOnlyList<T>
    {
    public int Count => items.Count;

    public T this[int index] => items[this.Count - index - 1];

    public void Add(T item)
        {
        items.Remove(item);
        items.Add(item);
        }

    public IEnumerator<T> GetEnumerator() => items.GetEnumerator();
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => this.GetEnumerator();
    }

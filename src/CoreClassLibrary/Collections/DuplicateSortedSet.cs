namespace CoreClassLibrary.Collections;

/// <summary>
/// Idem SortedSet pero admite items con el mismo orden.
/// </summary>
/// <see cref="https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.sortedset-1"/>
/// <see cref="https://stackoverflow.com/questions/5716423/c-sharp-sortable-collection-which-allows-duplicate-keys"/>
public class DuplicateSortedSet<T> : SortedSet<T> where T : IComparable<T>
    {
    private class DuplicateComparer : IComparer<T>
        {
        public int Compare(T? x, T? y)
            {
            int result = x?.CompareTo(y) ?? 0;
            return result == 0 ? 1 : result;    // Handle equality as beeing greater
            }
        }

    public DuplicateSortedSet(): base(new DuplicateComparer()) { }
    public DuplicateSortedSet(IEnumerable<T> collection) : base(collection, new DuplicateComparer()) { }
    }

using CoreClassLibrary.Extensions;

namespace CoreClassLibrary.Objects.Pooling;

/// <summary>
/// RAII list
/// </summary>
/// <see cref="ValueListBuilder"/>
public readonly record struct PooledList<T> : IDisposable
    {
    public readonly List<T> List = SharedPools<List<T>>.Default.Get().Configure(list => list.Clear());

    public PooledList() { }

    public void Dispose() 
        => SharedPools<List<T>>.Default.Return(this.List);
    }

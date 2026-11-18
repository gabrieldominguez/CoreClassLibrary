using CoreClassLibrary.Extensions;

namespace CoreClassLibrary.Objects.Pooling;

/// <summary>
/// RAII object ("Resource Acquisition Is Initialization")
/// </summary>
/// <see cref="https://learn.microsoft.com/en-us/cpp/cpp/object-lifetime-and-resource-management-modern-cpp"/>
public readonly record struct PooledObject<T> : IDisposable where T : class, new() 
    {
    public readonly T Instance = SharedPools<T>.Default.Get();

    public PooledObject(Action<T> initialize) 
        { 
        this.Instance.Configure(initialize);
        }

    public PooledObject() { }

    public void Dispose()
        => SharedPools<T>.Default.Return(this.Instance);
    }

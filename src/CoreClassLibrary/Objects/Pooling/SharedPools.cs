using Microsoft.Extensions.ObjectPool;

namespace CoreClassLibrary.Objects.Pooling;

/// <summary>
/// Shared object pool
/// </summary>
/// <see cref="http://stackoverflow.com/questions/2510975/c-sharp-object-pooling-pattern-implementation"/>
public static class SharedPools<T> where T : class, new()
    {
    public static readonly ObjectPool<T> Default = ObjectPool.Create<T>();
    }

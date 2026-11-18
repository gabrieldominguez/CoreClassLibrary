using CoreClassLibrary.Threading;

namespace CoreClassLibrary.Objects.Customizers;

public class NullAsyncCustomizer<T> : IAsyncCustomizer<T> where T : class
    {
    public Task<bool> CustomizeAsync(T value)
        => TasksExtensions.TrueTask;
    }

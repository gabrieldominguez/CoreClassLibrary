namespace CoreClassLibrary.Objects.Customizers;

public class AsyncCustomizer<T>(Func<T, Task<bool>> customizer) : IAsyncCustomizer<T> where T : class
    {
    public Task<bool> CustomizeAsync(T value)
        => customizer(value);
    }

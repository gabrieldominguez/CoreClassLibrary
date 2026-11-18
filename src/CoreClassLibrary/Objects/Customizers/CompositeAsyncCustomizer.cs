namespace CoreClassLibrary.Objects.Customizers;

public class CompositeAsyncCustomizer<T>(IReadOnlyCollection<IAsyncCustomizer<T>> customizers) : IAsyncCustomizer<T> where T : class
    {
    public async Task<bool> CustomizeAsync(T value)
        {
        foreach (IAsyncCustomizer<T> customizer in customizers)
            {
            if (!await customizer.CustomizeAsync(value))
                {
                return false;
                }
            }
        return true;
        }
    }

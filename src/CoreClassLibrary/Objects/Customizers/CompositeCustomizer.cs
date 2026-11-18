namespace CoreClassLibrary.Objects.Customizers;

public class CompositeCustomizer<T>(IReadOnlyCollection<ICustomizer<T>> customizers) : ICustomizer<T> where T : class
    {
    public bool Customize(T value)
        {
        foreach (ICustomizer<T> customizer in customizers)
            {
            if (!customizer.Customize(value))
                {
                return false;
                }
            }
        return true;
        }
    }

namespace CoreClassLibrary.Objects.Customizers;

public class Customizer<T>(Func<T, bool> customizer) : ICustomizer<T> where T : class
    {
    public bool Customize(T value)
        => customizer(value);
    }

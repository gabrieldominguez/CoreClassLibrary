namespace CoreClassLibrary.Objects.Customizers;

public class NullCustomizer<T> : ICustomizer<T> where T : class
    {
    public bool Customize(T value)
        => true;
    }

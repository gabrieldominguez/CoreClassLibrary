namespace CoreClassLibrary.Objects.Pipes;

public class Pipeline<T>(IReadOnlyCollection<IPipe<T>> pipes) : IPipe<T>
    {
    public T Transform(T x)
        {
        foreach (IPipe<T> pipe in pipes)
            {
            x = pipe.Transform(x);
            }
        return x;
        }
    }

namespace CoreClassLibrary.Objects.Pipes;

public class Pipe<T>(Func<T, T> f) : Pipe<T, T>(f), IPipe<T>
    {
    }

public class Pipe<TInput, TOutput>(Func<TInput, TOutput> f) : IPipe<TInput, TOutput>
    {
    public TOutput Transform(TInput x)
        => f(x);
    }

namespace CoreClassLibrary.Objects.Pipes;

public interface IPipe<T> : IPipe<T, T>;

public interface IPipe<TInput, TOutput>
    {
    TOutput Transform(TInput x);
    }

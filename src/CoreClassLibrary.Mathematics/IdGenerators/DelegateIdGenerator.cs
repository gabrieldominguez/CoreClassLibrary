namespace CoreClassLibrary.Mathematics.IdGenerators;

public class DelegateIdGenerator<T>(Func<T> generator) : IIdGenerator<T>
    {
    public T NextId() => generator();
    }

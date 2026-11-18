namespace CoreClassLibrary.Objects.Predicates;

public class BinaryAndPredicate<T>(IPredicate<T> predicateLeft, IPredicate<T> predicateRight) : IPredicate<T>
    {
    public bool Evaluate(T value)
        => predicateLeft.Evaluate(value) && predicateRight.Evaluate(value);
    }

public class BinaryOrPredicate<T>(IPredicate<T> predicateLeft, IPredicate<T> predicateRight) : IPredicate<T>
    {
    public bool Evaluate(T value)
        => predicateLeft.Evaluate(value) || predicateRight.Evaluate(value);
    }

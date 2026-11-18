namespace CoreClassLibrary.Objects.Predicates;

public class NotPredicate<T>(IPredicate<T> predicate) : IPredicate<T>
    {
    public bool Evaluate(T value)
        => !predicate.Evaluate(value);
    }

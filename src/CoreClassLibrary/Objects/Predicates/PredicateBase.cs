namespace CoreClassLibrary.Objects.Predicates;

public class PredicateBase<T>(Predicate<T> predicate) : IPredicate<T>
    {
    public bool Evaluate(T value)
        => predicate.Invoke(value);
    }

namespace CoreClassLibrary.Objects.Predicates;

public class FuncPredicate<T>(Func<T, bool> predicate) : IPredicate<T>
    {
    public bool Evaluate(T value)
        => predicate.Invoke(value);
    }

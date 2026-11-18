using CoreClassLibrary.Extensions;
using CoreClassLibrary.Objects.Predicates;

namespace CoreClassLibrary.Objects.Predicates;

public class AndPredicate<T>(IReadOnlyCollection<IPredicate<T>> predicates) : CompositePredicate<T>(predicates), IPredicate<T>
    {
    public bool Evaluate(T value)
        => this.EvaluatePredicates(value).And();
    }

public class OrPredicate<T>(IReadOnlyCollection<IPredicate<T>> predicates) : CompositePredicate<T>(predicates), IPredicate<T>
    {
    public bool Evaluate(T value)
        => this.EvaluatePredicates(value).Or();
    }

public class CompositePredicate<T>(IReadOnlyCollection<IPredicate<T>> predicates)
    {
    protected IEnumerable<bool> EvaluatePredicates(T value)
        => predicates.Select(predicate => predicate.Evaluate(value));
    }

namespace CoreClassLibrary.Objects.Predicates;

/// <summary>
/// Specification Pattern
/// </summary>
/// <see cref="https://en.wikipedia.org/wiki/Specification_pattern#C#_6.0_with_generics"/>
/// <see cref="https://www.martinfowler.com/apsupp/spec.pdf"/>
public interface IPredicate<T>
    {
    bool Evaluate(T value);
    }

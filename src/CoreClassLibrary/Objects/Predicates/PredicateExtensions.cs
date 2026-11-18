using CoreClassLibrary.Extensions;
using CoreClassLibrary.Objects.Predicates;

namespace CoreClassLibrary.Objects.Predicates;

public static class PredicateExtensions
    {
    public static Func<T, bool> ToFunc<T>(this IPredicate<T> predicate)
        => predicate.Evaluate;

    public static Predicate<T> ToPredicate<T>(this IPredicate<T> predicate)
        => predicate.Evaluate;

    public static Func<T, bool> ToFunc<T>(this Predicate<T> predicate)
        => t => predicate(t);

    public static Predicate<T> ToPredicate<T>(this Func<T, bool> func)
        => t => func(t);


    public static IPredicate<T> And<T>(this IPredicate<T> predicateLeft, IPredicate<T> predicateRight)
        => new BinaryAndPredicate<T>(predicateLeft, predicateRight);

    public static IPredicate<T> Or<T>(this IPredicate<T> predicateLeft, IPredicate<T> predicateRight)
        => new BinaryOrPredicate<T>(predicateLeft, predicateRight);

    public static IPredicate<T> Not<T>(IPredicate<T> predicate)
        => new NotPredicate<T>(predicate);


    public static Predicate<T> And<T>(this Predicate<T> predicateLeft, Predicate<T> predicateRight)
        => t => predicateLeft(t) && predicateRight(t);

    public static Predicate<T> Or<T>(this Predicate<T> predicateLeft, Predicate<T> predicateRight)
        => t => predicateLeft(t) || predicateRight(t);

    public static Predicate<T> Not<T>(Predicate<T> predicate)
        => t => !predicate(t);

    public static Predicate<T> Xor<T>(this Predicate<T> predicateLeft, Predicate<T> predicateRight)
        => t => predicateLeft(t) ^ predicateRight(t);


    public static Predicate<T> And<T>(IEnumerable<Predicate<T>> predicates)
        => t => predicates.Select(predicate => predicate(t)).And();

    public static Predicate<T> Or<T>(IEnumerable<Predicate<T>> predicates)
        => t => predicates.Select(predicate => predicate(t)).Or();
    }

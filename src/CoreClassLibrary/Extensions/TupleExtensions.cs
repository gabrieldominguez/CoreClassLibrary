namespace CoreClassLibrary.Extensions;

public static class TupleExtensions
    {
    public static Tuple<T, T> Swap<T>(this Tuple<T, T> tuple) => Tuple.Create(tuple.Item2, tuple.Item1);

    public static (T, T) Swap<T>(this (T, T) tuple) => (tuple.Item2, tuple.Item1);

    public static IEnumerable<T> Enumerate<T>(this Tuple<T, T> input)
        {
        yield return input.Item1;
        yield return input.Item2;
        }

    public static IEnumerable<T> Enumerate<T>(this Tuple<T, T, T> input)
        {
        yield return input.Item1;
        yield return input.Item2;
        yield return input.Item3;
        }
    }

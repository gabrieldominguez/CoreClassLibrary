namespace CoreClassLibrary.Objects.Comparers;

/// <summary>
/// Permite usar expresiones lambdas para definir un comparador.
/// </summary>
public class SelectComparer<TSource, TResult>(Func<TSource?, TResult?> selector) : FuncComparer<TSource>((x, y) => selector.Invoke(x)?.CompareTo(selector.Invoke(y)) ?? 0) where TResult : IComparable<TResult>
    {
    }

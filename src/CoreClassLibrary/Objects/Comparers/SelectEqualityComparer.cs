using CoreClassLibrary.Extensions;

namespace CoreClassLibrary.Objects.Comparers;

/// <summary>
/// Permite usar expresiones lambdas para definir un comparador.
/// </summary>
public class SelectEqualityComparer<TSource, TResult>(Func<TSource?, TResult?> selector) : FuncEqualityComparer<TSource>((x, y) => (selector.Invoke(x)?.Equals(selector.Invoke(y))).ToBoolean(), x => selector.Invoke(x)?.GetHashCode() ?? 0) where TResult : IEquatable<TResult>
    {
    }

namespace CoreClassLibrary.Objects.Comparers;

/// <summary>
/// Permite usar expresiones lambdas para definir un comparador.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <remarks>
/// http://www.lexparse.com/2009/11/02/c-lambdas-never-implement-icomparer-and-iequalitycomparer-again/
/// </remarks>
public class FuncComparer<T>(Func<T?, T?, int> comparerFunc) : Comparer<T>
    {
    public override int Compare(T? x, T? y) => comparerFunc(x, y);
    }

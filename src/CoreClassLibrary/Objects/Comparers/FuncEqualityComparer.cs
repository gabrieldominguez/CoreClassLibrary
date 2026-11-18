namespace CoreClassLibrary.Objects.Comparers;

/// <summary>
/// Permite usar expresiones lambdas para definir un comparador.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <remarks>
/// http://www.lexparse.com/2009/11/02/c-lambdas-never-implement-icomparer-and-iequalitycomparer-again/
/// </remarks>
public class FuncEqualityComparer<T>(Func<T?, T?, bool> equalsFunc, Func<T, int> getHashCodeFunc) : EqualityComparer<T>
    {
    public FuncEqualityComparer(IComparer<T> comparer, Func<T, int> getHashCodeFunc): 
        this((x, y) => comparer.Compare(x, y) == 0, getHashCodeFunc) { }

    public override bool Equals(T? x, T? y) => equalsFunc(x, y);

    public override int GetHashCode(T obj) => getHashCodeFunc(obj);
    }

namespace CoreClassLibrary.Objects.WeakMethods;

/// <summary>
/// Mantiene una referencia a un Func, permitiendo que el objeto al que pertenece el metodo
/// pueda ser reclamado por el Garbage Collector.
/// </summary>
public class WeakFunc<T, TResult>(Func<T, TResult> func) : WeakMethodReference(func)
    {
    public Func<T, TResult> Func => this.CreateDelegate<Func<T, TResult>>();
    }

public class WeakFunc<T1, T2, TResult>(Func<T1, T2, TResult> func) : WeakMethodReference(func)
    {
    public Func<T1, T2, TResult> Func => this.CreateDelegate<Func<T1, T2, TResult>>();
    }

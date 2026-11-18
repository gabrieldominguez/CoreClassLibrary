namespace CoreClassLibrary.Objects.WeakMethods;

/// <summary>
/// Mantiene una referencia a un Action, permitiendo que el objeto al que pertenece el metodo
/// pueda ser reclamado por el Garbage Collector.
/// </summary>
public class WeakAction<T>(Action<T> action) : WeakMethodReference(action)
    {
    public Action<T> Action => this.CreateDelegate<Action<T>>();
    }

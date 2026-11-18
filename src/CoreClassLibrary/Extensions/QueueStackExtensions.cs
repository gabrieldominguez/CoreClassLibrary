namespace CoreClassLibrary.Extensions;

public static class QueueStackExtensions
    {
    /// <summary>
    /// Quita y retorna el primer elemento de la cola.
    /// </summary>
    /// <remarks>
    /// Si la cola está vacia no lanza una excepcion, devuelve default(T).
    /// </remarks>
    public static T? TryDequeue<T>(this Queue<T> queue) => queue.Count > 0 ? queue.Dequeue() : default;

    /// <summary>
    /// Retorna el primer elemento de la cola.
    /// </summary>
    /// <remarks>
    /// Si la cola está vacia no lanza una excepcion, devuelve default(T).
    /// </remarks>
    public static T? TryPeek<T>(this Queue<T> queue) => queue.Count > 0 ? queue.Peek() : default;

    /// <summary>
    /// Quita y retorna el elemento superior de la pila.
    /// </summary>
    /// <remarks>
    /// Si la cola está vacia no lanza una excepcion, devuelve default(T).
    /// </remarks>
    public static T? TryPop<T>(this Stack<T> stack) => stack.Count > 0 ? stack.Pop() : default;

    /// <summary>
    /// Retorna el elemento superior de la pila.
    /// </summary>
    /// <remarks>
    /// Si la cola está vacia no lanza una excepcion, devuelve default(T).
    /// </remarks>
    public static T? TryPeek<T>(this Stack<T> stack) => stack.Count > 0 ? stack.Peek() : default;
    }

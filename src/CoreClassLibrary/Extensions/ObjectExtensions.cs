using System.Collections;
using System.Linq.Expressions;

namespace CoreClassLibrary.Extensions;

public static class ObjectExtensions
    {
    public static T CreateAndConfigure<T>(IEnumerable<Action<T>> actions) where T: new()
        => new T().Configure(actions);

    public static T CreateAndConfigure<T>(Action<T> action) where T: new()
        => new T().Configure(action);

    /// <summary>
    /// Permite aplicar Actions en una sintaxis fluida.
    /// </summary>
    /// <remarks>Al igual que con otros lambdas, no usar si implica crear un lambda con captura de variables (clousure).</remarks>
    public static T Configure<T>(this T t, IEnumerable<Action<T>> actions)
        {
        foreach (Action<T> action in actions)
            {
            action(t);
            }

        return t;
        }

    /// <summary>
    /// Permite aplicar un Action en una sintaxis fluida.
    /// </summary>
    /// <remarks>Al igual que con otros lambdas, no usar si implica crear un lambda con captura de variables (clousure).</remarks>
    public static T Configure<T>(this T t, Action<T> action)
        {
        action(t);
        return t;
        }

    public static T Configure<TContext, T>(this T t, TContext context, Action<TContext, T> action)
        {
        action(context, t);
        return t;
        }

    /// <summary>
    /// Permite aplicar un Action en una sintaxis fluida si ninguno de los parametros en null.
    /// </summary>
    /// <remarks>Al igual que con otros lambdas, no usar si implica crear un lambda con captura de variables (clousure).</remarks>
    public static T ConfigureIfNotNull<T>(this T t, Action<T>? action)
        => ObjectExtensions.ConfigureIf(t, action != null, action!);

    /// <summary>
    /// Permite aplicar un Action en una sintaxis fluida si la condicion es verdadera loqueando primero el recurso.
    /// </summary>
    /// <remarks>Al igual que con otros lambdas, no usar si implica crear un lambda con captura de variables (clousure).</remarks>
    public static T LockConfigureIf<T>(this T t, bool condition, Action<T> action, object syncObject)
        {
        lock (syncObject)
            {
            return ObjectExtensions.ConfigureIf(t, condition, action);
            }
        }

    /// <summary>
    /// Permite aplicar un Action en una sintaxis fluida si la condicion es verdadera.
    /// </summary>
    /// <remarks>Al igual que con otros lambdas, no usar si implica crear un lambda con captura de variables (clousure).</remarks>
    public static T ConfigureIf<T>(this T t, bool condition, Action<T> action)
        {
        if (condition)
            {
            action(t);
            }
        return t;
        }

    /// <summary>
    /// Permite aplicar un Action en una sintaxis fluida.
    /// </summary>
    /// <remarks>Al igual que con otros lambdas, no usar si implica crear un lambda con captura de variables (clousure).</remarks>
    public static async Task<T> ConfigureAsync<T>(this T t, Func<T, CancellationToken, Task> actionAsync, CancellationToken cancellationToken = default)
        {
        await actionAsync(t, cancellationToken);
        return t;
        }

    /// <summary>
    /// Permite aplicar un Action en una sintaxis fluida si ninguno de los parametros en null.
    /// </summary>
    /// <remarks>Al igual que con otros lambdas, no usar si implica crear un lambda con captura de variables (clousure).</remarks>
    public static Task<T> ConfigureIfNotNullAsync<T>(this T t, Func<T, CancellationToken, Task> actionAsync, CancellationToken cancellationToken = default)
        => ObjectExtensions.ConfigureIfAsync(t, t != null && actionAsync != null, actionAsync!, cancellationToken);

    /// <summary>
    /// Permite aplicar un Action en una sintaxis fluida si la condicion es verdadera.
    /// </summary>
    /// <remarks>Al igual que con otros lambdas, no usar si implica crear un lambda con captura de variables (clousure).</remarks>
    public static async Task<T> ConfigureIfAsync<T>(this T t, bool condition, Func<T, CancellationToken, Task> actionAsync, CancellationToken cancellationToken = default)
        {
        if (condition)
            {
            await actionAsync(t, cancellationToken);
            }

        return t;
        }

    /// <summary>
    /// Intercambia los valores de los parámetros.
    /// </summary>
    public static void Swap<T>(ref T item1, ref T item2) =>
        /*
        T tmp = item1;
        item1 = item2;
        item2 = tmp;
        */
        (item2, item1) = (item1, item2);

    /// <summary>
    /// Evalua si el parametro es null o, si es una collección, si está vacia.
    /// </summary>
    /// <returns>True si el parametro es null o si es una collección y está vacia.</returns>
    public static bool IsNullOrEmptyCollection<T>(T? obj)
        => obj == null || (obj is ICollection collection && collection.Count == 0);
    //=> ObjectExtensions.IsNull<T>(obj) || (obj is ICollection collection && collection.Count == 0);

    /*
    /// <summary>
    /// Evalua si el parametro es null.
    /// </summary>
    /// <returns>True si el parametro es null.</returns>
    public static bool IsNull<T>(T obj)
        => typeof(T) != typeof(ValueType) && Object.Equals(obj, default);
        //=> obj == null;
    */

    /// <summary>
    /// Si el objeto es null lo inicializa con la funcion pasada por parametro
    /// </summary>
    public static T GetLazy<T>(ref T storage, Func<T> valueFactory) where T : class
        => storage ??= valueFactory();

    /// <summary>
    /// Retorna el menor de los dos parametros.
    /// </summary>
    public static T Min<T>(T obj1, T obj2) where T : IComparable<T>
        => obj1.CompareTo(obj2) < 0 ? obj1 : obj2;

    /// <summary>
    /// Retorna el mayor de los dos parametros.
    /// </summary>
    public static T Max<T>(T obj1, T obj2) where T : IComparable<T>
        => obj1.CompareTo(obj2) > 0 ? obj1 : obj2;

    /// <summary>
    /// Trata de castear el objeto y si lo logra, ejecuta la acción sobre él.
    /// </summary>
    /// <typeparam name="T">Tipo al que castear el objeto.</typeparam>
    /// <param name="obj">Objeto a castear.</param>
    /// <param name="action">Acción a ejecutar si el objeto puede ser casteado.</param>
    /// <returns>Objeto casteado o null si no pudo serlo.</returns>
    public static T? TryCastAndInvoke<T>(this object obj, Action<T> action) where T : class
        {
        T? objCasted = obj as T;

        if (objCasted != null)
            {
            action(objCasted);
            }

        return objCasted;
        }

    public static ActionResponse TryInvoke(this object obj, Action<dynamic> action)
        {
        try
            {
            action(obj);
            return ActionResponse.Success;
            }
        catch (Exception ex)
            {
            return new ActionResponse(ex);
            }
        }

    public static FuncResponse<T> TryFunc<T>(this object obj, Func<dynamic, T> func, T? defaultValue = default)
        {
        try
            {
            return new FuncResponse<T>(func.Invoke(obj));
            }
        catch (Exception ex)
            {
            return new FuncResponse<T>(ex, defaultValue);
            }
        }

    /// <summary>
    /// Crea una instancia con un parametro.
    /// </summary>
    /// <remarks>
    /// Si no se necesitan parametros usar Activator.CreateInstance<T>()
    /// Si se necesitan más parametros usar CreateInstance<T>(params object[] args)
    /// </remarks>
    public static T CreateInstance<T>(object arg)
        => (T) (Activator.CreateInstance(typeof(T), arg) ?? throw new InvalidOperationException(String.Concat(typeof(T).FullName, " cannot be created.")));

    /// <summary>
    /// Crea una instancia con parametros.
    /// </summary>
    /// <remarks>
    /// Si no se necesitan parametros usar Activator.CreateInstance<T>()
    /// </remarks>
    public static T CreateInstance<T>(params object[] args)
        => (T) (Activator.CreateInstance(typeof(T), args) ?? throw new InvalidOperationException(String.Concat(typeof(T).FullName, " cannot be created.")));

    /// <summary>
    /// Devuelve una factory para crear objetos del tipo especificado.
    /// </summary>
    /// <remarks>
    /// Si se tienen que crear MUCHOS (millones) objetos de un tipo no conocido en tiempo de compilación,
    /// usar esta factory devuelta por este método es más eficiente que Activator.CreateInstance().
    /// </remarks>
    /// <example>
    /// <![CDATA[
    ///         [TestMethod]
    ///         public void PerfTest()
    ///             {
    ///             Type type = typeof(Deuda);
    ///             Stopwatch watch = new Stopwatch();
    ///             using (watch.BeginScope())
    ///                  {
    ///                  for (int i = 0; i < 1000000; i++)
    ///                      {
    ///                      Activator.CreateInstance(type);
    ///                      }
    ///                  }
    ///             Trace.WriteLine(String.Format("Activator.CreateInstance(type): {0}ms", watch.ElapsedMilliseconds));   // 175ms
    ///             using (watch.BeginScope())
    ///                 {
    ///                 Func<object> factory = ObjectExtensions.CreateFactory(type);
    ///                 for (int i = 0; i < 1000000; i++)
    ///                     {
    ///                     factory();
    ///                     }
    ///                 }
    ///             Trace.WriteLine(String.Format("ObjectExtensions.CreateFactory(type)(): {0}ms", watch.ElapsedMilliseconds)); // 57ms
    ///             }
    ///         }
    /// ]]>
    /// </example>
    /// <param name="type"></param>
    public static Func<object> CreateFactory(this Type type)
        => Expression.Lambda<Func<object>>(Expression.New(type)).Compile();
    }

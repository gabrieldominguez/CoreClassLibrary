#pragma warning disable IDE0046 // Convert to conditional expression

using System.Linq.Expressions;
using System.Reflection;
using System.Transactions;

namespace CoreClassLibrary.Extensions;

public static class DelegateExtensions
    {
    public static readonly Action NullAction = () => { };

    public static Action<T> CreateNullAction<T>()
        => _ => { };

    public static void CreateInvokeDispose<T>(Func<T> factory, Action<T> action) where T : IDisposable
        {
        using T obj = factory();
        action(obj);
        }

    public static TResult CreateInvokeDispose<T, TResult>(Func<T> factory, Func<T, TResult> func) where T : IDisposable
        {
        using T obj = factory();
        return func(obj);
        }

    /// <summary>
    /// Ejecuta la acción, reintentando hazta que la ejecución sea exitosa y atrapando las excepciones que pudieran lanzar.
    /// </summary>
    /// <remarks>
    /// Si retryCount = 0, la acción al menos se ejecuta una vez.
    /// Si todas las ejecuciones fallan, se guarda una AggregateException conteniendo todas las excepciones atrapadas.
    /// </remarks>
    /// <param name="action">Acción a ejecutar.</param>
    /// <param name="obj">Parametro de la acción</param>
    /// <param name="retryCount">Cantidad de reintentos.</param>
    public static ActionResponse ReTryInvoke<T>(this Action<T> action, T obj, int retryCount = 1)
        {
        Lazy<List<Exception>> exceptions = new Lazy<List<Exception>>(() => []);

        while (retryCount-- >= 0)
            {
            try
                {
                action.Invoke(obj);
                return ActionResponse.Success;
                }
            catch (Exception ex)
                {
                exceptions.Value.Add(ex);
                }
            }

        return new ActionResponse(new AggregateException(exceptions.Value));
        }

    /// <summary>
    /// Ejecuta la acción, reintentando hazta que la ejecución sea exitosa y atrapando las excepciones que pudieran lanzar.
    /// </summary>
    /// <remarks>
    /// Si retryCount = 0, la acción al menos se ejecuta una vez.
    /// Si todas las ejecuciones fallan, se lanza una AggregateException conteniendo todas las excepciones atrapadas.
    /// </remarks>
    /// <param name="action">Acción a ejecutar.</param>
    /// <param name="retryCount">Cantidad de reintentos.</param>
    public static ActionResponse ReTryInvoke(this Action action, int retryCount = 1)
        {
        Lazy<List<Exception>> exceptions = new Lazy<List<Exception>>(() => []);

        while (retryCount-- >= 0)
            {
            try
                {
                action.Invoke();
                return ActionResponse.Success;
                }
            catch (Exception ex)
                {
                exceptions.Value.Add(ex);
                }
            }

        return new ActionResponse(new AggregateException(exceptions.Value));
        }

    /// <summary>
    /// Ejecuta la función, reintentando hazta que la ejecución sea exitosa y atrapando las excepciones que pudieran lanzar.
    /// </summary>
    /// <remarks>
    /// Si retryCount = 0, la acción al menos se ejecuta una vez.
    /// Si todas las ejecuciones fallan, se guarda una AggregateException conteniendo todas las excepciones atrapadas.
    /// </remarks>
    /// <param name="func">Funcion a ejecutar.</param>
    /// <param name="retryCount">Cantidad de reintentos.</param>
    public static FuncResponse<T>? ReTryInvoke<T>(this Func<T> func, int retryCount = 1)
        {
        Lazy<List<Exception>> exceptions = new Lazy<List<Exception>>(() => []);

        while (retryCount-- >= 0)
            {
            try
                {
                return new FuncResponse<T>(func.Invoke());
                }
            catch (Exception ex)
                {
                exceptions.Value.Add(ex);
                }
            }

        if (exceptions.IsValueCreated)
            {
            throw new AggregateException(exceptions.Value);
            }

        return default;
        }

    /// <summary>
    /// Ejecuta la acción, atrapando la excepcion que pudiera lanzar.
    /// </summary>
    /// <remarks>
    /// La excepción atrapada es desechada.
    /// </remarks>
    /// <param name="action">Acción a ejecutar.</param>
    /// <see cref="RuntimeHelpers.TryCode"/>
    public static ActionResponse TryInvoke(this Action action)
        {
        try
            {
            action.Invoke();
            return ActionResponse.Success;
            }
        catch (Exception ex)
            {
            return new ActionResponse(ex);
            }
        }

    /// <summary>
    /// Ejecuta la acción, atrapando la excepcion que pudiera lanzar.
    /// </summary>
    /// <remarks>
    /// La excepción atrapada es desechada.
    /// </remarks>
    /// <param name="action">Acción a ejecutar.</param>
    /// <param name="obj">Parametro de la acción</param>
    /// <see cref="RuntimeHelpers.TryCode"/>
    public static ActionResponse TryInvoke<T>(this Action<T> action, T obj)
        {
        try
            {
            action.Invoke(obj);
            return ActionResponse.Success;
            }
        catch (Exception ex)
            {
            return new ActionResponse(ex);
            }
        }

    public static ActionResponse TryInvoke(this object obj, Action<dynamic> action)
        {
        try
            {
            action.Invoke(obj);
            return ActionResponse.Success;
            }
        catch (Exception ex)
            {
            return new ActionResponse(ex);
            }
        }


    /// <summary>
    /// Ejecuta una funcion, retornando un valor por defecto si la misma falla.
    /// </summary>
    /// <param name="func">Funcion a ejecutar.</param>
    /// <returns>El valor retornado por func o defaultValue si func lanza una excepcion.</returns>
    public static FuncResponse<T> TryInvoke<T>(this Func<T> func)
        {
        try
            {
            return new FuncResponse<T>(func.Invoke());
            }
        catch (Exception ex)
            {
            return new FuncResponse<T>(ex);
            }
        }

    public static FuncResponse<T> TryFunc<T>(this object obj, Func<dynamic, T> func)
        {
        try
            {
            return new FuncResponse<T>(func.Invoke(obj));
            }
        catch (Exception ex)
            {
            return new FuncResponse<T>(ex);
            }
        }

    /// <summary>
    /// Ejecuta las acciones, si la precedente falla.
    /// </summary>
    /// <remarks>
    /// La enumeración termina cuando se consigue la primer ejecución exitosa.
    /// Si todas las ejecuciones fallan, guarda en la respuesta una AggregateException conteniendo todas las excepciones atrapadas.
    /// </remarks>
    /// <param name="actions">Acciones a ejecutar.</param>
    public static ActionResponse TryInvoke(this IEnumerable<Action> actions)
        {
        Lazy<List<Exception>> exceptions = new Lazy<List<Exception>>(() => []);

        foreach (Action action in actions)
            {
            try
                {
                action.Invoke();
                return ActionResponse.Success;
                }
            catch (Exception ex)
                {
                exceptions.Value.Add(ex);
                }
            }

        return new ActionResponse(new AggregateException(exceptions.Value));
        }

    /// <summary>
    /// Ejecuta las funciones retornando el valor retornado por la primera que no falle.
    /// </summary>
    /// <remarks>
    /// La enumeración termina cuando se consigue la primer ejecución exitosa.
    /// Si todas las ejecuciones fallan, guarda en la respuesta una AggregateException conteniendo todas las excepciones atrapadas.
    /// </remarks>
    /// <param name="funcs">Funciones a ejecutar.</param>
    public static FuncResponse<T> TryInvoke<T>(this IEnumerable<Func<T>> funcs)
        {
        Lazy<List<Exception>> exceptions = new Lazy<List<Exception>>(() => []);

        foreach (Func<T> func in funcs)
            {
            try
                {
                return new FuncResponse<T>(func.Invoke());
                }
            catch (Exception ex)
                {
                exceptions.Value.Add(ex);
                }
            }

        return new FuncResponse<T>(new AggregateException(exceptions.Value));
        }

    /// <summary>
    /// Espera el tiempo especificado y ejecuta la acción.
    /// </summary>
    /// <remarks>La espera bloquea el thread actual.</remarks>
    /// <see cref="http://channel9.msdn.com/Events/Speakers/Stephen-Toub"/>
    public static void DelayInvoke(this Action action, TimeSpan delay)
        {
        Task.Delay(delay).Wait();
        action.Invoke();
        }

    /// <summary>
    /// Espera el tiempo especificado y ejecuta la acción.
    /// </summary>
    /// <remarks>La espera NO bloquea el thread actual.</remarks>
    /// <see cref="http://channel9.msdn.com/Events/Speakers/Stephen-Toub"/>
    public static async Task DelayInvokeAsync(this Action action, TimeSpan delay, CancellationToken cancellationToken = default)
        {
        await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
        action.Invoke();

        // Task.Delay(delay).ContinueWith(delegate { action.Invoke(); }).Start();

        // NO USAR: 'Fake' async
        // Task.Run(() => { Thread.Sleep(delay); action.Invoke(); });
        // Task.Factory.StartNew(() => { Thread.Sleep(delay); action.Invoke(); });
        }

    public static Action Combine(this IEnumerable<Action> actions)
        => () =>
            {
            foreach (Action a in actions)
                {
                a();
                }
            };

    public static Action<T> Combine<T>(this IEnumerable<Action<T>> actions)
        => x =>
            {
            foreach (Action<T> a in actions)
                {
                a(x);
                }
            };

    public static Func<T, T> Combine<T>(this IEnumerable<Func<T, T>> funcs)
        => x =>
            {
            foreach (Func<T, T> f in funcs)
                {
                x = f(x);
                }
            return x;
            };

    /// <summary>
    /// Retorna informacion de un método.
    /// </summary>
    /// <example>
    /// var cw = InfoOf(() => Console.WriteLine("{0} is {1}", "Bart", 26));
    /// </example>
    /// <param name="expression">Expresión con la definición del método.</param>
    /// <returns>Informacion de un método</returns>
    public static MemberInfo? InfoOf(this Expression<Action> expression)
        => expression.Body.NodeType switch
            {
            ExpressionType.Call => ((MethodCallExpression)expression.Body).Method,
            ExpressionType.MemberAccess => ((MemberExpression)expression.Body).Member,
            ExpressionType.New => ((NewExpression)expression.Body).Constructor,
            _ => throw new NotSupportedException(),
            };

    /// <summary>
    /// Retorna informacion de un método.
    /// </summary>
    /// <param name="expression">Expresión con la definición del método.</param>
    /// <returns>Informacion de un método</returns>
    public static MemberInfo? InfoOf<T>(this Expression<Func<T>> expression)
        => expression.Body.NodeType switch
            {
            ExpressionType.Call => ((MethodCallExpression)expression.Body).Method,
            ExpressionType.MemberAccess => ((MemberExpression)expression.Body).Member,
            ExpressionType.New => ((NewExpression)expression.Body).Constructor,
            _ => throw new NotSupportedException(),
            };

    /// <summary>
    /// Ejecuta la acción dentro de una transacción.
    /// </summary>
    /// <param name="action">Acción a ejecutar dentro de una transacción.</param>
    /// <example>
    ///
    ///    ((Action)HacerAlgo1).Transacted();
    ///
    ///    ((Action)(() =>  {HacerAlgo1(); HacerAlgo2();})).Transacted();
    ///
    /// </example>
    public static void TransactedInvoke(this Action action)
        {
        using TransactionScope transactionScope = new TransactionScope();

        action.Invoke();

        transactionScope.Complete();
        }

    public static async Task TransactedInvokeAsync(this Func<Task> actionAsync)
        {
        using TransactionScope transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        await actionAsync.Invoke();

        transactionScope.Complete();
        }
    }

namespace CoreClassLibrary.Objects;

/// <summary>
/// Mantiene un singleton de un objeto.
/// </summary>
/// <remarks>
/// <![CDATA[
/// Lazy<T> es thread-safe por lo que esta implementacion no requiere el bloqueo y doble chequeo
/// de la implemntación estandar.
///
/// C#: System.Lazy<T> and the Singleton Design Pattern
/// http://geekswithblogs.net/BlackRabbitCoder/archive/2010/05/19/c-system.lazylttgt-and-the-singleton-design-pattern.aspx
///
/// T debe poseer un constructor sin parametros que se usará para construir el objeto singleton.
/// ]]>
/// </remarks>
public sealed class Singleton<T> where T : class, new()
    {
    private static readonly Lazy<T> instance = new Lazy<T>();

    public static T Instance => Singleton<T>.instance.Value;
    }

/*
/// <summary>
/// Mantiene un singleton de un objeto.
/// </summary>
/// <remarks>
/// Multithreaded Singleton Pattern
/// http://msdn2.microsoft.com/en-us/library/ms998558.aspx
/// 
/// T debe poseer un constructor sin parametros que se usará para construir el objeto singleton.
/// </remarks>
public sealed class Singleton<T> where T: class, new()  
    {
    private static readonly object syncRoot = new Object();

    private static T instance;
    public static T Instance
        {
        get
            {                
            if (Singleton<T>.instance == null)
                {
                lock (Singleton<T>.syncRoot)
                    {
                    if (Singleton<T>.instance == null)
                        {
                        Singleton<T>.instance = new T();
                        }
                    }
                }

            return Singleton<T>.instance;
            }
        }
    }        
*/

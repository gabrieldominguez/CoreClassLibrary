using System.Reflection;

namespace CoreClassLibrary.Objects;

/// <summary>
/// Permite usar un Mutex de la misma forma que lock. 
/// </summary>
/// <remarks>
/// Consigue exclusividad a nivel de máquina.
/// </remarks>
/// <example>
/// using (new SingleGlobalInstance(TimeSpan.FromSeconds(1))) // timeout on global lock
///     {
///     RunSomeStuff(); // Only 1 of these runs at a time
///     }
/// </example>
/// <see cref="http://stackoverflow.com/a/7810107/103085"/>
public sealed class SingleGlobalInstance : IDisposable
    {
    private readonly bool hasHandle = false;
    private Mutex? mutex;

    public SingleGlobalInstance() : this(Timeout.InfiniteTimeSpan) { }

    public SingleGlobalInstance(TimeSpan timeout)
        {
        this.mutex = new Mutex(false, $"Global\\{{{{{Assembly.GetEntryAssembly()!.FullName}}}}}");
        
        try
            {
            this.hasHandle = this.mutex.WaitOne(timeout);

            if (!this.hasHandle)
                {
                throw new TimeoutException("Timeout waiting for exclusive access on SingleInstance");
                }
            }
        catch (AbandonedMutexException)
            {
            this.hasHandle = true;
            }
        }

    public void Dispose()
        {
        if (this.mutex != null && this.hasHandle)
            {
            this.mutex.ReleaseMutex();
            this.mutex.Dispose();
            this.mutex = null;
            }
        }
    }

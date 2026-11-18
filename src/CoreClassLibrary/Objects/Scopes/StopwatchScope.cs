using System.Diagnostics;

namespace CoreClassLibrary.Objects.Scopes;

/// <summary>
/// Crea un scope y mide el tiempo de ejecución del mismo.
/// </summary>
/// <remarks>
/// Tambien puede crearse usando StopwatchExtensions.BeginScope().
/// </remarks>
/// <example>
/// <![CDATA[
/// Stopwatch watch = new Stopwatch();
/// using (watch.BeginScope())
///     {
///     Thread.Sleep(150);
///     }
/// Trace.WriteLine(watch.ElapsedMilliseconds);
/// ]]>
/// </example>    
/// <see cref="StopwatchExtensions.BeginScope"/>
/// <seealso cref="StopwatchExtensions.InvokeAndMessure"/>
public sealed class StopwatchScope : IDisposable
    {
    private readonly Stopwatch watch;

    public StopwatchScope(Stopwatch watch)
        {
        (this.watch = watch).Restart();
        }

    /// <summary>
    /// Termina el scope, deteniendo el Stopwatch.
    /// </summary>
    public void Dispose() => this.watch.Stop();
    }

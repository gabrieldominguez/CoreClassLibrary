using System.Diagnostics;
using CoreClassLibrary.Objects.Scopes;

namespace CoreClassLibrary.Extensions;

public static class StopwatchExtensions
    {
    /// <summary>
    /// Ejecuta el action y mide el tiempo que tarda en terminar.
    /// </summary>
    /// <returns>Duración de la ejecución.</returns>
    public static TimeSpan InvokeAndMessure(this Stopwatch watch, Action action)
        {
        watch.Restart();
        action();
        watch.Stop();

        return watch.Elapsed;
        }

    /// <summary>
    /// Crea un StopwatchScope para medir el tiempo del mismo con el Stopwatch especificado.
    /// </summary>
    /// <param name="watch">Stopwatch con el cual medir el tiempo del scope.</param>
    /// <returns>StopwatchScope creado.</returns>
    /// <see cref="StopwatchScope"/>
    /// <seealso cref="InvokeAndMessure"/>
    public static StopwatchScope BeginScope(this Stopwatch watch)
        => new StopwatchScope(watch);

    /// <summary>
    /// Calculates the elapsed time, in milliseconds, since the specified starting timestamp.
    /// </summary>
    /// <remarks>
    /// This method avoids the overhead of creating a <see cref="System.Diagnostics.Stopwatch"/> instance.
    /// This method is useful for measuring elapsed time with high precision. 
    /// The returned value is based on the system's high-resolution performance counter.
    /// </remarks>
    /// <param name="start">The starting timestamp, as returned by Stopwatch.GetTimestamp(), from which to measure elapsed time.</param>
    /// <returns>The number of milliseconds that have elapsed since the specified starting timestamp.</returns>
    public static double GetElapsedMilliseconds(long start)
        => StopwatchExtensions.GetElapsedMilliseconds(start, Stopwatch.GetTimestamp());

    /// <summary>
    /// Calculates the elapsed time, in milliseconds, between two timestamp values obtained from the system timer.
    /// </summary>
    /// <remarks>
    /// This method avoids the overhead of creating a <see cref="System.Diagnostics.Stopwatch"/> instance.
    /// This method computes the elapsed time by subtracting the starting timestamp from the ending timestamp.
    /// 
    /// Both <paramref name="start"/> and <paramref name="stop"/> should be obtained from the same
    /// timing source, such as <see cref="System.Diagnostics.Stopwatch"/>. The calculation uses the system timer
    /// frequency to convert ticks to milliseconds.
    /// </remarks>
    /// <param name="start">The starting timestamp, typically retrieved from the system timer or Stopwatch. Must be less than or equal to
    /// <paramref name="stop"/>.</param>
    /// <param name="stop">The ending timestamp, typically retrieved from the system timer or Stopwatch. Must be greater than or equal to
    /// <paramref name="start"/>.</param>
    /// <returns>The elapsed time in milliseconds as a double-precision floating-point value.</returns>
    /// <example>
    /// long start = Stopwatch.GetTimestamp();
    /// double elapsedMilliseconds = StopwatchExtensions.GetElapsedMilliseconds(start, Stopwatch.GetTimestamp());
    /// </example>
    public static double GetElapsedMilliseconds(long start, long stop) 
        => (double)((stop - start) * 1000) / (double)Stopwatch.Frequency;
    }

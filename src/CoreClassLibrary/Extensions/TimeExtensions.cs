namespace CoreClassLibrary.Extensions;

/// <summary>
/// TimeSpan Extensions
/// </summary>
/// <see cref="https://stackoverflow.com/questions/1295751/how-can-i-achieve-a-modulus-operation-with-system-timespan-values-without-loopi"/>
/// <see cref="https://msdn.microsoft.com/en-us/library/system.timespan(v=vs.110).aspx"/>
/// <see cref="https://learn.microsoft.com/en-us/dotnet/standard/datetime/how-to-use-dateonly-timeonly"/>
public static class TimeExtensions
    {
    public const int SecondsPerDay = 86400;
    public const int SecondsPerHour = 3600;
    public const int SecondsPerMinute = 60;

    public const double DaysPerYear = 365.242199;

    public static TimeOnly GetNow(this TimeProvider timeProvider)
        => TimeOnly.FromTimeSpan(timeProvider.GetLocalNow().TimeOfDay);

    public static bool IsNow(this TimeProvider timeProvider, TimeOnly time, TimeSpan timeWindow)
        => TimeExtensions.AreEqual(time, timeProvider.GetNow(), timeWindow);

    public static TimeSpan TimeOfDay(this TimeSpan time)
        => time.Modulo(TimeSpan.FromDays(1));

    /// <summary>
    /// Compara dos valores, teniendo en cuenta un intervalo dentro del que se consideran iguales dentro de un dia.
    /// </summary>
    /// <remarks>
    /// El doble chequeo es para cuando val1 == TimeSpan.Zero o val2 == TimeSpan.Zero
    /// El valor 0 y 24 se consideran iguales.
    /// </remarks>
    /// <returns>True si los valores se consideran iguales.</returns>
    public static bool AreEqualDaily(TimeSpan val1, TimeSpan val2, TimeSpan timeWindow)
        => TimeExtensions.AreEqual(val1, val2, timeWindow)
                || val1.Add(TimeSpan.FromDays(1) - val2).Duration() <= timeWindow
                || val2.Add(TimeSpan.FromDays(1) - val1).Duration() <= timeWindow;

    /// <summary>
    /// Compara dos valores, teniendo en cuenta un intervalo dentro del que se consideran iguales.
    /// </summary>
    /// <returns>True si los valores se consideran iguales.</returns>
    public static bool AreEqual(TimeSpan val1, TimeSpan val2, TimeSpan timeWindow)
        => val1.Subtract(val2).Duration() <= timeWindow;

    private static bool AreEqual(TimeOnly val1, TimeOnly val2, TimeSpan timeWindow)
        => TimeExtensions.AreEqual(val1.ToTimeSpan(), val2.ToTimeSpan(), timeWindow);

    public static TimeSpan FromYears(double value)
        => TimeSpan.FromDays(value * TimeExtensions.DaysPerYear);

    public static double TotalYears(this TimeSpan span)
        => span.TotalDays / TimeExtensions.DaysPerYear;

    public static TimeSpan Multiply(this TimeSpan time, int times) 
        => TimeSpan.FromTicks(time.Ticks * times);

    public static TimeSpan Divide(this TimeSpan time, int divisor) 
        => TimeSpan.FromTicks(time.Ticks / divisor);

    public static double Divide(this TimeSpan time, TimeSpan divisor)
        => (double)time.Ticks / divisor.Ticks;

    public static TimeSpan Modulo(this TimeSpan time, TimeSpan divisor)
        => TimeSpan.FromTicks(time.Ticks % divisor.Ticks);
    }

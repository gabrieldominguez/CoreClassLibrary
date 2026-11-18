using CoreClassLibrary.Extensions;

namespace CoreClassLibrary.Objects;

/// <summary>
/// Representa un intervalo de tiempo dentro de una semana.
/// </summary>
/// <example>
/// Los siguientes son todos ejemplos válidos:
///     Intervalo completo, fulltime
///     WeeklyInterval.Parse("Sunday-Saturday|00:00:00-23:59:59.999")
/// 
///     Semana laboral
///     WeeklyInterval.Parse("Monday-Friday|10:00:00-18:00:00")
///
///     Fin de semana
///     WeeklyInterval.Parse("Saturday-Sunday|00:00:00-23:59:59.999")
/// 
///     Un solo dia
///     WeeklyInterval.Parse("Monday-Monday|10:00:00-18:00:00")
/// 
///     Entre semanas
///     WeeklyInterval.Parse("Thursday-Monday|00:00:00-23:59:59.999")
/// 
///     Entre dias
///     WeeklyInterval.Parse("Monday-Friday|20:00:00-05:00:00")
///
///     Entre semanas y dias
///     WeeklyInterval.Parse("Thursday-Monday|20:00:00-05:00:00")
/// 
/// </example>
/*
public enum DayOfWeek
        {
        Sunday    = 0,
        Monday    = 1,
        Tuesday   = 2,
        Wednesday = 3,
        Thursday  = 4,
        Friday    = 5,
        Saturday  = 6
        }
*/
public readonly record struct WeeklyInterval(DayOfWeek StartDay, DayOfWeek EndDay, TimeSpan StartTime, TimeSpan EndTime)
    {
    public static readonly WeeklyInterval FullTime = new WeeklyInterval(DayOfWeek.Sunday, DayOfWeek.Saturday, TimeSpan.Zero, new TimeSpan(0, 23, 59, 59, 999));

    /// <summary>
    /// Evalua si la hora correspondiente al dia actual pertenece al intervalo.
    /// </summary>
    /// <returns>True si lo contiene.</returns>
    public bool IsInInterval(DateTimeOffset dateTimeOffset)
        => this.IsInInterval(dateTimeOffset.DateTime);

    /// <summary>
    /// Evalua si el dia pasado por parametro pertenece al intervalo.
    /// </summary>
    /// <param name="dateTime">DateTime con la fecha a evaluar.</param>
    /// <returns>True si lo contiene.</returns>
    public bool IsInInterval(DateTime dateTime)
        => this.IsInInterval(dateTime.DayOfWeek, dateTime.TimeOfDay);

    /// <summary>
    /// Evalua si la hora correspondiente al dia actual pertenece al intervalo.
    /// </summary>
    /// <returns>True si lo contiene.</returns>
    public bool IsInInterval(TimeSpan timeOfDay)
        => this.IsInInterval(DateTime.Today.DayOfWeek, timeOfDay);

    /// <summary>
    /// Evalua si el dia pasado por parametro pertenece al intervalo.
    /// </summary>
    /// <param name="dayOfWeek">Dia del intervalo a evaluar.</param>
    /// <param name="timeOfDay">Hora del intervalo a evaluar.</param>
    /// <returns>True si lo contiene.</returns>
    public bool IsInInterval(DayOfWeek dayOfWeek, TimeSpan timeOfDay) 
        => (this.StartDay <= this.EndDay ?
                this.StartDay <= dayOfWeek && dayOfWeek <= this.EndDay :
                this.StartDay <= dayOfWeek || dayOfWeek <= this.EndDay)
            &&
            (this.StartTime <= this.EndTime ?
                this.StartTime <= timeOfDay && timeOfDay <= this.EndTime :
                this.StartTime <= timeOfDay || timeOfDay <= this.EndTime);

    /// <summary>
    /// Parsea una cadena
    /// </summary>
    /// <param name="s">Cadena con formato: "Sunday-Saturday|00:00:00-23:59:59.999"</param>
    /// <returns>WeeklyInterval parseado.</returns>
    public static WeeklyInterval Parse(string s)
        {
        try
            {
            string[] intervals = s.Split('|');
            string[] days = intervals[0].Split('-');
            string[] times = intervals[1].Split('-');
            return new WeeklyInterval(
                WeeklyInterval.ParseDayOfWeek(days[0]),
                WeeklyInterval.ParseDayOfWeek(days[1]),
                WeeklyInterval.ParseTimeOfDay(times[0]),
                WeeklyInterval.ParseTimeOfDay(times[1]));
            }
        catch (Exception ex)
            {
            throw new FormatException($"WeeklyInterval invalid format (StartDayOfWeek-EndDayOfWeek|HH:mm:ss-HH:mm:ss): {s}", ex);
            }
        }

    public static DayOfWeek ParseDayOfWeek(string s) => EnumExtensions.Parse<DayOfWeek>(s);

    public static TimeSpan ParseTimeOfDay(string s) => TimeSpan.Parse(s);

    public override string ToString() 
        => String.Concat(
            this.StartDay.ToString(), "-", this.EndDay.ToString(),
            "|",
            this.StartTime.ToString(), "-", this.EndTime.ToString()
            );
    }

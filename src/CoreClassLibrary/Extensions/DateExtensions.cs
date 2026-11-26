using CoreClassLibrary.Objects.Ranges;

namespace CoreClassLibrary.Extensions;

/// <summary>
/// Date Extensions
/// </summary>
/// <see cref="https://learn.microsoft.com/en-us/dotnet/standard/datetime/how-to-use-dateonly-timeonly"/>
public static class DateExtensions
    {
    /// <summary>
    /// True si el dia es un dia laborable (lunes-viernes).
    /// </summary>
    public static bool IsWorkingDay(this DayOfWeek day)
        => day != DayOfWeek.Saturday && day != DayOfWeek.Sunday;
    
    public static DateOnly GetToday()
        => DateOnly.FromDateTime(DateTime.Today);

    public static bool IsToday(this DateOnly date)
        => date == DateExtensions.GetToday();

    public static DateTime ToDateTime(this DateOnly date)
        => date.ToDateTime(TimeOnly.MinValue);

    /// <summary>
    /// Returns the short Julian date for a date.
    /// </summary>
    /// <see cref="https://todayjuliandate.com"/>
    public static string GetShortJulianDate(this DateOnly date)
        => $"{date.Year % 100:00}{date.DayOfYear:000}";

    /// <summary>
    /// Returns the Julian date for a date.
    /// </summary>
    /// <see cref="https://todayjuliandate.com"/>
    public static string GetJulianDate(this DateOnly date)
        => $"{date.Year}{date.DayOfYear:000}";

    /// <summary>
    /// Obtiene el rango de dias del mes.
    /// </summary>
    public static Range<DateOnly> GetMonthRange(this DateOnly date)
        => new Range<DateOnly>(DateExtensions.GetMonthFirstDay(date), DateExtensions.GetMonthLastDay(date));

    /// <summary>
    /// Obtiene el primer dia del mes.
    /// </summary>
    public static DateOnly GetMonthFirstDay(this DateOnly date)
        => date.AddDays(1 - date.Day);

    /// <summary>
    /// Obtiene el ultimo dia del mes.
    /// </summary>
    public static DateOnly GetMonthLastDay(this DateOnly date)
        => date.GetMonthFirstDay().AddMonths(1).AddDays(-1);

    /// <summary>
    /// Calcula la cantidad de meses entre las fechas.
    /// </summary>
    /// <param name="dateStart"></param>
    /// <param name="dateEnd"></param>
    /// <remarks>
    /// La componente del dia de las fechas es ignorada.
    /// Si dateEnd es menor a dateStart se retorna la diferencia como negativa.
    /// </remarks>
    public static int GetMonthSpan(this DateOnly dateStart, DateOnly dateEnd)
        => (dateEnd.Year - dateStart.Year) * 12 + dateEnd.Month - dateStart.Month;

    /// <summary>
    /// Crea una fecha correspondiente a las componentes pasadas por parametro.
    /// </summary>
    /// <remarks>
    /// Las componente se ajustan a cada rango y si hace falta se decrementa el dia.
    /// </remarks>
    /// <returns>DateTime creado.</returns>
    public static DateOnly CreateValidDate(int year, int month, int day)
        {
        if (day > 31)    { day   = 31; }
        if (day < 0)     { day   = 1;  }
        if (month > 12)  { month = 12; }
        if (month < 0)   { month = 1;  }

        if (year < 1 || year > 9999)  { year = 2000; }

        while(true)
            {
            try
                {
                return new DateOnly(year, month, day);
                }
            catch
                {
                if (--day < 0)
                    {
                    day = 31;
                    if (--month < 0)
                        {
                        month = 12;
                        }
                    }
                }
            }
        }
    }

using System.Globalization;

namespace CoreClassLibrary.Extensions;

/// <summary>
/// Date, Time, Extensions
/// </summary>
/// <remarks>
/// Formats:
///     G  (default)  05/25/2017 10:30:15
///     R             Tue, 03 Jan 2017 08:08:05 GMT       (RFC 1123)
///     l             tue, 03 jan 2017 08:08:05 gmt       (Lowercase RFC 1123)
///     O             2017-06-12T05:30:45.7680000-07:00   (ISO 8601)
///     
/// ISO 8601 UTC
/// The literal "Z" is actually part of the ISO 8601 datetime standard for UTC times. 
/// When "Z" (Zulu) is tacked on the end of a time, it indicates that that time is UTC.
/// </remarks>
/// <see cref="https://learn.microsoft.com/en-us/dotnet/standard/datetime/"/>
/// <see cref="https://learn.microsoft.com/en-us/dotnet/standard/datetime/how-to-use-dateonly-timeonly"/>
public static class DateTimeExtensions
    {
    public const string DateFormat = "yyyy-MM-dd";
    public const string TimeFormat = "HH:mm:ss.fff";
    public const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";
    public const string DateTimeZoneFormat = "yyyy-MM-dd HH:mm:ss.fff K";
    public const string TimeStampFormat = "yyyyMMddHHmmssfff";
    public const string ShortTimeStampFormat = "yyyyMMddHHmmss";

    public const int SecondsPerDay = 86400;

    // Http date header format
    // https://http.dev/date
    // Important: use with UTC datetime (no time zone)
    public const string RFC1123UTCFormat = "R"; //Tue, 03 Jan 2017 08:08:05 GMT

    // https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings#Roundtrip
    // https://www.iso.org/iso-8601-date-and-time-format.html
    // https://en.wikipedia.org/wiki/ISO_8601
    // DateTimeStyles.RoundtripKind
    public const string ISO8601Format = "o"; //ISO 8601: "yyyy-MM-ddTHH:mm:ss.fffffffK" - "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffffK" - "2024-04-22T11:33:28.4344966-03:00"
    public const string ISO8601UTCFormat = "yyyy-MM-ddTHH:mm:ss.fffZ"; //UTC ISO 8601 - "2024-04-22T11:33:28.434Z"

    // https://en.wikipedia.org/wiki/Unix_time
    public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);

    public static double ToUnixTime(this DateTime datetime)
        => datetime.Subtract(DateTimeExtensions.UnixEpoch).TotalSeconds;

    public static DateTime FromUnixTime(this double unixTime)
        => DateTimeExtensions.UnixEpoch.AddSeconds(unixTime);

    public static readonly double UnixEpochJulianDay = 2440587.5;

    /// <summary>
    /// Returns the Julian day string for a date.
    /// </summary>
    /// <param name="dateTime">The date and time used to calculate the Julian day.</param>
    /// <see cref="https://todayjuliandate.com/julian-day/"/>
    public static double GetJulianDay(this DateTimeOffset dateTime)
        => DateTimeExtensions.UnixEpochJulianDay + (dateTime.UtcTicks - DateTimeOffset.UnixEpoch.UtcTicks) / (double)TimeSpan.TicksPerDay;

    /// <summary>
    /// Returns a new System.DateTime that adds the specified number of working days to the value of this instance,
    /// taking working day as Monday to Friday.
    /// </summary>
    public static DateTime AddWorkingDays(this DateTime date, int value)
        => DateTimeExtensions.AddDays(date, value, DateTimeExtensions.IsWorkingDay);

    /// <summary>
    /// Returns a new System.DateTime that adds the specified number days that satisfied the predicate to the value of this instance.
    /// </summary>
    /// <remarks>
    /// Si value es negativo los dias se restan (al igual que en AddDays).
    /// Si value es cero retorna la misma fecha.
    /// </remarks>
    public static DateTime AddDays(this DateTime date, int value, Predicate<DateTime> predicate)
        => value == 0 ? date : DateTimeExtensions.EnumerateDays(date, value, predicate).Last();

    /// <summary>
    /// Enumera una cantidad de dias hábiles a partir de una fecha (hacia el futuro o el pasado, en esa dirección),
    /// considerando día hábil el contenido en el rango Lunes a Viernes.
    /// </summary>
    public static IEnumerable<DateTime> EnumerateWorkingDays(this DateTime date, int value)
        => DateTimeExtensions.EnumerateDays(date, value, DateTimeExtensions.IsWorkingDay);

    /// <summary>
    /// Enumera una cantidad de dias a partir de una fecha (hacia el futuro o el pasado, en esa dirección).
    /// </summary>
    public static IEnumerable<DateTime> EnumerateDays(this DateTime date, int value)
        => DateTimeExtensions.EnumerateDays(date, value, _ => true);

    /// <summary>
    /// Enumera dias a partir de una fecha (hacia el futuro o el pasado, en esa dirección).
    /// </summary>
    /// <remarks>
    /// Si value es negativo los dias se restan (al igual que en AddDays).
    /// Si value es cero se retorna una secuencia vacia.
    /// </remarks>
    /// <param name="date">Fecha a partir de la cual enumerar (no es incluida en la enumeración)</param>
    /// <param name="value">Cantidad de días a enumerar.</param>
    /// <param name="predicate">Función que debe retornar si un día debe ser enumerado.</param>
    /// <returns>Dias enumerados.</returns>
    public static IEnumerable<DateTime> EnumerateDays(this DateTime date, int value, Predicate<DateTime> predicate)
        {
        int oneday = Math.Sign(value);
        int total = Math.Abs(value);
        int count = 0;

        while (count < total)
            {
            if (predicate(date = date.AddDays(oneday)))
                {
                yield return date;
                count++;
                }
            }
        }

    /// <summary>
    /// True si el dia es un dia laborable (lunes-viernes).
    /// </summary>
    public static bool IsWorkingDay(this DateTime date)
        => date.DayOfWeek.IsWorkingDay();

    /// <summary>
    /// Returns a new System.DateTime that adds the specified number of weeks to the value of this instance.
    /// </summary>
    public static DateTime AddWeeks(this DateTime date, int value)
        => CultureInfo.InvariantCulture.Calendar.AddWeeks(date, value);

    public static int GetWeekOfYear(this DateTime date, CalendarWeekRule rule = CalendarWeekRule.FirstDay, DayOfWeek firstDayOfWeek = DayOfWeek.Sunday)
        => CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(date, rule, firstDayOfWeek);

    /// <summary>
    /// Calcula una edad de acuerdo a una fecha de nacimiento.
    /// </summary>
    /// <param name="birthDate">Fecha de nacimiento.</param>
    /// <returns>Edad calculada.</returns>
    public static int GetAge(this DateTime birthDate)
        => (int)(DateTime.Today - birthDate).TotalYears();
    
    public static bool IsToday(this DateTime date)
        => date.Date == DateTime.Today;

    public static bool IsDate(this DateTime date, DateOnly dateOnly)
        => DateOnly.FromDateTime(date) == dateOnly;

    /// <summary>
    /// Obtiene el ultimo milisegundo del dia.
    /// </summary>
    public static DateTime GetDayLastMillisecond(this DateTime date)
        => date.Date.AddDays(1).AddMilliseconds(-1);

    public static bool IsMidnight(this DateTime date)
        => date.TimeOfDay == TimeSpan.Zero;        //=> date == date.Date;

    public static string ToDateTimeZoneString(this DateTimeOffset date)
        => date.ToString(DateTimeExtensions.DateTimeZoneFormat, CultureInfo.InvariantCulture);

    public static string ToLocalDateTimeString(this DateTimeOffset date)
        => date.ToString(DateTimeExtensions.DateTimeFormat, CultureInfo.InvariantCulture);

    public static string ToLocalDateTimeString(this DateTime date)
        => date.ToString(DateTimeExtensions.DateTimeFormat, CultureInfo.InvariantCulture);

    public static string ToRFC1123UTCString(this DateTimeOffset date)
        => date.ToString(DateTimeExtensions.RFC1123UTCFormat, CultureInfo.InvariantCulture);

    public static string ToISO8601String(this DateTimeOffset date)
        => date.ToString(DateTimeExtensions.ISO8601Format, CultureInfo.InvariantCulture);

    public static string ToISO8601UTCString(this DateTimeOffset date)
        => date.ToString(DateTimeExtensions.ISO8601UTCFormat, CultureInfo.InvariantCulture);

    public static string ToISO8601String(this DateTime date)
        => date.ToString(DateTimeExtensions.ISO8601Format, CultureInfo.InvariantCulture);

    public static DateTime ParseISO8601String(string date)
        => DateTime.ParseExact(date, DateTimeExtensions.ISO8601Format, CultureInfo.InvariantCulture);

    public static string ToNullableString(this DateTime? dateTime)
        => DateTimeExtensions.ToNullableString(dateTime, DateTimeExtensions.DateTimeFormat);

    public static string ToNullableString(this DateTime? dateTime, string format)
        => dateTime.HasValue ? dateTime.Value.ToString(format, CultureInfo.InvariantCulture) : "";

    public static long ToLong(this DateTime date)
        => date.Year * 10000 + date.Month * 100 + date.Day;

    public static DateTime ToDateTime(this long yyyyMMdd)
        {
        if (yyyyMMdd > 99999999) { throw new IndexOutOfRangeException(); }

        int anio = (int)(yyyyMMdd / 10000);
        int mes = (int)((yyyyMMdd - anio * 10000) / 100);
        int dia = (int)(yyyyMMdd - anio * 10000 - mes * 100);

        return new DateTime(anio, mes, dia);
        }

    public static bool IsNow(this TimeProvider timeProvider, DateTimeOffset time, TimeSpan timeWindow)
        => DateTimeExtensions.AreEqual(time, timeProvider.GetLocalNow(), timeWindow);

    public static bool AreEqual(DateTimeOffset val1, DateTimeOffset val2, TimeSpan timeWindow)
        => DateTimeExtensions.AreEqual(val1.DateTime, val2.DateTime, timeWindow);

    /// <summary>
    /// Compara dos valores, teniendo en cuenta un intervalo dentro del que se consideran iguales.
    /// </summary>
    /// <returns>True si los valores se consideran iguales.</returns>
    public static bool AreEqual(DateTime val1, DateTime val2, TimeSpan timeWindow)
        => val1.Subtract(val2).Duration() <= timeWindow;
    }

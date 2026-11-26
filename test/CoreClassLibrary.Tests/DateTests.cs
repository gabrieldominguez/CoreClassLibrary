
using CoreClassLibrary.Extensions;
using CoreClassLibrary.Objects.Ranges;
using Xunit;

namespace CoreClassLibrary.Tests;

public class DateTests()
    {
    /*
     * https://www.mathsisfun.com/leap-years.html
    Leap Years are any year 
        - that can be exactly divided by 4 (such as 2020, 2024, 2028, etc)
 	    X but if it can be exactly divided by 100, then it isn't (such as 2100, 2200, etc)
 	 	- except if it can be exactly divided by 400, then it is (such as 2000, 2400)    
    */
    [Theory]
    [InlineData(1975, false)]
    [InlineData(2000, true)]
    [InlineData(2001, false)]
    [InlineData(2004, true)]
    [InlineData(2020, true)]
    [InlineData(2024, true)]
    [InlineData(2028, true)]
    [InlineData(2100, false)]
    [InlineData(2200, false)]
    [InlineData(2400, true)]
    public void IsLeapYear(int year, bool expected)
        {
        Assert.Equal(expected, year % 4 == 0 && (year % 100 != 0 || year % 400 == 0));
        Assert.Equal(expected, DateTime.IsLeapYear(year));
        }

    [Theory]
    [InlineData(1975, 01, 01, 1)]
    [InlineData(1975, 03, 15, 74)]
    [InlineData(1975, 11, 12, 316)]
    [InlineData(1975, 12, 25, 359)]
    [InlineData(1975, 12, 31, 365)]
    [InlineData(2000, 01, 01, 1)]
    [InlineData(2000, 03, 15, 75)]
    [InlineData(2000, 11, 12, 317)]
    [InlineData(2000, 12, 25, 360)]
    [InlineData(2000, 12, 31, 366)]
    [InlineData(2026, 11, 26, 330)]
    public void OrdinalDate(int year, int month, int day, int expected)
        => Assert.Equal(expected, new DateOnly(year, month, day).DayOfYear);

    [Theory]
    [InlineData(1975, 01, 01, "75001")]
    [InlineData(1975, 03, 15, "75074")]
    [InlineData(1975, 11, 12, "75316")]
    [InlineData(1975, 12, 25, "75359")]
    [InlineData(1975, 12, 31, "75365")]
    [InlineData(2000, 01, 01, "00001")]
    [InlineData(2000, 03, 15, "00075")]
    [InlineData(2000, 11, 12, "00317")]
    [InlineData(2000, 12, 25, "00360")]
    [InlineData(2000, 12, 31, "00366")]
    [InlineData(2026, 11, 26, "26330")]
    public void GetShortJulianDate(int year, int month, int day, string expected)
        => Assert.Equal(expected, new DateOnly(year, month, day).GetShortJulianDate());

    [Theory]
    [InlineData(1975, 01, 01, "1975001")]
    [InlineData(1975, 03, 15, "1975074")]
    [InlineData(1975, 11, 12, "1975316")]
    [InlineData(1975, 12, 25, "1975359")]
    [InlineData(1975, 12, 31, "1975365")]
    [InlineData(2000, 01, 01, "2000001")]
    [InlineData(2000, 03, 15, "2000075")]
    [InlineData(2000, 11, 12, "2000317")]
    [InlineData(2000, 12, 25, "2000360")]
    [InlineData(2000, 12, 31, "2000366")]
    [InlineData(2026, 11, 26, "2026330")]
    public void GetJulianDate(int year, int month, int day, string expected)
        => Assert.Equal(expected, new DateOnly(year, month, day).GetJulianDate());

    [Theory]
    [InlineData("1582-10-03T00:00:00+00:00", 2299148.5)]
    [InlineData("1582-10-04T00:00:00+00:00", 2299149.5)]
    [InlineData("1582-10-05T00:00:00+00:00", 2299150.5)]
    [InlineData("1969-12-31T00:00:00+00:00", 2440586.5)]
    [InlineData("1970-01-01T00:00:00+00:00", 2440587.5)]
    [InlineData("2000-01-01T12:00:00+00:00", 2451545.0)]
    [InlineData("2000-01-01T12:00:00-00:00", 2451545.0)]    
    [InlineData("2000-01-01T07:00:00-05:00", 2451545.0)]
    [InlineData("2026-07-21T00:00:00+00:00", 2461242.5)]
    [InlineData("2026-07-21T12:00:00+00:00", 2461243.0)]
    public void GetJulianDay(string dateTime, double expected)
        => Assert.Equal(expected, DateTimeOffset.Parse(dateTime).GetJulianDay());

    [Theory]
    [InlineData(2009, 2, 29, "2009-02-28")]
    [InlineData(2005, 25, 20, "2005-12-20")]
    public void CreateValidDate(int year, int month, int day, string validDateTime) 
        => Assert.Equal(DateOnly.Parse(validDateTime), DateExtensions.CreateValidDate(year, month, day));
    
    [Theory]
    [InlineData("2017-11-10", "2017-11-01", "2017-11-30")]
    [InlineData("2019-08-26", "2019-08-01", "2019-08-31")]
    [InlineData("2009-02-28", "2009-02-01", "2009-02-28")]
    [InlineData("2005-12-20", "2005-12-01", "2005-12-31")]
    [InlineData("2019-08-23", "2019-08-01", "2019-08-31")]
    [InlineData("2019-08-22", "2019-08-01", "2019-08-31")]
    [InlineData("2020-01-01", "2020-01-01", "2020-01-31")]
    [InlineData("2020-01-05", "2020-01-01", "2020-01-31")]
    [InlineData("2020-01-12", "2020-01-01", "2020-01-31")]
    [InlineData("2020-01-18", "2020-01-01", "2020-01-31")]
    [InlineData("2020-01-24", "2020-01-01", "2020-01-31")]
    [InlineData("2020-01-31", "2020-01-01", "2020-01-31")]
    [InlineData("2020-02-01", "2020-02-01", "2020-02-29")]
    [InlineData("2020-02-02", "2020-02-01", "2020-02-29")]
    [InlineData("1957-10-04", "1957-10-01", "1957-10-31")]
    [InlineData("1999-01-01", "1999-01-01", "1999-01-31")]
    [InlineData("1969-12-31", "1969-12-01", "1969-12-31")]
    [InlineData("1970-01-01", "1970-01-01", "1970-01-31")]
    [InlineData("1970-01-02", "1970-01-01", "1970-01-31")]
    [InlineData("2001-09-09", "2001-09-01", "2001-09-30")]
    [InlineData("2004-09-16", "2004-09-01", "2004-09-30")]
    [InlineData("2004-09-17", "2004-09-01", "2004-09-30")]
    [InlineData("1998-12-31", "1998-12-01", "1998-12-31")]
    [InlineData("1999-01-31", "1999-01-01", "1999-01-31")]
    [InlineData("2000-01-31", "2000-01-01", "2000-01-31")]
    [InlineData("2001-01-31", "2001-01-01", "2001-01-31")]
    [InlineData("2010-01-31", "2010-01-01", "2010-01-31")]
    public void GetMonthRange(string dateTime, string rangeStart, string rangeEnd)
        {
        Range<DateOnly> range = DateOnly.Parse(dateTime).GetMonthRange();
        Assert.Equal(DateOnly.Parse(rangeStart), range.Start);
        Assert.Equal(DateOnly.Parse(rangeEnd), range.End);
        }

    [Theory]
    [InlineData("2017-11-01", "2017-11-30", 0)]
    [InlineData("2019-08-01", "2019-08-31", 0)]
    [InlineData("2009-02-01", "2009-02-28", 0)]
    [InlineData("2005-12-01", "2005-12-31", 0)]
    [InlineData("2019-08-01", "2019-09-30", 1)]
    [InlineData("2019-08-01", "2019-10-31", 2)]
    [InlineData("2020-01-01", "2020-01-31", 0)]
    [InlineData("2020-01-01", "2020-02-28", 1)]
    [InlineData("2020-01-01", "2020-03-31", 2)]
    [InlineData("2020-01-01", "2020-04-30", 3)]
    [InlineData("2020-01-01", "2020-05-31", 4)]
    [InlineData("2020-01-01", "2020-06-30", 5)]
    [InlineData("2020-02-01", "2020-02-28", 0)]
    [InlineData("2020-02-01", "2020-03-29", 1)]
    [InlineData("1957-10-01", "1957-10-31", 0)]
    [InlineData("1999-01-01", "1999-01-31", 0)]
    [InlineData("1969-12-01", "1969-12-31", 0)]
    [InlineData("1970-01-01", "1970-01-31", 0)]
    [InlineData("1970-01-01", "1970-02-28", 1)]
    [InlineData("2001-09-01", "2001-09-30", 0)]
    [InlineData("2004-09-01", "2004-09-30", 0)]
    [InlineData("2004-09-01", "2004-10-30", 1)]
    [InlineData("1998-12-01", "1998-12-31", 0)]
    [InlineData("1999-01-01", "1999-02-28", 1)]
    [InlineData("2000-01-01", "2000-01-31", 0)]
    [InlineData("2001-01-01", "2001-01-31", 0)]
    [InlineData("2010-01-01", "2010-01-31", 0)]
    [InlineData("2020-02-28", "2020-01-01", -1)]
    [InlineData("2020-03-31", "2020-01-01", -2)]
    [InlineData("2020-04-30", "2020-01-01", -3)]
    [InlineData("2020-05-31", "2020-01-01", -4)]
    [InlineData("2020-06-30", "2020-01-01", -5)]
    [InlineData("2010-01-01", "2011-01-01", 12)]
    [InlineData("2010-01-01", "2009-01-01", -12)]
    public void GetMonthSpan(string dateTimeStart, string dateTimeEnd, int months)
        => Assert.Equal(months, DateOnly.Parse(dateTimeStart).GetMonthSpan(DateOnly.Parse(dateTimeEnd)));

    [Fact]
    public void DateTimeAreEqual()
        {
        DateTime midnight = DateTime.Today;
        DateTime oneSecBeforeMidnight = midnight.Subtract(TimeSpan.FromSeconds(1));
        DateTime oneSecAfterMidnight = midnight.Add(TimeSpan.FromSeconds(1));

        TimeSpan insideWindow = TimeSpan.FromSeconds(1.1);
        TimeSpan outsideWindow = TimeSpan.FromSeconds(0.9);

        Assert.True(DateTimeExtensions.AreEqual(midnight, oneSecBeforeMidnight, insideWindow));
        Assert.True(DateTimeExtensions.AreEqual(midnight, oneSecAfterMidnight, insideWindow));
        Assert.False(DateTimeExtensions.AreEqual(midnight, oneSecBeforeMidnight, outsideWindow));
        Assert.False(DateTimeExtensions.AreEqual(midnight, oneSecAfterMidnight, outsideWindow));

        Assert.True(DateTimeExtensions.AreEqual(oneSecBeforeMidnight, midnight, insideWindow));
        Assert.True(DateTimeExtensions.AreEqual(oneSecAfterMidnight, midnight, insideWindow));
        Assert.False(DateTimeExtensions.AreEqual(oneSecBeforeMidnight, midnight, outsideWindow));
        Assert.False(DateTimeExtensions.AreEqual(oneSecAfterMidnight, midnight, outsideWindow));
        }
    }

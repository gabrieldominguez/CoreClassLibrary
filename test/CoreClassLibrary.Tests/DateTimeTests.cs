
using CoreClassLibrary.Extensions;
using CoreClassLibrary.Objects;
using Xunit;

namespace CoreClassLibrary.Tests;

public class DateTimeTests(ITestOutputHelper output)
    {
    [Theory]
    [InlineData("2009-02-28", true)]
    [InlineData("2005-12-20", true)]
    [InlineData("2017-11-10", true)]
    [InlineData("2019-08-26", true)]
    [InlineData("2019-08-23", true)]
    [InlineData("2019-08-22", true)]
    [InlineData("2020-01-01", true)]
    [InlineData("2020-01-05", true)]
    [InlineData("2020-01-12", true)]
    [InlineData("2020-01-18", true)]
    [InlineData("2020-01-24", true)]
    [InlineData("2020-01-31", true)]
    [InlineData("2020-02-01", true)]
    [InlineData("2020-02-02", true)]
    [InlineData("1957-10-04 00:00:00", true)]
    [InlineData("1969-12-31 00:00:00", true)]
    [InlineData("1969-12-31 23:00:00", false)]
    [InlineData("1969-12-31 23:59:00", false)]
    [InlineData("1969-12-31 23:59:59", false)]
    [InlineData("1970-01-01 00:00:00", true)]
    [InlineData("1970-01-01 00:00:01", false)]
    [InlineData("1970-01-01 00:01:00", false)]
    [InlineData("1970-01-01 01:00:00", false)]
    [InlineData("1970-01-02 00:00:00", true)]
    [InlineData("2001-09-09 01:46:40", false)]
    [InlineData("2004-09-16 00:00:00", true)]
    [InlineData("2004-09-16 23:59:58.75", false)]
    [InlineData("2004-09-16 23:59:59.00", false)]
    [InlineData("2004-09-16 23:59:59.25", false)]
    [InlineData("2004-09-16 23:59:59.50", false)]
    [InlineData("2004-09-16 23:59:59.75", false)]
    [InlineData("2004-09-17 00:00:00.00", true)]
    [InlineData("2004-09-17 00:00:00.25", false)]
    [InlineData("2004-09-17 00:00:00.50", false)]
    [InlineData("2004-09-17 00:00:00.75", false)]
    [InlineData("2004-09-17 00:00:01.00", false)]
    [InlineData("2004-09-17 00:00:01.25", false)]
    [InlineData("1998-12-31 23:59:58.75", false)]
    [InlineData("1998-12-31 23:59:59.00", false)]
    [InlineData("1998-12-31 23:59:59.25", false)]
    [InlineData("1998-12-31 23:59:59.50", false)]
    [InlineData("1998-12-31 23:59:59.75", false)]
    [InlineData("1999-01-01 00:00:00.00", true)]
    [InlineData("1999-01-01 00:00:00.25", false)]
    [InlineData("1999-01-01 00:00:00.50", false)]
    [InlineData("1999-01-01 00:00:00.75", false)]
    [InlineData("1999-01-01 00:00:01.00", false)]
    [InlineData("1999-01-01 00:00:01.25", false)]
    public void IsMidnight(string dateTime, bool expected)
        => Assert.Equal(expected, DateTime.Parse(dateTime).IsMidnight());

    [Theory]
    [InlineData("2009-02-28", "2009-02-28 23:59:59.999")]
    [InlineData("2005-12-20", "2005-12-20 23:59:59.999")]
    [InlineData("2017-11-10", "2017-11-10 23:59:59.999")]
    [InlineData("2019-08-26", "2019-08-26 23:59:59.999")]
    [InlineData("2019-08-23", "2019-08-23 23:59:59.999")]
    [InlineData("2019-08-22", "2019-08-22 23:59:59.999")]
    [InlineData("2020-01-01", "2020-01-01 23:59:59.999")]
    [InlineData("2020-01-05", "2020-01-05 23:59:59.999")]
    [InlineData("2020-01-12", "2020-01-12 23:59:59.999")]
    [InlineData("2020-01-18", "2020-01-18 23:59:59.999")]
    [InlineData("2020-01-24", "2020-01-24 23:59:59.999")]
    [InlineData("2020-01-31", "2020-01-31 23:59:59.999")]
    [InlineData("2020-02-01", "2020-02-01 23:59:59.999")]
    [InlineData("2020-02-02", "2020-02-02 23:59:59.999")]
    [InlineData("1957-10-04 00:00:00", "1957-10-04 23:59:59.999")]
    [InlineData("1969-12-31 00:00:00", "1969-12-31 23:59:59.999")]
    [InlineData("1969-12-31 23:00:00", "1969-12-31 23:59:59.999")]
    [InlineData("1969-12-31 23:59:00", "1969-12-31 23:59:59.999")]
    [InlineData("1969-12-31 23:59:59", "1969-12-31 23:59:59.999")]
    [InlineData("1970-01-01 00:00:00", "1970-01-01 23:59:59.999")]
    [InlineData("1970-01-01 00:00:01", "1970-01-01 23:59:59.999")]
    [InlineData("1970-01-01 00:01:00", "1970-01-01 23:59:59.999")]
    [InlineData("1970-01-01 01:00:00", "1970-01-01 23:59:59.999")]
    [InlineData("1970-01-02 00:00:00", "1970-01-02 23:59:59.999")]
    [InlineData("2001-09-09 01:46:40", "2001-09-09 23:59:59.999")]
    [InlineData("2004-09-16 00:00:00", "2004-09-16 23:59:59.999")]
    [InlineData("2004-09-16 23:59:58.75", "2004-09-16 23:59:59.999")]
    [InlineData("2004-09-16 23:59:59.00", "2004-09-16 23:59:59.999")]
    [InlineData("2004-09-16 23:59:59.25", "2004-09-16 23:59:59.999")]
    [InlineData("2004-09-16 23:59:59.50", "2004-09-16 23:59:59.999")]
    [InlineData("2004-09-16 23:59:59.75", "2004-09-16 23:59:59.999")]
    [InlineData("2004-09-17 00:00:00.00", "2004-09-17 23:59:59.999")]
    [InlineData("2004-09-17 00:00:00.25", "2004-09-17 23:59:59.999")]
    [InlineData("2004-09-17 00:00:00.50", "2004-09-17 23:59:59.999")]
    [InlineData("2004-09-17 00:00:00.75", "2004-09-17 23:59:59.999")]
    [InlineData("2004-09-17 00:00:01.00", "2004-09-17 23:59:59.999")]
    [InlineData("2004-09-17 00:00:01.25", "2004-09-17 23:59:59.999")]
    [InlineData("1998-12-31 23:59:58.75", "1998-12-31 23:59:59.999")]
    [InlineData("1998-12-31 23:59:59.00", "1998-12-31 23:59:59.999")]
    [InlineData("1998-12-31 23:59:59.25", "1998-12-31 23:59:59.999")]
    [InlineData("1998-12-31 23:59:59.50", "1998-12-31 23:59:59.999")]
    [InlineData("1998-12-31 23:59:59.75", "1998-12-31 23:59:59.999")]
    [InlineData("1999-01-01 00:00:00.00", "1999-01-01 23:59:59.999")]
    [InlineData("1999-01-01 00:00:00.25", "1999-01-01 23:59:59.999")]
    [InlineData("1999-01-01 00:00:00.50", "1999-01-01 23:59:59.999")]
    [InlineData("1999-01-01 00:00:00.75", "1999-01-01 23:59:59.999")]
    [InlineData("1999-01-01 00:00:01.00", "1999-01-01 23:59:59.999")]
    [InlineData("1999-01-01 00:00:01.25", "1999-01-01 23:59:59.999")]
    public void GetDayLastMillisecond(string dateTime, string expectedDateTime)
        => Assert.Equal(DateTime.Parse(expectedDateTime), DateTime.Parse(dateTime).GetDayLastMillisecond());

    [Theory]
    [InlineData("2009-02-28", "2009-02-28 00:00:00")]
    [InlineData("2005-12-20", "2005-12-20 00:00:00")]
    [InlineData("2017-11-10", "2017-11-10 00:00:00")]
    [InlineData("2019-08-26", "2019-08-26 00:00:00")]
    [InlineData("2019-08-23", "2019-08-23 00:00:00")]
    [InlineData("2019-08-22", "2019-08-22 00:00:00")]
    [InlineData("2020-01-05", "2020-01-05 00:00:00")]
    [InlineData("2020-01-12", "2020-01-12 00:00:00")]
    [InlineData("2020-01-18", "2020-01-18 00:00:00")]
    [InlineData("2020-01-24", "2020-01-24 00:00:00")]
    [InlineData("2020-01-31", "2020-01-31 00:00:00")]
    [InlineData("2020-02-01", "2020-02-01 00:00:00")]
    [InlineData("2020-02-02", "2020-02-02 00:00:00")]
    [InlineData("1957-10-04 00:00:00", "1957-10-04 00:00:00")]
    [InlineData("1969-12-31 00:00:00", "1969-12-31 00:00:00")]
    [InlineData("1969-12-31 23:00:00", "1969-12-31 00:00:00")]
    [InlineData("1969-12-31 23:59:00", "1969-12-31 00:00:00")]
    [InlineData("1969-12-31 23:59:59", "1969-12-31 00:00:00")]
    [InlineData("1970-01-01 00:00:00", "1970-01-01 00:00:00")]
    [InlineData("1970-01-01 00:00:01", "1970-01-01 00:00:00")]
    [InlineData("1970-01-01 00:01:00", "1970-01-01 00:00:00")]
    [InlineData("1970-01-01 01:00:00", "1970-01-01 00:00:00")]
    [InlineData("1970-01-02 00:00:00", "1970-01-02 00:00:00")]
    [InlineData("2001-09-09 01:46:40", "2001-09-09 00:00:00")]
    [InlineData("2004-09-16 00:00:00", "2004-09-16 00:00:00")]
    [InlineData("2004-09-16 23:59:58.75", "2004-09-16 00:00:00")]
    [InlineData("2004-09-16 23:59:59.00", "2004-09-16 00:00:00")]
    [InlineData("2004-09-16 23:59:59.25", "2004-09-16 00:00:00")]
    [InlineData("2004-09-16 23:59:59.50", "2004-09-16 00:00:00")]
    [InlineData("2004-09-16 23:59:59.75", "2004-09-16 00:00:00")]
    [InlineData("2004-09-17 00:00:00.00", "2004-09-17 00:00:00")]
    [InlineData("2004-09-17 00:00:00.25", "2004-09-17 00:00:00")]
    [InlineData("2004-09-17 00:00:00.50", "2004-09-17 00:00:00")]
    [InlineData("2004-09-17 00:00:00.75", "2004-09-17 00:00:00")]
    [InlineData("2004-09-17 00:00:01.00", "2004-09-17 00:00:00")]
    [InlineData("2004-09-17 00:00:01.25", "2004-09-17 00:00:00")]
    [InlineData("1998-12-31 23:59:58.75", "1998-12-31 00:00:00")]
    [InlineData("1998-12-31 23:59:59.00", "1998-12-31 00:00:00")]
    [InlineData("1998-12-31 23:59:59.25", "1998-12-31 00:00:00")]
    [InlineData("1998-12-31 23:59:59.50", "1998-12-31 00:00:00")]
    [InlineData("1998-12-31 23:59:59.75", "1998-12-31 00:00:00")]
    [InlineData("1999-01-01 00:00:00.00", "1999-01-01 00:00:00")]
    [InlineData("1999-01-01 00:00:00.25", "1999-01-01 00:00:00")]
    [InlineData("1999-01-01 00:00:00.50", "1999-01-01 00:00:00")]
    [InlineData("1999-01-01 00:00:00.75", "1999-01-01 00:00:00")]
    [InlineData("1999-01-01 00:00:01.00", "1999-01-01 00:00:00")]
    [InlineData("1999-01-01 00:00:01.25", "1999-01-01 00:00:00")]
    public void ToDate(string dateTime, string expectedDateTime) => Assert.Equal(DateTime.Parse(expectedDateTime), DateTime.Parse(dateTime).Date);

    [Theory]
    [InlineData("2019-08-23", 0, "2019-08-23")]
    [InlineData("2019-08-23", -1, "2019-08-22")]
    [InlineData("2019-08-22", 1, "2019-08-23")]
    [InlineData("2019-08-23", 1, "2019-08-26")]
    [InlineData("2019-08-23", 5, "2019-08-30")]
    [InlineData("2019-08-23", 10, "2019-09-06")]
    [InlineData("2019-08-26", -1, "2019-08-23")]
    [InlineData("2019-08-22", 2, "2019-08-26")]
    public void AddWorkingDays(string date, int days, string expectedDate)
        => Assert.Equal(DateTime.Parse(expectedDate), DateTime.Parse(date).AddWorkingDays(days));

    [Theory]
    [InlineData("2019-08-22", "2019-08-23", "2019-08-26", "2019-08-27")]
    public void EnumerateWorkingDays(params string[] dates)
        => Assert.Equal(dates.Skip(1).Select(DateTime.Parse), DateTime.Parse(dates[0]).EnumerateWorkingDays(dates.Length - 1));

    [Theory]
    [InlineData("2019-08-22", "2019-08-23", "2019-08-24", "2019-08-25", "2019-08-26", "2019-08-27")]
    public void EnumerateDays(params string[] dates)
        => Assert.Equal(dates.Skip(1).Select(DateTime.Parse), DateTime.Parse(dates[0]).EnumerateDays(dates.Length - 1));

    [Theory]
    // https://en.wikipedia.org/wiki/Unix_time
    [InlineData("1957-10-04 00:00:00", -386380800)]
    [InlineData("1969-12-31 00:00:00", -86400)]
    [InlineData("1969-12-31 23:00:00", -3600)]
    [InlineData("1969-12-31 23:59:00", -60)]
    [InlineData("1969-12-31 23:59:59", -1)]
    [InlineData("1970-01-01 00:00:00", 0)]
    [InlineData("1970-01-01 00:00:01", 1)]
    [InlineData("1970-01-01 00:01:00", 60)]
    [InlineData("1970-01-01 01:00:00", 3600)]
    [InlineData("1970-01-02 00:00:00", 86400)]
    [InlineData("2001-09-09 01:46:40", 1000000000)]
    [InlineData("2004-09-16 00:00:00", 1095292800)]
    [InlineData("2004-09-16 23:59:58.75", 1095379198.75)]
    [InlineData("2004-09-16 23:59:59.00", 1095379199.00)]
    [InlineData("2004-09-16 23:59:59.25", 1095379199.25)]
    [InlineData("2004-09-16 23:59:59.50", 1095379199.50)]
    [InlineData("2004-09-16 23:59:59.75", 1095379199.75)]
    [InlineData("2004-09-17 00:00:00.00", 1095379200.00)]
    [InlineData("2004-09-17 00:00:00.25", 1095379200.25)]
    [InlineData("2004-09-17 00:00:00.50", 1095379200.50)]
    [InlineData("2004-09-17 00:00:00.75", 1095379200.75)]
    [InlineData("2004-09-17 00:00:01.00", 1095379201.00)]
    [InlineData("2004-09-17 00:00:01.25", 1095379201.25)]
    [InlineData("1998-12-31 23:59:58.75", 915148798.75)]
    [InlineData("1998-12-31 23:59:59.00", 915148799.00)]
    [InlineData("1998-12-31 23:59:59.25", 915148799.25)]
    [InlineData("1998-12-31 23:59:59.50", 915148799.50)]
    [InlineData("1998-12-31 23:59:59.75", 915148799.75)]
    [InlineData("1999-01-01 00:00:00.00", 915148800.00)]
    [InlineData("1999-01-01 00:00:00.25", 915148800.25)]
    [InlineData("1999-01-01 00:00:00.50", 915148800.50)]
    [InlineData("1999-01-01 00:00:00.75", 915148800.75)]
    [InlineData("1999-01-01 00:00:01.00", 915148801.00)]
    [InlineData("1999-01-01 00:00:01.25", 915148801.25)]
    public void UnixTime(string dateTime, double unixTime)
        {
        Assert.Equal(unixTime, DateTime.Parse(dateTime).ToUnixTime());
        Assert.Equal(DateTime.Parse(dateTime), unixTime.FromUnixTime());
        }

    [Theory]
    [InlineData(20171110, "2017-11-10")]
    [InlineData(20190826, "2019-08-26")]
    [InlineData(20090228, "2009-02-28")]
    [InlineData(20051220, "2005-12-20")]
    [InlineData(20190823, "2019-08-23")]
    [InlineData(20190822, "2019-08-22")]
    [InlineData(20200101, "2020-01-01")]
    [InlineData(20200105, "2020-01-05")]
    [InlineData(20200112, "2020-01-12")]
    [InlineData(20200118, "2020-01-18")]
    [InlineData(20200124, "2020-01-24")]
    [InlineData(20200131, "2020-01-31")]
    [InlineData(20200201, "2020-02-01")]
    [InlineData(20200202, "2020-02-02")]
    [InlineData(19571004, "1957-10-04")]
    [InlineData(19691231, "1969-12-31")]
    [InlineData(19700101, "1970-01-01")]
    [InlineData(19700102, "1970-01-02")]
    [InlineData(20010909, "2001-09-09")]
    [InlineData(20040916, "2004-09-16")]
    [InlineData(20040917, "2004-09-17")]
    [InlineData(19981231, "1998-12-31")]
    [InlineData(19990101, "1999-01-01")]
    public void LongToDateTime(long num, string dateTime)
        {
        DateTime dt = DateTime.Parse(dateTime);
        Assert.Equal(dt, num.ToDateTime());
        Assert.Equal(num, dt.ToLong());
        }

    [Theory]
    [InlineData("2020-01-01", 5)]
    [InlineData("2000-01-01", 25)]
    [InlineData("2000-01-24", 25)]
    [InlineData("2000-01-25", 24)]
    [InlineData("1975-11-12", 49)]
    [InlineData("1975-05-05", 49)]
    [InlineData("2010-01-12", 15)]
    [InlineData("2012-08-31", 12)]
    [InlineData("2015-11-10", 9)]
    public void GetAge(string dateTime, int minimunAge)
        {
        int age = DateTime.Parse(dateTime).GetAge();
        output.WriteLine(age.ToString());
        Assert.True(age >= minimunAge);
        }

    [Theory]
    [InlineData("2026-05-01", "2026-07-30", 90)]
    public void DateDiff(string fromDateTime, string toDateTime, int days)
        => Assert.Equal(days, (DateTime.Parse(toDateTime) - DateTime.Parse(fromDateTime)).Days);

    [Theory]
    [InlineData("2019-08-26", -4, "2019-07-29")]
    [InlineData("2019-08-26", -3, "2019-08-05")]
    [InlineData("2019-08-26", -2, "2019-08-12")]
    [InlineData("2019-08-26", -1, "2019-08-19")]
    [InlineData("2019-08-26", 0, "2019-08-26")]
    [InlineData("2019-08-26", 1, "2019-09-02")]
    [InlineData("2019-08-26", 2, "2019-09-09")]
    [InlineData("2019-08-26", 3, "2019-09-16")]
    [InlineData("2019-08-26", 4, "2019-09-23")]
    [InlineData("2019-08-26", 5, "2019-09-30")]
    [InlineData("2019-08-26", 6, "2019-10-07")]
    [InlineData("2019-09-02", 1, "2019-09-09")]
    public void AddWeeks(string dateTime, int weeks, string expectedDateTime)
        => Assert.Equal(DateTime.Parse(expectedDateTime), DateTime.Parse(dateTime).AddWeeks(weeks));

    [Theory]
    [InlineData("2020-01-01", 1)]
    [InlineData("2020-01-05", 2)]
    [InlineData("2020-01-12", 3)]
    [InlineData("2020-01-18", 3)]
    [InlineData("2020-01-24", 4)]
    [InlineData("2020-01-31", 5)]
    [InlineData("2020-02-01", 5)]
    [InlineData("2020-02-02", 6)]
    public void GetWeekOfYear(string dateTime, int expectedWeek)
        => Assert.Equal(expectedWeek, DateTime.Parse(dateTime).GetWeekOfYear());

    [Fact]
    public void WeeklyInterval()
        {
        WeeklyInterval weeklyInterval = new WeeklyInterval(DayOfWeek.Monday, DayOfWeek.Friday, TimeSpan.Zero, TimeSpan.FromHours(12));
        Assert.True(weeklyInterval.IsInInterval(new DateTime(2023, 4, 19, 9, 30, 0)));
        Assert.False(weeklyInterval.IsInInterval(new DateTime(2023, 4, 22, 9, 30, 0)));
        Assert.False(weeklyInterval.IsInInterval(new DateTime(2023, 4, 19, 19, 30, 0)));
        Assert.True(weeklyInterval.IsInInterval(DayOfWeek.Monday, TimeSpan.FromHours(6)));
        Assert.False(weeklyInterval.IsInInterval(DayOfWeek.Monday, TimeSpan.FromHours(16)));
        Assert.False(weeklyInterval.IsInInterval(DayOfWeek.Sunday, TimeSpan.FromHours(6)));
        }

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

    [Fact]
    public void DateTimeEqualDateTimeOffset()
        {
        DateTime now = DateTime.Now;
        DateTimeOffset nowOffset = DateTimeOffset.Now;

        Assert.Equal((DateTimeOffset)now, nowOffset, TimeSpan.FromSeconds(1));
        Assert.Equal(now, nowOffset.DateTime, TimeSpan.FromSeconds(1));
        }

    [Fact]
    public void UTC()
        {
        DateTime date = DateTime.Now;
        DateTime dateUTC = DateTime.UtcNow;
        Assert.True(TimeExtensions.AreEqual(TimeZoneInfo.Local.GetUtcOffset(date), date - dateUTC, TimeSpan.FromMilliseconds(1)));
        }

    [Fact]
    public void UTCOffset()
        {
        DateTimeOffset date = DateTimeOffset.Now;
        DateTimeOffset dateUTC = DateTimeOffset.UtcNow;
        Assert.True(TimeExtensions.AreEqual(TimeSpan.Zero, date - dateUTC, TimeSpan.FromSeconds(1)));
        Assert.True(TimeExtensions.AreEqual(TimeZoneInfo.Local.GetUtcOffset(date), date.DateTime - dateUTC.DateTime, TimeSpan.FromMilliseconds(1)));
        }
    }

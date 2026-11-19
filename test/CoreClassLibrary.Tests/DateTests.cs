
using CoreClassLibrary.Extensions;
using CoreClassLibrary.Objects.Ranges;
using Xunit;

namespace CoreClassLibrary.Tests;

public class DateTests()
    {
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
